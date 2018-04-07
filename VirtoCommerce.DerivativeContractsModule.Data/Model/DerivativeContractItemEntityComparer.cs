using System.Collections.Generic;

namespace VirtoCommerce.DerivativeContractsModule.Data.Model
{
    public class DerivativeContractItemEntityComparer : IEqualityComparer<DerivativeContractItemEntity>
    {
        public bool Equals(DerivativeContractItemEntity x, DerivativeContractItemEntity y)
        {
            return x != null && y != null &&
                   x.DerivativeContractId == y.DerivativeContractId &&
                   x.FulfillmentCenterId == y.FulfillmentCenterId &&
                   x.ProductId == y.ProductId;
        }

        public int GetHashCode(DerivativeContractItemEntity obj)
        {
            var hashCode = 0;

            // Using prime numbers
            hashCode += 17 * obj.DerivativeContractId?.GetHashCode() ?? 19;
            hashCode += 23 * obj.FulfillmentCenterId?.GetHashCode() ?? 29;
            hashCode += 31 * obj.ProductId?.GetHashCode() ?? 37;

            return hashCode;
        }
    }
}