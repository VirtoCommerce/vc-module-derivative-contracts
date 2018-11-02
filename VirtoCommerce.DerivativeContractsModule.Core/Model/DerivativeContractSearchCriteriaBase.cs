using VirtoCommerce.Domain.Commerce.Model.Search;

namespace VirtoCommerce.DerivativeContractsModule.Core.Model
{
    public abstract class DerivativeContractSearchCriteriaBase : SearchCriteriaBase
    {
        public string[] MemberIds { get; set; }

        public DerivativeContractType[] Types { get; set; }

        public DateTimeRange[] StartDateRanges { get; set; }

        public DateTimeRange[] EndDateRanges { get; set; }

        public bool OnlyActive { get; set; }

        public bool OnlyRemaining { get; set; }
    }
}
