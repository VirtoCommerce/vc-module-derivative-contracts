using VirtoCommerce.Domain.Commerce.Model.Search;

namespace VirtoCommerce.DerivativeContractsModule.Core.Model
{
    public abstract class DerivativeContractSearchCriteriaBase : SearchCriteriaBase
    {
        public string[] MemberIds { get; set; }

        public DerivativeContractType[] Types { get; set; }

        public DateTimeRange StartDateRange { get; set; }

        public DateTimeRange EndDateRange { get; set; }

        public bool OnlyActive { get; set; }
    }
}
