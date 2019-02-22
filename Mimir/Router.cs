using Mimir.Response.AuthServer;
using Mimir.Response.SessionServer.Session.Minecraft;
using Mimir.Response.Users;
using RUL;
using RUL.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace Mimir
{
    /// <summary>
    /// 路由类
    /// </summary>
    class Router
    {
        private static Logger log = new Logger("Router");

        /// <summary>
        /// 路由方法
        /// </summary>
        /// <param name="httpReq">请求内容</param>
        /// <param name="socket">Socket对象</param>
        /// <param name="sslStream">SslStream对象</param>
        public static void Route(string httpReq, Socket socket, SslStream sslStream)
        {
            // 解析请求
            HttpReq req = HttpProtocol.Solve(httpReq);

            log.Info($"Got request {req.Method} {req.Url} from {socket.RemoteEndPoint}.");

            // 默认的返回ValueTuple结构
            (int status, string type, string content) response = (403, "text/plain", "");

            // 防止CC攻击
            if (Program.IPSecurity.ContainsKey(socket.RemoteEndPoint.ToString().Split(':')[0]))
            {
                if (Program.IPSecurity[socket.RemoteEndPoint.ToString().Split(':')[0]] > 50)
                {
                    socket.Close();
                    return;
                }
                else
                {
                    Program.IPSecurity[socket.RemoteEndPoint.ToString().Split(':')[0]]++;
                }
            }
            else
            {
                Program.IPSecurity.Add(socket.RemoteEndPoint.ToString().Split(':')[0], 1);
            }

            try
            {
                // 路由请求
                switch (req.Method)
                {
                    // 当请求是Get时
                    case Method.Get:
                        switch (req.Url)
                        {
                            #region ExtendAPI
                            case "/":
                                response = Response.Root.OnGet();
                                break;
                            #endregion

                            #region SessionServer
                            case "/sessionserver/session/minecraft/hasJoined":
                                response = HasJoined.OnGet(req.Get);
                                break;
                            #endregion

                            #region Skins
                            case "/skins":
                                (int status, string type, byte[] content) = Response.Skins.Root.OnGet(req.Get);
                                Post(status, type, content, socket, sslStream);
                                break;
                            #endregion

                            default:
                                #region SessionServer
                                // Get /sessionserver/session/minecraft/profile/{uuid}?unsigned={unsigned}
                                if (req.Url.Split('/').Length == 6)
                                {
                                    if (Guid.TryParse(req.Url.Split('/')[5], out Guid guid))
                                    {
                                        response = Response.SessionServer.Session.Minecraft.Profile.Root.OnGet(req.Get, guid);
                                    }
                                }
                                #endregion

                                // 这个实现不好，不能作为Http服务器发送图片
                                #region HttpServer
                                string reqFilePath = $@"Public/{req.Url}";
                                FileInfo reqFileInfo = new FileInfo(reqFilePath);
                                string reqFileInfoExt = Path.GetExtension(reqFileInfo.FullName);

                                if (reqFileInfoExt == "")
                                {
                                    reqFilePath += ".html";
                                    reqFileInfoExt = ".html";
                                }

                                if (File.Exists(reqFilePath))
                                {
                                    string reqFileContectType = "text/plain";
                                    string reqFileContect = File.ReadAllText(reqFilePath);
                                    byte[] reqFileContectBytes = Encoding.Default.GetBytes(reqFileContect);

                                    // 这里是MIME类型的switch
                                    switch (reqFileInfoExt)
                                    {
                                        case ".html":
                                        case ".htm":
                                            reqFileContectType = "text/html";
                                            break;
                                        case ".css":
                                            reqFileContectType = "text/css";
                                            break;
                                        case ".js":
                                            reqFileContectType = "application/javascript";
                                            break;
                                        case ".jpg":
                                        case ".png":
                                            reqFileContectType = "image";
                                            reqFileContectBytes = File.ReadAllBytes(reqFilePath);
                                            break;
                                        case ".ico":
                                            reqFileContectType = "image/x-icon";
                                            reqFileContectBytes = File.ReadAllBytes(reqFilePath);
                                            break;
                                        default:
                                            break;
                                    }

                                    Post(200, reqFileContectType, reqFileContectBytes, socket, sslStream);
                                    return;
                                }
                                #endregion
                                break;
                        }
                        break;
                    // 当请求是Post时
                    case Method.Post:
                        switch (req.Url)
                        {
                            #region AuthServer
                            case "/authserver/authenticate":
                                response = Authenticate.OnPost(req.PostData);
                                break;
                            case "/authserver/refresh":
                                response = Refresh.OnPost(req.PostData);
                                break;
                            case "/authserver/validate":
                                response = Validate.OnPost(req.PostData);
                                break;
                            case "/authserver/invalidate":
                                response = Invalidate.OnPost(req.PostData);
                                break;
                            case "/authserver/signout":
                                response = Signout.OnPost(req.PostData);
                                break;
                            #endregion

                            #region SessionServer
                            case "/sessionserver/session/minecraft/join":
                                IPEndPoint ipEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                                response = Join.OnPost(req.PostData, ipEndPoint.Address.ToString());
                                break;
                            #endregion

                            #region API
                            case "/api/profiles/minecraft":
                                response = Response.API.Profiles.Minecraft.Root.OnPost(req.PostData);
                                break;
                            #endregion

                            #region Users
                            case "/users/register":
                                response = Register.OnPost(req.PostData);
                                break;
                            case "/users/changepassword":
                                //response = ChangePassword.OnPost(req.PostData);
                                break;
                            #endregion

                            default:
                                break;
                        }
                        break;
                    // 当请求是其他方法时
                    default:
                        response = (405, "", "");
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            Post(response.status, response.type, response.content, socket, sslStream);
        }

        /// <summary>
        /// Post方法（当返回类型是string时）
        /// </summary>
        /// <param name="status">Http状态码</param>
        /// <param name="responseType">返回MIME类型</param>
        /// <param name="response">返回内容</param>
        /// <param name="socket">Socket对象</param>
        /// <param name="sslStream">SslStream对象</param>
        private static void Post(int status, string responseType, string response, Socket socket, SslStream sslStream)
        {
            InternalPost(status, responseType, Encoding.Default.GetBytes(response), socket, sslStream);
        }

        /// <summary>
        /// Post方法（当返回类型是byte[]时）
        /// </summary>
        /// <param name="status">Http状态码</param>
        /// <param name="responseType">返回MIME类型</param>
        /// <param name="response">返回内容</param>
        /// <param name="socket">Socket对象</param>
        /// <param name="sslStream">SslStream对象</param>
        private static void Post(int status, string responseType, byte[] response, Socket socket, SslStream sslStream)
        {
            InternalPost(status, responseType, response, socket, sslStream);
        }

        /// <summary>
        /// 内部Post封装方法
        /// </summary>
        /// <param name="status">Http状态码</param>
        /// <param name="responseType">返回MIME类型</param>
        /// <param name="byteResponse">返回内容</param>
        /// <param name="socket">Socket对象</param>
        /// <param name="sslStream">SslStream对象</param>
        private static void InternalPost(int status, string responseType, byte[] byteResponse, Socket socket, SslStream sslStream)
        {
            string responseHeader = "";
            byte[] byteResponseHeader;

            Dictionary<string, string> header = new Dictionary<string, string>()
            {
                { "Server", "Mimir"},
                { "Author", "Romonov"},
                { "Version", Program.Version},
                { "X-Authlib-Injector-API-Location", "/"}
            };

            responseHeader = HttpProtocol.Build(status, responseType, byteResponse.Length, header);
            byteResponseHeader = Encoding.Default.GetBytes(responseHeader);

            if (Program.IsDebug)
            {
                log.Debug($"Response header: {responseHeader}");
                log.Debug($"Response contect: {Encoding.Default.GetString(byteResponse)}");
            }

            try
            {
                if (Program.SslIsEnable)
                {
                    sslStream.Write(byteResponseHeader);
                    sslStream.Write(byteResponse);
                    sslStream.Flush();
                }
                else
                {
                    socket.Send(byteResponseHeader);
                    socket.Send(byteResponse);
                }
            }
            catch (Exception e)
            {
                if (Program.IsDebug)
                {
                    log.Error(e);
                }
                else
                {
                    log.Error(e.Message);
                }
            }
            finally
            {
                if (Program.SslIsEnable)
                {
                    sslStream.Close();
                }
                socket.Close();
            }
        }
    }
}
