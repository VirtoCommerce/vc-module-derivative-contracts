using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DerivativeContractsModule.Core.Model
{
    public class DerivativeContract : AuditableEntity
    {
        public string MemberId { get; set; }

        public DerivativeContractType Type { get; set; }
        
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; }

        public ICollection<DerivativeContractItem> Items { get; set; }
    }
}
