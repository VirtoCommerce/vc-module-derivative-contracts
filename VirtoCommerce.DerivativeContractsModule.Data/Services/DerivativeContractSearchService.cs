using System;
using System.Linq;
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

                if (!criteria.FulfillmentCenterIds.IsNullOrEmpty())
                {
                    query = query.Where(dc => dc.Items.Any(dci => criteria.FulfillmentCenterIds.Contains(dci.FulfillmentCenterId)));
                }

                if (!criteria.ProductIds.IsNullOrEmpty())
                {
                    query = query.Where(dc => dc.Items.Any(dci => criteria.ProductIds.Contains(dci.ProductId)));
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

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = "StartDate", SortDirection = SortDirection.Descending } };
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
    }
}
