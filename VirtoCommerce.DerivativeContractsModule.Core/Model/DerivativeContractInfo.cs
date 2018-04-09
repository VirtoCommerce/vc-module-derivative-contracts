using VirtoCommerce.Domain.Common;

namespace VirtoCommerce.DerivativeContractsModule.Core.Model
{
    public class DerivativeContractInfo: ValueObject
    {
        public string ProductId { get; set; }

        public DerivativeContractType Type { get; set; }

        public decimal ContractSize { get; set; }

        public decimal PurchasedQuantity { get; set; }

        public decimal RemainingQuantity { get; set; }
    }
}