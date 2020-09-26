using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Abpluz.Abp.LocalizableContents;
using Pluz.Sample.DemoProducts.Dto;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Localization;

namespace Pluz.Sample.DemoProducts
{
    public class DemoProductAppService : SampleAppService, IDemoProductAppService
    {
        private readonly IRepository<DemoProduct, Guid> _ProductRepository;

        private readonly IDataFilter _dataFilter;

        public DemoProductAppService(
            IRepository<DemoProduct, Guid> productRepository,
            IDataFilter dataFilter)
        {
            _ProductRepository = productRepository;
            _dataFilter = dataFilter;
        }

        /// <summary>
        /// (数据库内容本地化demo)增加本地化语言副本entry 不算create，走update
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task CreateAsync(ProductDto input)
        {
            // 以主表的id为主，增加本地化语言副本entry 不算create，走update
            await _ProductRepository.InsertAsync(new DemoProduct(GuidGenerator.Create())
            {
                ProductCode = input.ProductCode,
                Entries = new List<DemoProductLocalizableEntry>
                {
                   new DemoProductLocalizableEntry
                   {
                       Name=input.Name,
                       Title = input.Title,
                       CultureName = input.CultureName,
                       Description = input.Description
                   }
                }
            });
        }

        /// <summary>
        /// (数据库内容本地化demo) TODO 级联删除？
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(Guid id)
        {
            // TODO cacade?
            await _ProductRepository.DeleteAsync(id);
        }

        /// <summary>
        /// (数据库内容本地化demo) 根据cookie\querystring\header获取指定语言版本 单个对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ProductDto> GetAsync(Guid id)
        {
            var entity = await _ProductRepository.GetAsync(id);
            return new ProductDto
            {
                ProductCode = entity.ProductCode,
                CultureName = entity.Entries.FirstOrDefault()?.CultureName ?? CultureInfo.CurrentCulture.Name,
                Title = entity.Entries.FirstOrDefault()?.Title ?? "",
                Description = entity.Entries.FirstOrDefault()?.Description ?? "",
                Name = entity.Entries.FirstOrDefault()?.Name ?? ""
            }; // TODO autoMap?
        }

        /// <summary>
        /// (数据库内容本地化demo) 获取所有语言副本
        /// </summary>
        /// <returns></returns>
        public async Task<ListResultDto<ProductWithAllEntriesDto>> GetListWithAllEntriesAsync()
        {
            using (_dataFilter.Disable<IHasLocalizableContent>())
            {
                var entities = await AsyncExecuter.ToListAsync(_ProductRepository.WithDetails(s => s.Entries));

                return new ListResultDto<ProductWithAllEntriesDto>
                {
                    Items = entities.Select(e => new ProductWithAllEntriesDto
                    {
                        ProductCode = e.ProductCode,
                        Entries = e.Entries.Select(ee => new ProductLocalizableEntryDto
                        {
                            CultureName = ee.CultureName,
                            Description = ee.Description,
                            Name = ee.Name,
                            Title = ee.Title
                        }).ToList()
                    }).ToList()
                };
            }
        }

        /// <summary>
        /// (数据库内容本地化demo) 根据cookie\querystring\header获取指定语言版本 查询列表
        /// </summary>
        /// <returns></returns>
        public async Task<ListResultDto<ProductDto>> GetListAsync()
        {
            var entities = await AsyncExecuter.ToListAsync(_ProductRepository.WithDetails(s => s.Entries));
            return new ListResultDto<ProductDto>
            {
                Items = entities.Select(e => new ProductDto
                {
                    ProductCode = e.ProductCode,
                    CultureName = e.Entries.FirstOrDefault()?.CultureName ?? CultureInfo.CurrentCulture.Name,
                    Title = e.Entries.FirstOrDefault()?.Title ?? "",
                    Description = e.Entries.FirstOrDefault()?.Description ?? "",
                    Name = e.Entries.FirstOrDefault()?.Name ?? ""
                }).ToList() // TODO IHasCultureEntry autoMap?
            };
        }

        /// <summary>
        /// (数据库内容本地化demo) 更新指定语言版本
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task UpdateAsync(Guid id, ProductDto input)
        {
            using (CultureHelper.Use(input.CultureName))
            {
                var entity = await AsyncExecuter.FirstOrDefaultAsync(_ProductRepository.WithDetails(s => s.Entries).Where(s => s.Id == id));
                entity.ProductCode = input.ProductCode;

                if (!entity.Entries.Any())
                {
                    entity.Entries.Add(new DemoProductLocalizableEntry
                    {
                        CultureName = input.CultureName,
                        Title = input.Title,
                        Description = input.Description,
                        Name = input.Name
                    });
                }
                else
                {
                    entity.Entries.First().Title = input.Title;
                    entity.Entries.First().Description = input.Description;
                    entity.Entries.First().Name = input.Name;
                }
            }
        }
    }
}
