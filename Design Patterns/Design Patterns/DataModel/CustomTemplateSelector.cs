using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Design_Patterns.DataModel;

namespace Design_Patterns
{
    public class CustomTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NormalTemplate { get; set; }
        public DataTemplate AdvertTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject c)
        {
            if (item is AdItem)
            {
                return AdvertTemplate; 
            }

            return NormalTemplate; 
        }
    }
}
