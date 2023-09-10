using System;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000004 RID: 4
	public class AdLevelBlockBehaviour : MonoBehaviour
	{
		// Token: 0x0600000F RID: 15 RVA: 0x00002155 File Offset: 0x00000355
		public void Awake()
		{
			this.EntityData = base.GetComponent<LevelEntity>();
			this.EntityBehaviour = this.EntityData.behaviour;
			this.FunctionToggle = this.EntityBehaviour.AddToggle("Beacon", "AdBeacon", false);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00004A7C File Offset: 0x00002C7C
		public void OnDisable()
		{
			bool isActive = this.FunctionToggle.IsActive;
			if (isActive)
			{
				bool flag = AdCustomModuleMod.mod2.EntityMarkerContainer.ContainsKey(this.EntityData.identifier);
				if (flag)
				{
					AdCustomModuleMod.mod2.EntityMarkerContainer.Remove(this.EntityData.identifier);
				}
			}
			bool init = this.Init;
			if (init)
			{
				this.Init = false;
			}
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002191 File Offset: 0x00000391
		public void OnEnable()
		{
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00004AEC File Offset: 0x00002CEC
		public void Update()
		{
			bool levelSimulating = StatMaster.levelSimulating;
			if (levelSimulating)
			{
				bool flag = !this.Init;
				if (flag)
				{
					this.Init = true;
					bool isActive = this.FunctionToggle.IsActive;
					if (isActive)
					{
						bool flag2 = !AdCustomModuleMod.mod2.EntityMarkerContainer.ContainsKey(this.EntityData.identifier);
						if (flag2)
						{
							this.entityData = new AdShootingModule.EntityData();
							this.entityData.Name = this.EntityBehaviour.LogicName();
							this.entityData.transform = base.transform;
							AdCustomModuleMod.mod2.EntityMarkerContainer.Add(this.EntityData.identifier, this.entityData);
						}
					}
				}
			}
			else
			{
				bool init = this.Init;
				if (init)
				{
					this.Init = false;
				}
			}
		}

		// Token: 0x04000033 RID: 51
		private bool Init = false;

		// Token: 0x04000034 RID: 52
		private int ID = 0;

		// Token: 0x04000035 RID: 53
		private LevelEntity EntityData;

		// Token: 0x04000036 RID: 54
		private GenericEntity EntityBehaviour;

		// Token: 0x04000037 RID: 55
		public AdShootingModule.EntityData entityData;

		// Token: 0x04000038 RID: 56
		private MToggle FunctionToggle;
	}
}
