

namespace CompiledBindings;

public class MauiGenerateCodeTask : XFGenerateCodeTask
{
	public MauiGenerateCodeTask() : base(new MauiPlatformConstants())
	{
	}
}

public class MauiPlatformConstants : PlatformConstants
{
	public override string DefaultNamespace => "http://schemas.microsoft.com/dotnet/2021/maui";
	public override string BaseClrNamespace => "Microsoft.Maui.Controls";
}
