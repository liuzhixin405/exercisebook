using System;
using System.Collections.Generic;
using System.Text;

namespace Trady.Core.Period
{
    public class Per8Hour : IntradayPeriodBase
    {
        public override uint NumberOfSecond => 8 * 60 * 60;

        public override bool IsTimestamp(DateTimeOffset dateTime)
            => dateTime.Hour % 8 == 0 && dateTime.Minute == 0 && dateTime.Second == 0 && dateTime.Millisecond == 0;
    }
}
