using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pluz.Sample.DemoProducts
{
    public class DemoProduct : FullAuditedAggregateRoot<Guid>
    {
        public DemoProduct(Guid id) : this()
        {
            Id = id;
        }

        protected DemoProduct()
        {
            Entries = new List<DemoProductLocalizableEntry>();
        }

        public string ProductCode { get; set; }

        public List<DemoProductLocalizableEntry> Entries { get; set; }
    }
}
