using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VirtoCommerce.DerivativeContractsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DerivativeContractsModule.Data.Model
{
    public class DerivativeContractItemEntity : AuditableEntity
    {
        [StringLength(128)]
        public string DerivativeContractId { get; set; }

        public virtual DerivativeContractEntity DerivativeContract { get; set; }

        [StringLength(128)]
        public string FulfillmentCenterId { get; set; }

        [Required]
        [StringLength(128)]
        public string ProductId { get; set; }

        [Required]
        public decimal ContractSize { get; set; }

        [Required]
        public decimal PurchasedQuantity { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public virtual decimal RemainingQuantity
        {
            get
            {
                return ContractSize - PurchasedQuantity;
            }
            private set { }
        }


        public virtual DerivativeContractItem ToModel(DerivativeContractItem derivativeContractItem)
        {
            if (derivativeContractItem == null)
                throw new ArgumentNullException(nameof(derivativeContractItem));

            derivativeContractItem.Id = Id;

            derivativeContractItem.CreatedBy = CreatedBy;
            derivativeContractItem.CreatedDate = CreatedDate;
            derivativeContractItem.ModifiedBy = ModifiedBy;
            derivativeContractItem.ModifiedDate = ModifiedDate;

            derivativeContractItem.DerivativeContractId = DerivativeContractId;
            derivativeContractItem.FulfillmentCenterId = FulfillmentCenterId;
            derivativeContractItem.ProductId = ProductId;
            derivativeContractItem.ContractSize = ContractSize;
            derivativeContractItem.PurchasedQuantity = PurchasedQuantity;

            return derivativeContractItem;
        }

        public virtual DerivativeContractItemEntity FromModel(DerivativeContractItem derivativeContractItem, PrimaryKeyResolvingMap pkMap)
        {
            if (derivativeContractItem == null)
                throw new ArgumentNullException(nameof(derivativeContractItem));

            pkMap.AddPair(derivativeContractItem, this);

            Id = derivativeContractItem.Id;

            CreatedBy = derivativeContractItem.CreatedBy;
            CreatedDate = derivativeContractItem.CreatedDate;
            ModifiedBy = derivativeContractItem.ModifiedBy;
            ModifiedDate = derivativeContractItem.ModifiedDate;

            DerivativeContractId = derivativeContractItem.DerivativeContractId;
            FulfillmentCenterId = derivativeContractItem.FulfillmentCenterId;
            ProductId = derivativeContractItem.ProductId;
            ContractSize = derivativeContractItem.ContractSize;
            PurchasedQuantity = derivativeContractItem.PurchasedQuantity;

            return this;
        }

        public virtual void Patch(DerivativeContractItemEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.DerivativeContractId = DerivativeContractId;
            target.FulfillmentCenterId = FulfillmentCenterId;
            target.ProductId = ProductId;
            target.ContractSize = ContractSize;
            target.PurchasedQuantity = PurchasedQuantity;
        }
    }
}
