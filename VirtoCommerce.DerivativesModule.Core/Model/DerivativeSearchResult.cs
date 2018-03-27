namespace VirtoCommerce.DerivativesModule.Core.Model
{
    public class DerivativeSearchResult
    {
        public Derivative[] Derivatives { get; set; }
        public int TotalDerivativesCount { get; set; }
        public DerivativeItem[] Items { get; set; }
        public int TotalItemsCount { get; set; }
    }
}
