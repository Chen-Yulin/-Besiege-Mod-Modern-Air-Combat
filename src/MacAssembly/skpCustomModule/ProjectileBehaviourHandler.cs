using System;
using Modding.Blocks;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000057 RID: 87
	public class ProjectileBehaviourHandler : BlockBehaviour, ILimitsDisplay
	{
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x060001E7 RID: 487 RVA: 0x0001F7B8 File Offset: 0x0001D9B8
		public PlayerMachine Machine
		{
			get
			{
				bool flag = this._machine == null || this._machine.InternalObject == null;
				PlayerMachine result;
				if (flag)
				{
					result = (this._machine = PlayerMachine.From(base.ParentMachine));
				}
				else
				{
					result = this._machine;
				}
				return result;
			}
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x0001F810 File Offset: 0x0001DA10
		public virtual Transform GetLimitsDisplay()
		{
			return this.VisualController.Block.MeshRenderer.transform;
		}

		// Token: 0x04000417 RID: 1047
		private PlayerMachine _machine;
	}
}
