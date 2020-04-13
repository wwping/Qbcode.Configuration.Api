using Bumblebee;
using Bumblebee.Events;
using Bumblebee.Plugins;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Qbcode.Configuration.Api
{
    [RouteBinder(RouteUrl = "^/favicon\\.ico", ApiLoader = false)]
    public class IconPlugins : IRequestingHandler, IPlugin
    {
        public string Name
        {
            get
            {
                return "qbcode.icon";
            }
        }

        public string Description
        {
            get
            {
                return "展示favicon图标";
            }
        }

        PluginLevel IPlugin.Level => PluginLevel.None;

        public void Execute(EventRequestingArgs e)
        {
            e.Cancel = true;
            e.ResultType = 0;
        }

        public void Init(Gateway gateway, Assembly assembly)
        {
        }


        public void LoadSetting(JToken setting)
        {
        }


        public object SaveSetting()
        {
            return null;
        }
    }
}
