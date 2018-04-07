using System;

namespace VirtoCommerce.DerivativeContractsModule.Core.Model
{
    [Flags]
    public enum DerivativeContractSearchResponseGroup
    {
        WithDerivativeContracts = 1 >> 0,
        WithDerivativeContractItems = 1 >> 1
    }
}