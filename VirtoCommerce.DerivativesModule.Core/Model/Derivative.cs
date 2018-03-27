using System;
using System.Collections.Generic;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DerivativesModule.Core.Model
{
    public class Derivative : AuditableEntity
    {
        public string MemberId { get; set; }
        public DerivativeType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; }

        public ICollection<DerivativeItem> Items { get; set; }
    }
}
