using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Infrastructure;
using System;
using System.Linq;
using VirtoCommerce.DerivativeContractsModule.Core.Services;
using VirtoCommerce.DerivativeContractsModule.Core.Model;
using VirtoCommerce.DerivativeContractsModule.Data.Repositories;
using VirtoCommerce.DerivativeContractsModule.Data.Model;

namespace VirtoCommerce.DerivativeContractsModule.Data.Services
{
    public class DerivativeContractService : ServiceBase, IDerivativeContractService
    {
        private readonly Func<IDerivativeContractRepository> _repositoryFactory;

        public DerivativeContractService(Func<IDerivativeContractRepository> repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
        }

        public DerivativeContract[] GetDerivativeContractsByIds(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                return repository.GetDerivativeContractsByIds(ids).Select(x => x.ToModel(AbstractTypeFactory<DerivativeContract>.TryCreateInstance())).ToArray();
            }
        }

        public DerivativeContractItem[] GetDerivativeContractItemsByIds(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                return repository.GetDerivativeContractItemsByIds(ids).Select(x => x.ToModel(AbstractTypeFactory<DerivativeContractItem>.TryCreateInstance())).ToArray();
            }
        }

        public void SaveDerivativeContracts(DerivativeContract[] derivativeContracts)
        {
            if (derivativeContracts == null)
                throw new ArgumentNullException(nameof(derivativeContracts));

            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            {
                using (var changeTracker = GetChangeTracker(repository))
                {
                    var alreadyExistEntities = repository.GetDerivativeContractsByIds(derivativeContracts.Where(m => !m.IsTransient()).Select(x => x.Id).ToArray());
                    foreach (var derivativeContract in derivativeContracts)
                    {
                        var sourceEntity = AbstractTypeFactory<DerivativeContractEntity>.TryCreateInstance().FromModel(derivativeContract, pkMap);
                        var targetEntity = alreadyExistEntities.FirstOrDefault(x => x.Id == sourceEntity.Id);
                        if (targetEntity != null)
                        {
                            changeTracker.Attach(targetEntity);
                            sourceEntity.Patch(targetEntity);
                        }
                        else
                        {
                            repository.Add(sourceEntity);
                        }
                    }

                    CommitChanges(repository);
                    pkMap.ResolvePrimaryKeys();
                }
            }
        }

        public void SaveDerivativeContractItems(DerivativeContractItem[] derivativeContractItems)
        {
            if (derivativeContractItems == null)
                throw new ArgumentNullException(nameof(derivativeContractItems));

            var pkMap = new PrimaryKeyResolvingMap();
            using (var repository = _repositoryFactory())
            {
                using (var changeTracker = GetChangeTracker(repository))
                {
                    var alreadyExistEntities = repository.GetDerivativeContractItemsByIds(derivativeContractItems.Where(m => !m.IsTransient()).Select(x => x.Id).ToArray());
                    foreach (var derivativeContractItem in derivativeContractItems)
                    {
                        var sourceEntity = AbstractTypeFactory<DerivativeContractItemEntity>.TryCreateInstance().FromModel(derivativeContractItem, pkMap);
                        var targetEntity = alreadyExistEntities.FirstOrDefault(x => x.Id == sourceEntity.Id);
                        if (targetEntity != null)
                        {
                            changeTracker.Attach(targetEntity);
                            sourceEntity.Patch(targetEntity);
                        }
                        else
                        {
                            repository.Add(sourceEntity);
                        }
                    }

                    CommitChanges(repository);
                    pkMap.ResolvePrimaryKeys();
                }
            }
        }

        public void DeleteDerivativeContracts(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                repository.RemoveDerivativeContracts(ids);
                CommitChanges(repository);
            }
        }

        public void DeleteDerivativeContractItems(string[] ids)
        {
            using (var repository = _repositoryFactory())
            {
                repository.RemoveDerivativeContractItems(ids);
                CommitChanges(repository);
            }
        }
    }
}
