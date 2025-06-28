namespace CompiledBindings;

public class LazyLoadCollection<T>
{
	private IEnumerator<T>? _innerCollectionEnumerator;
	private List<T>? _cashedItems;

	public LazyLoadCollection(IEnumerable<T> innerCollection)
	{
		_innerCollectionEnumerator = innerCollection.GetEnumerator();
	}

	public IEnumerable<T> Enumerate()
	{
		if (_cashedItems == null)
		{
			_cashedItems = new List<T>();
		}
		else
		{
			foreach (var item in _cashedItems)
			{
				yield return item;
			}
		}
		if (_innerCollectionEnumerator != null)
		{
			while (_innerCollectionEnumerator.MoveNext())
			{
				var item = _innerCollectionEnumerator.Current;
				_cashedItems.Add(item);
				yield return item;
			}
			_innerCollectionEnumerator = null;
		}
	}
}
