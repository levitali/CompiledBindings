using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XFTest.ViewModels;

namespace CompiledBindings.Tests;

[TestClass]
public class TypeInfoTests
{
	[TestMethod]
	public void TestTypeInfo()
	{
		int substr = "file:///".Length;
		TypeInfoUtils.LoadReferences(new string[]
		{
			typeof(string).Assembly.CodeBase.Substring(substr),
			Assembly.GetExecutingAssembly().CodeBase.Substring(substr)
		});

		var class1TypeInfo = new TypeInfo(TypeInfo.GetTypeThrow(typeof(Class1)), false);
		var funcProp2 = class1TypeInfo.Properties.Single(p => p.Definition.Name == nameof(Class1.FuncProp2));
		var method = funcProp2.PropertyType.Methods.Single(m => m.Definition.Name == "Invoke");
		var retType = method.ReturnType;
		var prop = retType.Properties.Single(p => p.Definition.Name == nameof(ItemViewModel.GuidProp));

		var arrayProp = class1TypeInfo.Properties.Single(p => p.Definition.Name == nameof(Class1.ArrayProp));
		Assert.IsFalse(arrayProp?.PropertyType.GetElementType()?.IsNullable);
	}
}

