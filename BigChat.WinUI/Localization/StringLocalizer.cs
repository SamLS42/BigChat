using Microsoft.Extensions.Localization;
using Windows.ApplicationModel.Resources;

namespace BigChat.Localization;
internal sealed class StringLocalizer(IFormatProvider? provider = null) : IStringLocalizer
{
    private readonly ResourceLoader resourceLoader = ResourceLoader.GetForViewIndependentUse();

    public LocalizedString this[string name]
    {
        get
        {
            string value = resourceLoader.GetString(name);
            bool resourceNotFound = string.IsNullOrEmpty(value);
            return new LocalizedString(name, value, resourceNotFound);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            string format = resourceLoader.GetString(name);
            string value = string.Format(provider, format, arguments);
            bool resourceNotFound = string.IsNullOrEmpty(format);
            return new LocalizedString(name, value, resourceNotFound);
        }
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        throw new NotSupportedException(nameof(GetAllStrings));
    }
}
