using System.Collections.Generic;

namespace Pluz.Sample.DemoProducts.Dto
{
    public class ProductWithAllEntriesDto
    {
        public string ProductCode { get; set; }

        public List<ProductLocalizableEntryDto> Entries { get; set; }
    }
}
