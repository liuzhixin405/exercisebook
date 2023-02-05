﻿using System;

namespace Trady.Core.Period
{
    public class PerMinute : IntradayPeriodBase
    {
        public override uint NumberOfSecond => 60;

        public override bool IsTimestamp(DateTimeOffset dateTime)
            => dateTime.Millisecond == 0 && dateTime.Second == 0;
    }
}