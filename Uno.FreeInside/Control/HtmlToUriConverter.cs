using Microsoft.UI.Xaml.Data;

namespace Uno.FreeInside.Control;

public class HtmlToUriConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is string htmlContent)
        {
            return new Uri($"data:text/html,{Uri.EscapeDataString(htmlContent)}");
        }
        return null!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
