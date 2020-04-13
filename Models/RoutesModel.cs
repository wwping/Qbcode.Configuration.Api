using System;
using System.Collections.Generic;
using System.Text;

namespace Qbcode.Configuration.Api.Models
{
    public class RoutesModel
    {
        public string Url { get; set; }

        public string HashPattern { get; set; }

        public string Remark { get; set; }

        public bool Editor { get; set; }

        public bool Show { get; set; }
        public long TimeOut { get; set; }

        public RouteServersModel[] Servers { get; set; } = new RouteServersModel[0];
    }

    public class RouteServersModel
    {
        public string Host { get; set; }

        public int Weight { get; set; }

        public bool Available { get; set; }

        public int MaxRps { get; set; }

        public bool Standby { get; set; }
    }
}
