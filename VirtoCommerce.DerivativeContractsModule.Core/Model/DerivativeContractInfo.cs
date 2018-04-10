using System.Collections.Generic;
using VirtoCommerce.Domain.Common;

namespace VirtoCommerce.DerivativeContractsModule.Core.Model
{
    public class DerivativeContractInfo: ValueObject
    {
        public string ProductId { get; set; }

        public DerivativeContractType Type { get; set; }

        public long ContractSize { get; set; }

        public long PurchasedQuantity { get; set; }

        public long RemainingQuantity { get; set; }

        #region Overrides of ValueObject

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return ProductId;
            yield return Type;
        }

        #endregion
    }
}