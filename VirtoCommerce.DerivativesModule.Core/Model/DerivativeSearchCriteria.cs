using VirtoCommerce.Domain.Commerce.Model.Search;

namespace VirtoCommerce.DerivativesModule.Core.Model
{
    public class DerivativeSearchCriteria : SearchCriteriaBase
    {
        public string[] MemberIds { get; set; }
        public string[] FulfillmentCenterIds { get; set; }
        public string[] ProductIds { get; set; }
        public DerivativeType[] Types { get; set; }
        public bool OnlyActive { get; set; }
    }
}
