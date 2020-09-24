using System.ComponentModel.DataAnnotations;
using Volo.Abp.Localization;

namespace Abpluz.Abp.LocalizableContents
{
    /// <summary>
    /// <see cref="CultureName"/> should be valid by <see cref="CultureHelper.IsValidCultureCode(string)"/>
    /// </summary>
    public interface IHasLocalizableContent : IValidatableObject
    {
        string CultureName { get; set; }
    }
}
