using System;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Summary description for PNGFix.
	/// </summary>
	public class PNGFix
	{
		public PNGFix()
		{
		}

		public static byte[] RemoveImageGamma(byte[] input)
		{
			int offset = FindOffset("gAMA", input);
			if (offset > -1)
			{
				byte[] newData = new byte[input.Length - 16];
				Array.Copy(input, 0, newData, 0, offset);
				Array.Copy(input, offset + 16, newData, offset, input.Length - offset - 16);
				return newData;
			}
			else
			{
				return input;
			}
		}
		private static int FindOffset(string chunkName, byte[] data)
		{
			if (chunkName.Length == 4)
			{
				for (int i = 0; i < data.Length-7; i++)
				{
					if (data[i + 4] == chunkName[0] && data[i + 5] == chunkName[1] &&
						data[i + 6] == chunkName[2] && data[i + 7] == chunkName[3])
					{
						return i;
					}
				}
			}
			return -1;
		}
	}

}
