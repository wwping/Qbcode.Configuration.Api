using BeetleX;
using BeetleX.EventArgs;
using BeetleX.FastHttpApi;
using BeetleX.FastHttpApi.Data;
using Qbcode.Configuration.Api.Models;
using Bumblebee.Events;
using Bumblebee.Plugins;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using Qbcode.Configuration.Api.Extends;
using Bumblebee;

namespace Qbcode.Configuration.Api.Controllers
{
    [Controller(BaseUrl = "__system/admin", SkipPublicFilter = true)]
    public class AdminController
    {
        private readonly Gateway gateway;
        private readonly DayCounter<DelayCounter> mDayDelayCounter;
        private readonly DayCounter<StatusCounter> mDayStatusCounter;

        public AdminController(Gateway gateway)
        {
            this.gateway = gateway;
            gateway.RequestIncrement += this.OnRequestIncrement;

            this.mDayStatusCounter = new DayCounter<StatusCounter>(gateway, "day_status_counter");
            this.mDayStatusCounter.Load();
            this.mDayDelayCounter = new DayCounter<DelayCounter>(gateway, "day_delay_counter");
            this.mDayDelayCounter.Load();

        }

        /// <summary>
        /// 接管请求 用于记录请求信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnRequestIncrement(object sender, EventRequestIncrementArgs e)
        {
            this.mDayDelayCounter.Add(e);
            this.mDayStatusCounter.Add(e);
        }

        /// <summary>
        /// 获取系统版本信息
        /// </summary>
        /// <returns></returns>
        public object GetVersions()
        {
            return new
            {
                BeetleX = typeof(ServerHandlerBase).Assembly.GetName().Version.ToString(),
                FastHttpApi = typeof(Header).Assembly.GetName().Version.ToString(),
                Bumblebee = typeof(Gateway).Assembly.GetName().Version.ToString(),
                Configuration = typeof(AdminController).Assembly.GetName().Version.ToString()
            };
        }

        #region 统计

        /// <summary>
        /// 服务状态
        /// </summary>
        /// <returns></returns>
        public object Status()
        {
            StatisticsData data = this.gateway.Statistics.GetData();
            ServerCounter.ServerStatus serverStatus = this.gateway.HttpServer.ServerCounter.Next(false);
            CodeStatisticsData codeStatisticsData = this.gateway.Statistics.ListStatisticsData(311);
            return new
            {
                Status = serverStatus,
                SuccessRps = data._2xx.Rps,
                ErrorRps = data._5xx.Rps,
                Times = data.Times,
                Queue = this.gateway.IOQueue.Count,
                Memory = serverStatus.Memory / 1024L,
                Hit = codeStatisticsData.Rps,
                this.gateway.BufferSize,
            };
        }

        public object GetDayDelay()
        {
            return new
            {
                XName = this.mDayDelayCounter.GetXNames(),
                YName = this.mDayDelayCounter.GetYNames(),
                Data = this.mDayDelayCounter.GetData()
            };
        }

        public object GetDayStatus()
        {
            return new
            {
                XName = this.mDayStatusCounter.GetXNames(),
                YName = this.mDayStatusCounter.GetYNames(),
                Data = this.mDayStatusCounter.GetData()
            };
        }

        public object GetDelayTopUrl(string name, int index, int count = 20)
        {
            return this.mDayDelayCounter.GetTopUrl(name, index, count);
        }

        #endregion

        #region 插件设置


        /// <summary>
        /// 插件列表
        /// </summary>
        /// <returns></returns>
        public object ListPluginInfos()
        {
            return from a in this.gateway.PluginCenter.ListPluginInfos().ToArray<PluginInfo>()
                   orderby a.Name
                   select a;
        }

        /// <summary>
        /// 获取插件信息
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object GetPlugin(string name)
        {
            IPlugin plugin = this.gateway.PluginCenter.GetPlugin(name);
            if (plugin != null)
            {
                object obj = plugin.SaveSetting();
                string setting = (obj == null) ? "" : JsonConvert.SerializeObject(obj);
                return new
                {
                    Info = new PluginInfo(plugin),
                    Setting = setting
                };
            }
            return new object();
        }

        /// <summary>
        /// 设置插件状态
        /// </summary>
        /// <param name="name"></param>
        /// <param name="enabled"></param>
        [Post]
        public void PluginEnabled(string name, bool enabled)
        {
            IPlugin plugin = this.gateway.PluginCenter.GetPlugin(name);
            this.gateway.PluginCenter.SetPluginEnabled(plugin, enabled);
        }

        /// <summary>
        /// 保存插件配置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="setting"></param>
        [Post]
        public void SavePluginSetting(string name, string setting)
        {
            JToken jtoken = (JToken)JsonConvert.DeserializeObject(setting);
            IPlugin plugin = this.gateway.PluginCenter.GetPlugin(name);
            if (plugin != null)
            {
                plugin.LoadSetting(jtoken);
                this.gateway.PluginCenter.SaveSetting(plugin, true);
            }
        }

        #endregion

