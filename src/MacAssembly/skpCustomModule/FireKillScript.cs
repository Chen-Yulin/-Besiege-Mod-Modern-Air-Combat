using System;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000011 RID: 17
	public class FireKillScript : AddScriptBase
	{
		// Token: 0x06000052 RID: 82 RVA: 0x0000B4A8 File Offset: 0x000096A8
		public override void SafeAwake()
		{
			bool flag = base.gameObject.GetComponent<Rigidbody>();
			if (flag)
			{
				this.FireControllerCompo = base.gameObject.GetComponent<FireController>();
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x0000B4E0 File Offset: 0x000096E0
		public void FixedUpdate()
		{
			bool flag = this.FireControllerCompo != null;
			if (flag)
			{
				bool flag2 = this.FireControllerCompo.fireProgress >= 0.75f;
				if (flag2)
				{
					bool enabled = this.FireControllerCompo.enabled;
					if (enabled)
					{
						this.FireControllerCompo.enabled = false;
					}
				}
			}
			else
			{
				this.FireControllerCompo = base.gameObject.GetComponent<FireController>();
			}
		}

		// Token: 0x04000105 RID: 261
		private FireTag FireTagCompo;

		// Token: 0x04000106 RID: 262
		private FireController FireControllerCompo;
	}
}
