using System.Linq;
using Omu.ValueInjecter;
using System;
using System.ComponentModel.DataAnnotations;
using VirtoCommerce.DerivativesModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;
using System.Collections.Generic;
using VirtoCommerce.Platform.Data.Common.ConventionInjections;

namespace VirtoCommerce.DerivativesModule.Data.Model
{
    public class DerivativeEntity : AuditableEntity
    {
        [Required]
        [StringLength(128)]
        public string MemberId { get; set; }

        [Required]
        [StringLength(32)]
        public string Type { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public ICollection<DerivativeItemEntity> Items { get; set; }

        public virtual DerivativeEntity FromModel(Derivative item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            this.InjectFrom(item);
            Type = item.Type.ToString();
            Items = item.Items?.Select(x => AbstractTypeFactory<DerivativeItemEntity>.TryCreateInstance().FromModel(x)).ToArray();
            return this;
        }

        public virtual Derivative ToModel(Derivative item)
        {
            item.InjectFrom(this);
            item.Items = Items?.Select(x => x.ToModel(AbstractTypeFactory<DerivativeItem>.TryCreateInstance())).ToArray();

            return item;
        }

        public virtual void Patch(DerivativeEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            var patchInjectionPolicy = new PatchInjection<DerivativeEntity>(x => x.MemberId, x => x.Type, x => x.StartDate, x => x.EndDate, x => x.IsActive);
            target.InjectFrom(patchInjectionPolicy, this);

            if (!Items.IsNullCollection())
            {
                var comparer = AnonymousComparer.Create((DerivativeItemEntity x) => $"{x.FulfillmentCenterId}_{x.ProductId}");
                Items.Patch(target.Items, comparer, (sourceItem, targetItem) => sourceItem.Patch(targetItem));
            }
        }
    }
}
