using Omu.ValueInjecter;
using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.CoreModule.Data.Model;
using VirtoCommerce.DerivativesModule.Core.Model;
using VirtoCommerce.Domain.Common;
using VirtoCommerce.Platform.Core.Common;
using VirtoCommerce.Platform.Data.Common.ConventionInjections;

namespace VirtoCommerce.DerivativesModule.Data.Model
{
    public class DerivativeItemEntity : AuditableEntity
    {
        [StringLength(128)]
        public string DerivativeId { get; set; }
        public virtual DerivativeEntity Derivative { get; set; }

        [StringLength(128)]
        public string FulfillmentCenterId { get; set; }
        public virtual FulfillmentCenter FulfillmentCenter { get; set; }

        [Required]
        [StringLength(128)]
        public string ProductId { get; set; }

        [Required]
        public decimal ContractSize { get; set; }

        [Required]
        public decimal PurchasedQuantity { get; set; }


        internal DerivativeItem ToModel(DerivativeItem item)
        {
            item.InjectFrom(this);
            return item;
        }

        public DerivativeItemEntity FromModel(DerivativeItem item)
        {
            this.InjectFrom(item);
            return this;
        }

        public void Patch(DerivativeItemEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var patchInjection = new PatchInjection<DerivativeItemEntity>(x => x.FulfillmentCenterId, x => x.ProductId, x => x.ContractSize, x => x.PurchasedQuantity);
            target.InjectFrom(patchInjection, this);
        }

        public override string ToString()
        {
            return $"{DerivativeId}_{FulfillmentCenterId}_{ProductId}_{ContractSize}_{PurchasedQuantity}";
        }
    }
}
