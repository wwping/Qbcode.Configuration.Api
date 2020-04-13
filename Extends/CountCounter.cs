using Bumblebee;
using Bumblebee.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Qbcode.Configuration.Api.Extends
{
    public class CountCounter : TimeConuter
    {
        public CountCounter(Gateway gateway)
        {
            //gateway.server
        }


        public override void Add(EventRequestIncrementArgs e)
        {
            //this.CurrentItem.Add(e);
        }
    }
}
