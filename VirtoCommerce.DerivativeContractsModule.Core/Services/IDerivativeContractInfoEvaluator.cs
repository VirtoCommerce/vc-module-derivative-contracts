using VirtoCommerce.DerivativeContractsModule.Core.Model;

namespace VirtoCommerce.DerivativeContractsModule.Core.Services
{
    public interface IDerivativeContractInfoEvaluator
    {
        DerivativeContractInfo[] EvaluateDerivativeInfos(DerivativeContractInfoEvaluationContext evaluationContext);
    }
}