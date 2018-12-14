using System;

namespace anygames.ashley.utils
{
	public static class MathUtils
	{
		public static  Random _random = new Random ();

		/** Returns a random number between start (inclusive) and end (inclusive). */
		public static int Random (int start, int end)
		{
			return start + _random.Next (end - start + 1);
		}

		/** Returns a random number between 0 (inclusive) and the specified value (inclusive). */
		public static  int Random (int range)
		{
			return _random.Next (range + 1);
		}

		/** Returns the next power of two. Returns the specified value if the value is already a power of two. */
		public static int NextPowerOfTwo (int value)
		{
			if (value == 0) return 1;
			value--;
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			return value + 1;
		}

		public static int NumTrailingBinaryZeros (int n)
		{
			int mask = 1;
			for (int i = 0; i < 32; i++, mask <<= 1)
				if ((n & mask) != 0)
					return i;

			return 32;
		}

		public static int NumberOfTrailingZeros (int i)
		{
			// HD, Figure 5-14
			int y;
			if (i == 0) return 32;
			int n = 31;
			y = i << 16; if (y != 0) { n = n - 16; i = y; }
			y = i << 8; if (y != 0) { n = n - 8; i = y; }
			y = i << 4; if (y != 0) { n = n - 4; i = y; }
			y = i << 2; if (y != 0) { n = n - 2; i = y; }
			return n - ( RightBitMove((i << 1), 31)  );
		}

		//Alternative java >>> operator
		public static int RightBitMove (int value, int pos)
		{
			if (pos != 0) {
				int mask = 0x7fffffff;
				value >>= 1;
				value &= mask;
				value >>= pos - 1;
			}
			return value;
		}
	}
}
