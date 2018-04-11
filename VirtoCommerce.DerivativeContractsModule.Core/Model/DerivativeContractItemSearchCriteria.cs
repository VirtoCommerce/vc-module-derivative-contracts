namespace VirtoCommerce.DerivativeContractsModule.Core.Model
{
    public class DerivativeContractItemSearchCriteria: DerivativeContractSearchCriteriaBase
    {
        public string[] DerivativeContractIds { get; set; }

        public string[] FulfillmentCenterIds { get; set; }

        public string[] ProductIds { get; set; }
    }
}