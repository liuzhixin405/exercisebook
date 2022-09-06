using AspNetCore.DBExtend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AspNetCore.Visitor
{
    public class ConditionBuilderVisitor: ExpressionVisitor
    {
        private Stack<string> _stringStack = new Stack<string>();

        public string Condition()
        {
            string condition = string.Concat(this._stringStack.ToArray());
            this._stringStack.Clear();
            return condition;
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node == null) throw new Exception("BinaryExpression is null");
            this._stringStack.Push(")");
            base.Visit(node.Right);
            this._stringStack.Push(" " + node.NodeType.ToSqlOperator() + " ");
            base.Visit(node.Left);
            this._stringStack.Push("(");
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node == null) throw new Exception("MemberExpression is null");
            this._stringStack.Push(" [" + node.Member.Name + "] ");
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node == null) throw new Exception("ConstantExpression is null");
            this._stringStack.Push("'" + node.Value + "'");
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression method)
        {
            if (method == null) throw new Exception("MethodCallExpression is null");
            string format;
            switch (method.Method.Name)
            {
                case "StartsWith":
                    format = "({0} LIKE '{1}%')";
                    break;
                case "Contains":
                    format = "({0} LIKE '%{1}%')";
                    break;
                case "EndsWith":
                    format = "({0} LIKE '%{1}')";
                    break;

                default:
                    throw new NotSupportedException(method.NodeType + " is not supported!");
            }
            this.Visit(method.Object);
            this.Visit(method.Arguments[0]);
            string right = this._stringStack.Pop();
            string left = this._stringStack.Pop();
            this._stringStack.Push(string.Format(format, left.Replace("'",""), right.Replace("'", "")));
            return method;
        }
    }
}
