using System.Linq;
using VirtoCommerce.DerivativeContractsModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DerivativeContractsModule.Data.Repositories
{
    public interface IDerivativeContractRepository : IRepository
    {
        IQueryable<DerivativeContractEntity> DerivativeContracts { get; }

        IQueryable<DerivativeContractItemEntity> DerivativeContractItems { get; }

        DerivativeContractEntity[] GetDerivativeContractsByIds(string[] ids);

        DerivativeContractItemEntity[] GetDerivativeContractItemsByIds(string[] ids);

        void RemoveDerivativeContracts(string[] ids);

        void RemoveDerivativeContractItems(string[] ids);
    }
}
