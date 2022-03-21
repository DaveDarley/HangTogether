using Xamarin.Forms.Xaml;
using System;
using System.Reflection;
using Xamarin.Forms;

//Comprends pas encore le fonctionnement de cette classe
// But : pouvoir utilise les images locales dans mon xaml
// src:https://www.youtube.com/watch?v=VVpbklb6vDc

namespace HangTogether
{
    [ContentProperty (nameof(Source))]
    public class ImageRessourceExtension : IMarkupExtension
    {

        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }

            var imageSource = ImageSource.FromResource(Source, typeof(ImageRessourceExtension).GetTypeInfo().Assembly);
            return imageSource;
        }
        
    }
}