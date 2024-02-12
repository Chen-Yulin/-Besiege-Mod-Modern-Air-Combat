using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Modding;
using Modding.Common;
using UnityEngine;
using UnityEngine.Networking.Types;

namespace Navalmod
{
    public class H3NetCompression
    {
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

    }
}
