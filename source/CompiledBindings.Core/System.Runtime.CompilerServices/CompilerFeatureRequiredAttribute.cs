namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.All, AllowMultiple = true, Inherited = false)]
public sealed class CompilerFeatureRequiredAttribute : Attribute
{
	public CompilerFeatureRequiredAttribute(string featureName)
	{
		FeatureName = featureName;
	}

	public string FeatureName { get; }

	public bool IsOptional { get; init; }

	public const string RefStructs = nameof(RefStructs);

	public const string RequiredMembers = nameof(RequiredMembers);
}