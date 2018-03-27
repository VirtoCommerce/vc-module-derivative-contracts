using System.Collections.Generic;
using System.Linq;
using VirtoCommerce.DerivativesModule.Data.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DerivativesModule.Data.Repositories
{
    public interface IDerivativeRepository : IRepository
    {
        IQueryable<DerivativeEntity> Derivatives { get; }

        DerivativeEntity[] GetDerivativesByIds(IEnumerable<string> ids);
        void RemoveDerivativesByIds(IEnumerable<string> ids);
    }
}
