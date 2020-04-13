using BeetleX.EventArgs;
using Bumblebee.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Qbcode.Configuration.Api.Extends
{
    public abstract class TimeConuter
    {
        public TimeConuter()
        {
        }

        public List<TimeConuter.ConuterItem> Items { get; private set; } = new List<TimeConuter.ConuterItem>();

        public IDayCounter DayCounter { get; internal set; }

        public int Index { get; set; }

        public string Name { get; set; }

        public int[] GetValues()
        {
            return (from a in this.Items
                    select a.Count).ToArray<int>();
        }

        public string[] GetDisplayName()
        {
            return (from a in this.Items
                    select a.DisplayName).ToArray<string>();
        }

        public int Count
        {
            get
            {
                return this.mCount;
            }
            set
            {
                this.mCount = value;
            }
        }

        public virtual void Add(EventRequestIncrementArgs e)
        {
            Interlocked.Increment(ref this.mCount);
        }

        public int[] Save()
        {
            return (from a in this.Items
                    select a.Count).ToArray<int>();
        }

        public void Load(int[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                this.Items[i].Count = data[i];
                this.Count += data[i];
            }
        }

        private string GetPercent(int value, int count)
        {
            return ((double)((int)((double)value / (double)count * 10000.0)) / 100.0).ToString();
        }

        public object GetTopUrl(int index, int count)
        {
            TimeConuter.ConuterItem item = this.Items[index];
            string path = string.Concat(new string[]
            {
                this.DayCounter.Path,
                this.Name.Replace(":", "_"),
                "_",
                item.Name,
                ".log"
            });
            List<object> list = new List<object>();
            if (File.Exists(path))
            {
                using (StreamReader streamReader = new StreamReader(path))
                {
                    string text = streamReader.ReadLine();
                    while (!string.IsNullOrEmpty(text))
                    {
                        string[] array = text.Split('|', StringSplitOptions.None);
                        list.Add(new
                        {
                            Url = array[0],
                            Count = array[1],
                            Percent = this.GetPercent(int.Parse(array[1]), item.Count)
                        });
                        if (list.Count >= count)
                        {
                            break;
                        }
                        text = streamReader.ReadLine();
                    }
                    return list;
                }
            }
            return (from a in item.Urls.Values
                    orderby a.Count descending
                    select new
                    {
                        Url = a.Url,
                        Count = a.Count,
                        Percent = this.GetPercent(a.Count, item.Count)
                    }).Take(count).ToArray();
        }

        public void SaveUrlDetail()
        {
            foreach (TimeConuter.ConuterItem conuterItem in this.Items)
            {
                try
                {
                    using (StreamWriter streamWriter = new StreamWriter(string.Concat(new string[]
                    {
                        this.DayCounter.Path,
                        this.Name.Replace(":", "_"),
                        "_",
                        conuterItem.Name,
                        ".log"
                    }), false))
                    {
                        conuterItem.Save(streamWriter);
                        streamWriter.Flush();
                        conuterItem.Clear();
                    }
                }
                catch (Exception ex)
                {
                    if (this.DayCounter.Gateway.HttpServer.EnableLog(LogType.Error))
                    {
                        this.DayCounter.Gateway.HttpServer.Log(LogType.Error, string.Concat(new string[]
                        {
                            "Gateway ",
                            this.Name,
                            " day counter ",
                            this.Name,
                            "/",
                            conuterItem.Name,
                            " save url detail error ",
                            ex.Message,
                            "@",
                            ex.StackTrace
                        }));
                    }
                }
            }
        }

        public void LoadUrlDetail()
        {
            foreach (TimeConuter.ConuterItem conuterItem in this.Items)
            {
                try
                {
                    string path = string.Concat(new string[]
                    {
                        this.DayCounter.Path,
                        this.Name.Replace(":", "_"),
                        "_",
                        conuterItem.Name,
                        ".log"
                    });
                    if (File.Exists(path))
                    {
                        using (StreamReader streamReader = new StreamReader(path))
                        {
                            conuterItem.Load(streamReader);
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (this.DayCounter.Gateway.HttpServer.EnableLog(LogType.Error))
                    {
                        this.DayCounter.Gateway.HttpServer.Log(LogType.Error, string.Concat(new string[]
                        {
                            "Gateway ",
                            this.Name,
                            " day counter ",
                            this.Name,
                            "/",
                            conuterItem.Name,
                            " load url detail error ",
                            ex.Message,
                            "@",
                            ex.StackTrace
                        }));
                    }
                }
            }
        }

        private int mCount;

        public class ConuterItem
        {
            public ConuterItem(string name, string displayName = null)
            {
                this.Name = name;
                if (displayName == null)
                {
                    displayName = name;
                }
                this.DisplayName = displayName;
            }

            public string DisplayName { get; set; }

            public ConcurrentDictionary<string, TimeConuter.UrlConuter> Urls { get; private set; } = new ConcurrentDictionary<string, TimeConuter.UrlConuter>(StringComparer.OrdinalIgnoreCase);

            public string Name { get; set; }

            public int Count
            {
                get
                {
                    return this.mCount;
                }
                set
                {
                    this.mCount = value;
                }
            }

            public void Add(EventRequestIncrementArgs e)
            {
                Interlocked.Increment(ref this.mCount);
                TimeConuter.UrlConuter urlConuter = new TimeConuter.UrlConuter
                {
                    Url = e.Request.GetSourceBaseUrl()
                };
                if (!this.Urls.TryAdd(urlConuter.Url, urlConuter))
                {
                    this.Urls.TryGetValue(urlConuter.Url, out urlConuter);
                }
                urlConuter.Add();
            }

            public void Clear()
            {
                this.Urls.Clear();
            }

            public void Save(StreamWriter writer)
            {
                foreach (TimeConuter.UrlConuter urlConuter in from a in this.Urls.Values
                                                              orderby a.Count descending
                                                              select a)
                {
                    writer.WriteLine(string.Format("{0}|{1}", urlConuter.Url, urlConuter.Count));
                }
            }
            public void Load(StreamReader reader)
            {
                string text = reader.ReadLine();
                while (!string.IsNullOrEmpty(text))
                {
                    string[] array = text.Split('|', StringSplitOptions.None);
                    TimeConuter.UrlConuter urlConuter = new TimeConuter.UrlConuter
                    {
                        Url = array[0]
                    };
                    if (!this.Urls.TryAdd(urlConuter.Url, urlConuter))
                    {
                        this.Urls.TryGetValue(urlConuter.Url, out urlConuter);
                    }
                    urlConuter.Count = int.Parse(array[1]);
                    text = reader.ReadLine();
                }
            }

            private int mCount;
        }

        public class UrlConuter
        {
            public string Url { get; set; }

            public int Count
            {
                get
                {
                    return this.mCount;
                }
                set
                {
                    this.mCount = value;
                }
            }

            public void Add()
            {
                Interlocked.Increment(ref this.mCount);
            }

            private int mCount;
        }
    }
}
