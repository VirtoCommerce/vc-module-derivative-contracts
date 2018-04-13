using System;
using System.Linq;
using System.Linq.Expressions;
using LinqKit;
using VirtoCommerce.DerivativeContractsModule.Core.Model;
using VirtoCommerce.DerivativeContractsModule.Core.Services;
using VirtoCommerce.DerivativeContractsModule.Data.Extensions;
using VirtoCommerce.DerivativeContractsModule.Data.Model;
using VirtoCommerce.DerivativeContractsModule.Data.Repositories;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using PredicateBuilder = VirtoCommerce.Platform.Core.Common.PredicateBuilder;

namespace VirtoCommerce.DerivativeContractsModule.Data.Services
{
    public class DerivativeContractSearchService : ServiceBase, IDerivativeContractSearchService
    {
        private readonly Func<IDerivativeContractRepository> _repositoryFactory;
        private readonly IDerivativeContractService _derivativeContractService;

        public DerivativeContractSearchService(Func<IDerivativeContractRepository> repositoryFactory, IDerivativeContractService derivativeContractService)
        {
            _repositoryFactory = repositoryFactory;
            _derivativeContractService = derivativeContractService;
        }

        public virtual GenericSearchResult<DerivativeContract> SearchDerivativeContracts(DerivativeContractSearchCriteria criteria)
        {
            using (var repository = _repositoryFactory())
            {
                repository.DisableChangesTracking();

                var query = repository.DerivativeContracts;
                
                if (!criteria.MemberIds.IsNullOrEmpty())
                {
                    query = query.Where(dc => criteria.MemberIds.Contains(dc.MemberId));
                }

                if (!criteria.Types.IsNullOrEmpty())
                {
                    var criteriaTypes = criteria.Types.Select(x => x.ToString()).ToArray();
                    query = query.Where(dc => criteriaTypes.Contains(dc.Type));
                }

                if (criteria.OnlyActive)
                {
                    var now = DateTime.Now;
                    query = query.Where(dc => dc.IsActive && dc.StartDate <= now && (dc.EndDate == null || dc.EndDate >= now) && dc.Items.Any(dci => dci.RemainingQuantity > 0));
                }

                var predicate = GetQueryPredicate(criteria);
                query = query.Where(predicate.Expand());

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[]
                    {
                        new SortInfo { SortColumn = "StartDate", SortDirection = SortDirection.Descending },
                        new SortInfo { SortColumn = "EndDate", SortDirection = SortDirection.Ascending }
                    };
                }
                query = query.OrderBySortInfos(sortInfos);

                var totalCount = query.Count();
                var derivativeContractIds = query.Select(dc => dc.Id).Skip(criteria.Skip).Take(criteria.Take).ToArray();

                return new GenericSearchResult<DerivativeContract>
                {
                    Results = _derivativeContractService.GetDerivativeContractsByIds(derivativeContractIds).OrderBy(dc => dc.Id).ToList(),
                    TotalCount = totalCount
                };
            }
        }

