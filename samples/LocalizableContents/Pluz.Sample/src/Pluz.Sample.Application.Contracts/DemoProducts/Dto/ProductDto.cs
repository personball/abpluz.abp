using System;
using System.ComponentModel.DataAnnotations;
using Abpluz.Abp.LocalizableContents;
using Volo.Abp.Application.Dtos;

namespace Pluz.Sample.DemoProducts.Dto
{
    public class ProductDto : EntityDto<Guid>
    {
        [Required]
        public string ProductCode { get; set; }

        [Required]
        [MaxLength(LocalizableContentConsts.CultureNameMaxLength)]
        public string CultureName { get; set; }

        [Required]
        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}
