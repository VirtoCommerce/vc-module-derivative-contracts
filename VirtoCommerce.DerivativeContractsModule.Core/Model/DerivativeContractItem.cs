using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DerivativeContractsModule.Core.Model
{
    public class DerivativeContractItem : AuditableEntity
    {
        public string DerivativeContractId { get; set; }

        public string FulfillmentCenterId { get; set; }
        
        public string ProductId { get; set; }

        public decimal ContractSize { get; set; }

        public decimal PurchasedQuantity { get; set; }

        public virtual decimal RemainingQuantity => ContractSize - PurchasedQuantity;
    }
}
