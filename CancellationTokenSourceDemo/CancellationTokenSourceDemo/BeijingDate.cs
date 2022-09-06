using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CancellationTokenSourceDemo
{
    public class BeijingDate
    {
        private CancellationTokenSource cts;
        private DateTime date;
        public BeijingDate()
        {
            cts = new();
            var timer = new Timer(TimeChange, null, 0, 1000);
        }
        private void TimeChange(object state)
        {
            date = DateTime.Now;
            var old = cts;
            cts = new CancellationTokenSource();
            old.Cancel();
        }

        public DateTime GetDate() => date;

        public CancellationChangeToken GetChangeToken()
        {
            return new CancellationChangeToken(cts.Token);
        }
    }
}
