using Mimir.Response;
using Mimir.Response.AuthServer;
using Mimir.Response.SessionServer.Session.Minecraft;
using Mimir.Response.Users;
using RUL;
using RUL.Net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Mimir
{
    class Router
    {
        private static Logger log = new Logger("Router");

        public static void Route(string httpReq, Socket socket)
        {
            HttpReq req = HttpProtocol.Solve(httpReq);

            log.Info($"Got request {req.Method} {req.Url} from {socket.RemoteEndPoint}.");

            (int status, string type, string content) response = (403, "text/plain", "");

            try
            {
                switch (req.Method)
                {
                    case Method.Get:
                        switch (req.Url)
                        {
                            #region ExtendAPI
                            case "/":
                                response = Root.OnGet();
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
                                Poster(status, type, content, socket);
                                break;
                            #endregion

                            #region Users
                            case "/users/register":
                                response = Register.OnGet();
                                break;
                            #endregion

                            default:
                                #region SessionServer
                                // Get /sessionserver/session/minecraft/profile/{uuid}?unsigned={unsigned}
                                if (req.Url.Split('/').Length == 5)
                                {
                                    if (Guid.TryParse(req.Url.Split('/')[5], out Guid guid))
                                    {
                                        response = Response.SessionServer.Session.Minecraft.Profile.Root.OnGet(req.Get, guid);
                                    }
                                }
                                # endregion
                                break;
                        }
                        break;
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
                            #endregion

                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            Poster(response.status, response.type, response.content, socket);
        }

        private static void Poster(int status, string responseType, string response, Socket socket)
        {
            Post(status, responseType, Encoding.Default.GetBytes(response), socket);
        }

        private static void Poster(int status, string responseType, byte[] response, Socket socket)
        {
            Post(status, responseType, response, socket);
        }

        private static void Post(int status, string responseType, byte[] byteResponse, Socket socket)
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
                log.Debug($"Response contect: {byteResponse}");
            }

            try
            {
                socket.Send(byteResponseHeader);
                socket.Send(byteResponse);
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
    }
}
