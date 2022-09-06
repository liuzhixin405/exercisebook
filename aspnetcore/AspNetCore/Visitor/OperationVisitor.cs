using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AspNetCore.Visitor
{
    public class OperationVisitor:ExpressionVisitor
    {
        public Expression Modify(Expression expression)
        {
            return this.Visit(expression);
        }

        public override Expression Visit(Expression node)
        {
            Console.WriteLine($"Visit {node.ToString()}");
            return base.Visit(node);
        }

        protected override Expression VisitBinary(BinaryExpression be)
        {
            Expression left = this.Visit( be.Left );
            Expression right = this.Visit(be.Right );
            if(be.NodeType == ExpressionType.Add)
            {
                return Expression.Subtract(left, right);
            }
            else if(be.NodeType == ExpressionType.Multiply)
            {
                return Expression.Divide(left, right);
            }
            return base.VisitBinary(be);
        }

        protected override Expression VisitConstant(ConstantExpression ce)
        {
            return base.VisitConstant(ce);
        }
    }
}
