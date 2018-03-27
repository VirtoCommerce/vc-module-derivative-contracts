using VirtoCommerce.DerivativesModule.Core.Model;

namespace VirtoCommerce.DerivativesModule.Core.Services
{
    public interface IDerivativeSearchService
    {
        DerivativeSearchResult SearchDerivatives(DerivativeSearchCriteria criteria);
    }
}
