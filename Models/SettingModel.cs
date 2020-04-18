using BeetleX.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qbcode.Configuration.Api.Models
{
    public class SettingModel
    {
        public LogType LogLevel { get; set; } = LogType.Error;
        public bool LogConsole { get; set; } = true;
        public int LogCacheSize { get; set; } = 0;
        public bool AutoGzip { get; set; } = false;
        public int BufferPoolSize { get; set; } = 0;
        public int BufferSize { get; set; } = 0;
        public bool OutputServerAddress { get; set; } = false;
        public bool StatisticsEnabled { get; set; } = false;
        public string StatisticsExts { get; set; } = string.Empty;

        public int MaxConnections { get; set; } = 0;
        public int MaxWaitQueue { get; set; } = 0;
        public int MaxBodyLength { get; set; } = 0;
        public int GatewayQueueSize { get; set; } = 0;
        public int AgentMaxConnection { get; set; } = 0;
        public int PoolMaxSize { get; set; } = 0;
        public int MaxStatsUrls { get; set; } = 0;
        public int BufferPoolMaxMemory { get; set; } = 0;

        public bool UseIPv6 { get; set; } = false;

        public string NoGzipFiles { get; set; } = string.Empty;


    }
}
