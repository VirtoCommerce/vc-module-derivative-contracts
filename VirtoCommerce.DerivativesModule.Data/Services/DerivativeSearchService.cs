using System;
using System.Data.Entity;
using System.Linq;
using VirtoCommerce.DerivativesModule.Core.Model;
using VirtoCommerce.DerivativesModule.Core.Services;
using VirtoCommerce.DerivativesModule.Data.Repositories;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;

namespace VirtoCommerce.DerivativesModule.Data.Services
{
    public class DerivativeSearchService : ServiceBase, IDerivativeSearchService
    {
        private readonly Func<IDerivativeRepository> _repositoryFactory;

        public DerivativeSearchService(Func<IDerivativeRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public DerivativeSearchResult SearchDerivatives(DerivativeSearchCriteria criteria)
        {
            Derivative[] results;
            using (var repository = _repositoryFactory())
            {
                var query = repository.Derivatives;

                if (!criteria.MemberIds.IsNullOrEmpty())
                {
                    query = query.Where(x => criteria.MemberIds.Contains(x.MemberId));
                }

                if (!criteria.ObjectIds.IsNullOrEmpty())
                {
                    query = query.Where(x => criteria.ObjectIds.Contains(x.Id));
                }

                if (!criteria.Types.IsNullOrEmpty())
                {
                    var criteriaTypes = criteria.Types.Select(x => x.ToString()).ToArray();
                    query = query.Where(x => criteriaTypes.Contains(x.Type));
                }

                if (criteria.OnlyActive)
                {
                    var now = DateTime.Now;
                    query = query.Where(x => x.IsActive && x.StartDate <= now && (x.EndDate == null || x.EndDate >= now));
                }


                if (!criteria.FulfillmentCenterIds.IsNullOrEmpty())
                {
                    query = query.Where(x => x.Items.Any(y => criteria.FulfillmentCenterIds.Contains(y.FulfillmentCenterId)));
                }

                if (!criteria.ProductIds.IsNullOrEmpty())
                {
                    query = query.Where(x => x.Items.Any(y => criteria.ProductIds.Contains(y.ProductId)));
                }

                var sortInfos = criteria.SortInfos;
                if (sortInfos.IsNullOrEmpty())
                {
                    sortInfos = new[] { new SortInfo { SortColumn = "CreatedDate", SortDirection = SortDirection.Descending } };
                }
                query = query.OrderBySortInfos(sortInfos);

                results = query.Include(x => x.Items)
                            .ToArray()
                            .Select(x => x.ToModel(AbstractTypeFactory<Derivative>.TryCreateInstance()))
                            .ToArray();

            }
            if (criteria.OnlyActive)
            {
                results = results.Where(x => x.Items.Sum(y => y.RemainingQuantity) > 0).ToArray();
            }

            var result = new DerivativeSearchResult();
            var items = results.SelectMany(x => x.Items).ToArray();
            result.TotalItemsCount = items.Length;
            result.Items = items
                .Skip(criteria.Skip)
                .Take(criteria.Take)
                .ToArray();

            result.TotalDerivativesCount = results.Length;
            result.Derivatives = results
                .Skip(criteria.Skip)
                .Take(criteria.Take)
                .ToArray();

            foreach (var d in result.Derivatives)
            {
                d.Items = null;
            }

            return result;
        }
    }
}
