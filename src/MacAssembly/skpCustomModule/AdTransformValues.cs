using System;
using Modding.Serialization;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace skpCustomModule
{
	// Token: 0x02000050 RID: 80
	public class AdTransformValues
	{
		// Token: 0x060001AF RID: 431 RVA: 0x0001AB80 File Offset: 0x00018D80
		public void SetOnTransform(Transform t)
		{
			t.localPosition = this.Position;
			t.localRotation = Quaternion.Euler(this.Rotation);
			bool flag = this.hasScale;
			if (flag)
			{
				t.localScale = this.Scale;
			}
		}

		// Token: 0x060001B0 RID: 432 RVA: 0x0001ABC8 File Offset: 0x00018DC8
		public void FlipTransform()
		{
			this.Position.x = -1f * this.Position.x;
			this.Rotation.y = -1f * this.Rotation.y;
			this.Scale.x = -1f * this.Scale.x;
		}

		// Token: 0x060001B1 RID: 433 RVA: 0x0001AC2C File Offset: 0x00018E2C
		public static implicit operator AdTransformValues(TransformValues transformV)
		{
			Vector3 position = transformV.Position;
			Vector3 rotation = transformV.Rotation;
			Vector3 scale = transformV.Scale;
			return new AdTransformValues
			{
				Position = position,
				Rotation = rotation,
				Scale = scale
			};
		}

		// Token: 0x0400037A RID: 890
		public Vector3 Position;

		// Token: 0x0400037B RID: 891
		public Vector3 Rotation;

		// Token: 0x0400037C RID: 892
		public Vector3 Scale;

		// Token: 0x0400037D RID: 893
		private bool hasScale = true;
	}
}
