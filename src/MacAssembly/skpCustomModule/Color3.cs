using System;
using System.Xml.Serialization;

namespace skpCustomModule
{
	// Token: 0x02000021 RID: 33
	public struct Color3
	{
		// Token: 0x060000BD RID: 189 RVA: 0x000024D0 File Offset: 0x000006D0
		public Color3(float cr, float cg, float cb)
		{
			this.r = cr;
			this.g = cg;
			this.b = cb;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x000115E4 File Offset: 0x0000F7E4
		public override string ToString()
		{
			return string.Format("({0}, {1}, {2})", this.r, this.g, this.b);
		}

		// Token: 0x0400019A RID: 410
		[XmlAttribute]
		public float r;

		// Token: 0x0400019B RID: 411
		[XmlAttribute]
		public float g;

		// Token: 0x0400019C RID: 412
		[XmlAttribute]
		public float b;
	}
}
