using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Omu.ValueInjecter;
using VirtoCommerce.DerivativeContractsModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DerivativeContractsModule.Data.Model
{
    public class DerivativeContractEntity : AuditableEntity
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

        public ObservableCollection<DerivativeContractItemEntity> Items { get; set; } = new NullCollection<DerivativeContractItemEntity>();

        public virtual DerivativeContract ToModel(DerivativeContract derivativeContract)
        {
            if (derivativeContract == null)
                throw new ArgumentNullException(nameof(derivativeContract));

            derivativeContract.Id = Id;

            derivativeContract.CreatedBy = CreatedBy;
            derivativeContract.CreatedDate = CreatedDate;
            derivativeContract.ModifiedBy = ModifiedBy;
            derivativeContract.ModifiedDate = ModifiedDate;

            derivativeContract.MemberId = MemberId;
            
            derivativeContract.Type = EnumUtility.SafeParse(Type, DerivativeContractType.Forward);
            derivativeContract.StartDate = StartDate;
            derivativeContract.EndDate = EndDate;
            derivativeContract.IsActive = IsActive;

            derivativeContract.Items = Items.Select(i => i.ToModel(AbstractTypeFactory<DerivativeContractItem>.TryCreateInstance())).ToArray();

            return derivativeContract;
        }

        public virtual DerivativeContractEntity FromModel(DerivativeContract derivativeContract, PrimaryKeyResolvingMap pkMap)
        {
            if (derivativeContract == null)
                throw new ArgumentNullException(nameof(derivativeContract));

            pkMap.AddPair(derivativeContract, this);

            Id = derivativeContract.Id;

            CreatedBy = derivativeContract.CreatedBy;
            CreatedDate = derivativeContract.CreatedDate;
            ModifiedBy = derivativeContract.ModifiedBy;
            ModifiedDate = derivativeContract.ModifiedDate;

            MemberId = derivativeContract.MemberId;
            Type = derivativeContract.Type.ToString();
            StartDate = derivativeContract.StartDate;
            EndDate = derivativeContract.EndDate;
            IsActive = derivativeContract.IsActive;

            if (derivativeContract.Items != null)
            {
                Items = new ObservableCollection<DerivativeContractItemEntity>(derivativeContract.Items.Select(i => AbstractTypeFactory<DerivativeContractItemEntity>.TryCreateInstance().FromModel(i)));
            }

            return this;
        }

        public virtual void Patch(DerivativeContractEntity target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            target.MemberId = MemberId;
            target.Type = Type;
            target.StartDate = StartDate;
            target.EndDate = EndDate;
            target.IsActive = IsActive;

            if (!Items.IsNullCollection())
            {
                Items.Patch(target.Items, AbstractTypeFactory<DerivativeContractItemEntityComparer>.TryCreateInstance(), (sourceItem, targetItem) => sourceItem.Patch(targetItem));
            }
        }
    }
}
