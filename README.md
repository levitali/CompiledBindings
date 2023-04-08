# CompiledBindings

<b>CompiledBindings.WPF</b> [![NuGet version (CompiledBindings.WPF)](https://img.shields.io/nuget/v/CompiledBindings.WPF)](https://www.nuget.org/packages/CompiledBindings.WPF/)

<b>CompiledBindings.XF</b> [![NuGet version (CompiledBindings.XF)](https://img.shields.io/nuget/v/CompiledBindings.XF)](https://www.nuget.org/packages/CompiledBindings.XF/)

<b>CompiledBindings.MAUI</b> [![NuGet version (CompiledBindings.MAUI)](https://img.shields.io/nuget/v/CompiledBindings.MAUI)](https://www.nuget.org/packages/CompiledBindings.MAUI/)

<b>CompiledBindings.WinUI</b> [![NuGet version (CompiledBindings.WinUI)](https://img.shields.io/nuget/v/CompiledBindings.WinUI)](https://www.nuget.org/packages/CompiledBindings.WinUI/)

This library provides {x:Bind} Markup Extension for WPF, MAUI and Xamarin Forms. You can read about {x:Bind} Markup Extension for UWP [here](https://docs.microsoft.com/en-us/windows/uwp/xaml-platform/x-bind-markup-extension). Most functionality of {x:Bind} for UWP and also many other features are supported.

At XAML compile time, {x:Bind} is converted into C# code. Thus you can't use it in Visual Basis projects.

## x:Bind Markup Extension

{x:Bind} Markup Extension have an expression as its first parameter, or the expression is specified by the Path parameter, following by other parameters like Mode, BindBack, Converter, ConverterParameter.

### x:Bind usage

Note, that in some examples bellow the TextBlock (WPF) control is used, in others Label (Xamarin Forms).

**Property paths**
 ```xaml
<TextBlock Text="{x:Bind Title}"/>
<TextBlock Text="{x:Bind Movie.Year}"/>
 ```
 
**Function calls**
 ```xaml
<TextBlock Text="{x:Bind Movie.Description.Trim()}"/>
<TextBlock Text="{x:Bind local:MainPage.GenerateSongTitle(Title, Description)}"/>
 ```
 
The first example above is the call of instance method Trim(). The second is the call of static method GenerateSongTitle of MainPage class, wich takes two parameters (multi-binding).
 
 The expression of the {x:Bind} can also have the following operators:
 - **binary operators** (in round brackets alternatives, which are better for XML): +, -, \*, /, = (eq), != (ne), < (lt), > (gt), <= (le), >= (ge) 
  ```xaml
<Label IsVisible="{x:Bind Movie.Year gt 2000}"/>
 ```
 
  - **unary operators**: -, +, ! (not)
 ```xaml
<Label IsVisible="{x:Bind not IsChanged}"/>
 ```
  - **logical operators**: && (and), || (or)
 ```xaml
<Label IsVisible="{x:Bind NoTitle and IsChanged}"/>
 ```
 - **coalesce operator**
 ```xaml
<TextBlock Visibility="{x:Bind IsChange ? Collapsed : Visible}"/>
 ```
Note, that the Collapsed and Visible values here are inferred from Visibility property.

 - **null check operator**
 ```xaml
<Label IsVisible="{x:Bind Movie.Title ?? 'no title'}"/>
 ```
 
 - **cast operator**. The class, to which cast is made, must be fully specified with namespace (the namespace must be declared)
 ```xaml
<CollectionView x:Name="itemsList"/>
<Label x:DataType="{x:Null}" Text="{x:Bind ((local:Movie)itemsList.SelectedItem).Title}"/>
 ```
 
 - **as operator**. The class, to which cast is made, must be fully specified with namespace (the namespace must be declared).
 ```xaml
<CollectionView x:Name="itemsList"/>
<Label x:DataType="{x:Null}" Text="{x:Bind (itemsList.SelectedItem as local:Movie).Title}"/>
 ```
In the example, if the SelectedItem is not a Movie, empty text is set.

- **new operator**. The class must be fully specified with namespace
 ```xaml
<Label Text="{x:Bind SomeFunction(Property1, new local:Class1(Property2)}" />
 ```
 
   - **interpolated string**. The interpolated string must start with $ symbol following a string literal in quotes. The string may contain interpolated expressions in {} brackets, with optional format part. The string is resolved when the binding applies, or one of the values of any expression is changed.

 ```xaml
<Label Text="{x:Bind $'Decimal value: {DecimalProp:0.###}, Boolean value: {BooleanProp}, String value: {StringProp.TrimStart('0')}'}" />
 ```
 
 - **is operator**. 

The is-operator can be used, if you have an expression and you need to compare it with many other expressions. It allows to avoid repeating the compared expression. For example, you have this expression:
 
```xaml
IntProp1 eq 0 or IntProp1 eq 1
 ```
 
With is-operator you can have the IntProp1 expression only once:

```xaml
IntProp1 is 0 or 1
 ```
 
In the example above the IntProp1 is compared whether it is equal to 0 or 1. If you need to do other comparisons, like greater, lower, you use the comparison operator before the right expressions. For example:

```xaml
IntProp1 is ge 0 and le 10
 ```
 
The "eq" operator is still valid. The following two expressions are the same

```xaml
IntProp1 is eq 0 or eq 10
IntProp1 is 0 or 10
 ```

The "not" keyword can be used before comparison operator to negate the expression:

```xaml
ListProp.Count is not gt 0
``` 

To compare whether the left expression is not equal, you can use either "not" or "ne" or "not eq" keywords:

```xaml
IntProp1 is not 0
IntProp1 is ne 0
IntProp1 is not eq 0
```



The is-operator is like the is matching operator in C# with the difference, that on the right side you can have any expression, not only constants. For example you can compare the IntProp1 with other properties

```xaml
IntProp1 is gt 0 and le (IntProp2 + 1)
```
 
The right expression of the is operator continues untill there are "and" or "or" operators. If you need to do some other compares after the is-operator, you have to include the whole is-operator expression in parens. For example, if you have the following expression:

```xaml
IntProp1 is 0 or 1 and IntProp2 eq 3
```

the parser consinders the "IntProp2" as bellonging to the is-operator. So the following C# expression will be generated:

```C#
(IntProp1 == 0 || IntProp1 == 1 && IntProp1 == IntProp2) == 3
```

which is of course not valid. If you have this:

```xaml
(IntProp1 is 0 or 1) and IntProp2 eq 3
```

the following correct C# expression will be generated:

```C#
(IntProp1 == 0 || IntProp1 == 1) && IntProp2 == 3
```

Like in C#, the is-operator also supports checking whether the left expression is of a given type. For example:

```xaml
ObjProp is system:String or not system:Int32
```

 
**Contants**
  
You can use following **constants** in the expression:
- numbers with or wihout decimal seperator (point). For example 2.3
- true, false
- null
- this
- strings. For example 'some string'
- characters. The same as strings, for example 'a'. Which type is actually used is mostly inferred during compilation. In case of ambiquity, the cast operator must be used.

### Data source

If not specified, the data source of {x:Bind} is the root control/page/window itself. In Xamarin Forms you can specify the data source type with x:DataType attribute.

Because x:DataType attribute is not available for WPF, you can do the following.
- Declare namespace http://compiledbindings.com/x.  For example
```xaml
  xmlns:mx="http://compiledbindings.com/x"
```
- Set the prefix of the namespace as ignorable (in the example together with d namespace)
```xaml
  mc:Ignorable="d mx"
  ```
  - Now you can set the data source type like this
  ```xaml
  xmlns:local="clr-namespace:CompiledBindingsDemo"
  mx:DataType="local:MainViewModel"
  ```

Note, that if the data source is specified, the {x:Bind} extensions are only applied if the DataContext (or BindingContext in XF) of the root or corresponding controls is set to an object of the specified type.

If the {x:Bind} Markup Extension is used in a DataTemplate and you don't set the IsItemsSource property to true to the binding, setting the ItemsSource of a collection control (see below), you must specify the data type. For Xamarin Forms with x:DataType attribute. For WPF either with DataType attribute, or alternative with mx:DataType attribute.

You can change the data type anywhere in XAML by setting x:DataType (mx:DataType). You can also use {x:Null} as DataType, except in DataTemplates, to reset the data type. Note, that {x:Null} works differently for standard {Binding} and {x:Bind} extensions. For the first one, it turns off producing compiled binding at compile time, so the expression is only resolved at runtime. For the second one, it sets the data type of the control/page/window itself.

**DataType** property

You can set the DataType only for a one x:Bind, by setting the corresponding DataType property.

 ```xaml
<CollectionView x:Name="itemsList"/>
<Label Text="{x:Bind ((local:Movie)itemsList.SelectedItem).Title, DataType={x:Null}}"/>
 ```
 
 **IsItemsSource** property

If you use {x:Bind} to set the ItemsSource property of a collection control (ListBox, ListView, CollectionView etc), you can set the IsItemsSource property to *true*, so that the data type of a elements is automatically detected. Than you don't have to specify it for the ItemTemplate.

 ```xaml
<CollectionView ItemsSource="{x:Bind Movies, IsItemsSource=true}">
    <CollectionView.ItemTemplate>
        <DataTemplate>
	        <Label Text="{x:Bind Title}"/>
	    </DataTemplate>
    </CollectionView.ItemTemplate>
</CollectionView>
 ```
 
 Note, that it works only if the DataTemplate is specifed as a XAML child to the collection control.

### x:Bind other parameters

- **Mode** Specifies the binding mode, as one of these strings: "OneTime", "OneWay", "TwoWay" or "OneWayToSource". The default is "OneWay" 
- **Converter** Specifies the converter.
- **ConverterParameter** Specifies the converter parameter.
- **BindBack** Specifies a expression to use for the reverse direction of a two-way binding. If the property is set, the Mode is automatically set two TwoWay.
- **FallbackValue** Specifies a value to display when the source or path cannot be resolved.
- **TargetNullValue** Specifies a value to display when the source value resolves but is explicitly null.
- **UpdateSourceEventNames** Specifies an event, or a list of events separated by | symbol, to use for updating the binding's source.

The **Converter**, **ConverterParameter**, **FallbackValue** and **TargetNullValue** can be either an expression, or a {StaticResource } markup extension.

### Observing changes

If the Mode is not OneTime or OneWayToSource, than a code is generated to observe changes of properties in the {x:Bind} expression. The changes are observed to Dependency Properties, if there are any in the expression, as well as to objects of classes, implementing INotifyPropertyChanged interface.

**\\. and /. operators**
\
Checking whether a class implements the INotifyPropertyChanged is done during compile time. With the <c>\\.</c> (backslash dot) operator before a property you can force checking at runtime whether the source class implements the interface.

For example, you have a ListBox, and you need to bind to SelectedItems.Count property. The SelectedItems property is of type IList, so normally no listing to changes of Count property is generated. By using \\. operator the code is generated to check at runtime, whether the object returned by the property implements INotifyPropertyChanged.

``` xaml
<ListBox x:Name="serialNumbersList"/>
<TextBlock Text="{x:Bind serialNumbersList.SelectedItems\.Count}"/>
```
You can use the /. operator to stop listening to property changes. In the example above, the SelectedItems property is the dependency one, so normally the code is generated to check whether the property is changed. But actually the object (collection) returned by the property never changes. Only the collection itselft can change. So to optimize code generation you can use this:

``` xaml
<ListBox x:Name="serialNumbersList"/>
<TextBlock Text="{x:Bind serialNumbersList/.SelectedItems\.Count}"/>
```
**get-only properties and ReadOnlyAttribute**
\
To optimize the generated code, no code is generated for listining to changes of auto-generated get-only properties. For example:

```c#
class EntityViewModel : INotifyPropertyChanged
{
    public EntityViewModel(EntityModel model)
    {
    	Model = model;
    }
    
    public EntityModel Model { get; }
}
```

So actually the Model property cannot be changed later, so no code is generated for listining whether the property is changed.

You can use the ReadOnlyAttribute for a property with false or true, to force generation or not generation of code for property changes.

If in the example above the object returned by Model property is changed, but it doesn't itselft implement INotifyPropertyChanged, you can want to force refreshing bindings by raising the PropertyChanged event with the "Model" property name. In this case you can force generating listing code with the [ReadOnly(false)] attribute:

```c#
    
    [ReadOnly(false)]
    public EntityModel Model { get; }
```

In contrast, if a property is not an auto-generated get-only one, but you know that it never changes, you can optimize code generation by turning notifications for this property off.

```c#    
    [ReadOnly(true)]
    public string Title => // some logic
```

### Nullability checking

You can use expressions, which or parts of which can return nulls. The bindings code generator determes which expressions can be null, and generates corresponding handling of them. But note! For null reference types, which are decleared as not-nullable, no nullibility checking is generated.

### Binding to asynchronous (returning Task&lt;T&gt;) properties and functions.

If a property's or a function's returning type is Task&lt;T&gt;, its value is taking with await.

For example, you have a function LoadImageAsync in your ViewModel, which asynchronously downloads an image. You can set the image like this:

 ```xaml
<Image Source="{x:Bind LoadImageAsync()}" />
 ```
 
While waiting for the value to arrive, the {x:Bind} reports the *FallbackValue*, if one is available, or the default value of the binding target property. For example:

```xaml
<Page.Resources>
    <BitmapImage x:Key="defaultImage" UriSource="/Images/download.png" />
</Page.Resources>

<Image Source="{x:Bind LoadImageAsync(), FallbackValue={StaticResource defaultImage}}" />
 ```

If an asynchronous function throws an exception, the exception is ignored. The value of the target property is not changed.

### Binding to tuples.

Sometimes it is needed to calculate values for many UI properties using the same logic. For example, foreground and background of a TextBlock must be set according to some state or other logic. You can define one property (function), which returns both foreground and background.

``` c#
public (Brush foreground, Brush background) Colors
{
	get
	{
		switch (this.State)
		{
			case EntityState.Saved:
				return (Brushes.Beige, Brushes.Green);
			case EntityState.NotEdited:
				return (Brushes.White, Brushes.Black);
					
		       ...
		}
	}
}
```

Than you can use it:

``` xaml
<TextBlock Foreground="{x:Bind Colors.foreground}" Background="{x:Bind Colors.background}"/>
```

Note, that the Color property is not called twice. Its value is taken only once, saved as local variable in the generated C# code, and than the values are set to the Control's properties.

### Binding to events as target.

You can binding to a controls event and have a function at the end of expression, which will be called when the event is triggered. If you want to receive the event parameters, the signature of the function must be the same as the event handler. The function in the expression must be used without parameters.

``` xaml
<TextBox Drop="{x:Bind _viewModel.OnDrop}"/>
```
``` c#
public void OnDrop(object sender, System.Windows.DragEventArgs e)
{
}
```

You can use also a function with any signature, passing the parameters in XAML.

``` xaml
<Button Click="{x:Bind _viewModel.Save(true)}"/>
```
``` c#
public void Save(bool parameter)
{
}
```

## x:Set Markup Extension

This library also provides {x:Set} Markup Extension. It can be used to set once a property with some expression. The differences between x:Set and x:Bind are:

1) x:Set expressions are always evaluated at the end of a constructor.
x:Bind expressions, if x:DataType is specified, are evaluated when the DataContext/BindingContext is set.

2) For x:Set no code is generated for listening to changes of properties if there are some notifiable properties in the expression.
If no code listening to changes of properties is needed for x:Bind, it is needed to set Mode=OneTime

3) The data root for x:Set is always the view itself. The data root for x:Bind depends on x:DataType attribute. Using static members in expressions is the same in x:Set and x:Bind

## Binding to methods and extension methods

Instead of a property, you can use a method or an extension method as target of {x:Bind} or {x:Set} Markup Extension with OneTime and OneWay modes. If an instance method is used as a target, it must have only one parameter. An extension method must have two parameters where the first one is the "this" parameter of the control, and the second is the parameter, to which the {x:Bind} or {x:Set} expression is set. To use it you do the following

- Declare namespace http://compiledbindings.com.  For example
```xaml
  xmlns:m="http://compiledbindings.com/"
```
- Set the prefix of the namespace as ignorable
```xaml
  mc:Ignorable="d m mx"
  ```
  - For extension methods, the corresponding namespace of the extension class must be specified
```xaml
  xmlns:extension="using:CompiledBindingsDemo.Extensions"
  ```
  - Now use can use methods as target like this
```xaml
  <Entry m:SetFocused="{x:Bind IsNameFocused} "/>
  ```
  
The extension method for example above can look like this:
 ```c#
public static void SetFocused(this VisualElement visualElement, bool focused)
{
    if (focused)
    {
        visualElement.Focus();
    }
}
``` 

## Using m: Namespace to bind to any property

Just like binding to methods, you can use m: Namespace to bind to properties. This is usefull, if the {x:Bind} or {x:Set} expression is correct, but you still receive errors. This can happen, because some other part of the build process determins the markup extension as invalid.

For example, in WPF if using single quotes in the expression, you receive the following error:

```xaml
xmlns:system="using:System"
```
```xaml
<TextBlock Text="{x:Bind system:String.Format('{0:0.###} {1}', Quantity, Unit)}" />
```
```text  
Names and Values in a MarkupExtension cannot contain quotes. The MarkupExtension arguments ' system:String.Format('{0:0.###} {1}', Quantity, Unit)}' are not valid.
```

You can overcome this problem by using m: Namespace to set the Text property:
```xaml
<TextBlock m:Text="{x:Bind system:String.Format('{0:0.###} {1}', Quantity, Unit)}"
```

### Declaring CLR-Namespaces namespaces with "using" and "global using"

For CLR-Namespaces you can use the "using" syntax. For example
 ```xaml
  xmlns:local="using:CompiledBindingsDemo"
  ```

You can also declare the CLR-Namespaces globally with "global using" syntax. For {x:Bind} markup extensions in other XAML files you do not have to declare the namespace. Note, this works only for {x:Bind}. If a namespace is used for other purposes, it must be decleared locally.

 ```xaml
  xmlns:local="global using:CompiledBindingsDemo"
  ```

### Settings breakpoints for x:Bind and x:Set extensions

You can set breakpoints in XAML on the lines with x:Bind or x:Set extensions. A breakpoint will be hit whenever the property is set.

![BreakpointInXaml](https://user-images.githubusercontent.com/884112/174088009-4426bccb-c681-44a4-8bcc-36d30913b830.jpg)

Note, that only one breakpoint per line is possible.

To inspect values of the binding expression you can use the "dataRoot" variable in the Watch window to refer to the binding root object.
