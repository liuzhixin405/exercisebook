using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackGroundMediatR
{
    internal class TimedNotification:INotification
    {
        public TimeOnly Time { get; set; }
        public TimedNotification(TimeOnly time)
        {
            Time = time;
        }
    }
}
