using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using VirtoCommerce.DerivativeContractsModule.Core.Model;
using VirtoCommerce.DerivativeContractsModule.Core.Services;
using VirtoCommerce.DerivativeContractsModule.Data.Extensions;
using VirtoCommerce.DerivativeContractsModule.Data.Model;
using VirtoCommerce.DerivativeContractsModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using PredicateBuilder = VirtoCommerce.Platform.Core.Common.PredicateBuilder;

namespace VirtoCommerce.DerivativeContractsModule.Data.Services
{
    public class DerivativeContractInfoEvaluator : IDerivativeContractInfoEvaluator
    {
        private readonly Func<IDerivativeContractRepository> _repositoryFactory;

        public DerivativeContractInfoEvaluator(Func<IDerivativeContractRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        #region Implementation of IDerivativeContractInfoEvaluator

        public virtual DerivativeContractInfo[] EvaluateDerivativeInfos(DerivativeContractInfoEvaluationContext evaluationContext)
        {
            using (var repository = _repositoryFactory())
            {
                repository.DisableChangesTracking();

                var query = repository.DerivativeContractItems;

                if (!string.IsNullOrEmpty(evaluationContext.MemberId))
                {
                    query = query.Where(dci => evaluationContext.MemberId == dci.DerivativeContract.MemberId);
                }

                if (!evaluationContext.ProductIds.IsNullOrEmpty())
                {
                    query = query.Where(dci => evaluationContext.ProductIds.Contains(dci.ProductId));
                }

                if (!evaluationContext.Types.IsNullOrEmpty())
                {
                    var evaluationContextTypes = evaluationContext.Types.Select(x => x.ToString()).ToArray();
                    query = query.Where(dci => evaluationContextTypes.Contains(dci.DerivativeContract.Type));
                }

                var predicate = GetQueryPredicate(evaluationContext);
                query = query.Where(predicate.Expand());

                if (evaluationContext.OnlyActive)
                {
                    var now = DateTime.Now;
                    query = query.Where(dci => dci.DerivativeContract.IsActive && dci.DerivativeContract.StartDate <= now && (dci.DerivativeContract.EndDate == null || dci.DerivativeContract.EndDate >= now) && dci.RemainingQuantity > 0);
                }

                if (evaluationContext.OnlyRemaining)
                {
                    query = query.Where(dci => dci.DerivativeContract.IsActive && dci.RemainingQuantity > 0);
                }

                var derivativeContractItemIds = query.Select(dci => dci.Id).ToArray();
                var derivativeContractItems = repository.GetDerivativeContractItemsByIds(derivativeContractItemIds);
                var derivativeContractInfos = (evaluationContext.StartDateRanges.IsNullOrEmpty() ? new DateTimeRange[] { null } : evaluationContext.StartDateRanges).SelectMany(startDateRange =>
                    (evaluationContext.EndDateRanges.IsNullOrEmpty() ? new DateTimeRange[] { null } : evaluationContext.EndDateRanges).SelectMany(endDateRange =>
                        derivativeContractItems.Where(GetEvaluationPredicate(startDateRange, endDateRange).Compile()).GroupBy(dci => new { dci.ProductId, dci.DerivativeContract.Type }).Select(dcig =>
                        {
                            var derivativeContractInfo = AbstractTypeFactory<DerivativeContractInfo>.TryCreateInstance();
                            derivativeContractInfo.ProductId = dcig.Key.ProductId;
                            derivativeContractInfo.Type = EnumUtility.SafeParse(dcig.Key.Type, DerivativeContractType.Forward);
                            derivativeContractInfo.StartDateRange = startDateRange;
                            derivativeContractInfo.EndDateRange = endDateRange;
                            derivativeContractInfo.ContractSize = dcig.Sum(dci => dci.ContractSize);
                            derivativeContractInfo.PurchasedQuantity = dcig.Sum(dci => dci.PurchasedQuantity);
                            derivativeContractInfo.RemainingQuantity = dcig.Sum(dci => dci.RemainingQuantity);
                            return derivativeContractInfo;
                        })));

                return derivativeContractInfos.ToArray();
            }
        }

        #endregion
        
        /// <summary>
        /// Used to define extra where clause for derivative contract items search
        /// </summary>
        /// <param name="evaluationContext"></param>
        /// <returns></returns>
        protected virtual Expression<Func<DerivativeContractItemEntity, bool>> GetQueryPredicate(DerivativeContractInfoEvaluationContext evaluationContext)
        {
            if (!evaluationContext.StartDateRanges.IsNullOrEmpty())
            {
                var predicate = PredicateBuilder.False<DerivativeContractItemEntity>();
                predicate = PredicateBuilder.Or(predicate, ((Expression<Func<DerivativeContractItemEntity, DateTime?>>)(dci => dci.DerivativeContract.StartDate)).IsInRanges(evaluationContext.StartDateRanges));
                return predicate.Expand();
            }
            if (!evaluationContext.EndDateRanges.IsNullOrEmpty())
            {
                var predicate = PredicateBuilder.False<DerivativeContractItemEntity>();
                predicate = PredicateBuilder.Or(predicate, ((Expression<Func<DerivativeContractItemEntity, DateTime?>>)(dci => dci.DerivativeContract.EndDate)).IsInRanges(evaluationContext.EndDateRanges));
                return predicate.Expand();
            }
            return PredicateBuilder.True<DerivativeContractItemEntity>();
        }

        /// <summary>
        /// Used to define extra where clause for derivative contract items search
        /// </summary>
        /// <param name="startDateRange"></param>
        /// <param name="endDateRange"></param>
        /// <returns></returns>
        protected virtual Expression<Func<DerivativeContractItemEntity, bool>> GetEvaluationPredicate(DateTimeRange startDateRange, DateTimeRange endDateRange)
        {
            if (startDateRange != null || endDateRange != null)
            {
                var predicate = PredicateBuilder.False<DerivativeContractItemEntity>();
                if (startDateRange != null)
                {
                    predicate = PredicateBuilder.Or(predicate, ((Expression<Func<DerivativeContractItemEntity, DateTime?>>) (dci => dci.DerivativeContract.StartDate)).IsInRange(startDateRange));
                }
                if (endDateRange != null)
                {

                    predicate = PredicateBuilder.Or(predicate, ((Expression<Func<DerivativeContractItemEntity, DateTime?>>) (dci => dci.DerivativeContract.EndDate)).IsInRange(endDateRange));
                }
                return predicate.Expand();
            }
            return PredicateBuilder.True<DerivativeContractItemEntity>();
        }
    }
}