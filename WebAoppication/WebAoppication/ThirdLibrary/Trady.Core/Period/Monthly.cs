﻿using System;

namespace Trady.Core.Period
{
    public class Monthly : InterdayPeriodBase
    {
        public Monthly() : base()
        {
        }

        public override uint OrderOfTransformation => 30;

        public override bool IsTimestamp(DateTimeOffset dateTime)
            => dateTime.Day == 1 && dateTime.TimeOfDay == new TimeSpan(0, 0, 0);

        protected override DateTimeOffset ComputeTimestampByCorrectedPeriodCount(DateTimeOffset dateTime, int correctedPeriodCount)
            => new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(correctedPeriodCount);

        protected override DateTimeOffset FloorByDay(DateTimeOffset dateTime, bool isPositivePeriodCount)
            => dateTime.AddDays(1);
    }
}