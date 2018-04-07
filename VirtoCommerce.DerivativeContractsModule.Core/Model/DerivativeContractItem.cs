using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;
using ValueObject = VirtoCommerce.Domain.Common.ValueObject;

namespace VirtoCommerce.DerivativeContractsModule.Core.Model
{
    public class DerivativeContractItem : ValueObject, IAuditable
    {
        #region Implementation of IAuditable

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ModifiedBy { get; set; }

        #endregion

        public string DerivativeContractId { get; set; }

        public string FulfillmentCenterId { get; set; }
        
        public string ProductId { get; set; }

        public decimal ContractSize { get; set; }

        public decimal PurchasedQuantity { get; set; }

        public virtual decimal RemainingQuantity => ContractSize - PurchasedQuantity;

        #region Overrides of ValueObject

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return DerivativeContractId;
            yield return FulfillmentCenterId;
            yield return ProductId;
        }

        #endregion
    }
}
