using AspNetCore.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AspNetCore.Visitor
{
    public class VisitTest
    {
        public static void Show()
        {
            {
                Expression<Func<People, bool>> lambda = x => x.Age > 5 && x.Id<5 && x.Name.StartsWith("he")&&x.Name.EndsWith("w")&&x.Name.Contains("1");
                ConditionBuilderVisitor visitor = new ConditionBuilderVisitor();
                visitor.Visit(lambda);
                Console.WriteLine(visitor.Condition());
            }
            {
                Expression<Func<int, int, int>> exp = (n, m) => m * n + 10;
                OperationVisitor visitor = new OperationVisitor();
                Expression exnew = visitor.Modify(exp);
            }
            
        }
    }
}
