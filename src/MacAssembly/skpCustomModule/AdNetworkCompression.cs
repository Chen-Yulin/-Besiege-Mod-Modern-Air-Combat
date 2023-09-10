using System;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000016 RID: 22
	public static class AdNetworkCompression
	{
		// Token: 0x06000081 RID: 129 RVA: 0x0000E530 File Offset: 0x0000C730
		public static void BoundarySize()
		{
			AdNetworkCompression.intmax = 4294967295.0;
			AdNetworkCompression.wMinX = -AdNetworkCompression.maxSize / 10.0;
			AdNetworkCompression.wMinY = -AdNetworkCompression.maxSize / 10.0;
			AdNetworkCompression.wMinZ = -AdNetworkCompression.maxSize / 10.0;
			AdNetworkCompression.wMaxX = AdNetworkCompression.maxSize / 10.0;
			AdNetworkCompression.wMaxY = AdNetworkCompression.maxSize / 10.0;
			AdNetworkCompression.wMaxZ = AdNetworkCompression.maxSize / 10.0;
			AdNetworkCompression.wShortMaxX = AdNetworkCompression.maxSize / AdNetworkCompression.intmax;
			AdNetworkCompression.wShortMaxY = AdNetworkCompression.maxSize / AdNetworkCompression.intmax;
			AdNetworkCompression.wShortMaxZ = AdNetworkCompression.maxSize / AdNetworkCompression.intmax;
			AdNetworkCompression.wMaxShortX = AdNetworkCompression.intmax / AdNetworkCompression.maxSize;
			AdNetworkCompression.wMaxShortY = AdNetworkCompression.intmax / AdNetworkCompression.maxSize;
			AdNetworkCompression.wMaxShortZ = AdNetworkCompression.intmax / AdNetworkCompression.maxSize;
		}

		// Token: 0x06000082 RID: 130 RVA: 0x0000E628 File Offset: 0x0000C828
		public static void CompressPosition(Vector3 pos, byte[] posBuffer, int offset)
		{
			byte[] bytes = BitConverter.GetBytes(pos.x);
			posBuffer[offset] = bytes[0];
			posBuffer[offset + 1] = bytes[1];
			posBuffer[offset + 2] = bytes[2];
			posBuffer[offset + 3] = bytes[3];
			bytes = BitConverter.GetBytes(pos.y);
			posBuffer[offset + 4] = bytes[0];
			posBuffer[offset + 5] = bytes[1];
			posBuffer[offset + 6] = bytes[2];
			posBuffer[offset + 7] = bytes[3];
			bytes = BitConverter.GetBytes(pos.z);
			posBuffer[offset + 8] = bytes[0];
			posBuffer[offset + 9] = bytes[1];
			posBuffer[offset + 10] = bytes[2];
			posBuffer[offset + 11] = bytes[3];
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000E6C0 File Offset: 0x0000C8C0
		public static void DecompressPosition(byte[] data, int offset, out Vector3 vec)
		{
			vec.x = BitConverter.ToSingle(data, offset);
			vec.y = BitConverter.ToSingle(data, offset + 4);
			vec.z = BitConverter.ToSingle(data, offset + 8);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000023AB File Offset: 0x000005AB
		public static void CompressFloat(float num, byte[] Buffer, int offset)
		{
			NetworkCompression.WriteUInt16((ushort)((num - AdNetworkCompression.MinNum) * AdNetworkCompression.MaxShortNum), Buffer, offset);
		}

		// Token: 0x06000085 RID: 133 RVA: 0x000023C4 File Offset: 0x000005C4
		public static void DecompressFloat(byte[] data, int offset, out float num)
		{
			num = AdNetworkCompression.MinNum + AdNetworkCompression.ShortMaxNum * (float)NetworkCompression.ReadUInt16(data, offset);
		}

		// Token: 0x04000124 RID: 292
		public static double maxSize = 20000000.0;

		// Token: 0x04000125 RID: 293
		private static double intmax = 4294967295.0;

		// Token: 0x04000126 RID: 294
		public static double wMinX = -10000000.0;

		// Token: 0x04000127 RID: 295
		public static double wMinY = -10000000.0;

		// Token: 0x04000128 RID: 296
		public static double wMinZ = -10000000.0;

		// Token: 0x04000129 RID: 297
		public static double wMaxX = 10000000.0;

		// Token: 0x0400012A RID: 298
		public static double wMaxY = 10000000.0;

		// Token: 0x0400012B RID: 299
		public static double wMaxZ = 10000000.0;

		// Token: 0x0400012C RID: 300
		private static double wShortMaxX = 0.004656612874161595;

		// Token: 0x0400012D RID: 301
		private static double wShortMaxY = 0.004656612874161595;

		// Token: 0x0400012E RID: 302
		private static double wShortMaxZ = 0.004656612874161595;

		// Token: 0x0400012F RID: 303
		private static double wMaxShortX = 214.74836475;

		// Token: 0x04000130 RID: 304
		private static double wMaxShortY = 214.74836475;

		// Token: 0x04000131 RID: 305
		private static double wMaxShortZ = 214.74836475;

		// Token: 0x04000132 RID: 306
		public static float MinNum = -1000f;

		// Token: 0x04000133 RID: 307
		public static float MaxNum = 1000f;

		// Token: 0x04000134 RID: 308
		public static float ShortMaxNum = 0f;

		// Token: 0x04000135 RID: 309
		public static float MaxShortNum = 65f;
	}
}
