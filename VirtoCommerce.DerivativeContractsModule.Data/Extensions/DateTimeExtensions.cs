using System;
using System.Linq;
using System.Linq.Expressions;
using VirtoCommerce.DerivativeContractsModule.Core.Model;
using PredicateBuilder = VirtoCommerce.Platform.Core.Common.PredicateBuilder;

namespace VirtoCommerce.DerivativeContractsModule.Data.Extensions
{
    public static class DateTimeExtensions
    {
        public static Expression<Func<T, bool>> IsInRanges<T>(this Expression<Func<T, DateTime?>> propertySelector, DateTimeRange[] ranges)
        {
            var expression = IsInRanges(ranges);
            return Expression.Lambda<Func<T, bool>>(expression.Body.Replace(expression.Parameters[0], propertySelector.Body), propertySelector.Parameters);
        }

        public static Expression<Func<DateTime?, bool>> IsInRanges(this DateTimeRange[] ranges)
        {
            var predicate = PredicateBuilder.False<DateTime?>();
            predicate = ranges.Aggregate(predicate, (current, range) => PredicateBuilder.Or(current, IsInRange(range)));
            return LinqKit.Extensions.Expand(predicate);
        }

        public static Expression<Func<T, bool>> IsInRange<T>(this Expression<Func<T, DateTime?>> propertySelector, DateTimeRange range)
        {
            var expression = IsInRange(range);
            return Expression.Lambda<Func<T, bool>>(expression.Body.Replace(expression.Parameters[0], propertySelector.Body), propertySelector.Parameters);
        }

        public static Expression<Func<DateTime?, bool>> IsInRange(this DateTimeRange range)
        {
            var predicate = PredicateBuilder.False<DateTime?>();
            predicate = PredicateBuilder.Or(predicate, date => date == null || (range.FromDate == null || range.IncludeFrom ? range.FromDate <= date : range.FromDate < date) && (range.ToDate == null || range.IncludeTo ? range.ToDate >= date : range.ToDate > date));
            return LinqKit.Extensions.Expand(predicate);
        }
    }
}