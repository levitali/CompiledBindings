using System.IO;

namespace CompiledBindings;

public static class Crc32
{
	private static readonly uint[] _crc32Table;

	static Crc32()
	{
		const uint Polynomial = 0xEDB88320;

		_crc32Table = new uint[256];
		for (uint i = 0; i < 256; i++)
		{
			uint dw = i;
			for (int j = 8; j > 0; j--)
			{
				if ((dw & 1) != 0)
				{
					dw = (dw >> 1) ^ Polynomial;
				}
				else
				{
					dw = (dw >> 1);
				}
			}
			_crc32Table[i] = dw;
		}
	}

	public static uint GetCrc32(string fileName)
	{
		const int BufferSize = 4096;

		using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, BufferSize))
		{
			return GetCrc32(fs);
		}
	}

	public static uint GetCrc32(byte[] byteBuffer)
	{
		using (var ms = new MemoryStream(byteBuffer))
		{
			ms.Position = 0;
			return GetCrc32(ms);
		}
	}

	private static uint GetCrc32(Stream stream)
	{
		const int BufferSize = 4096;

		uint result = 0xFFFFFFFF;
		byte[] buffer = new byte[BufferSize];

		int count;
		while ((count = stream.Read(buffer, 0, BufferSize)) > 0)
		{
			for (int i = 0; i < count; i++)
			{
				uint n = (result & 0xFF) ^ buffer[i];
				result = (result >> 8) ^ _crc32Table[n];
			}
		}

		result = ~result;
		return result;
	}
}

