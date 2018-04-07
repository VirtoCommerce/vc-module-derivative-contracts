using VirtoCommerce.DerivativeContractsModule.Core.Model;

namespace VirtoCommerce.DerivativeContractsModule.Core.Services
{
    public interface IDerivativeContractService
    {
        DerivativeContract[] GetDerivativeContractsByIds(string[] ids);

        void SaveDerivativeContracts(DerivativeContract[] derivativeContracts);

        void DeleteDerivativeContracts(string[] ids);
    }
}
