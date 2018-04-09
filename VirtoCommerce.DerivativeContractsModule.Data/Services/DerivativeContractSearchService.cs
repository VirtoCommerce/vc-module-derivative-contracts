using System;
using System.Linq;
using VirtoCommerce.DerivativeContractsModule.Core;
using VirtoCommerce.DerivativeContractsModule.Core.Model;
using VirtoCommerce.DerivativeContractsModule.Core.Services;
using VirtoCommerce.DerivativeContractsModule.Data.Repositories;
using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

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

                if (criteria.StartDateRange?.EarlierDate != null)
                {
                    query = query.Where(dc => criteria.StartDateRange.IncludeEarlier ? criteria.StartDateRange.EarlierDate <= dc.StartDate : criteria.StartDateRange.EarlierDate < dc.StartDate);
                }

                if (criteria.StartDateRange?.LaterDate != null)
                {
                    query = query.Where(dc => criteria.StartDateRange.IncludeLater ? dc.StartDate >= criteria.StartDateRange.LaterDate : dc.StartDate > criteria.StartDateRange.LaterDate);
                }

                if (criteria.EndDateRange?.EarlierDate != null)
                {
                    query = query.Where(dc => criteria.EndDateRange.IncludeEarlier ? criteria.EndDateRange.EarlierDate <= dc.EndDate : criteria.EndDateRange.EarlierDate < dc.EndDate);
                }

                if (criteria.EndDateRange?.LaterDate != null)
                {
                    query = query.Where(dc => criteria.EndDateRange.IncludeLater ? dc.EndDate >= criteria.EndDateRange.LaterDate : dc.EndDate > criteria.EndDateRange.LaterDate);
                }

                if (criteria.OnlyActive)
                {
                    var now = DateTime.Now;
                    query = query.Where(dc => dc.IsActive && dc.StartDate <= now && (dc.EndDate == null || dc.EndDate >= now) && repository.DerivativeContractItems.Where(dci => dci.DerivativeContractId == dc.Id).Any(dci => dci.RemainingQuantity > 0));
                }

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

                if (criteria.StartDateRange?.EarlierDate != null)
                {
                    query = query.Where(dci => criteria.StartDateRange.IncludeEarlier ? criteria.StartDateRange.EarlierDate <= dci.DerivativeContract.StartDate : criteria.StartDateRange.EarlierDate < dci.DerivativeContract.StartDate);
                }

                if (criteria.StartDateRange?.LaterDate != null)
                {
                    query = query.Where(dci => criteria.StartDateRange.IncludeLater ? dci.DerivativeContract.StartDate >= criteria.StartDateRange.LaterDate : dci.DerivativeContract.StartDate > criteria.StartDateRange.LaterDate);
                }

                if (criteria.EndDateRange?.EarlierDate != null)
                {
                    query = query.Where(dci => criteria.EndDateRange.IncludeEarlier ? criteria.EndDateRange.EarlierDate <= dci.DerivativeContract.EndDate : criteria.EndDateRange.EarlierDate < dci.DerivativeContract.EndDate);
                }

                if (criteria.EndDateRange?.LaterDate != null)
                {
                    query = query.Where(dci => criteria.EndDateRange.IncludeLater ? dci.DerivativeContract.EndDate >= criteria.EndDateRange.LaterDate : dci.DerivativeContract.EndDate > criteria.EndDateRange.LaterDate);
                }

                if (criteria.OnlyActive)
                {
                    var now = DateTime.Now;
                    query = query.Where(dci => dci.DerivativeContract.IsActive && dci.DerivativeContract.StartDate <= now && (dci.DerivativeContract.EndDate == null || dci.DerivativeContract.EndDate >= now) && dci.RemainingQuantity > 0);
                }

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
    }
}
