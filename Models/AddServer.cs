using System;
using System.Collections.Generic;
using System.Text;

namespace Qbcode.Configuration.Api.Models
{
    public class AddServer
    {
        public string Host { get; set; } = string.Empty;
        public int MaxConnections { get; set; } = 100;
        public string Remark { get; set; } = string.Empty;
        public string Category { get; set; } = "None";
    }
}
