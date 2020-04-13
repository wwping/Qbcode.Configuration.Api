using Bumblebee;
using Bumblebee.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Qbcode.Configuration.Api.Extends
{
    public class DayCounter<T> : IDayCounter where T : TimeConuter, new()
    {
        public DayCounter(Gateway gateway, string name)
        {
            this.OnCreateItems();
            this.Gateway = gateway;
            this.Name = name;
            this.mTimer = new Timer(new TimerCallback(this.OnTrack), null, 1000, 1000);
        }

        public Gateway Gateway { get; private set; }

        private void OnCreateItems()
        {
            this.Items.Clear();
            for (int i = 0; i < 24; i++)
            {
                T t = Activator.CreateInstance<T>();
                t.Name = string.Format("{0:00}:00", i);
                t.Index = i * 2;
                t.DayCounter = this;
                this.Items.Add(t);
                t = Activator.CreateInstance<T>();
                t.Name = string.Format("{0:00}:30", i);
                t.Index = i * 2 + 1;
                t.DayCounter = this;
                this.Items.Add(t);
            }
            DateTime now = DateTime.Now;
            this.mDay = now.Day;
            int num = now.Hour * 2;
            if (now.Minute >= 30)
            {
                num++;
            }
            this.CurrentItem = this.Items[num];
        }

        protected TimeConuter CurrentItem { get; set; }

        public string Name { get; private set; }

        public void Add(EventRequestIncrementArgs e)
        {
            this.CurrentItem.Add(e);
        }

        public List<TimeConuter> Items { get; private set; } = new List<TimeConuter>();

        public string Path
        {
            get
            {
                string text = AppDomain.CurrentDomain.BaseDirectory + string.Format("day_counters{0}{1}{2}{3}{4}", new object[]
                {
                    System.IO.Path.DirectorySeparatorChar,
                    this.Name,
                    System.IO.Path.DirectorySeparatorChar,
                    DateTime.Now.ToString("yyyyMMdd"),
                    System.IO.Path.DirectorySeparatorChar
                });
                if (!Directory.Exists(text))
                {
                    Directory.CreateDirectory(text);
                }
                return text;
            }
        }

        private void OnTrack(object state)
        {
            this.mTimer.Change(-1, -1);
            try
            {
                DateTime now = DateTime.Now;
                if (now.Day != this.mDay)
                {
                    this.Save();
                    if (this.CurrentItem != null)
                    {
                        this.CurrentItem.SaveUrlDetail();
                    }
                    this.OnCreateItems();
                }
                int num = now.Hour * 2;
                if (now.Minute >= 30)
                {
                    num++;
                }
                TimeConuter timeConuter = this.Items[num];
                if (timeConuter != this.CurrentItem)
                {
                    this.Save();
                    if (this.CurrentItem != null)
                    {
                        this.CurrentItem.SaveUrlDetail();
                    }
                    this.CurrentItem = timeConuter;
                }
            }
            catch (Exception ex)
            {
                if (this.Gateway.HttpServer.EnableLog(BeetleX.EventArgs.LogType.Error))
                {
                    this.Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Error, string.Concat(new string[]
                    {
                        "Gateway ",
                        this.Name,
                        " day counter error ",
                        ex.Message,
                        "@",
                        ex.StackTrace
                    }));
                }
            }
            finally
            {
                this.mTimer.Change(999, 999);
            }
        }

        public object GetTopUrl(string name, int index, int count = 20)
        {
            TimeConuter timeConuter = this.Items.FirstOrDefault((TimeConuter p) => p.Name == name);
            if (timeConuter != null)
            {
                return timeConuter.GetTopUrl(index, count);
            }
            return new object[0];
        }

        public string[] GetXNames()
        {
            return (from a in this.Items
                    select a.Name).ToArray<string>();
        }

        public string[] GetYNames()
        {
            return this.Items[0].GetDisplayName();
        }

        public List<int[]> GetData()
        {
            List<int[]> list = new List<int[]>();
            foreach (TimeConuter timeConuter in this.Items)
            {
                list.Add(timeConuter.Save());
            }
            return list;
        }

        public void Save()
        {
            try
            {
                using (StreamWriter streamWriter = new StreamWriter(this.Path + "total.json", false))
                {
                    string value = JsonConvert.SerializeObject(this.GetData());
                    streamWriter.Write(value);
                    streamWriter.Flush();
                }
            }
            catch (Exception ex)
            {
                if (this.Gateway.HttpServer.EnableLog(BeetleX.EventArgs.LogType.Error))
                {
                    this.Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Error, string.Concat(new string[]
                    {
                        "Gateway ",
                        this.Name,
                        " day counter save error ",
                        ex.Message,
                        "@",
                        ex.StackTrace
                    }));
                }
            }
        }

        public void Load()
        {
            try
            {
                string path = this.Path + "total.json";
                if (File.Exists(path))
                {
                    using (StreamReader streamReader = new StreamReader(path))
                    {
                        string value = streamReader.ReadToEnd();
                        if (!string.IsNullOrEmpty(value))
                        {
                            List<int[]> list = JsonConvert.DeserializeObject<List<int[]>>(value);
                            for (int i = 0; i < list.Count; i++)
                            {
                                this.Items[i].Load(list[i]);
                            }
                        }
                    }
                }
                this.CurrentItem.LoadUrlDetail();
            }
            catch (Exception ex)
            {
                if (this.Gateway.HttpServer.EnableLog(BeetleX.EventArgs.LogType.Error))
                {
                    this.Gateway.HttpServer.Log(BeetleX.EventArgs.LogType.Error, string.Concat(new string[]
                    {
                        "Gateway ",
                        this.Name,
                        " day counter load error ",
                        ex.Message,
                        "@",
                        ex.StackTrace
                    }));
                }
            }
        }

        private Timer mTimer;

        private int mDay = -1;
    }
}
