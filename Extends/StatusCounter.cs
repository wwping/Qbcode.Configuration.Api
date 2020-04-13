using Bumblebee.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qbcode.Configuration.Api.Extends
{
    public class StatusCounter : TimeConuter
    {
        public StatusCounter()
        {
            TimeConuter.ConuterItem item = new TimeConuter.ConuterItem("All", null);
            base.Items.Add(item);
            item = new TimeConuter.ConuterItem("2xx", null);
            base.Items.Add(item);
            item = new TimeConuter.ConuterItem("Error", null);
            base.Items.Add(item);
            item = new TimeConuter.ConuterItem("Hit", null);
            base.Items.Add(item);
        }

        public override void Add(EventRequestIncrementArgs e)
        {
            base.Add(e);
            base.Items[0].Add(e);
            if (e.Code >= 200 && e.Code <= 300)
            {
                base.Items[1].Add(e);
                return;
            }
            if (e.Code >= 500 && e.Code <= 600)
            {
                base.Items[2].Add(e);
                return;
            }
            if (e.Code == 311)
            {
                base.Items[3].Add(e);
            }
        }
    }
}
