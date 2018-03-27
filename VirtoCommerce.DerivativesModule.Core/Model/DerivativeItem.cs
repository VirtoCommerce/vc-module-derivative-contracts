using System;
using VirtoCommerce.Domain.Common;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DerivativesModule.Core.Model
{
    public class DerivativeItem : ValueObject, IAuditable
    {
        public string DerivativeId { get; set; }
        public string FulfillmentCenterId { get; set; }
        public string ProductId { get; set; }
        public decimal ContractSize { get; set; }
        public decimal PurchasedQuantity { get; set; }
        public virtual decimal RemainingQuantity => ContractSize - PurchasedQuantity;

        #region IAuditable
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        #endregion
    }
}
