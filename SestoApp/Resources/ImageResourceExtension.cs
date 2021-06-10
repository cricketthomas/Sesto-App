using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace SestoApp.Resources
{

    [ContentProperty(nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
                return null;
            var imageSource = ImageSource.FromResource(Source, typeof(ImageResourceExtension));
            return imageSource;
        }
    }
}
