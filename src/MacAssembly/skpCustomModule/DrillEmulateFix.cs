using System;

namespace skpCustomModule
{
	// Token: 0x02000012 RID: 18
	public class DrillEmulateFix : AddScriptBase
	{
		// Token: 0x06000055 RID: 85 RVA: 0x00002316 File Offset: 0x00000516
		public override void SafeAwake()
		{
			this.ObjectBehavior.Prefab.RegisterEmulationUpdate = true;
		}
	}
}
