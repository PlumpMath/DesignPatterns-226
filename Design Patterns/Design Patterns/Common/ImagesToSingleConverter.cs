using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Design_Patterns.Data;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;
using Design_Patterns.DataModel;

namespace Design_Patterns.Common
{
    public class ImagesToSingleConverter : IValueConverter
    {
 
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;

            string selectedImage = null;
            if (value.GetType() == typeof(ObservableCollection<DesignPatternCommon>))
            {
                var items = (ObservableCollection<DesignPatternCommon>)value;
                //var images = items.SelectMany(i => i.Images).Where(list => list != null && list.Length > 0).ToList();
                //selectedImage = Helpers.GetRandomImage(images);
                selectedImage = items[0].Image.ToString(); 
            }
            //else
            //{
            //    var images = (ObservableCollection<string>)value;
            //    selectedImage = Helpers.GetRandomImage(images.ToList());
            //}

       

            if (selectedImage == null) return null;

            var img = new BitmapImage(new Uri("ms-appdata:///Local/" + selectedImage));
            img.DecodePixelHeight = 250;
            img.DecodePixelWidth = 250;

            return img;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
