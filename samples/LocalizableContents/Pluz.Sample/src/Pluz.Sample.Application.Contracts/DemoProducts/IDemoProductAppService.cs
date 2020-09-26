using System;
using System.Threading.Tasks;
using Pluz.Sample.DemoProducts.Dto;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Pluz.Sample.DemoProducts
{
    public interface IDemoProductAppService : IApplicationService
    {
        Task CreateAsync(ProductDto input);

        Task UpdateAsync(Guid id, ProductDto input);

        Task DeleteAsync(Guid id);

        Task<ProductDto> GetAsync(Guid id);

        Task<ListResultDto<ProductDto>> GetListAsync();

        Task<ListResultDto<ProductWithAllEntriesDto>> GetListWithAllEntriesAsync();

    }
}
