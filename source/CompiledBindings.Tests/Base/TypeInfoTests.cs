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

		var typeInfo = new TypeInfo(TypeInfoUtils.GetTypeThrow(typeof(Class1)));
		var funcProp2 = typeInfo.Properties.Single(p => p.Definition.Name == nameof(Class1.FuncProp2));
		var method = funcProp2.PropertyType.Methods.Single(m => m.Definition.Name == "Invoke");
		var retType = method.ReturnType;
		var prop = retType.Properties.Single(p => p.Definition.Name == nameof(ItemViewModel.GuidProp));
	}
}

