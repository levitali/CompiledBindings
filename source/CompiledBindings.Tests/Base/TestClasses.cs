using System;
using System.Collections.Generic;
using XFTest.ViewModels;

#nullable enable

namespace CompiledBindings.Tests;

public class Class1
{
	public Class2? RefProp { get; set; }

	public IList<Class2?>? ListProp { get; set; }

	public int? NullIntProp { get; set; }

	public bool? NullBoolProp { get; set; }

	public Func<int> FuncProp { get; set; }

	public Func<string, ItemViewModel> FuncProp2 { get; set; }

	public static Class2 Instance { get; } = new Class2();

	public static string? Method1(Type t) => null;
}

public class Class2
{
	public Struct1 StructProp { get; set; }

	public Struct1? NullStructProp { get; set; }

	public decimal DecimalProp { get; set; }

	public string? StringProp { get; set; }
}

public struct Struct1
{
	public int TestMethod()
	{
		return 0;
	}

	public static Struct1 operator +(Struct1 left, Struct1 right)
	{
		return left;
	}
}

public class BindingsTargetClass
{
}

