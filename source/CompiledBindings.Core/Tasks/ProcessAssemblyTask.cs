namespace CompiledBindings;

public class ProcessAssemblyTask : Microsoft.Build.Utilities.Task
{
	[Required]
	public required ITaskItem[] ReferenceAssemblies { get; init; }

	[Required]
	public required string TargetAssembly { get; init; }

	[Required]
	public required string DebugType { get; init; }

	public required bool AttachDebugger { get; init; }

	public override bool Execute()
	{
		try
		{
			if (AttachDebugger)
			{
				System.Diagnostics.Debugger.Launch();
			}

			TypeInfoUtils.LoadReferences(ReferenceAssemblies.Select(a => a.ItemSpec));

			bool symbols = string.Compare(DebugType, "none", true) != 0;

			var prm = new ReaderParameters(ReadingMode.Immediate)
			{
				ReadWrite = true,
				ReadSymbols = symbols,
				AssemblyResolver = new TypeInfoUtils.AssemblyResolver(),
			};

			var assembly = AssemblyDefinition.ReadAssembly(TargetAssembly, prm);
			try
			{
				foreach (var type in assembly.MainModule.GetAllTypes())
				{
					foreach (var attr in type.CustomAttributes.Where(a => a.AttributeType.FullName == "System.CodeDom.Compiler.GeneratedCodeAttribute"))
					{
						if (attr.ConstructorArguments[0].Value as string == "CompiledBindings")
						{
							foreach (var method in type.Methods.Where(m => m.Name.StartsWith("InitializeBeforeConstructor") || m.Name.StartsWith("InitializeAfterConstructor")))
							{
								foreach (var ctor in type.GetConstructors().Where(c => !c.IsStatic))
								{
									var body = ctor.Body;
									var instructions = body.Instructions;
									if (method.Name.StartsWith("InitializeBeforeConstructor"))
									{
										instructions.Insert(0, Instruction.Create(OpCodes.Ldarg_0));
										instructions.Insert(1, Instruction.Create(OpCodes.Call, method));
									}
									else
									{
										ImplementCallAtEnd(instructions, method);
									}
								}
							}

							foreach (var method in type.Methods.Where(m => m.Name.StartsWith("DeinitializeAfterDestructor")))
							{
								var dtor = type.Methods.FirstOrDefault(m => m.Name == "Finalize");
								if (dtor != null)
								{
									ImplementCallAtEnd(dtor.Body.Instructions, method);
								}
							}

							type.CustomAttributes.Remove(attr);
							break;
						}
					}
				}
				assembly.Write(new WriterParameters { WriteSymbols = symbols });
				return true;
			}
			finally
			{
				assembly.Dispose();
			}
		}
		catch (GeneratorException ex)
		{
			Log.LogError(null, null, null, ex.File, ex.LineNumber, ex.ColumnNumber, ex.EndLineNumber, ex.EndColumnNumber, ex.Message);
			return false;
		}
		catch (Exception ex)
		{
			Log.LogError(ex.Message);
			return false;
		}
		finally
		{
			TypeInfoUtils.Cleanup();
		}
	}

	private static void ImplementCallAtEnd(Mono.Collections.Generic.Collection<Instruction> instructions, MethodDefinition method)
	{
		var firstInstruction = Instruction.Create(OpCodes.Ldarg_0);
		instructions.Add(firstInstruction);
		instructions.Add(Instruction.Create(OpCodes.Call, method));
		instructions.Add(Instruction.Create(OpCodes.Ret));

		for (int i = 0, n = instructions.Count - 3; i < n; i++)
		{
			var retInstr = instructions[i];
			if (retInstr.OpCode == OpCodes.Ret)
			{
				instructions[i] = Instruction.Create(OpCodes.Leave, firstInstruction);
				for (int j = 0; j < n; j++)
				{
					if (instructions[j].Operand == retInstr)
					{
						instructions[j].Operand = firstInstruction;
					}
				}
			}
		}
	}
}

