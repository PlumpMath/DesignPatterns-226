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
                "Provides an interface for creating families of related or dependent objects without specifying their concrete classes.",
                "Assets/AbstractFactoryUML.png",
                "Use an Abstract Factory pattern when:\n\n\tA system should be independent of how its products are created, composed and represented; a system should be configured with one of multiple families of products; a family of related product objects is designed to be used together, and you need to enforce this constraint. You want to provide a class library of products, and you want to reveal just their interfaces, not their implementations.\n\nAdvantages: \n\n\tAbstract factory isolates concrete classes from the client, which helps you control the classes of objects that an application creates. Exchanging product families is easy since the class of a concrete factory appears only once in an application: where it is instantiated. It promotes consistency among products. When prouct objects in a family are designed to work together, it's important that an application use objects from only one family at a time. Abstract Factory makes this easy to enforce.\n\nDisadvantages: \n\n\tSupporting new kinds of products requires extending the abstract interface which implies that all of its derived concrete classes also must change. Extending abstract factories to produce new kinds of Products isn't easy because the Abstract Factory interface fixes the set of product than can be created. Supporting new kinds of products essentially involves changing the Abstract Factory class and all of its subclasses.", 
                creational));

            creational.Items.Add(new DesignPattern("Creational-Item-2",
                "Builder",
                "Separate the construction of a complex object from its representation so that the same construction process can create different representations.",
                "Assets/Builder.png",
                "Separates the construction of a complex object from its representation so that the same construction process can create different representations",
                "Assets/BuilderUML.png",
                "Use a builder pattern when:\n\n\t The algorithm for creating a complex object should be independent of the parts that make up the object and how they're assembled; the construction process must allow different representations for the object that's constructed. \n\nAdvantages:\n\n\tAllows you to vary a product's internal representation by providing the director with an abstract interface for constructing the product. Builder pattern encapsulates code for construction and representation and also provides control over steps for construction process.\n\nDisadvantages: \n\n\tRequires creating a separate ConcreteBuilder for each different type of Product.",
                creational));

            creational.Items.Add(new DesignPattern("Creational-Item-3",
                "Factory Method",
                "Define an interface for creating an object but allows subclasses to decide which class to instantiate.",
                "Assets/FactoryMethod.png",
                "Defines an interface for creating an object but allows subclasses to decide which class to instantiate. Factory method allows a class to defer instantiation to subclasses.",
                "Assets/FactoryMethodUML.jpeg",
                "Use a Factory Method when:\n\n\tA class can't anticipate the class of objects it must create; a class wants its subclasses to specify the objects it creates; classes delegate responsibility to one of several helper subclasses, and you want to localize the knowledge of which helper subclass is the delegate.\n\nAdvantages:\n\n\tFactory methods provide hooks for sub-classes to create different concrete products. Factory methods connect parallel class hierarchies in such a way that it localizes the knowledge of which classes belong together.\n\nDisadvantages:\n\n\tA potential disadvantage of Factory methods is that clients might have to sub-class the creator class in order to create a particular concrete product object. Furthermore, the factory used for creating the objects is bound with the client code making it difficult to use a different factory for creating objects.",
                creational));

            creational.Items.Add(new DesignPattern("Creational-Item-4",
                "Prototype",
                "Specify the kinds of objects to create using a prototypical instance, and create new objects by copying this prototype.",
                "Assets/Prototype.png",
                "Specify the kinds of objects to create using a prototypical instance, and create new objects by copying this prototype.",
                "Assets/PrototypeUML.png",
                "Use a Prototype pattern when:\n\n\tThe classes to instantiate are specified at run-time (i.e., dynamic loading); to avoid building a class hierarchy of factories that parallels the class hierarchy of products; instances of a class can have one of only a few different combinations of state. It may be more convenient to install a corresponding number of prototypes and clone them rather than instantiating the class manually each time with the appropriate state.\n\nAdvantages:\n\n\tAdding and removing products at the same time. Since Prototypes let you incorporate a new concrete product class into a system simply by registering a prototypical instance, it's possible for a client to install and remove prototypes at run-time. Specifying new objects by varying values; this allows users to define new 'classes' without programming. Specifying new objects by varying structure; build objects from parts and subparts. Reduced subclassing due to the ability to clone rather than asking a factory method to create a new object. Configuring an application with class dynamically.\n\nDisadvantages:\n\n\tEach subclass of Prototype must implement the Clone operation, which may be difficult.",
                creational));

            creational.Items.Add(new DesignPattern("Creational-Item-5",
                "Singleton",
                "Ensure a class only has one instance, and provide a global point of access to it.",
                "Assets/Singleton.png",
                "Ensure a class only has one instance, and provide a global point of access to it.",
                "Assets/SingletonUML.png",
                "Use a Singleton pattern when:\n\n\tThere must be exactly one instance of a class and it must be accessible to clients from a well-known access point; when the sole instance should be extensible by subclassing, and clients should be able to use an extended instance without modifying their code.\n\nAdvantages:\n\n\tAmongst the several benefits of the Singleton pattern are controlled access to sole instance. since the Singleton class encapsulates its sole instance, it can have strict control over how and when clients access it. The Singleton pattern is an improvement over global variables and thus is said to have reduced name space. The Singleton class may be subclassed, and it's easy to configure an application with an instance of this extended class, thus it's said to permit refinement of operations and representations.\n\nDisadvantages:\n\n\tSometimes difficult to test. Impossible to work with instance variables.",
                creational)); 
                

            this.AllGroups.Add(creational);

            var behavioral = new DesignPatternGroup("Behavioral-Group",
                "Behavioral",
                "Design patterns which identify common communication patterns between objects and realize these patterns.",
                "Assets/Behavioral.png",
                "Group description: Design patterns which identify common communication patterns between objects and realize these patterns. ",
                "");

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-1",
                "Chain of Responsibility",
                "Provides a way of passing a request between a chain of objects.",
                "Assets/ChainOfResponsibility.png",
                "Provides a way of passing a request between a chain of objects.",
                "Assets/ChainOfResponsibilityUML.png",
                "Use a Chain of Responsibility when:\n\n\nThe design needs more than one object to handle a request and the handler is not known prior. The handler should be attained automatically. The Chain of Responsibility pattern is also useful when the design needs to issue a request to one of several objects without explicitly specifying the object.\n\nAdvantages: \n\n\tReduced coupling and added flexibility in assigning responsibilities to objects.\n\nDisadvantages:\n\n\tReceipt is not guaranteed since a request has no explicit receiver.",
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-2",
                "Command",
                "Encapsulate a request as an object, thereby letting you parameterize clients with different requests, queue or log requests, and support undoable operations.",
                "Assets/Command.png",
                "Encapsulate a request as an object, thereby letting you parameterize clients with different requests, queue or log requests, and support undoable operations.",
                "Assets/CommandUML.png",
                "Use a Command pattern when:\n\n\tYou want to parameterize objects by an action to perform; you want to specify, queue, and execute requests at different times; you want to support undo; you want to support logging changes so that they can be reapplied in case of a system crash; you want to structure a system around high-level operations built on primitives operations. \n\n\nAdvantages:\n\n\tDecouples the object that invokes the operation from the one that knows how to perform it. The Command pattern is also said to help your program by making it my flexible and extendible. \n\nDisadvantages:\n\n\tBlah \n\n",
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-3",
                "Interpreter",
                "Given a language, define a representation for its grammar along with an interpreter that uses the representation to interpret sentences in the language.",
                "Assets/Interpreter.png",
                "Given a language, define a representation for its grammar along with an interpreter that uses the representation to interpret sentences in the language.",
                "Assets/Interpreter.jpg",
                "Use an Interpreter pattern when:\n\n\tThere is a language to interpret and you can represent statements in the language as abstract syntax trees. The interpreter pattern works best for simple grammars and when efficiency is not a critical concern. For complex grammars, the class hierarchy for the grammar becomes large. Tools such as parser generators are a better alternative in such cases.\n\n\tThe interpreter pattern is widely used in compilers implemented with object-oriented languages. The interpreter pattern should be reserved for those cases in which you want to think of the class hierarchy as defining a language.\n\n\nAdvantages:\n\n\tIt's easy to change and extend the grammer because th e pattern uses classes to represent rules. Thus, you can use inheritance to change or extend the grammar. Furthermore, implementing a new grammar is easy and can often be automated and adding new ways to interpret existing expressions is also fairly simple. \n\nDisadvantages:\n\n\tComplex grammars are difficult to maintain. The interpreter pattern defines at least one class for every rule in the grammar, so grammars containing many rules quickly become unmanageable. \n\n",
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-4",
                "Iterator",
                "Provide a way to access the elements of an aggregate object sequentially without exposing its underlying representation.",
                "Assets/Iterator.png",
                "Provide a way to access the elements of an aggregate object sequentially without exposing its underlying representation.",
                "Assets/IteratorUML.png",
                "Use an Iterator pattern when:\n\n\tYou want to access the contents of a collection of objects without exposing its internal representation. The Iterator pattern provides a uniform interface for traversing different aggregate structures to support polymorphic iteration and is able to support multiple types of traversals.\n\n\tIterators are common in object-oriented systems and most major class libraries offer iterators of some form.\n\n\nAdvantages:\n\n\tThe Iterator pattern has three important advantages: (1) It supports variations in the traversal of an aggregate allowing them to be traversed in many ways (2) Iterators simplify the aggregate interface (3) More than one traversal can be pending on an aggregate.",
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-5",
                "Mediator",
                "Define an object that encapsulates how a set of objects interact. Mediator promotes loose coupling by keeping objects from referring to each other explicitly, and it lets you vary their interaction independently.",
                "Assets/Mediator.png",
                "Define an object that encapsulates how a set of objects interact. Mediator promotes loose coupling by keeping objects from referring to each other explicitly, and it lets you vary their interaction independently.",
                "Assets/MediatorUML.png",
                "Use a Mediator pattern when:\n\n\tA set of objects communicate in well-defined but complex ways. In such scenarios the resulting interdependencies are often unstructured and difficult to understand. The Mediator pattern is useful when the behavior that's distributed between several classes should be customizable without a lot of subclassing. \n\n\nAdvantages:\n\n\tMediator pattern simplifies object protocols making the relationships between objects easier to understand, maintain and extend. Mediator localizes behavior that would otherwise be distributed among several objects thereby limiting subclassing. A mediator promotes loose coupling between colleagues making it easy to vary and reuse classes independently. ",
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-6",
                "Memento",
                "Without violating encapsulation, capture and externalize an object's internal state so that the object can be restored to this state later.",
                "Assets/Memento.png",
                "Without violating encapsulation, capture and externalize an object's internal state so that the object can be restored to this state later.",
                "Assets/MementoUML.png",
                "Use a Memento pattern when:\n\n\tA snapshot of an object's state must be saved so that it can be restored to that state later and when a direct interface to obtaining the state of an object would expose implementation details which breaks the object's encapsulation.\n\n\nAdvantages:\n\n\tThe Memento pattern provides a way of recording the internal state of an object without violating encapsulation of the object. \n\nDisadvantages:\n\n\tThe Memento pattern is known to be fairly expensive in terms of implementation and memory space. \n\n",
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-7",
                "Observer",
                "Define a one-to-many dependency between objects so that when one object changes state, all its dependents are notifed and updated automatically.",
                "Assets/Observer.png",
                "Define a one-to-many dependency between objects so that when one object changes state, all its dependents are notifed and updated automatically.",
                "Assets/ObserverUML.png",
                "Use an Observer pattern when:\n\n\tAn abstraction has two asepcts with one being dependent on the other or when a change to one object requires changing others and you don't know how many objects need to be changed. Observer pattern is useful when an object should  be able to notify other objects without makign assumptions about who these objects are. That is, Observer pattern promotes loose coupling.\n\n\nAdvantages:\n\n\tObserver pattern allows you to reuse subjects without reusing their observers, and vice versa. Because Subject and Observer are not tightly coupled, they can belong to different layers of abstraction in the system. Furthermore, Observer pattern supports broadcast communication in that notifications may be sent between all interested objects that are subscribed to it.  \n\nDisadvantages:\n\n\tObserver pattern may cause problems when unexpected updates arrive. A seemingly small update to a subject may cause a cascade of updates to its observers. This problem is aggravated by the fact that the simple update protocol provides no details on what changed in the subject, forcing observers to deduce changes for themselves. \n\n",
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-8",
                "State",
                "Allow an object to alter its behavior when its internal state changes. The object will appear to change its class.",
                "Assets/State.png",
                "Allow an object to alter its behavior when its internal state changes. The object will appear to change its class.",
                "Assets/StateUML.png",
                "Use a State pattern when:\n\n\tAn object's behavior depends on its state and it must change its behavior at run-time depending on that state. State pattern is helpful when operations have large, multipart conditional statements that depend on an object's state. State pattern allows you to put each branch of the conditional into a separate class, allowing you to treat the object's state as an object in its own right that can vary independently from other objects.\n\n\nAdvantages:\n\n\tState pattern localizes state-specific behavior and partitions behavior for different states. State pattern puts all behavior associated with a particular state into one object. Because all state-specific code lives in a state subclass, new states and transitions can be added easily by defining new subclasses. \n\nDisadvantages:\n\n\tSince State pattern distributes state-specific behavior for different states across several State subclasses, the number of classes is increased and can be very broad. Large conditional states are undesirable because each path may require its own subclass. \n\n",
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-9",
                "Strategy",
                "Define a family of algorithms, encapsulate each one, and make them interchangeable. Strategy lets the algorithm vary independently from clients that use it.",
                "Assets/Strategy.png",
                "Define a family of algorithms, encapsulate each one, and make them interchangeable. Strategy lets the algorithm vary independently from clients that use it.",
                "Assets/StrategyUML.png",
                "Use a Strategy pattern when:\n\n\tMany related class differ only in their behavior. Strategy pattern provides a way to configure a class with one of many behaviors. Strategy pattern helps when you need different variants of an algorithm or when an algorithm uses data that clients shouldn't know about. \n\n\nAdvantages:\n\n\tStrategy pattern promotes families of related algorithms or behaviors. Inheritance can help factor out common functionality of the algorithms. Strategy pattern could potentially eliminate conditional statements for selecting desired behaviors. Additionally, strategies can provide different implementations of the same behavior such that the client can choose. \n\nDisadvantages:\n\n\tClients must be aware of different strategies and understand how they differ before it can select the appropriate one. Therefore you should use strategy pattern only when the variation in behavior is relevant to clients. Additionally, there is potential for some communication overhead between Strategy and Context. That is, it's likely that some context classes may use some of the algorithms that are implemented for them, or they may use none at all. Strategy pattern is known to cause an increase in the number of objects defined in an application. \n\n", 
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-10",
                "Template Method",
                "Define a skeleton of an algorithm in an operation, deferring some steps to subclasses. Template Method lets subclasses refine certain steps of an algorithm without changing the algorithm's structure.",
                "Assets/Template.png",
                "Define a skeleton of an algorithm in an operation, deferring some steps to subclasses. Template Method lets subclasses refine certain steps of an algorithm without changing the algorithm's structure.",
                "Assets/TemplateUML.png",
                "Use a Template Method when:\n\n\tYou want to implement the invariant parts of an algorithm once and leave it up to subclasses to implement the behavior that can vary. Common behavior among subclasses should  be factored and localized in a common class to avoid code duplication. Finally, Template Method pattern helps to control subclass extensions by allowing you to define a template method that calls 'hook' operations at specific points, thereby permitting extensions only at those points. Hook operations provide default behavior that subclasses can extend if necessary.\n\n\nAdvantages:\n\n\tTemplate methods are a fundamental technique for code reuse and are particularly important in class libraries, because they are the means for factoring out common behavior in library classes. Template methods lead to an inverted control structure that's sometimes referred to as 'the Hollywood principle', that is,'Don't call us, we'll call you'. This is in reference to how a parent class calls the operations of a subclass and not the other way around.",
                behavioral));

            behavioral.Items.Add(new DesignPattern("Behavioral-Item-11",
                "Visitor",
                "Represent an operation to be performed on the elements of an object structure. Visitor lets you define a new operation without changing the classes of the elements on which it operates.",
                "Assets/Visitor.png",
                "Represent an operation to be performed on the elements of an object structure. Visitor lets you define a new operation without changing the classes of the elements on which it operates.",
                "Assets/VisitorUML.png",
                "Use a Visitor pattern when:\n\n\tAn object structure contains many classes of objects with differing interfaces and you want to perform operations on these objects that depend on their concrete classes. Additionally Visitor pattern is useful when you have many distinct and unrelated objects in an object structure, and you want to avoid polluting their classes with these operations.\n\n\nAdvantages:\n\n\tVisitor makes adding new operations easy. You can define a new operation over an object structure simply by adding a new visitor. While the Iterator pattern is restricted to visiting all elements that have a common parent class, Visitor pattern does not have this restriction. You can add any type of object to a Visitor interface. \n\nDisadvantages:\n\n\tVisitor pattern makes it difficult to add new subclasses of Element. The key consideration in applying the Visitor pattern is whether you are mostly likely to change the algorithm applied over an object structure or the classes of objects that make up the structure. Visitor pattern may compromise an object's encapsulation because the pattern innately assumes that the element interface is powerful enough to let visitors do their job. \n\n",
                behavioral)); 

            this.AllGroups.Add(behavioral);

            var structural = new DesignPatternGroup("Structural-Group",
                "Structural",
                "Design patterns which ease the design by identifying a simple way to realize relationships between entities.",
                "Assets/Structural.png",
                "Group description: Design patterns which ease the design by identifying a simple way to realize relationships between entities.",
                "");

            structural.Items.Add(new DesignPattern("Structural-Item-1",
                "Adapter",
                "Convert the interface of a class into another interface clients expect. Adapter lets classes work together that couldn't otherwise because of incompatible interfaces.",
                "Assets/Adapter.png",
                "Convert the interface of a class into another interface clients expect. Adapter lets classes work together that couldn't otherwise because of incompatible interfaces.",
                "Assets/AdapterUML.png",
                "Use an Adapter when:\n\n\tYou want to use an existing class and its interface does not match the one ou need. Adapter pattern allows you to create a reusable class that cooperates with unrelated or unforeseen classes which may have incompatible interfaces.\n\n\tThere are two types of adapter: class adapter and object adapter. A class adapter adapts Adaptee to Target by committing to a concrete Adaptee class. An object adapter lets a single Adapter work with many Adaptees - that is, the Adaptee itself and all of its subclasses. \n\n\nAdvantages:\n\n\tAdapater pattern allows for a flexible design and is able to connect two incompatible interfaces. \n\nDisadvantages:\n\n\tSometimes there are many adaptions that are needed in a chain-like effect which can result in added complexity. \n\n",
                structural));

            structural.Items.Add(new DesignPattern("Structural-Item-2",
                "Bridge",
                "Decouple an abstraction from its implementation so that the two can vary independently.",
                "Assets/Bridge.png",
                "Decouple an abstraction from its implementation so that the two can vary independently.",
                "Assets/BridgeUML.png",
                "Use a Bridge pattern when:\n\n\tYou want to avoid a permanent binding between an abstraction and its implementation (i.e., when the implementation must be selected or switched at run-time). Bridge pattern is useful when both the abstractions and their implementations should be extensible by subclassing. In this case, the Bridge pattern lets you combine the different abstractions and implementations and extend them independently. \n\n\nAdvantages:\n\n\tBridge pattern decouples abstraction from implementation to avoid the binding between abstraction and implementation and to select the implementation at run time. Adapater pattern allows the interface and implementation to be varied independently.\n\n\n\n",
                structural));

            structural.Items.Add(new DesignPattern("Structural-Item-3",
                "Decorator",
                "Attach additional responsibilities to an object dynamically. Decorators provide a flexible alternative to subclassing for extending functionality.",
                "Assets/Decorator.png",
                "Attach additional responsibilities to an object dynamically. Decorators provide a flexible alternative to subclassing for extending functionality.",
                "Assets/DecoratorUML.png",
                "Use a Decorator pattern when:\n\n\tYou want to add responsibilities to individual objects dynamically and transparently without affecting other objects. Decorator pattern is useful when extension by subclassing is impractical. Sometimes a large number of independent extensions are possible and would produce an explosion of subclasses to support every combination. \n\n\nAdvantages:\n\n\tAdapter pattern helps to decompose optional or extra behavior into separate classes. Decorator provides a relatively low risk way to extend a system with new behavior like security, caching or extra instrumentation at configuration time. \n\nDisadvantages:\n\n\tThe main disadvantage of Decorator pattern is poor code maintainability. \n\n",
                structural));

            structural.Items.Add(new DesignPattern("Structural-Item-4",
                "Facade",
                "Provide a unified interface to a set of interfaces in a subsystem. Facade defines a higher-level interface that makes the subsystem easier to use.",
                "Assets/Facade.png",
                "Provide a unified interface to a set of interfaces in a subsystem. Facade defines a higher-level interface that makes the subsystem easier to use.",
                "Assets/FacadeUML.png",
                "Use a Facade pattern when:\n\n\tYou want to provide a simple interface to a complex subsystem. A facade can provide a simple default view of the subsystem that will suffice for most clients. Facade pattern is useful when there are many dependencies between clients and the implementation classes of an abstraction. Finally, Facade pattern allows you to layer your subsystems and allow you to simplify the dependencies between layers. \n\n\nAdvantages:\n\n\tFacade pattern shields clients from subsystem components, thereby reducing the number of objects that clients deal with and making the subsystem easier to use. Facade pattern promotes weak coupling between the subsystem and its clients. Weak coupling allows you to vary the components of the subsystem without affecting its clients. Finally, Facade pattern does not prevent applications from using subsystem classes if they need to. \n\nDisadvantages:\n\n\tYou may lose some functionality contained in the lower level of classes. \n\n",
                structural));

            structural.Items.Add(new DesignPattern("Structural-Item-5",
                "Proxy",
                "Provide a surrogate or placeholder for another object to control access to it.",
                "Assets/Proxy.png",
                "Provide a surrogate or placeholder for another object to control access to it.",
                "Assets/ProxyUML.png",
                "Use a Proxy pattern when:\n\n\tThere is a need for a more versatile or sophisticated reference to an object than a simpler pointer. A remote proxy provides a local representative for an object in a different address space. A virtual proxy creates expensive objects on demand. A protection proxy controls access to the original object. A smart reference is a replacement for a bare pointer that performs additional actions when an object is accessed.\n\n\nAdvantages:\n\n\t Improved security and avoidance of duplicating objects of huge size.",
                structural));

            structural.Items.Add(new DesignPattern("Structural-Item-6",
                "Flyweight",
                "Use sharing to support large numbers of fine-grained objects efficiently.",
                "Assets/Flyweight.png",
                "Use sharing to support large numbers of fine-grained objects efficiently.",
                "Assets/FlyweightUML.png",
                "Use a Flyweight pattern when all of the following are true: An application uses a large number of objects, storage costs are high because of the sheer quantity of objects, most object states can be made extrinsic, many groups of objects  may be replaced by relatively few shared objects and the application doesn't depend on object identify. \n\nAdvantages:\n\n\tSpace saving, which increase as more flyweights are shared. \n\nDisadvantages:\n\n\tFlyweights may introduce run-time costs associated with transferring, finding, and/or computing extrinsic state. \n\n",
                structural));
                

            this.AllGroups.Add(structural); 
        }
    }
}
