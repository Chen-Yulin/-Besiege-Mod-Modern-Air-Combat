using System;
using UnityEngine;

namespace ModernAirCombat
{
	// Token: 0x0200001D RID: 29
	public static class AdNetworkCompression
	{
		// Token: 0x06000117 RID: 279 RVA: 0x00012DD0 File Offset: 0x00010FD0
		public static void BoundarySize()
		{
			AdNetworkCompression.intmax = 4294967295.0;
			AdNetworkCompression.wMinX = -AdNetworkCompression.maxSize / 2.0;
			AdNetworkCompression.wMinY = -AdNetworkCompression.maxSize / 2.0;
			AdNetworkCompression.wMinZ = -AdNetworkCompression.maxSize / 2.0;
			AdNetworkCompression.wMaxX = AdNetworkCompression.maxSize / 2.0;
			AdNetworkCompression.wMaxY = AdNetworkCompression.maxSize / 2.0;
			AdNetworkCompression.wMaxZ = AdNetworkCompression.maxSize / 2.0;
			AdNetworkCompression.wShortMaxX = AdNetworkCompression.maxSize / AdNetworkCompression.intmax;
			AdNetworkCompression.wShortMaxY = AdNetworkCompression.maxSize / AdNetworkCompression.intmax;
			AdNetworkCompression.wShortMaxZ = AdNetworkCompression.maxSize / AdNetworkCompression.intmax;
			AdNetworkCompression.wMaxShortX = AdNetworkCompression.intmax / AdNetworkCompression.maxSize;
			AdNetworkCompression.wMaxShortY = AdNetworkCompression.intmax / AdNetworkCompression.maxSize;
			AdNetworkCompression.wMaxShortZ = AdNetworkCompression.intmax / AdNetworkCompression.maxSize;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x00012EC8 File Offset: 0x000110C8
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

		// Token: 0x06000119 RID: 281 RVA: 0x00012F5B File Offset: 0x0001115B
		public static void DecompressPosition(byte[] data, int offset, out Vector3 vec)
		{
			vec.x = BitConverter.ToSingle(data, offset);
			vec.y = BitConverter.ToSingle(data, offset + 4);
			vec.z = BitConverter.ToSingle(data, offset + 8);
		}

		// Token: 0x0600011A RID: 282 RVA: 0x00012F89 File Offset: 0x00011189
		public static void CompressFloat(float num, byte[] Buffer, int offset)
		{
			NetworkCompression.WriteUInt16((ushort)((num - AdNetworkCompression.MinNum) * AdNetworkCompression.MaxShortNum), Buffer, offset);
		}

		// Token: 0x0600011B RID: 283 RVA: 0x00012FA2 File Offset: 0x000111A2
		public static void DecompressFloat(byte[] data, int offset, out float num)
		{
			num = AdNetworkCompression.MinNum + AdNetworkCompression.ShortMaxNum * (float)NetworkCompression.ReadUInt16(data, offset);
		}

		// Token: 0x04000170 RID: 368
		public static double maxSize = 20000000.0;

		// Token: 0x04000171 RID: 369
		private static double intmax = 4294967295.0;

		// Token: 0x04000172 RID: 370
		public static double wMinX = -10000000.0;

		// Token: 0x04000173 RID: 371
		public static double wMinY = -10000000.0;

		// Token: 0x04000174 RID: 372
		public static double wMinZ = -10000000.0;

		// Token: 0x04000175 RID: 373
		public static double wMaxX = 10000000.0;

		// Token: 0x04000176 RID: 374
		public static double wMaxY = 10000000.0;

		// Token: 0x04000177 RID: 375
		public static double wMaxZ = 10000000.0;

		// Token: 0x04000178 RID: 376
		private static double wShortMaxX = 0.004656612874161595;

		// Token: 0x04000179 RID: 377
		private static double wShortMaxY = 0.004656612874161595;

		// Token: 0x0400017A RID: 378
		private static double wShortMaxZ = 0.004656612874161595;

		// Token: 0x0400017B RID: 379
		private static double wMaxShortX = 214.74836475;

		// Token: 0x0400017C RID: 380
		private static double wMaxShortY = 214.74836475;

		// Token: 0x0400017D RID: 381
		private static double wMaxShortZ = 214.74836475;

		// Token: 0x0400017E RID: 382
		public static float MinNum = -1000f;

		// Token: 0x0400017F RID: 383
		public static float MaxNum = 1000f;

		// Token: 0x04000180 RID: 384
		public static float ShortMaxNum = 0f;

		// Token: 0x04000181 RID: 385
		public static float MaxShortNum = 65f;
	}
}
