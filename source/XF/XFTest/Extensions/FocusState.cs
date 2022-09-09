using System.Diagnostics;

namespace XFTest;

public class FocusState<T>
	where T : struct
{
	public bool this[T key]
	{
		get => object.Equals(State, key);
		set
		{
			if (value)
			{
				State = key;
				Debug.WriteLine("Focused: " + State);
			}
			else if (object.Equals(State, key))
			{
				State = null;
				Debug.WriteLine("Focused: " + State);
			}
		}
	}

	public T? State { get; private set; }

	public static implicit operator FocusState<T>(T? value)
	{
		return new FocusState<T> { State = value };
	}

	public static implicit operator T?(FocusState<T> value)
	{
		return value.State;
	}
}
