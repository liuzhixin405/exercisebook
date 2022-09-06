using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrategyPattern03
{
    public class StringBuilderPooledObjectPolicy : PooledObjectPolicy<StringBuilder>
    {
        public int InitialCapacity { get; set; } = 100;
        public int MaximmRetainedCapacity { get; set; } = 1024 * 4;
        public override StringBuilder Create()
        {
            return new StringBuilder(InitialCapacity);
        }

        public override bool Return(StringBuilder obj)
        {
            if (obj.Capacity > MaximmRetainedCapacity)
                return false;
            obj.Clear();
            return true;
        }
    }
}
