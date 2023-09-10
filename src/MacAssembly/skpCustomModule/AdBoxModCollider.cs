using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Modding.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;

namespace skpCustomModule
{
	// Token: 0x02000022 RID: 34
	[Serializable]
	public class AdBoxModCollider : ModCollider
	{
		// Token: 0x1700001D RID: 29
		// (get) Token: 0x060000BF RID: 191 RVA: 0x000024E8 File Offset: 0x000006E8
		// (set) Token: 0x060000C0 RID: 192 RVA: 0x000024F0 File Offset: 0x000006F0
		[XmlElement]
		public Vector3 Rotation { get; internal set; }

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x000024F9 File Offset: 0x000006F9
		// (set) Token: 0x060000C2 RID: 194 RVA: 0x00002501 File Offset: 0x00000701
		[XmlElement]
		public Vector3 Scale { get; internal set; }

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x060000C3 RID: 195 RVA: 0x0000250A File Offset: 0x0000070A
		// (set) Token: 0x060000C4 RID: 196 RVA: 0x00002512 File Offset: 0x00000712
		[XmlElement]
		public Vector3 ColliderCenter { get; internal set; }

		// Token: 0x060000C5 RID: 197 RVA: 0x0000251B File Offset: 0x0000071B
		public AdBoxModCollider()
		{
			base.Position = Vector3.zero;
			this.Rotation = Vector3.zero;
			this.Scale = Vector3.one;
			this.ColliderCenter = Vector3.zero;
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00011624 File Offset: 0x0000F824
		protected override bool Validate(string elemName)
		{
			bool flag = !base.Validate(elemName);
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = GameObjectHelper.IsVectorNegative(this.Scale);
				result = !flag2 || base.InvalidData(elemName, "Scale may not be negative!");
			}
			return result;
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x00011670 File Offset: 0x0000F870
		public override Collider CreateCollider(Transform parent)
		{
			GameObject gameObject = new GameObject("Box Collider");
			BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
			boxCollider.center = this.ColliderCenter;
			boxCollider.size = Vector3.one;
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = base.Position;
			gameObject.transform.localEulerAngles = this.Rotation;
			gameObject.transform.localScale = this.Scale;
			gameObject.layer = this.SetLayer;
			boxCollider.isTrigger = base.Trigger;
			return boxCollider;
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00011724 File Offset: 0x0000F924
		public override Transform CreateVisual(Transform parent)
		{
			GameObject gameObject = GameObject.CreatePrimitive((PrimitiveType)3);
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = base.Position;
			gameObject.transform.localEulerAngles = this.Rotation;
			gameObject.transform.localScale = this.Scale;
			gameObject.name = "Box Collider";
			gameObject.layer = 25;
			Object.DestroyImmediate(gameObject.GetComponent<Collider>());
			return gameObject.transform;
		}

		// Token: 0x040001A0 RID: 416
		[XmlElement]
		[DefaultValue(0)]
		public int SetLayer;
	}
}
