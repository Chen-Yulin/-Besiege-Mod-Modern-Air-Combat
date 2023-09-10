using System;
using System.Xml.Serialization;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000060 RID: 96
	[Serializable]
	public struct ModVector3
	{
		// Token: 0x06000244 RID: 580 RVA: 0x000229C8 File Offset: 0x00020BC8
		public override string ToString()
		{
			return string.Format("({0}, {1}, {2})", this.x, this.y, this.z);
		}

		// Token: 0x06000245 RID: 581 RVA: 0x00022A08 File Offset: 0x00020C08
		public static implicit operator Vector3(ModVector3 sV)
		{
			Vector3 result = default(Vector3);
			result.x = sV.x;
			result.y = sV.y;
			result.z = sV.z;
			return result;
		}

		// Token: 0x04000481 RID: 1153
		[XmlAttribute]
		public float x;

		// Token: 0x04000482 RID: 1154
		[XmlAttribute]
		public float y;

		// Token: 0x04000483 RID: 1155
		[XmlAttribute]
		public float z;
	}
}
