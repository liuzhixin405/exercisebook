using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Domain.SeedWork
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class IgnoreMemberAttribute:Attribute
    {
    }
}
