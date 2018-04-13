using VirtoCommerce.Domain.Common;

namespace VirtoCommerce.DerivativeContractsModule.Core.Model
{
    public class DerivativeContractInfoEvaluationContext: EvaluationContextBase
    {
        public string MemberId { get; set; }

        public string[] ProductIds { get; set; }

        public DerivativeContractType[] Types { get; set; }

        public DateTimeRange[] StartDateRanges { get; set; }

        public DateTimeRange[] EndDateRanges { get; set; }

        public bool OnlyActive { get; set; }
    }
}