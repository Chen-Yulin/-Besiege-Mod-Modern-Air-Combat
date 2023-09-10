using System;
using System.Collections;
using Modding;
using UnityEngine;
using Object = UnityEngine.Object;

namespace skpCustomModule
{
	// Token: 0x02000017 RID: 23
	public class AdCustomModule : SingleInstance<AdCustomModule>
	{
		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000087 RID: 135 RVA: 0x000023DE File Offset: 0x000005DE
		public override string Name
		{
			get
			{
				return "AdCustomModuleObject";
			}
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000023E5 File Offset: 0x000005E5
		private void Awake()
		{
			base.StartCoroutine(this.CheckVersion());
		}

		// Token: 0x06000089 RID: 137 RVA: 0x0000E7F8 File Offset: 0x0000C9F8
		private void Update()
		{
			bool inGlobalPlayMode = StatMaster.InGlobalPlayMode;
			if (inGlobalPlayMode)
			{
				bool flag = !this.SimulationStartState;
				if (flag)
				{
					bool isClient = StatMaster.isClient;
					if (!isClient)
					{
						bool isHosting = StatMaster.isHosting;
						if (isHosting)
						{
						}
					}
					this.SimulationStartState = true;
				}
			}
			else
			{
				bool simulationStartState = this.SimulationStartState;
				if (simulationStartState)
				{
					this.SimulationStartState = false;
				}
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x000023F5 File Offset: 0x000005F5
		public override void SetUp()
		{
			this.SetSubObjectTemplate();
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000E85C File Offset: 0x0000CA5C
		public void SetSubObjectTemplate()
		{
			bool flag = this.SubObjectTemplate != null;
			if (!flag)
			{
				this.SubObjectTemplate = new GameObject();
				this.SubObjectTemplate.name = "SubObjectTemplate";
				this.SubObjectTemplate.transform.localScale = new Vector3(1f, 1f, 1f);
				Transform transform = new GameObject("Vis").transform;
				transform.parent = this.SubObjectTemplate.transform;
				transform.parent = transform;
				transform.gameObject.AddComponent<MeshFilter>();
				transform.gameObject.AddComponent<MeshRenderer>();
				Renderer component = transform.gameObject.GetComponent<Renderer>();
				Shader shader = Shader.Find("Instanced/Block Shader (GPUI off)");
				component.material.shader = shader;
				this.SubObjectTemplate.gameObject.SetActive(false);
				Object.DontDestroyOnLoad(this.SubObjectTemplate);
			}
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00002401 File Offset: 0x00000601
		private IEnumerator CheckVersion()
		{
			yield return new WaitForSeconds(1f);
			string str = "ACM Version :";
			Version version = Mods.GetVersion(new Guid("a4577151-2173-4084-a456-4b29e8d3e01f"));
			Debug.Log(str + ((version != null) ? version.ToString() : null));
			yield break;
		}

		// Token: 0x04000136 RID: 310
		private bool SimulationStartState = false;

		// Token: 0x04000137 RID: 311
		public GameObject SubObjectTemplate;
	}
}
