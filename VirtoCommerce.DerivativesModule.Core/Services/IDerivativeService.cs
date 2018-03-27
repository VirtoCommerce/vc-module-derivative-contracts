using System.Collections.Generic;
using VirtoCommerce.DerivativesModule.Core.Model;

namespace VirtoCommerce.DerivativesModule.Core.Services
{
    public interface IDerivativeService
    {
        IEnumerable<Derivative> GetDerivativesByIds(IEnumerable<string> ids);

        void SaveDerivatives(IEnumerable<Derivative> items);

        void DeleteDerivatives(IEnumerable<string> ids);
    }
}
