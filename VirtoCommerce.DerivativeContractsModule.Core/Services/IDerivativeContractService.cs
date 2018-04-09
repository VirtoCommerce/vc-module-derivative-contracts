using VirtoCommerce.DerivativeContractsModule.Core.Model;

namespace VirtoCommerce.DerivativeContractsModule.Core.Services
{
    public interface IDerivativeContractService
    {
        DerivativeContract[] GetDerivativeContractsByIds(string[] ids);

        DerivativeContractItem[] GetDerivativeContractItemsByIds(string[] ids);

        void SaveDerivativeContracts(DerivativeContract[] derivativeContracts);

        void SaveDerivativeContractItems(DerivativeContractItem[] derivativeContractItems);

        void DeleteDerivativeContracts(string[] ids);

        void DeleteDerivativeContractItems(string[] ids);
    }
}
