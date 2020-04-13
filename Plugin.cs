using BeetleX.FastHttpApi;
using Bumblebee.Events;
using Bumblebee.Plugins;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Bumblebee;
using Qbcode.Configuration.Api.Controllers;

namespace Qbcode.Configuration.Api
{
    /// <summary>
    /// 插件信息和初始化
    /// </summary>
    [RouteBinder(RouteUrl = "^/__system.*", ApiLoader = false)]
    public class Plugin : IRequestingHandler, IPlugin
    {
        
        public string Name => "qbcode.configuration.api";

        public string Description => "网关可视化配置api";

        public PluginLevel Level => PluginLevel.None;

        public bool Enabled { get; set; } = true;

        public void Execute(EventRequestingArgs e)
        {
            e.Cancel = true;
            e.ResultType = ResultType.None;
        }

        public void Init(Gateway gateway, Assembly assembly)
        {
            //注册controller
            this.admin = new AdminController(gateway);
            gateway.HttpServer.ActionFactory.Register(this.admin);
            try
            {
                gateway.HttpServer.ResourceCenter.LoadManifestResource(assembly);
            }
            catch (Exception)
            {

            }
            /*
            gateway.HttpServer.WebSocketConnect = (o, e) =>
            {
            };
            gateway.HttpServer.WebSocketReceive = (o, e) =>
            {
                //Console.WriteLine(e.Frame.Body);
                var freame = e.CreateFrame($"{DateTime.Now}" + e.Frame.Body.ToString());
                e.Response(freame);
            };

            Task.Run(() =>
            {
                while (true)
                {
                    gateway.HttpServer.SendToWebSocket(gateway.HttpServer.CreateDataFrame(new ActionResult
                    {
                        Data = new
                        {
                            type = "status",
                            message = new
                            {
                                type = "status",
                                data = admin.Status()
                            }
                        }
                    }));

                    gateway.HttpServer.SendToWebSocket(gateway.HttpServer.CreateDataFrame(new ActionResult
                    {
                        Data = new
                        {
                            type = "status",
                            message = new
                            {
                                type = "dayStatus",
                                data = admin.GetDayStatus()
                            }
                        }
                    }));

                    gateway.HttpServer.SendToWebSocket(gateway.HttpServer.CreateDataFrame(new ActionResult
                    {
                        Data = new
                        {
                            type = "status",
                            message = new
                            {
                                type = "dayDelay",
                                data = admin.GetDayDelay()
                            }
                        }
                    }));


                    System.Threading.Thread.Sleep(500);
                }
            });
            */
        }

        public void LoadSetting(JToken setting)
        {

        }

        public object SaveSetting()
        {
            return null;
        }



        private AdminController admin;

    }
}
