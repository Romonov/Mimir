﻿using Mimir.Response;
using Mimir.Response.AuthServer;
using Mimir.Response.SessionServer.Session.Minecraft;
using Mimir.Response.Users;
using RUL;
using RUL.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mimir.Common
{
    class Router
    {
        private static Logger log = new Logger("Router");

        /// <summary>
        /// 路由并处理请求
        /// </summary>
        /// <param name="httpReq">请求字符串</param>
        /// <param name="socket">Socket实例</param>
        /// <param name="EndPoint">远程终结点</param>
        public static void Route(string httpReq, Socket socket)
        {
            HttpReq req = HttpProtocol.Solve(httpReq);

            log.Info($"Got request {req.Method} {req.Url} from {socket.RemoteEndPoint}.");

            // 这个实现不好
            bool isReqFile = false;
            string reqFilePath = Program.Path + @"/Html" + req.Url;
            byte[] reqFileContectBytes = new byte[0];
            bool reqFileIsImage = false;

            Tuple<int, string, string> Response = new Tuple<int, string, string>(403, "text/plain", "");

            // 处理请求
            try
            {
                if (req.Method == Method.Get)
                {
                    switch (req.Url)
                    {
                        case "/":
                            Response = Root.OnGet();
                            break;

                        #region SessionServer
                        case "/sessionserver/session/minecraft/hasJoined":
                            Response = HasJoined.OnGet(req.Get);
                            break;
                        #endregion

                        default:
                            // Get /sessionserver/session/minecraft/profile/{uuid}?unsigned={unsigned}
                            if (Guid.TryParse(req.Url.Split('/')[6], out Guid guid))
                            {
                                Response = Mimir.Response.SessionServer.Session.Minecraft.Profile.Root.OnGet(req.Get, guid);
                            }
                            else
                            {
                                if (File.Exists(reqFilePath))
                                {
                                    FileInfo reqFile = new FileInfo(reqFilePath);
                                    string reqFileExt = Path.GetExtension(reqFile.FullName);
                                    string reqFileType = "text/plain";
                                    string reqFileContect = File.ReadAllText(reqFilePath);

                                    switch (reqFileExt)
                                    {
                                        case ".html":
                                        case ".htm":
                                            reqFileType = "text/html";
                                            break;
                                        case ".css":
                                            reqFileType = "text/stylesheet";
                                            break;
                                        case ".js":
                                            reqFileType = "application/javascript";
                                            break;
                                        case ".jpg":
                                        case ".png":
                                            reqFileType = "image";
                                            reqFileContectBytes = File.ReadAllBytes(reqFilePath);
                                            reqFileIsImage = true;
                                            break;
                                        case ".ico":
                                            reqFileType = "image/x-icon";
                                            reqFileContectBytes = File.ReadAllBytes(reqFilePath);
                                            reqFileIsImage = true;
                                            break;
                                        default:
                                            break;
                                    }
                                    Response = new Tuple<int, string, string>(200, reqFileType, reqFileContect);
                                }
                            }
                            break;
                    }
                }
                else if (req.Method == Method.Post)
                {
                    switch (req.Url)
                    {
                        #region Users
                        case "/users/register":
                            Response = Register.OnPost(req.PostData);
                            break;
                        case "/users/login":
                            Response = Login.OnPost(req.PostData);
                            break;
                        case "/users/logout":
                            Response = LogOut.OnPost(req.PostData);
                            break;
                        #endregion

                        #region AuthServer
                        case "/authserver/authenticate":
                            Response = Authenticate.OnPost(req.PostData);
                            break;
                        case "/authserver/refresh":
                            Response = Refresh.OnPost(req.PostData);
                            break;
                        case "/authserver/validate":
                            Response = Validate.OnPost(req.PostData);
                            break;
                        case "/authserver/invalidate":
                            Response = Invalidate.OnPost(req.PostData);
                            break;
                        case "/authserver/signout":
                            Response = Signout.OnPost(req.PostData);
                            break;
                        #endregion

                        #region SessionServer
                        case "/sessionserver/session/minecraft/join":
                            IPEndPoint ipEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                            Response = Join.OnPost(req.PostData, ipEndPoint.Address.ToString());
                            break;
                        #endregion

                        #region
                        case "/api/profiles/minecraft":
                            Response = Mimir.Response.API.Profiles.Minecraft.Root.OnPost(req.PostData);
                            break;
                        #endregion
                        default:
                            
                            Response = new Tuple<int, string, string>(403, "text/plain", "");
                            break;
                    }
                }
                else
                {
                    Response = new Tuple<int, string, string>(403, "text/plain", "");
                }
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            if (reqFileIsImage)
            {
                Post(Response.Item1, Response.Item2, reqFileContectBytes, socket);
            }
            else
            {
                Post(Response.Item1, Response.Item2, Response.Item3, socket);
            }
        }

        /// <summary>
        /// 发送返回信息
        /// </summary>
        /// <param name="status">Http状态码</param>
        /// <param name="responseType">返回信息的类型</param>
        /// <param name="response">返回信息的内容</param>
        /// <param name="socket">Socket实例</param>
        private static void Post(int status, string responseType, string response, Socket socket)
        {
            byte[] bcontect;
            string responseHeader = "";
            byte[] bresponse;

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Server", "Mimir");
            header.Add("Author", "Romonov");
            header.Add("Version", Program.Version);
            header.Add("X-Authlib-Injector-API-Location", "/");

            // 发送返回
            bcontect = Encoding.Default.GetBytes(response);
            responseHeader = HttpProtocol.Build(status, responseType, bcontect.Length, header);
            bresponse = Encoding.Default.GetBytes(responseHeader);

            if (Program.IsDebug)
            {
                log.Debug($"Response header: {responseHeader}");
                log.Debug($"Response contect: {response}");
            }

            try
            {
                socket.Send(bresponse);
                socket.Send(bcontect);
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
                socket.Close();
            }
        }

        /// <summary>
        /// 发送返回信息
        /// </summary>
        /// <param name="status">Http状态码</param>
        /// <param name="responseType">返回信息的类型</param>
        /// <param name="response">返回信息的内容</param>
        /// <param name="socket">Socket实例</param>
        private static void Post(int status, string responseType, byte[] response, Socket socket)
        {
            byte[] bcontect;
            string responseHeader = "";
            byte[] bresponse;

            Dictionary<string, string> header = new Dictionary<string, string>();
            header.Add("Server", "Mimir");
            header.Add("Author", "Romonov");
            header.Add("Version", Program.Version);
            header.Add("X-Authlib-Injector-API-Location", "/");

            // 发送返回
            bcontect = response;
            responseHeader = HttpProtocol.Build(status, responseType, bcontect.Length, header);
            bresponse = Encoding.Default.GetBytes(responseHeader);

            if (Program.IsDebug)
            {
                log.Debug($"Response header: {responseHeader}");
                //log.Debug($"Response contect: {Encoding.Default.GetString(response)}");
            }

            try
            {
                socket.Send(bresponse);
                socket.Send(bcontect);
                //socket.Send(bcontect, 0, 100, SocketFlags.None);
                //socket.Send(bcontect, 100, bcontect.Length - 100, SocketFlags.None);
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
                socket.Close();
            }
        }

        /// <summary>
        /// 错误信息返回的自定义类型
        /// </summary>
        public struct ReturnError
        {
            public string error;
            public string errorMessage;
            public string cause;
        }
    }
}
