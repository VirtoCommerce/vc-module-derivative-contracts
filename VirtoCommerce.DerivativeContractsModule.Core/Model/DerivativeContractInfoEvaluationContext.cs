using VirtoCommerce.Domain.Common;

namespace VirtoCommerce.DerivativeContractsModule.Core.Model
{
    public class DerivativeContractInfoEvaluationContext: EvaluationContextBase
    {
        public string MemberId { get; set; }

        public string[] ProductIds { get; set; }

        public DerivativeContractType[] Types { get; set; }

        public DateTimeRange StartDateRange { get; set; }

        public DateTimeRange EndDateRange { get; set; }

        public bool OnlyActive { get; set; }
    }
}