# CompiledBindings

This library provides {x:Bind} Markup Extension for WPF and Xamarin Forms. You can read about {x:Bind} Markup Extension for UWP [here](https://docs.microsoft.com/en-us/windows/uwp/xaml-platform/x-bind-markup-extension). The whole functionality of {x:Bind} for UWP and also many other features are supported.

At XAML compile time, {x:Bind} is converted into C# code. Thus you can't use it in Visual Basis projects.

## x:Bind Markup Extension

{x:Bind} Markup Extension must have an expression (without Path attribute) as its first parameter, following by other parameters like Mode, BindBack, Converter, ConverterParameter.

If not specified, the data source of {x:Bind} is the root control/page/window itself. In Xamarin Forms you can specify the data source type with x:DataType attribute.

Because x:DataType attribute is not available for WPF, you can do the following.
- Declare namespace http://compiledbindings.com/x.  For example
```xaml
  xmlns:mx="http://compiledbindings.com/x"
```
- Set the prefix of the namespace as ingorable (in the example together with d namespace)
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

If the {x:Bind} Markup Extension is used in a DataTemplate, you must specify the data type. For Xamarin Forms with x:DataType attribute. For WPF either with DataType attribute, or alternative with mx:DataType attribute.

### x:Bind usage by examples

Note, that in some examples bellow the TextBlock (WPF) control is used, in others Label (Xamarin Forms).

Property pahs
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

### x:Bind other parameters

- **Mode** Specifies the binding mode, as one of these strings: "OneTime", "OneWay", "TwoWay" or "OneWayToSource". The default is "OneWay" 
- **Converter** Specifies the converter. The value must be a StaticResource expression.
- **ConverterParameter** Specifies the converter parameter. Note, that here you can use any expression like in the first expression parameter. The only difference is, that it is never observed whethere the values in the converter parameter expression are changed.
- **BindBack** Specifies a expression to use for the reverse direction of a two-way binding. If the property is set, the Mode is automatically set two TwoWay.

## x:Set Markup Extension

This library also provide {x:Set} Markup Extension. It has an expression parameter, similar like {x:Bind}, and no other parameters. The data source of {x:Set} is always the root page/control/window itself. The expression is evaluated and set only once at the end of constructors of the page/control/window. If an expression is a static property of some type, than it is similar to {x:Static} Markup Extension.

## Binding to methods and extension methods

Instead of a property, you can use a method or an extension method as target of {x:Bind} or {x:Set} Markup Extension. If an instance method is used as a target, it must have only one parameter. An extension method must have two parameters where the first one is the "this" parameter of the control, and the second is the parameter, to which the {x:Bind} or {x:Set} expression is set. To use it you do the following

- Declare namespace http://compiledbindings.com.  For example
```xaml
  xmlns:m="http://compiledbindings.com/"
```
- Set the prefix of the namespace as ingorable
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
  
