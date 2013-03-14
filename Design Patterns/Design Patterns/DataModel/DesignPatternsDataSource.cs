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

        public DesignPatternCommon(String uniqueId, String title, String subtitle, String imagePath, String description, String umlImagePath)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
            this._umlImagePath = umlImagePath; 
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

        private ImageSource _umlImage = null;
        private String _umlImagePath = null;
        public ImageSource UMLImage
        {
            get
            {
                if (this._umlImage == null && this._umlImagePath != null)
                {
                    this._umlImage = new BitmapImage(new Uri(DesignPatternCommon._baseUri, this._umlImagePath));
                }

                return this._umlImage;
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public void SetUMLImage(String path)
        {
            this._umlImage = null;
            this._umlImagePath = path;
            this.OnPropertyChanged("UMLImage"); 
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    public class DesignPattern : DesignPatternCommon
    {

        
        public DesignPattern(String uniqueId, String title, String subtitle, String imagePath, String description, String umlImagePath, String content, DesignPatternGroup group)
            : base(uniqueId, title, subtitle, imagePath, description, umlImagePath)
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

        public DesignPatternGroup(String uniqueId, String title, String subtitle, String imagePath, String description, String umlImagePath)
            : base(uniqueId, title, subtitle, imagePath, description, umlImagePath)
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
        public AdItem(String uniqueId, String title, String subtitle, String imagePath, String description, String umlImagePath, String content, DesignPatternGroup group)
            : base(uniqueId, title, subtitle, imagePath, description, umlImagePath, content, group)
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
                "Group description: Design patterns which deal with object creation and help to make a system independent of how its objects are created, composed and represented.",
                "");
            creational.Items.Add(new DesignPattern("Creational-Item-1",
                "Abstract Factory",
                "Declare an interface for creating different types of objects.",
                "Assets/AbstractFactory.png",
                "Description: Provides an interface for creating families of related or dependent objects without specifying their concrete classes.",
                "Assets/AbstractFactoryUML.png",
                "Use an Abstract Factory pattern when:\n\n\tA system should be independent of how its products are created, composed and represented; a system should be configured with one of multiple families of products; a family of related product objects is designed to be used together, and you need to enforce this constraint. You want to provide a class library of products, and you want to reveal just their interfaces, not their implementations.\n\nAdvantages: \n\n\tAbstract factory isolates concrete classes from the client, which helps you control the classes of objects that an application creates. Exchanging product families is easy since the class of a concrete factory appears only once in an application: where it is instantiated. It promotes consistency among products. When prouct objects in a family are designed to work together, it's important that an application use objects from only one family at a time. Abstract Factory makes this easy to enforce.\n\nDisadvantages: \n\n\tSupporting new kinds of products requires extending the abstract interface which implies that all of its derived concrete classes also must change. Extending abstract factories to produce new kinds of Products isn't easy because the Abstract Factory interface fixes the set of product than can be created. Supporting new kinds of products essentially involves changing the Abstract Factory class and all of its subclasses.", 
                creational));

            creational.Items.Add(new DesignPattern("Creational-Item-2",
                "Builder",
                "Separate the construction of a complex object from its representation so that the same construction process can create different representations.",
                "Assets/Builder.png",
                "Description: Separates the construction of a complex object from its representation so that the same construction process can create different representations",
                "Assets/BuilderUML.png",
                "Use a builder pattern when:\n\n\t The algorithm for creating a complex object should be independent of the parts that make up the object and how they're assembled; the construction process must allow different representations for the object that's constructed. \n\nAdvantages:\n\n\tAllows you to vary a product's internal representation by providing the director with an abstract interface for constructing the product. Builder pattern encapsulates code for construction and representation and also provides control over steps for construction process.\n\nDisadvantages: \n\n\tRequires creating a separate ConcreteBuilder for each different type of Product.",
                creational));

            creational.Items.Add(new DesignPattern("Creational-Item-3",
                "Factory Method",
                "Define an interface for creating an object but allows subclasses to decide which class to instantiate.",
                "Assets/FactoryMethod.png",
                "Description: Defines an interface for creating an object but allows subclasses to decide which class to instantiate. Factory method allows a class to defer instantiation to subclasses.",
                "Assets/FactoryMethodUML.jpeg",
                "Use a Factory Method when:\n\n\tA class can't anticipate the class of objects it must create; a class wants its subclasses to specify the objects it creates; classes delegate responsibility to one of several helper subclasses, and you want to localize the knowledge of which helper subclass is the delegate.\n\nAdvantages:\n\n\tFactory methods provide hooks for sub-classes to create different concrete products. Factory methods connect parallel class hierarchies in such a way that it localizes the knowledge of which classes belong together.\n\nDisadvantages:\n\n\tA potential disadvantage of Factory methods is that clients might have to sub-class the creator class in order to create a particular concrete product object. Furthermore, the factory used for creating the objects is bound with the client code making it difficult to use a different factory for creating objects.",
                creational));

            creational.Items.Add(new DesignPattern("Creational-Item-4",
                "Prototype",
                "Specify the kinds of objects to create using a prototypical instance, and create new objects by copying this prototype.",
                "Assets/Prototype.png",
                "Description: Specify the kinds of objects to create using a prototypical instance, and create new objects by copying this prototype.",
                "Assets/PrototypeUML.png",
                "Use a Prototype pattern when:\n\n\tThe classes to instantiate are specified at run-time (i.e., dynamic loading); to avoid building a class hierarchy of factories that parallels the class hierarchy of products; instances of a class can have one of only a few different combinations of state. It may be more convenient to install a corresponding number of prototypes and clone them rather than instantiating the class manually each time with the appropriate state.\n\nAdvantages:\n\n\tAdding and removing products at the same time. Since Prototypes let you incorporate a new concrete product class into a system simply by registering a prototypical instance, it's possible for a client to install and remove prototypes at run-time. Specifying new objects by varying values; this allows users to define new 'classes' without programming. Specifying new objects by varying structure; build objects from parts and subparts. Reduced subclassing due to the ability to clone rather than asking a factory method to create a new object. Configuring an application with class dynamically.\n\nDisadvantages:\n\n\tEach subclass of Prototype must implement the Clone operation, which may be difficult.",
                creational));

            creational.Items.Add(new DesignPattern("Creational-Item-5",
                "Singleton",
                "Ensure a class only has one instance, and provide a global point of access to it.",
                "Assets/Singleton.png",
                "Description: Ensure a class only has one instance, and provide a global point of access to it.",
                "Assets/SingletonUML.png",
                "Use a Singleton pattern when:\n\n\tThere must be exactly one instance of a class and it must be accessible to clients from a well-known access point; when the sole instance should be extensible by subclassing, and clients should be able to use an extended instance without modifying their code.\n\nAdvantages:\n\n\tAmongst the several benefits of the Singleton pattern are controlled access to sole instance. since the Singleton class encapsulates its sole instance, it can have strict control over how and when clients access it. The Singleton pattern is an improvement over global variables and thus is said to have reduced name space. The Singleton class may be subclassed, and it's easy to configure an application with an instance of this extended class, thus it's said to permit refinement of operations and representations.\n\nDisadvantages:\n\n\tSometimes difficult to test. Impossible to work with instance variables.",
                creational)); 
                

            this.AllGroups.Add(creational);

            //var behavioral = new DesignPatternGroup("Behavioral-Group",
            //    "Behavioral",
            //    "Design patterns which identify common communication patterns between objects and realize these patterns.",
            //    "Assets/Behavioral.png",
            //    "Group description: Design patterns which identify common communication patterns between objects and realize these patterns. ");

            //behavioral.Items.Add(new DesignPattern("Behavioral-Item-1", 
            //    "Chain of Responsibility", 
            //    "", 
            //    "Assets/ChainOfResponsibility.png", 
            //    "Description: ", 
            //    ITEM_CONTENT, 
            //    behavioral));

            //behavioral.Items.Add(new DesignPattern("Behavioral-Item-2",
            //    "Command",
            //    "",
            //    "Assets/Command.png",
            //    "Description: ",
            //    ITEM_CONTENT,
            //    behavioral));

            //behavioral.Items.Add(new DesignPattern("Behavioral-Item-3",
            //    "Interpreter",
            //    "",
            //    "Assets/Interpreter.png",
            //    "Description: ",
            //    ITEM_CONTENT,
            //    behavioral)); 

            //behavioral.Items.Add(new DesignPattern("Behavioral-Item-4", 
            //    "Iterator", 
            //    "", 
            //    "Assets/Iterator.png", 
            //    "Description: ",
            //    ITEM_CONTENT, 
            //    behavioral));

            //behavioral.Items.Add(new DesignPattern("Behavioral-Item-5",
            //    "Mediator",
            //    "",
            //    "Assets/Mediator.png",
            //    "Description: ",
            //    ITEM_CONTENT,
            //    behavioral));

            //behavioral.Items.Add(new DesignPattern("Behavioral-Item-6",
            //    "Memento",
            //    "",
            //    "Assets/Memento.png",
            //    "Description: ",
            //    ITEM_CONTENT,
            //    behavioral));

            //behavioral.Items.Add(new DesignPattern("Behavioral-Item-7",
            //    "Observer",
            //    "",
            //    "Assets/Observer.png",
            //    "Description: ",
            //    ITEM_CONTENT,
            //    behavioral));

            //behavioral.Items.Add(new DesignPattern("Behavioral-Item-8",
            //    "State",
            //    "",
            //    "Assets/State.png",
            //    "Description: ",
            //    ITEM_CONTENT,
            //    behavioral));

            //behavioral.Items.Add(new DesignPattern("Behavioral-Item-9",
            //    "Strategy",
            //    "",
            //    "Assets/Strategy.png",
            //    "Description: ",
            //    ITEM_CONTENT, behavioral));

            //behavioral.Items.Add(new DesignPattern("Behavioral-Item-10",
            //    "Template",
            //    "",
            //    "Assets/Template.png",
            //    "Description: ",
            //    ITEM_CONTENT, 
            //    behavioral));

            //behavioral.Items.Add(new DesignPattern("Behavioral-Item-11",
            //    "Visitor",
            //    "",
            //    "Assets/Visitor.png",
            //    "Description: ",
            //    ITEM_CONTENT,
            //    behavioral)); 

            //this.AllGroups.Add(behavioral);

            //var structural = new DesignPatternGroup("Structural-Group",
            //    "Structural",
            //    "Design patterns which ease the design by identifying a simple way to realize relationships between entities.",
            //    "Assets/Structural.png",
            //    "Group description: Design patterns which ease the design by identifying a simple way to realize relationships between entities.");

            //structural.Items.Add(new DesignPattern("Structural-Item-1",
            //    "Adapter",
            //    "",
            //    "Assets/Adapter.png",
            //    "Description: ",
            //    ITEM_CONTENT,
            //    structural));

            //structural.Items.Add(new DesignPattern("Structural-Item-2",
            //    "Bridge",
            //    "",
            //    "Assets/Bridge.png",
            //    "Description: ",
            //    ITEM_CONTENT,
            //    structural));

            //structural.Items.Add(new DesignPattern("Structural-Item-3",
            //    "Decorator",
            //    "",
            //    "Assets/Decorator.png",
            //    "Description: ",
            //    ITEM_CONTENT,
            //    structural));

            //structural.Items.Add(new DesignPattern("Structural-Item-4",
            //    "Facade",
            //    "",
            //    "Assets/Facade.png",
            //    "Description: ",
            //    ITEM_CONTENT,
            //    structural));

            //structural.Items.Add(new DesignPattern("Structural-Item-5",
            //    "Proxy",
            //    "",
            //    "Assets/Proxy.png",
            //    "Description: ",
            //    ITEM_CONTENT,
            //    structural));

            //structural.Items.Add(new DesignPattern("Structural-Item-6",
            //    "Flyweight",
            //    "",
            //    "Assets/Flyweight.png",
            //    "Description: ",
            //    ITEM_CONTENT,
            //    structural));
                

            //this.AllGroups.Add(structural); 
        }
    }
}
