using System;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000046 RID: 70
	public class WingDragBehavour : MonoBehaviour
	{
		// Token: 0x06000170 RID: 368 RVA: 0x0000299E File Offset: 0x00000B9E
		public void Awake()
		{
			this.myTransform = base.transform;
			this.myRigibody = base.GetComponent<Rigidbody>();
		}

		// Token: 0x06000171 RID: 369 RVA: 0x000174F4 File Offset: 0x000156F4
		public void FixedUpdate()
		{
			this.myVelocity = this.myRigibody.velocity;
			this.myLocalVelocity = this.myTransform.InverseTransformDirection(this.myVelocity);
			this.xyz = Vector3.Scale(-this.myLocalVelocity, this.AxisDrag);
			this.sqr = Math.Min(this.myVelocity.sqrMagnitude, this.cap);
			this.myRigibody.AddRelativeForce(this.xyz.normalized * this.sqr);
		}

		// Token: 0x040002D0 RID: 720
		private Transform myTransform;

		// Token: 0x040002D1 RID: 721
		private Rigidbody myRigibody;

		// Token: 0x040002D2 RID: 722
		private Vector3 AxisDrag = new Vector3(0f, 1f, 0f);

		// Token: 0x040002D3 RID: 723
		private Vector3 myVelocity;

		// Token: 0x040002D4 RID: 724
		private Vector3 myLocalVelocity;

		// Token: 0x040002D5 RID: 725
		private Vector3 xyz;

		// Token: 0x040002D6 RID: 726
		private float sqr;

		// Token: 0x040002D7 RID: 727
		public float cap = 100f;

		// Token: 0x040002D8 RID: 728
		public float resistance = 3f;
	}
}
