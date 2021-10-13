# CompiledBindings

This library provides {x:Bind} Markup Extension for WPF and Xamarin Forms. You can read about {x:Bind} Markup Extension for UWP [here](https://docs.microsoft.com/en-us/windows/uwp/xaml-platform/x-bind-markup-extension). The whole functionality of {x:Bind} for UWP and also many other features are supported.

At XAML compile time, {x:Bind} is converted into C# code. Thus you can't use it in Visual Basis projects.

## x:Bind Markup Extension

{x:Bind} Markup Extension have an expression as its first parameter, or the expression is specified by the Path parameter, following by other parameters like Mode, BindBack, Converter, ConverterParameter.

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
For CLR-Namespaces you can also use the "using" syntax. For example
 ```xaml
  xmlns:local="using:CompiledBindingsDemo"
  ```
Note, that if the data source is specified, the {x:Bind} extensions are only applied if the DataContext (or BindingContext in XF) of the root or corresponding controls is set to an object of the specified type.

If the {x:Bind} Markup Extension is used in a DataTemplate, you must specify the data type. For Xamarin Forms with x:DataType attribute. For WPF either with DataType attribute, or alternative with mx:DataType attribute.

You can change the data type anywhere in XAML by setting x:DataType (mx:DataType). You can also use {x:Null} as DataType, except in DataTemplates, to reset the data type. Note, that {x:Null} works differently for standard {Binding} and {x:Bind} extensions. For the first one, it turns off producing compiled binding at compile time, so the expression is only resolved at runtime. For the second one, it sets the data type of the control/page/window itself.

### x:Bind usage

Note, that in some examples bellow the TextBlock (WPF) control is used, in others Label (Xamarin Forms).

Property paths
 ```xaml
<TextBlock Text="{x:Bind Title}"/>
<TextBlock Text="{x:Bind Movie.Year}"/>
 ```
 
 Function calls
 ```xaml
<TextBlock Text="{x:Bind Movie.Description.Trim()}"/>
<TextBlock Text="{x:Bind local:MainPage.GenerateSongTitle(Title)}"/>
 ```
 
 The first example above is the call of instance method Trim(). The second is the call of static method GenerateSongTitle of MainPage class.
 
 The expression of the {x:Bind} can also have the following operators:
 - binary operators (in round brackets alternatives, which are better for XML): +, -, \*, /, = (eq), != (ne), < (lt), > (gt), <= (le), >= (ge) 
  ```xaml
<Label IsVisible="{x:Bind Movie.Year gt 2000}"/>
 ```
 
  - unary operators: -, +, ! (not)
 ```xaml
<Label IsVisible="{x:Bind not IsChanged}"/>
 ```
  - logical operators: && (and), || (or)
 ```xaml
<Label IsVisible="{x:Bind NoTitle and IsChanged}"/>
 ```
 - coalesce operator
 ```xaml
<TextBlock Visibility="{x:Bind IsChange ? Collapsed : Visible}"/>
 ```
Note, that the Collapsed and Visible values here are inferred from Visibility property.

 - null check operator
 ```xaml
<Label IsVisible="{x:Bind Movie.Title ?? '<no title>'}"/>
 ```
 
 - cast operator. The class, to which cast is made, must be fully specified with namespace (the namespace must be declared)
 ```xaml
<CollectionView x:Name="itemsList"/>
<Label x:DataType="{x:Null}" Text="{x:Bind ((local:Movie)itemsList.SelectedItem).Title}"/>
 ```
 
You can use following constants in the expression:
- numbers with or wihout decimal seperator (point). For example 2.3
- true, false
- null
- this
- strings. For example 'some string'
- characters. The same as strings, for example 'a'. Which type is actually used is mostly inferred during compilation. In case of ambiquity, the cast operator must be used.


### x:Bind other parameters

- **Mode** Specifies the binding mode, as one of these strings: "OneTime", "OneWay", "TwoWay" or "OneWayToSource". The default is "OneWay" 
- **Converter** Specifies the converter. The value must be a StaticResource expression.
- **ConverterParameter** Specifies the converter parameter. Note, that here you can use any expression like in the first expression parameter.
- **BindBack** Specifies a expression to use for the reverse direction of a two-way binding. If the property is set, the Mode is automatically set two TwoWay.

### Observing changes

If the Mode is not OneTime or OneWayToSource, than a code is generated to observe changes of properties in the {x:Bind} expression. The changes are observed to Dependency Properties, if there are any in the expression, as well as to objects of classes, implementing INotifyPropertyChanged interface.

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
