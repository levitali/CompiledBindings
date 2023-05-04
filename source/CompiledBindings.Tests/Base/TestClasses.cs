using XFTest.ViewModels;

namespace CompiledBindings.Tests;

public class Class1
{
	public object? ObjProp { get; set; }

	public TestMode Mode => CompiledBindings.Tests.TestMode.Mode3;

	public string? Mode3 { get; set; }

	public bool TestMode => false;

	public void Check(bool mode)
	{
	}

	public int IntProp { get; set; }

	public Class2? RefProp { get; set; }

	public string[] ArrayProp { get; set; } = new string[0];

	public IList<Class2?>? ListProp { get; set; }

	public int? NullIntProp { get; set; }

	public bool? NullBoolProp { get; set; }

	public DateTime? NullDateTimeProp { get; set; }

	public Func<int> FuncProp { get; set; } = null!;

	public Func<string, ItemViewModel?>? FuncProp2 { get; set; }

	public static Class2 Instance { get; } = new Class2();

	public static string? Method1(Type t)
	{
		return null;
	}

	public bool Method2(float value)
	{
		return false;
	}
}

public class Class2
{
	public Struct1 StructProp { get; set; }

	public Struct1? NullStructProp { get; set; }

	public decimal DecimalProp { get; set; }

	public string? StringProp { get; set; }

	public object? ObjProp { get; set; }

	public int Method1(string? str)
	{
		return 0;
	}
}

public struct Struct1
{
	public int Prop1 { get; set; }

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

public enum TestMode
{
	Mode1,
	Mode2,
	Mode3,
}

#nullable disable

public class NullableDisabledBaseClass
{
	public object SelectedItem { get; set; }
}

public class NullableEnabledDerivedClass : NullableDisabledBaseClass
{
	public string Dummy { get; set; } = "";
}

