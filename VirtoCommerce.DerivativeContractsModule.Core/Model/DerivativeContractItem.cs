using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DerivativeContractsModule.Core.Model
{
    public class DerivativeContractItem : AuditableEntity
    {
        public string DerivativeContractId { get; set; }

        public string FulfillmentCenterId { get; set; }
        
        public string ProductId { get; set; }

        public long ContractSize { get; set; }

        public long PurchasedQuantity { get; set; }

        public virtual long RemainingQuantity => ContractSize - PurchasedQuantity;
    }
}
