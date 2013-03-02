using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;


namespace Design_Patterns.DataModel
{
    public abstract class DesignPatternCommon : Design_Patterns.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public DesignPatternCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(DesignPatternCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    public class DesignPattern : DesignPatternCommon
    {

        
        public DesignPattern(String uniqueId, String title, String subtitle, String imagePath, String description, String content, DesignPatternGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private DesignPatternGroup _group;
        public DesignPatternGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    public class DesignPatternGroup : DesignPatternCommon
    {

        public DesignPatternGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<DesignPattern> _items = new ObservableCollection<DesignPattern>();
        public ObservableCollection<DesignPattern> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<DesignPattern> _topItem = new ObservableCollection<DesignPattern>();
        public ObservableCollection<DesignPattern> TopItems
        {
            get { return this._topItem; }
        }
    }

    public class AdItem :  DesignPattern
    {
        public AdItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, DesignPatternGroup group)
            : base(uniqueId, title, subtitle, imagePath, description, content, group)
        {
            
        }
    }

    public sealed class DesignPatternsDataSource
    {
        private static DesignPatternsDataSource _datasource = new DesignPatternsDataSource();

        private ObservableCollection<DesignPatternGroup> _allGroups = new ObservableCollection<DesignPatternGroup>();
        public ObservableCollection<DesignPatternGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<DesignPatternGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _datasource.AllGroups;
        }

        public static DesignPatternGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _datasource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static DesignPattern GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _datasource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public DesignPatternsDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat");

            var creational = new DesignPatternGroup("Creational-Group",
                "Creational",
                "Design patterns which deal with object creation.",
                "Assets/Creational.png",
                "Group description: Design patterns which deal with object creation.");
            creational.Items.Add(new DesignPattern("Creational-Item-1",
                "Abstract Factory",
                "",
                "Assets/AbstractFactory.png",
                "Description: Provides an abstract interface for creating families of related or dependent objects without specifying their concrete classes.",
                ITEM_CONTENT, 
                creational));

            creational.Items.Add(new DesignPattern("Creational-Item-2", 
                "Builder", 
                "",
                "Assets/Builder.png", 
                "Description: Separates the construction of a complex object from its representation so that the same construction process can create different representations", 
                ITEM_CONTENT, 
                creational));

            creational.Items.Add(new DesignPattern("Creational-Item-3",
                "Factory Method",
                "",
                "Assets/FactoryMethod.png",
                "Description: Defines an interface for creating an object but allows subclasses to decide which class to instantiate. Factory method allows a class to defer instantiation to subclasses.",
                ITEM_CONTENT,
                creational));

            creational.Items.Add(new DesignPattern("Creational-Item-4",
                "Prototype",
                "",
                "Assets/Prototype.png",
                "Description: ",
                ITEM_CONTENT,
                creational)); 

            creational.Items.Add(new DesignPattern("Creational-Item-5", 
                "Singleton", 
                "", 
                "Assets/Singleton.png", 
                "Description", 
                ITEM_CONTENT, 
                creational)); 
                

            this.AllGroups.Add(creational);

            var behavioral = new DesignPatternGroup("Behavioral-Group",
                "Behavioral",
                "Design patterns which identify common communication patterns between objects and realize these patterns.",
                "Assets/Behavioral.png",
                "Group description: Design patterns which identify common communication patterns between objects and realize these patterns. ");

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-1", 
                "Chain of Responsibility", 
                "", 
                "Assets/ChainOfResponsibility.png", 
                "Description: ", 
                ITEM_CONTENT, 
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-2",
                "Command",
                "",
                "Assets/Command.png",
                "Description: ",
                ITEM_CONTENT,
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-3",
                "Interpreter",
                "",
                "Assets/Interpreter.png",
                "Description: ",
                ITEM_CONTENT,
                behavioral)); 

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-4", 
                "Iterator", 
                "", 
                "Assets/Iterator.png", 
                "Description: ",
                ITEM_CONTENT, 
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-5",
                "Mediator",
                "",
                "Assets/Mediator.png",
                "Description: ",
                ITEM_CONTENT,
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-6",
                "Memento",
                "",
                "Assets/Memento.png",
                "Description: ",
                ITEM_CONTENT,
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-7",
                "Observer",
                "",
                "Assets/Observer.png",
                "Description: ",
                ITEM_CONTENT,
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-8",
                "State",
                "",
                "Assets/State.png",
                "Description: ",
                ITEM_CONTENT,
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-9",
                "Strategy",
                "",
                "Assets/Strategy.png",
                "Description: ",
                ITEM_CONTENT, behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-10",
                "Template",
                "",
                "Assets/Template.png",
                "Description: ",
                ITEM_CONTENT, 
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-11",
                "Visitor",
                "",
                "Assets/Visitor.png",
                "Description: ",
                ITEM_CONTENT,
                behavioral)); 

            this.AllGroups.Add(behavioral);

            var structural = new DesignPatternGroup("Structural-Group",
                "Structural",
                "Design patterns which ease the design by identifying a simple way to realize relationships between entities.",
                "Assets/Structural.png",
                "Group description: Design patterns which ease the design by identifying a simple way to realize relationships between entities.");

            structural.Items.Add(new DesignPattern("Structural-Item-1",
                "Adapter",
                "",
                "Assets/Adapter.png",
                "Description: ",
                ITEM_CONTENT,
                structural));

            structural.Items.Add(new DesignPattern("Structural-Item-2",
                "Bridge",
                "",
                "Assets/Bridge.png",
                "Description: ",
                ITEM_CONTENT,
                structural));

            structural.Items.Add(new DesignPattern("Structural-Item-3",
                "Decorator",
                "",
                "Assets/Decorator.png",
                "Description: ",
                ITEM_CONTENT,
                structural));

            structural.Items.Add(new DesignPattern("Structural-Item-4",
                "Facade",
                "",
                "Assets/Facade.png",
                "Description: ",
                ITEM_CONTENT,
                structural));

            structural.Items.Add(new DesignPattern("Structural-Item-5",
                "Proxy",
                "",
                "Assets/Proxy.png",
                "Description: ",
                ITEM_CONTENT,
                structural));

            structural.Items.Add(new DesignPattern("Structural-Item-6",
                "Flyweight",
                "",
                "Assets/Flyweight.png",
                "Description: ",
                ITEM_CONTENT,
                structural));
                

            this.AllGroups.Add(structural); 
        }
    }
}
