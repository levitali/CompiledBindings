namespace CompiledBindings;

public static class PathUtils
{
	public static string GetRelativePath(string relativeTo, string path)
	{
		var relativeToParts = GetPathParts(relativeTo);
		var pathParts = GetPathParts(path);

		int i, c;
		for (i = 0, c = Math.Min(relativeToParts.Count, pathParts.Count); i < c; i++)
		{
			if (relativeToParts[i] != pathParts[i])
			{
				break;
			}
		}

		string result = string.Empty;
		for (int j = i; j < relativeToParts.Count; j++)
		{
			result = Path.Combine(result, "..");
		}
		for (int j = i; j < pathParts.Count; j++)
		{
			result = Path.Combine(result, pathParts[j]);
		}

		return result;

		static List<string> GetPathParts(string path)
		{
			if (path.EndsWith("/") || path.EndsWith("\\"))
			{
				path = path.Substring(0, path.Length - 1);
			}
			var res = new List<string>();
			while (true)
			{
				var part = Path.GetFileName(path);
				if (string.IsNullOrEmpty(part))
				{
					res.Insert(0, path);
					break;
				}
				res.Insert(0, part);
				path = Path.GetDirectoryName(path)!;
			}
			return res;
		}
	}
}

