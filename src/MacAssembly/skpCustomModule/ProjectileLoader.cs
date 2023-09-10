using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace skpCustomModule
{
	// Token: 0x0200005F RID: 95
	public class ProjectileLoader : SingleInstance<ProjectileLoader>
	{
		// Token: 0x1700004D RID: 77
		// (get) Token: 0x0600023C RID: 572 RVA: 0x00002DEA File Offset: 0x00000FEA
		public override string Name
		{
			get
			{
				return "ProjectileLoader";
			}
		}

		// Token: 0x0600023D RID: 573 RVA: 0x00002191 File Offset: 0x00000391
		private void Awake()
		{
		}

		// Token: 0x0600023E RID: 574 RVA: 0x00002DF1 File Offset: 0x00000FF1
		public override void SetUp()
		{
			this.SetTemplate();
			this.SetVisualTemplate();
			this.SetShootingSoundControl();
		}

		// Token: 0x0600023F RID: 575 RVA: 0x00002DF1 File Offset: 0x00000FF1
		public void Setting()
		{
			this.SetTemplate();
			this.SetVisualTemplate();
			this.SetShootingSoundControl();
		}

		// Token: 0x06000240 RID: 576 RVA: 0x000226E8 File Offset: 0x000208E8
		public void SetTemplate()
		{
			bool flag = this.ProjectileTemplate != null;
			if (!flag)
			{
				this.ProjectileTemplate = new GameObject();
				this.ProjectileTemplate.name = "AdProjectile";
				this.ProjectileTemplate.transform.localScale = new Vector3(1f, 1f, 1f);
				this.ProjectileTemplate.AddComponent<Rigidbody>();
				Transform transform = new GameObject("Vis").transform;
				Transform transform2 = new GameObject("Gyro").transform;
				transform2.parent = this.ProjectileTemplate.transform;
				transform.parent = transform2;
				Transform transform3 = new GameObject("Colliders").transform;
				transform3.parent = transform2;
				Transform transform4 = new GameObject("FireControl").transform;
				transform4.parent = transform2;
				Transform transform5 = new GameObject("Fire Particles").transform;
				transform5.parent = transform2;
				transform.gameObject.AddComponent<MeshFilter>();
				transform.gameObject.AddComponent<MeshRenderer>();
				this.ProjectileTemplate.AddComponent<ProjectileInfo>();
				ProjectileInfo component = this.ProjectileTemplate.GetComponent<ProjectileInfo>();
				component.noRigidbody = false;
				this.ProjectileTemplate.AddComponent<AdProjectileScript>();
				this.ProjectileTemplate.AddComponent<WaterProjectileBehaviour>();
				this.ProjectileTemplate.AddComponent<AdNetworkProjectile>();
				this.ProjectileTemplate.AddComponent<AdCollisionExplosionComponent>();
				this.ProjectileTemplate.AddComponent<RandomSoundController>();
				this.ProjectileTemplate.AddComponent<AudioSource>();
				Renderer component2 = transform.gameObject.GetComponent<Renderer>();
				Shader shader = Shader.Find("Instanced/Block Shader (GPUI off)");
				component2.material.shader = shader;
				this.ProjectileTemplate.gameObject.SetActive(false);
				Object.DontDestroyOnLoad(this.ProjectileTemplate);
			}
		}

		// Token: 0x06000241 RID: 577 RVA: 0x000228AC File Offset: 0x00020AAC
		public void SetShootingSoundControl()
		{
			bool flag = this.ShootingSoundControl != null;
			if (!flag)
			{
				this.ShootingSoundControl = new GameObject();
				this.ShootingSoundControl.name = "AdSoundControl";
				this.ShootingSoundControl.AddComponent<RandomSoundController>();
				this.ShootingSoundControl.AddComponent<AudioSource>();
				AudioSource component = this.ShootingSoundControl.GetComponent<AudioSource>();
				component.spatialBlend = 1f;
				component.minDistance = 10f;
				this.ShootingSoundControl.gameObject.SetActive(false);
				Object.DontDestroyOnLoad(this.ShootingSoundControl);
			}
		}

		// Token: 0x06000242 RID: 578 RVA: 0x00022948 File Offset: 0x00020B48
		public void SetVisualTemplate()
		{
			bool flag = this.ShootingDirectionVisual != null;
			if (!flag)
			{
				this.ShootingDirectionVisual = new GameObject();
				this.ShootingDirectionVisual.name = "AdShootingVisual";
				Transform transform = new GameObject("Vis").transform;
				transform.parent = this.ShootingDirectionVisual.transform;
				this.ShootingDirectionVisual.gameObject.SetActive(false);
				Object.DontDestroyOnLoad(this.ShootingDirectionVisual);
			}
		}

		// Token: 0x0400047D RID: 1149
		public GameObject ProjectileTemplate;

		// Token: 0x0400047E RID: 1150
		public GameObject ShootingDirectionVisual;

		// Token: 0x0400047F RID: 1151
		public GameObject ShootingSoundControl;

		// Token: 0x04000480 RID: 1152
		public bool flag = false;
	}
}
