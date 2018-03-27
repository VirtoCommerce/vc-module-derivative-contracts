using VirtoCommerce.Domain.Commerce.Model.Search;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VirtoCommerce.DerivativesModule.Core.Services;
using VirtoCommerce.DerivativesModule.Core.Model;
using VirtoCommerce.DerivativesModule.Data.Repositories;
using VirtoCommerce.DerivativesModule.Data.Model;

namespace VirtoCommerce.DerivativesModule.Data.Services
{
    public class DerivativeService : ServiceBase, IDerivativeService
    {
        private readonly Func<IDerivativeRepository> _repositoryFactory;

        public DerivativeService(Func<IDerivativeRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public IEnumerable<Derivative> GetDerivativesByIds(IEnumerable<string> ids)
        {
            using (var repository = _repositoryFactory())
            {
                return repository.GetDerivativesByIds(ids).Select(x => x.ToModel(AbstractTypeFactory<Derivative>.TryCreateInstance()));
            }
        }

        public void SaveDerivatives(IEnumerable<Derivative> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            using (var repository = _repositoryFactory())
            using (var changeTracker = GetChangeTracker(repository))
            {
                var targetEntities = repository.GetDerivativesByIds(items.Select(x => x.Id).ToArray());
                foreach (var item in items)
                {
                    var modifiedEntity = AbstractTypeFactory<DerivativeEntity>.TryCreateInstance().FromModel(item);
                    var originalEntity = targetEntities.FirstOrDefault(x => x.Id == modifiedEntity.Id);
                    if (originalEntity != null)
                    {
                        changeTracker.Attach(originalEntity);
                        modifiedEntity.Patch(originalEntity);
                    }
                    else
                    {
                        repository.Add(modifiedEntity);
                    }
                }

                CommitChanges(repository);
            }

            ResetCache();
        }

        public void DeleteDerivatives(IEnumerable<string> ids)
        {
            using (var repository = _repositoryFactory())
            {
                repository.RemoveDerivativesByIds(ids);
                CommitChanges(repository);
            }

            ResetCache();
        }

        protected virtual void ResetCache()
        {
            //_cacheManager.ClearRegion(CacheRegionName);
        }
    }
}