        public virtual GenericSearchResult<DerivativeContractItem> SearchDerivativeContractItems(DerivativeContractItemSearchCriteria criteria)
        {
            using (var repository = _repositoryFactory())
            {
                repository.DisableChangesTracking();

                var query = repository.DerivativeContractItems;

                if (!criteria.DerivativeContractIds.IsNullOrEmpty())
                {
                    query = query.Where(dci => criteria.DerivativeContractIds.Contains(dci.DerivativeContractId));
                }
                
                if (!criteria.MemberIds.IsNullOrEmpty())
                {
                    query = query.Where(dci => criteria.MemberIds.Contains(dci.DerivativeContract.MemberId));
                }

                if (!criteria.FulfillmentCenterIds.IsNullOrEmpty())
                {
                    query = query.Where(dci => criteria.FulfillmentCenterIds.Contains(dci.FulfillmentCenterId));
                }

                if (!criteria.ProductIds.IsNullOrEmpty())
                {
                    query = query.Where(dci => criteria.ProductIds.Contains(dci.ProductId));
                }

                if (!criteria.Types.IsNullOrEmpty())
                {
                    var criteriaTypes = criteria.Types.Select(x => x.ToString()).ToArray();
                    query = query.Where(dci => criteriaTypes.Contains(dci.DerivativeContract.Type));
                }

                if (criteria.OnlyActive)
                {
                    var now = DateTime.Now;
                    query = query.Where(dci => dci.DerivativeContract.IsActive && dci.DerivativeContract.StartDate <= now && (dci.DerivativeContract.EndDate == null || dci.DerivativeContract.EndDate >= now) && dci.RemainingQuantity > 0);
                }

                var predicate = GetQueryPredicate(criteria);
                query = query.Where(predicate.Expand());

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[]
                    {
                        new SortInfo { SortColumn = "DerivativeContract.StartDate", SortDirection = SortDirection.Descending },
                        new SortInfo { SortColumn = "DerivativeContract.EndDate", SortDirection = SortDirection.Ascending }
                    };
                }
                query = query.OrderBySortInfos(sortInfos);

                var totalCount = query.Count();
                var derivativeContractIds = query.Select(dc => dc.Id).Skip(criteria.Skip).Take(criteria.Take).ToArray();

                return new GenericSearchResult<DerivativeContractItem>
                {
                    Results = _derivativeContractService.GetDerivativeContractItemsByIds(derivativeContractIds).OrderBy(dc => dc.Id).ToList(),
                    TotalCount = totalCount
                };
            }
        }

        /// <summary>
        /// Used to define extra where clause for derivative contracts search
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected virtual Expression<Func<DerivativeContractEntity, bool>> GetQueryPredicate(DerivativeContractSearchCriteria criteria)
        {
            if (!criteria.StartDateRanges.IsNullOrEmpty() || !criteria.EndDateRanges.IsNullOrEmpty())
            {
                var predicate = PredicateBuilder.False<DerivativeContractEntity>();
                if (!criteria.StartDateRanges.IsNullOrEmpty())
                {
                    predicate = PredicateBuilder.Or(predicate, ((Expression<Func<DerivativeContractEntity, DateTime?>>) (dci => dci.StartDate)).IsInRanges(criteria.StartDateRanges));
                }

                if (!criteria.EndDateRanges.IsNullOrEmpty())
                {
                    predicate = PredicateBuilder.Or(predicate, ((Expression<Func<DerivativeContractEntity, DateTime?>>) (dci => dci.EndDate)).IsInRanges(criteria.EndDateRanges));
                }
                return predicate.Expand();
            }
            return PredicateBuilder.True<DerivativeContractEntity>();
        }

        /// <summary>
        /// Used to define extra where clause for derivative contract items search
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        protected virtual Expression<Func<DerivativeContractItemEntity, bool>> GetQueryPredicate(DerivativeContractItemSearchCriteria criteria)
        {
            if (!criteria.StartDateRanges.IsNullOrEmpty() || !criteria.EndDateRanges.IsNullOrEmpty())
            {
                var predicate = PredicateBuilder.False<DerivativeContractItemEntity>();
                if (!criteria.StartDateRanges.IsNullOrEmpty())
                {
                    predicate = PredicateBuilder.Or(predicate, ((Expression<Func<DerivativeContractItemEntity, DateTime?>>) (dci => dci.DerivativeContract.StartDate)).IsInRanges(criteria.StartDateRanges));
                }
                if (!criteria.EndDateRanges.IsNullOrEmpty())
                {
                    predicate = PredicateBuilder.Or(predicate, ((Expression<Func<DerivativeContractItemEntity, DateTime?>>) (dci => dci.DerivativeContract.EndDate)).IsInRanges(criteria.EndDateRanges));
                }
                return predicate.Expand();
            }
            return PredicateBuilder.True<DerivativeContractItemEntity>();
        }
    }
}
