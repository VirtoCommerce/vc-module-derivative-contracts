using System.Linq.Expressions;

namespace VirtoCommerce.DerivativeContractsModule.Data.Extensions
{
    public static class ExpressionExtensions
    {
        public static Expression Replace(this Expression expression, Expression searchExtension, Expression replaceExtension)
        {
            return new ReplaceVisitor(searchExtension, replaceExtension).Visit(expression);
        }

        private class ReplaceVisitor : ExpressionVisitor
        {
            private readonly Expression _from, _to;

            public ReplaceVisitor(Expression from, Expression to)
            {
                _from = from;
                _to = to;
            }

            public override Expression Visit(Expression node)
            {
                return node == _from ? _to : base.Visit(node);
            }
        }
    }
}