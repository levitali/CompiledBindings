namespace CompiledBindings;

public class UWPGenerateCodeTask : WinUIGenerateCodeTask
{
	public UWPGenerateCodeTask() : base(new UWPPlatformConstants())
	{
	}
}

public class UWPPlatformConstants : PlatformConstants
{
	public override string FrameworkId => "UWP";
	public override string BaseClrNamespace => "Windows";
}
