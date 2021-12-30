# CompiledBindings

This library provides {x:Bind} Markup Extension for WPF and Xamarin Forms. You can read about {x:Bind} Markup Extension for UWP [here](https://docs.microsoft.com/en-us/windows/uwp/xaml-platform/x-bind-markup-extension). The whole functionality of {x:Bind} for UWP and also many other features are supported.

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
 
  - **new operator**. The class must be fully specified with namespace
 ```xaml
<Label Text="{x:Bind SomeFunction(Property1, new local:Class1(Property2)}" />
 ```
 
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

#### New in Version 1.0.6

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

The **Converter**, **ConverterParameter**, **FallbackValue** and **TargetNullValue** can be either an expression, or a {x:Static} markup extension.

### Observing changes

If the Mode is not OneTime or OneWayToSource, than a code is generated to observe changes of properties in the {x:Bind} expression. The changes are observed to Dependency Properties, if there are any in the expression, as well as to objects of classes, implementing INotifyPropertyChanged interface.

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

This library also provides {x:Set} Markup Extension. It has an expression parameter, similar like {x:Bind}, and no other parameters. The data source of {x:Set} is always the root page/control/window itself. The expression is evaluated and set only once at the end of constructors of the page/control/window. If an expression is a static property of some type, than it is similar to {x:Static} Markup Extension.

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

## Performance

## Performance in Xamarin Forms app

I compared list scrolling performance of a Xamarin Forms app. I first created UI with hard-coded texts, to ensure the UI visual tree is quick enough. Than I put bindings to it:
- Not compiled Bindings
- Xamarin compiled Bindings
- My compiled Bindings.

The result are:
- Not compiled bindings perform noticeably bad
- Xamarin compiled bindings perform better, but not as well as without bindings
- Scrolling ist the best. No difference to scrolling with hard-coded texts.

I tested on an industrial Android handheld. Here are the videos:

- Not compiled (reflection) Bindings
https://user-images.githubusercontent.com/884112/145461225-e0c9952a-32a9-4409-ab43-d02cd725754c.mp4


- Xamarin compiled Bindings
https://user-images.githubusercontent.com/884112/145461241-0b0eba2b-2960-43dc-9f10-2d2435d557c0.mp4

- This compiled Bindings
https://user-images.githubusercontent.com/884112/145461306-c6af2cb9-9437-4606-9133-56f33d614f69.mp4

## Performance in WPF app

To achieve visual difference between WPF and my bindings was for me not possible. All the WPF projects I have at the time perform visually good on modern computers also with classing bindings.

In order to compare the performance, I created a demo project (you find it in source code). In the app a list of entities is displayed. The performance is measured by measuring how much time in average (the last 1000 measures) it was needed between taking the first and the last properties of an entity. Then by scrolling back and forth the list, it is possible to get the average time.

The results are (in ticks):

Classing Bindings:

![Binding](https://user-images.githubusercontent.com/884112/147760454-4e99e845-3e2d-4b17-9b1e-4d0785c2860c.png)

x:Bind:

![xBind](https://user-images.githubusercontent.com/884112/147760483-c24b2ff0-43fe-4956-a8f0-2b3105cd4df2.png)

As you can see, x:Bind is about 10 times faster
