using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;
namespace skpCustomModule
{
	// Token: 0x02000045 RID: 69
	public class OnJointBrakeBehavour : MonoBehaviour
	{
		// Token: 0x0600016E RID: 366 RVA: 0x000174C0 File Offset: 0x000156C0
		public void Update()
		{
			bool flag = !this.jointTransform.gameObject.activeSelf;
			if (flag)
			{
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x040002CF RID: 719
		public Transform jointTransform;
	}
}
