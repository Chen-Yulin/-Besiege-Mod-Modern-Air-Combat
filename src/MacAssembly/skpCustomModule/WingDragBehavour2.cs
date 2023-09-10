using System;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000047 RID: 71
	public class WingDragBehavour2 : MonoBehaviour
	{
		// Token: 0x06000173 RID: 371 RVA: 0x000029F2 File Offset: 0x00000BF2
		public void Awake()
		{
			this.myTransform = base.transform;
			this.myRigibody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06000174 RID: 372 RVA: 0x00017584 File Offset: 0x00015784
		public void FixedUpdate()
		{
			bool flag = this.DelayTime <= 0f;
			if (flag)
			{
				this.resistance = 10f;
			}
			else
			{
				this.resistance = 1f;
				this.DelayTime -= Time.fixedDeltaTime;
			}
			Vector3 vector = Vector3.Cross(this.myRigibody.velocity, this.myTransform.forward);
			vector = Vector3.Cross(vector, this.myTransform.forward);
			vector = Vector3.Project(this.myRigibody.velocity, vector);
			this.sqr = Math.Min(vector.magnitude * this.resistance, this.cap);
			this.myRigibody.AddForce(-vector.normalized * this.sqr);
		}

		// Token: 0x040002D9 RID: 729
		private Transform myTransform;

		// Token: 0x040002DA RID: 730
		private Rigidbody myRigibody;

		// Token: 0x040002DB RID: 731
		public float cap = 500f;

		// Token: 0x040002DC RID: 732
		private float sqr;

		// Token: 0x040002DD RID: 733
		public float resistance = 10f;

		// Token: 0x040002DE RID: 734
		public float DelayTime = 0f;
	}
}
