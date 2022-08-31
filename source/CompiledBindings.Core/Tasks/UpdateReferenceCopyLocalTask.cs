namespace CompiledBindings;

public class UpdateReferenceCopyLocalTask : Microsoft.Build.Utilities.Task
{
	[Required]
	public ITaskItem[] ReferenceCopyLocalFiles { get; set; } = null!;

	[Output]
	public ITaskItem[] UpdatedReferenceCopyLocalFiles { get; set; } = null!;

	public bool AttachDebugger { get; set; }

	public override bool Execute()
	{
		if (AttachDebugger)
		{
			System.Diagnostics.Debugger.Launch();
		}

		UpdatedReferenceCopyLocalFiles = ReferenceCopyLocalFiles.Where(f => !f.ItemSpec.Contains("CompiledBindings")).ToArray();

		return true;
	}
}

