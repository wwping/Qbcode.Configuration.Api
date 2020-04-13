using Bumblebee.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qbcode.Configuration.Api.Extends
{
    public class DelayCounter : TimeConuter
    {
        public DelayCounter()
        {
            TimeConuter.ConuterItem item = new TimeConuter.ConuterItem("10", "<10ms");
            base.Items.Add(item);
            item = new TimeConuter.ConuterItem("10_50", "10ms-50ms");
            base.Items.Add(item);
            item = new TimeConuter.ConuterItem("50_100", "50ms-100ms");
            base.Items.Add(item);
            item = new TimeConuter.ConuterItem("100_200", "100ms-200ms");
            base.Items.Add(item);
            item = new TimeConuter.ConuterItem("200_500", "200ms-500ms");
            base.Items.Add(item);
            item = new TimeConuter.ConuterItem("500_1000", "500ms-1s");
            base.Items.Add(item);
            item = new TimeConuter.ConuterItem("1000_5000", "1s-5s");
            base.Items.Add(item);
            item = new TimeConuter.ConuterItem("5000", ">5s");
            base.Items.Add(item);
        }

        public override void Add(EventRequestIncrementArgs e)
        {
            base.Add(e);
            if (e.Time < 10L)
            {
                base.Items[0].Add(e);
                return;
            }
            if (e.Time >= 10L && e.Time < 50L)
            {
                base.Items[1].Add(e);
                return;
            }
            if (e.Time >= 50L && e.Time < 100L)
            {
                base.Items[2].Add(e);
                return;
            }
            if (e.Time >= 100L && e.Time < 200L)
            {
                base.Items[3].Add(e);
                return;
            }
            if (e.Time >= 200L && e.Time < 500L)
            {
                base.Items[4].Add(e);
                return;
            }
            if (e.Time >= 500L && e.Time < 1000L)
            {
                base.Items[5].Add(e);
                return;
            }
            if (e.Time >= 1000L && e.Time < 5000L)
            {
                base.Items[6].Add(e);
                return;
            }
            if (e.Time >= 5000L)
            {
                base.Items[7].Add(e);
            }
        }
    }
}
