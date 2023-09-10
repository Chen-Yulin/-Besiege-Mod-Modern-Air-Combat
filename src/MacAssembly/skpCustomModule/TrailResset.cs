using System;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x0200003F RID: 63
	public class TrailResset : MonoBehaviour
	{
		// Token: 0x0600014A RID: 330 RVA: 0x00002191 File Offset: 0x00000391
		private void Awake()
		{
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00016F18 File Offset: 0x00015118
		private void OnEnable()
		{
			try
			{
				this.component1 = base.GetComponent<TrailRenderer>();
				this.component1.Clear();
			}
			catch
			{
				this.Trailditect = false;
			}
			this.count = 0f;
			this.trailposition = Vector3.zero;
			this.preposition = Vector3.zero;
			this.Normalizedposition = Vector3.zero;
			this.NetworkTrailstop = false;
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00016F94 File Offset: 0x00015194
		private void LateUpdate()
		{
			bool flag = StatMaster.levelSimulating && !this.NetworkTrailstop && this.Trailditect;
			if (flag)
			{
				bool isClient = StatMaster.isClient;
				if (isClient)
				{
					bool flag2 = base.transform.position != this.Normalizedposition;
					if (flag2)
					{
						this.preposition = this.Normalizedposition;
					}
					this.Normalizedposition = base.transform.position;
				}
			}
			bool networkTrailstop = this.NetworkTrailstop;
			if (networkTrailstop)
			{
				float num = TimeSlider.Instance.DeltaTime();
				this.count += num;
				bool flag3 = this.count > 3f;
				if (flag3)
				{
					base.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00015BE4 File Offset: 0x00013DE4
		private Transform GetPhysGoal()
		{
			return ReferenceMaster.physicsGoalInstance;
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00002828 File Offset: 0x00000A28
		public void SetComponent()
		{
			this.component2 = base.transform.parent.GetComponent<AdNetworkProjectile>();
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00016F18 File Offset: 0x00015118
		public void ResetTrail()
		{
			try
			{
				this.component1 = base.GetComponent<TrailRenderer>();
				this.component1.Clear();
			}
			catch
			{
				this.Trailditect = false;
			}
			this.count = 0f;
			this.trailposition = Vector3.zero;
			this.preposition = Vector3.zero;
			this.Normalizedposition = Vector3.zero;
			this.NetworkTrailstop = false;
		}

		// Token: 0x04000298 RID: 664
		public TrailRenderer component1;

		// Token: 0x04000299 RID: 665
		public AdNetworkProjectile component2;

		// Token: 0x0400029A RID: 666
		public GameObject TrailsubPrefab;

		// Token: 0x0400029B RID: 667
		public Transform TrailsubTransform;

		// Token: 0x0400029C RID: 668
		public string BlockName;

		// Token: 0x0400029D RID: 669
		public TrailRenderer component3;

		// Token: 0x0400029E RID: 670
		public TrailResset component4;

		// Token: 0x0400029F RID: 671
		public Rigidbody rigibody1;

		// Token: 0x040002A0 RID: 672
		public Vector3 trailposition;

		// Token: 0x040002A1 RID: 673
		public Vector3 preposition;

		// Token: 0x040002A2 RID: 674
		public Quaternion trailrotation;

		// Token: 0x040002A3 RID: 675
		public Quaternion prerotation;

		// Token: 0x040002A4 RID: 676
		public Vector3 Normalizedposition;

		// Token: 0x040002A5 RID: 677
		public bool NetworkTrailstop = false;

		// Token: 0x040002A6 RID: 678
		public float count = 0f;

		// Token: 0x040002A7 RID: 679
		public bool Trailditect = true;
	}
}
