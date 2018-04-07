using VirtoCommerce.Domain.Commerce.Model.Search;

namespace VirtoCommerce.DerivativeContractsModule.Core.Model
{
    public class DerivativeContractSearchCriteria : SearchCriteriaBase
    {
        public string[] MemberIds { get; set; }

        public string[] FulfillmentCenterIds { get; set; }

        public string[] ProductIds { get; set; }

        public DerivativeContractType[] Types { get; set; }

        public bool OnlyActive { get; set; }
    }
}