        #region 服务设置

        /// <summary>
        /// 服务列表
        /// </summary>
        /// <returns></returns>
        public object ListServer()
        {
            return from a in this.gateway.Agents.Servers
                   orderby a.Category
                   select new
                   {
                       Category = a.Category,
                       Remark = a.Remark,
                       Host = a.Uri.ToString(),
                       MaxConnections = a.MaxConnections,
                       WaitQueue = a.WaitQueue,
                       Editor = true,
                       Available = a.Available,
                       Selected = false,
                       Status = new
                       {
                           Count = a.Statistics.All.Count,
                           Rps = a.Statistics.All.GetData().Rps,
                           _2xCount = a.Statistics.Status_2xx.Count,
                           _2xRps = a.Statistics.Status_2xx.GetData().Rps,
                           _5xCount = a.Statistics.Status_5xx.Count,
                           _5xRps = a.Statistics.Status_5xx.GetData().Rps,
                           _4xCount = a.Statistics.Status_4xx.Count,
                           _4xRps = a.Statistics.Status_4xx.GetData().Rps
                       }
                   };
        }

        /// <summary>
        /// 服务请求量
        /// </summary>
        /// <returns></returns>
        public object ServiceStatus()
        {
            return from a in this.gateway.Agents.Servers
                   select new
                   {
                       WaitQueue = a.WaitQueue,
                       Category = a.Category,
                       Host = a.Uri.ToString(),
                       Count = a.Statistics.All.Count,
                       _2xCount = a.Statistics.Status_2xx.Count,
                       _5xCount = a.Statistics.Status_5xx.Count,
                       _4xCount = a.Statistics.Status_4xx.Count,
                   };
        }


        /// <summary>
        /// 删除服务
        /// </summary>
        /// <param name="server"></param>
        [Post]
        public void RemoveServer(string server)
        {
            this.gateway.RemoveServer(server);
            this.gateway.SaveConfig();
        }

        /// <summary>
        /// 分类列表
        /// </summary>
        /// <returns></returns>
        public object ListServerCategories()
        {
            return from a in this.gateway.Agents.Servers
                   group a by a.Category into g
                   select g.Key;
        }

        /// <summary>
        /// 增加修改服务
        /// </summary>
        [Post]
        [JsonDataConvert]
        public void AddServer(AddServer body, IHttpContext context)
        {
            this.gateway.SetServer(body.Host, body.Category, body.Remark, body.MaxConnections);
            this.gateway.SaveConfig();
        }

        #endregion

        #region 网关配置项

        /// <summary>
        /// 获取一些设置
        /// </summary>
        /// <returns></returns>
        public object GetSetting()
        {
            return new
            {
                LogTypes = Enum.GetNames(typeof(LogType)),
                LogCacheSize = this.gateway.HttpServer.Options.CacheLogMaxSize,
                LogConsole = this.gateway.HttpServer.Options.LogToConsole,
                this.gateway.HttpServer.Options.AutoGzip,
                this.gateway.HttpServer.Options.BufferPoolSize,
                this.gateway.HttpServer.Options.BufferSize,
                LogLevel = this.gateway.HttpServer.Options.LogLevel.ToString(),
                this.gateway.HttpServer.Options.MaxConnections,
                this.gateway.HttpServer.Options.MaxWaitQueue,
                this.gateway.HttpServer.Options.MaxBodyLength,
                this.gateway.HttpServer.Options.NoGzipFiles,
                OutputServerAddress = this.gateway.OutputServerAddress,
                StatisticsEnabled = this.gateway.StatisticsEnabled,
                StatisticsExts = this.gateway.GetStatisticsExts(),
            };
        }

        /// <summary>
        /// 保存一些设置
        /// </summary>
        [Post]
        [JsonDataConvert]
        public void SetSetting(SettingModel body, IHttpContext context)
        {
            this.gateway.HttpServer.Options.LogLevel = body.LogLevel;
            this.gateway.HttpServer.BaseServer.Options.LogLevel = body.LogLevel;
            this.gateway.HttpServer.Options.AutoGzip = body.AutoGzip;
            this.gateway.HttpServer.Options.BufferSize = body.BufferSize;
            this.gateway.HttpServer.Options.BufferPoolSize = body.BufferPoolSize;
            this.gateway.HttpServer.Options.CacheLogMaxSize = body.LogCacheSize;
            this.gateway.HttpServer.Options.LogToConsole = body.LogConsole;
            this.gateway.HttpServer.Options.MaxConnections = body.MaxConnections;
            this.gateway.HttpServer.Options.MaxWaitQueue = body.MaxWaitQueue;
            this.gateway.HttpServer.Options.MaxBodyLength = body.MaxBodyLength;
            this.gateway.HttpServer.Options.NoGzipFiles = body.NoGzipFiles;
            this.gateway.HttpServer.SaveOptions();
            this.gateway.OutputServerAddress = body.OutputServerAddress;
            this.gateway.StatisticsEnabled = body.StatisticsEnabled;
            this.gateway.SetStatisticsExts(body.StatisticsExts);
            this.gateway.SaveConfig();
        }

