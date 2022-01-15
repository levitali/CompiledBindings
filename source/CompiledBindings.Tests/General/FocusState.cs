namespace UI;

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
			}
			else if (object.Equals(State, key))
			{
				State = null;
			}
		}
	}

	public T? State { get; private set; }

	public static implicit operator FocusState<T>(T? value)
	{
		return new FocusState<T> { State = value };
	}
}
