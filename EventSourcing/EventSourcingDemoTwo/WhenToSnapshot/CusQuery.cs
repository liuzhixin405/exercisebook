using System.Linq.Expressions;
using System.Text;

namespace WhenToSnapshot
{
    class CusQuery
    {
        void Main()
        {
            Select(a => new { a.Id, a.Name });
        }
        public class Entity
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public string Select(Expression<Func<Entity, object>> selector)
        {

            var sb = new StringBuilder();
            var visitor = new SqlExpressionVisitor(sb);
            visitor.Visit(selector);
            return sb.ToString();
        }

        public class SqlExpressionVisitor : ExpressionVisitor
        {
            StringBuilder _sb;
            public SqlExpressionVisitor(StringBuilder sb)
            {
                _sb = sb;
            }
            protected override Expression VisitLambda<T>(Expression<T> node)
            {
                _sb.Append("select ");
                //Visit(node.Body);
                var result = base.VisitLambda( node);
                _sb.Append(" from ");
                _sb.Append(node.Parameters.First().Type.Name);
                _sb.Append(";");
                return result;
            }

            protected override Expression VisitNew(NewExpression node)
            {
                foreach (var m in node.Members)
                {

                    _sb.Append(m.Name);
                    _sb.Append(", ");
                }
                _sb.Remove(_sb.Length - 2, 2);
                return base.VisitNew(node);
            }
        }
        // You can define other methods, fields, classes and namespaces here
    }

}
