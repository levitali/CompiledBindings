using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace CompiledBindings;

public static class EnumerableExtensions
{
	public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		if (predicate == null)
		{
			throw new ArgumentNullException("predicate");
		}
		int index = 0;
		foreach (T value in source)
		{
			if (predicate(value))
			{
				return index;
			}
			index++;
		}
		return -1;
	}

	public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		if (source == null)
		{
			return;
		}
		foreach (T item in source)
		{
			action(item);
		}
	}

	public static IEnumerable<T> AsEnumerable<T>(T element)
	{
		yield return element;
	}

	public static IEnumerable<T> SelectSequence<T>(T first, Func<T, T?> nextSelector, bool includeFirst)
		where T : class
	{
		return SelectSequence(first, nextSelector, t => t == null, includeFirst);
	}

	public static IEnumerable<T> SelectSequence<T>(T source, Func<T, T?> parentSelector, Func<T, bool> endDetector, bool includeThis)
	{
		if (includeThis)
			yield return source;
		var parent = parentSelector(source);
		if (parent != null && !endDetector(parent))
		{
			foreach (T parent2 in SelectSequence(parent, parentSelector, endDetector, true))
			{
				yield return parent2;
			}
		}
	}

	public static IEnumerable<T> SelectTree<T>(T root, Func<T, IEnumerable<T>> selector, bool includeRoot)
		where T : class
	{
		if (includeRoot)
			yield return root;
		var children = selector(root);
		if (children != null)
		{
			foreach (var element in children.SelectTree(selector))
			{
				yield return element;
			}
		}
	}

	public static IEnumerable<T> SelectTree<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
	{
		if (source != null)
		{
			foreach (var t1 in source)
			{
				yield return t1;
				var children = selector(t1);
				if (children != null)
				{
					foreach (var t2 in children.SelectTree(selector))
					{
						yield return t2;
					}
				}
			}
		}
	}

	public static IEnumerable<T> SelectTree<T>(T root, Func<T, IEnumerable<T>> selector)
		where T : class
	{
		return SelectTree(root, selector, false);
	}

	public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
	{
		return source.Distinct(new ComparisonEqualityComparer<T, TKey>(keySelector));
	}

	public static IEnumerable<T> Union<T, TKey>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, TKey> keySelector)
	{
		return first.Union(second, new ComparisonEqualityComparer<T, TKey>(keySelector));
	}
}

public class ComparisonEqualityComparer<T, TKey> : IEqualityComparer<T>
{
	private Func<TKey, TKey, bool> _predicate;
	private Func<T, TKey> _keySelector;

	public ComparisonEqualityComparer(Func<T, TKey> keySelector)
	{
		_keySelector = keySelector;

		var comparer = EqualityComparer<TKey>.Default;
		_predicate = (k1, k2) => comparer.Equals(k1, k2);
	}

	public ComparisonEqualityComparer(Func<T, TKey> keySelector, Func<TKey, TKey, bool> predicate)
	{
		_keySelector = keySelector;
		_predicate = predicate;
	}

	#region IEqualityComparer<T> Members

	public bool Equals(T x, T y)
	{
		TKey xKey = _keySelector(x);
		TKey yKey = _keySelector(y);
		return _predicate(xKey, yKey);
	}

	public int GetHashCode(T obj)
	{
		if (obj == null)
		{
			return 0;
		}

		TKey key = _keySelector(obj);
		if (key == null)
		{
			return 0;
		}
		return key.GetHashCode();
	}

	#endregion
}
