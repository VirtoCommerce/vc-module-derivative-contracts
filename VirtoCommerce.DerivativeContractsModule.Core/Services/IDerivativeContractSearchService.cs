using VirtoCommerce.DerivativeContractsModule.Core.Model;
using VirtoCommerce.Domain.Commerce.Model.Search;

namespace VirtoCommerce.DerivativeContractsModule.Core.Services
{
    public interface IDerivativeContractSearchService
    {
        GenericSearchResult<DerivativeContract> SearchDerivativeContracts(DerivativeContractSearchCriteria criteria);
    }
}