        #endregion

        #region 日志

        /// <summary>
        /// 日志列表
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public object ListLog(int p, int ps)
        {
            LogRecord[] cacheLog = this.gateway.HttpServer.GetCacheLog();
            return new
            {
                PageSize = ps,
                PageIndex = p,
                Count = cacheLog.Length,
                Items = cacheLog.Reverse<LogRecord>().Skip((p - 1) * ps).Take(ps)
            };
        }

        #endregion

        #region 路由

        /// <summary>
        /// 某个路由下的服务
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public object ListRouteServers(string url)
        {
            Bumblebee.Routes.UrlRoute route = this.gateway.Routes.GetRoute(url);
            return from a in route?.Servers
                   select new RouteServersModel
                   {
                       Host = a.Agent.Uri.ToString(),
                       Weight = a.Weight,
                       Available = a.Agent.Available,
                       MaxRps = a.MaxRPS,
                       Standby = a.Standby
                   };
        }

        /// <summary>
        /// 路由列表
        /// </summary>
        /// <returns></returns>
        public object ListRoute()
        {
            List<RoutesModel> list = new List<RoutesModel>();
            list.AddRange(from a in this.gateway.Routes.Urls
                          where a.ApiLoader
                          select new RoutesModel
                          {
                              Url = a.Url,
                              HashPattern = a.HashPattern,
                              Remark = a.Remark,
                              Editor = false,
                              Show = true,
                              TimeOut = a.TimeOut
                          });
            list.Add(new RoutesModel
            {
                Url = this.gateway.Routes.Default.Url,
                HashPattern = this.gateway.Routes.Default.HashPattern,
                Remark = this.gateway.Routes.Default.Remark,
                Editor = false,
                Show = true,
                TimeOut = this.gateway.Routes.Default.TimeOut
            });
            list.Sort((RoutesModel x, RoutesModel y) => x.Url.CompareTo(y.Url));
            foreach (RoutesModel gatewayRouteDTO in list)
            {
                gatewayRouteDTO.Servers = ((IEnumerable<RouteServersModel>)this.ListRouteServers(gatewayRouteDTO.Url)).ToArray<RouteServersModel>();
            }
            return list;
        }

        [Post]
        public void RemoveRoute(string url)
        {
            this.gateway.RemoveRoute(url);
            this.gateway.SaveConfig();
        }

        [Post]
        [JsonDataConvert]
        public void RouteSetting(RoutesModel body)
        {
            RouteServersModel[] array = ((IEnumerable<RouteServersModel>)this.ListRouteServers(body.Url)).ToArray<RouteServersModel>();
            for (int i = 0; i < array.Length; i++)
            {
                RouteServersModel s = array[i];
                if (body.Servers == null || body.Servers.FirstOrDefault((RouteServersModel p) => p.Host == s.Host) == null)
                {
                    this.RemoveRouteServer(body.Url, s.Host);
                }
            }
            var res = this.gateway.SetRoute(body.Url, body.Remark, body.HashPattern);
            res.TimeOut = body.TimeOut;
            if (body.Servers != null)
            {
                foreach (RouteServersModel gatewayRouteServerDTO in body.Servers)
                {
                    Bumblebee.Routes.UrlRoute route = this.gateway.Routes.GetRoute(body.Url);
                    if (route != null)
                    {
                        route.AddServer(gatewayRouteServerDTO.Host, gatewayRouteServerDTO.Weight, gatewayRouteServerDTO.MaxRps, gatewayRouteServerDTO.Standby);
                    }
                }
            }
            this.gateway.SaveConfig();
        }

        [Post]
        [JsonDataConvert]
        public void SetRouteServers(string url, List<RouteServersModel> body)
        {
            if (body != null)
            {
                foreach (RouteServersModel item in body)
                {
                    this.SetRouteServer(url, item.Host, item.Weight, item.MaxRps, item.Standby);
                }
            }
            //return JsonConvert.SerializeObject(body);
        }

        [Post]
        public void SetRouteServer(string url, string server, int weight, int maxRps, bool standby = false)
        {
            new Uri(server);
            Bumblebee.Routes.UrlRoute route = this.gateway.Routes.GetRoute(url);
            if (route != null)
            {
                route.AddServer(server, weight, maxRps, standby);
            }
            this.gateway.SaveConfig();
            this.gateway.Routes.UpdateUrlTable();
        }

        [Post]
        public void RemoveRouteServer(string url, string server)
        {
            Bumblebee.Routes.UrlRoute route = this.gateway.Routes.GetRoute(url);
            if (route != null)
            {
                route.RemoveServer(server);
            }
            this.gateway.SaveConfig();
            this.gateway.Routes.UpdateUrlTable();
        }

        [Post]
        public void SetRoute(string url, string remark, string hashPattern)
        {
            this.gateway.SetRoute(url, remark, hashPattern);
            this.gateway.SaveConfig();
        }

        #endregion


    }
}
