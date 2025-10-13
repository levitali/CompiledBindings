using XFTest.ViewModels;

namespace CompiledBindings.Tests;

public class TypeInfoTests
{
	[Test]
	public void TestTypeInfo()
	{
		TypeInfoUtils.LoadReferences(new string[]
		{
			typeof(string).Assembly.Location,
			Assembly.GetExecutingAssembly().Location
		});

		var class1TypeInfo = TypeInfo.GetTypeThrow(typeof(Class1));
		Assert.IsTrue(class1TypeInfo.IsNullable);

		var funcProp2 = class1TypeInfo.Properties.Single(p => p.Definition.Name == nameof(Class1.FuncProp2));
		Assert.IsTrue(funcProp2.PropertyType.IsNullable);

		var genericArgs = funcProp2.PropertyType.GetGenericArguments();
		Assert.IsFalse(genericArgs![0].IsNullable);
		Assert.IsTrue(genericArgs[1].IsNullable);

		var method = funcProp2.PropertyType.Methods.Single(m => m.Definition.Name == "Invoke");
		var retType = method.ReturnType;

		var prop = retType.Properties.Single(p => p.Definition.Name == nameof(ItemViewModel.GuidProp));

		var arrayProp = class1TypeInfo.Properties.Single(p => p.Definition.Name == nameof(Class1.ArrayProp));
		Assert.IsFalse(arrayProp?.PropertyType.GetElementType()?.IsNullable);

		var page2ViewModel = TypeInfo.GetTypeThrow(typeof(Page2ViewModel));
		var getIcon = page2ViewModel.Methods.Single(m => m.Definition.Name == nameof(Page2ViewModel.GetIcon));
		Assert.IsTrue(getIcon.ReturnType.Fields.Count == 2);
		Assert.IsFalse(getIcon.ReturnType.Fields[0].FieldType.IsNullable);
		Assert.IsTrue(getIcon.ReturnType.Fields[1].FieldType.IsNullable);

		var nullableDerived = TypeInfo.GetTypeThrow(typeof(NullableEnabledDerivedClass));
		prop = nullableDerived.Properties.Single(p => p.Definition.Name == nameof(NullableDisabledBaseClass.SelectedItem));
		Assert.IsTrue(prop.PropertyType.IsNullable);
		prop = nullableDerived.Properties.Single(p => p.Definition.Name == nameof(NullableEnabledDerivedClass.Dummy));
		Assert.IsFalse(prop.PropertyType.IsNullable);
	}
}

