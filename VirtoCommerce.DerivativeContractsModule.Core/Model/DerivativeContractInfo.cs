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
    }
}