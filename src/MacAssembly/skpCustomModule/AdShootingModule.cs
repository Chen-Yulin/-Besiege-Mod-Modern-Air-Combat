using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Modding;
using Modding.Blocks;
using Modding.Levels;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace skpCustomModule
{
	// Token: 0x02000029 RID: 41
	public class AdShootingModule : SingleInstance<AdShootingModule>
	{
		// Token: 0x17000020 RID: 32
		// (get) Token: 0x060000D0 RID: 208 RVA: 0x000025C6 File Offset: 0x000007C6
		public override string Name
		{
			get
			{
				return "AdShootingModuleObject";
			}
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x060000D1 RID: 209 RVA: 0x000025CD File Offset: 0x000007CD
		// (set) Token: 0x060000D2 RID: 210 RVA: 0x000025D5 File Offset: 0x000007D5
		internal PlayerMachineInfo PMI { get; set; }

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x060000D3 RID: 211 RVA: 0x000025DE File Offset: 0x000007DE
		// (set) Token: 0x060000D4 RID: 212 RVA: 0x000025E6 File Offset: 0x000007E6
		public Asset_Explosion Explosion { get; private set; }

		// Token: 0x060000D5 RID: 213 RVA: 0x0001194C File Offset: 0x0000FB4C
		private void Awake()
		{
			this.SetProjectileManager();
			this.SetAudioManager();
			this.AdShootingModuleProjectiles = new Dictionary<int, string>();
			RuntimePlatform platform = Application.platform;
			RuntimePlatform runtimePlatform = platform;
			/*
			if (runtimePlatform != (RuntimePlatform)1)
			{
				if (runtimePlatform != (RuntimePlatform)2)
				{
					if (runtimePlatform != (RuntimePlatform)13)
					{
						this.modAssetBundle = ModResource.GetAssetBundle("acmasset");
					}
					else
					{
						this.modAssetBundle = ModResource.GetAssetBundle("acmassetMac");
					}
				}
				else
				{
					this.modAssetBundle = ModResource.GetAssetBundle("acmasset");
				}
			}
			else
			{
				this.modAssetBundle = ModResource.GetAssetBundle("acmassetMac");
			}*/
			for (int i = 0; i < 20; i++)
			{
				this.MissileToTargetListContainer.Add(i, new List<int>());
			}
			this.SetUIcanvas();
			Events.OnBlockInit += this.AddScript;
			base.StartCoroutine(this.AttachScript());
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00011A24 File Offset: 0x0000FC24
		public void Update()
		{
			bool levelSimulating = StatMaster.levelSimulating;
			if (levelSimulating)
			{
				bool flag = !this.SimulationStartState;
				if (flag)
				{
					bool isClient = StatMaster.isClient;
					if (!isClient)
					{
						bool isHosting = StatMaster.isHosting;
						if (!isHosting)
						{
							bool isLocalSim = StatMaster.isLocalSim;
							if (isLocalSim)
							{
							}
						}
					}
					this.SimulationStartState = true;
					base.StartCoroutine(this.CheckEntity());
				}
				bool flag2 = !this.CoreScriptEnable;
				if (flag2)
				{
				}
			}
			else
			{
				bool simulationStartState = this.SimulationStartState;
				if (simulationStartState)
				{
					this.SimulationStartState = false;
					this.EntityMarkerContainer.Clear();
					this.EntityCount = 0;
					this.MissileNum = 0;
				}
			}
			bool isClient2 = StatMaster.isClient;
			if (isClient2)
			{
			}
			bool isHosting2 = StatMaster.isHosting;
			if (isHosting2)
			{
			}
			bool isLocalSim2 = StatMaster.isLocalSim;
			if (isLocalSim2)
			{
			}
			bool isMP = StatMaster.isMP;
			if (isMP)
			{
			}
		}

		// Token: 0x060000D7 RID: 215 RVA: 0x00011B00 File Offset: 0x0000FD00
		public void PostRegisterPrefabs()
		{
			Debug.Log("PostRegisterPrefabs");
			this.AdShootingModuleProjectiles.Clear();
			List<BlockPrefab> source = new List<BlockPrefab>(PrefabMaster.BlockPrefabs.Values);
			IOrderedEnumerable<BlockPrefab> orderedEnumerable = from b in source
				orderby b.ID
				select b;
			int num = 0;
			foreach (BlockPrefab blockPrefab in orderedEnumerable)
			{
				AdShootingBehavour[] components = blockPrefab.gameObject.GetComponents<AdShootingBehavour>();
				bool flag = components != null;
				if (flag)
				{
					for (int i = 0; i < components.Length; i++)
					{
						Debug.Log("ProjectileId : " + num.ToString());
						components[i].ProjectileId = num;
						num++;
						this.AdShootingModuleProjectiles.Add(components[i].ProjectileId, blockPrefab.name);
					}
				}
			}
			bool flag2 = StatMaster.isMP && AdProjectileManager.Instance;
			if (flag2)
			{
				AdProjectileManager instance = AdProjectileManager.Instance;
				instance.ClearAdditionalProjectiles();
				foreach (BlockPrefab blockPrefab2 in orderedEnumerable)
				{
					AdShootingBehavour[] components2 = blockPrefab2.gameObject.GetComponents<AdShootingBehavour>();
					bool flag3 = components2 != null;
					if (flag3)
					{
						for (int j = 0; j < components2.Length; j++)
						{
							instance.AddAdditionalProjectile(components2[j].ProjectileId, components2[j].GetAdProjectilePrefab());
						}
					}
				}
			}
		}

		// Token: 0x060000D8 RID: 216 RVA: 0x000025EF File Offset: 0x000007EF
		private IEnumerator LoadAssetBundle()
		{
			ModAssetBundle modAssetBundle = ModResource.GetAssetBundle("Effect");
			yield return new WaitUntil(() => modAssetBundle.Available);
			this.Explosion = new Asset_Explosion(modAssetBundle);
			yield break;
		}

		// Token: 0x060000D9 RID: 217 RVA: 0x000025FE File Offset: 0x000007FE
		private IEnumerator AttachScript()
		{
			yield return new WaitForSeconds(2f);
			int num;
			for (int i = 0; i < 20000; i = num + 1)
			{
				try
				{
					bool flag = AdShootingModule.EntityScriptAttachList.Contains(i);
					if (flag)
					{
						PrefabMaster.LevelPrefabs[9].GetValue(i).gameObject.AddComponent<AdLevelBlockBehaviour>();
					}
				}
				catch
				{
					Debug.Log("ACM : Failed to attach script for Entity");
				}
				num = i;
			}
			yield break;
		}

		// Token: 0x060000DA RID: 218 RVA: 0x0000260D File Offset: 0x0000080D
		private IEnumerator CheckEntity()
		{
			yield return new WaitForSeconds(1f);
			foreach (KeyValuePair<long, AdShootingModule.EntityData> contanier in this.EntityMarkerContainer)
			{
				this.EntityCount++;
				bool flag = !this.UIEntitytextContainer.ContainsKey(this.EntityCount);
				if (flag)
				{
					this.UIEntitytextContainer.Add(this.EntityCount, Object.Instantiate(this.UIEntitytextContainer[0], this.UIcanvas.transform) as GameObject);
					this.UIEntityimageContainer.Add(this.EntityCount, Object.Instantiate(this.UIEntityimageContainer[0], this.UIcanvas.transform) as GameObject);
					this.UIEntityDistanceContainer.Add(this.EntityCount, Object.Instantiate(this.UIEntityDistanceContainer[0], this.UIcanvas.transform) as GameObject);
				}
				//contanier = default(KeyValuePair<long, AdShootingModule.EntityData>);
			}
			Dictionary<long, AdShootingModule.EntityData>.Enumerator enumerator = default(Dictionary<long, AdShootingModule.EntityData>.Enumerator);
			yield break;
		}

		// Token: 0x060000DB RID: 219 RVA: 0x00011CD0 File Offset: 0x0000FED0
		private void SetProjectileManager()
		{
			this.projectileManager = new GameObject("PManager");
			this.projectileManager.AddComponent<AdProjectileManager>();
			this.PMcomponent = this.projectileManager.GetComponent<AdProjectileManager>();
			Object.DontDestroyOnLoad(this.projectileManager);
			this.PMEffectPool = new GameObject("EffectPool");
			this.PMEffectPool.transform.SetParent(this.projectileManager.transform);
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0000261C File Offset: 0x0000081C
		private void SetAudioManager()
		{
			this.AdAudioManager = new GameObject("AdAudioManager");
			this.AdAudioManager.AddComponent<AdSoundController>();
			Object.DontDestroyOnLoad(this.AdAudioManager);
		}

		// Token: 0x060000DD RID: 221 RVA: 0x00011D44 File Offset: 0x0000FF44
		private void SetUIcanvas()
		{
			this.UIcanvas = new GameObject("AdUIcanvas");
			Canvas canvas = this.UIcanvas.AddComponent<Canvas>();
			canvas.renderMode = (RenderMode)1;
			CanvasScaler canvasScaler = this.UIcanvas.AddComponent<CanvasScaler>();
			canvasScaler.uiScaleMode = (CanvasScaler.ScaleMode)1;
			canvasScaler.referenceResolution = new Vector2(1920f, 1080f);
			canvasScaler.screenMatchMode = 0;
			canvasScaler.matchWidthOrHeight = 0f;
			canvasScaler.referencePixelsPerUnit = 100f;
			Object.DontDestroyOnLoad(this.UIcanvas);
			GameObject gameObject = new GameObject("UItext");
			gameObject.transform.SetParent(this.UIcanvas.transform);
			Text text = gameObject.AddComponent<Text>();
			//this.UIfont = this.modAssetBundle.LoadAsset<Font>("FontNumGreen");
			text.font = this.UIfont;
			text.color = Color.white;
			text.alignment = 0;
			RectTransform component = gameObject.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(500f, 200f);
			component.pivot = new Vector2(0f, 1f);
			gameObject.SetActive(false);
			for (int i = 0; i < 40; i++)
			{
				bool flag = !this.UItextContainer.ContainsKey(i);
				if (flag)
				{
					this.UItextContainer.Add(i, Object.Instantiate(gameObject, this.UIcanvas.transform) as GameObject);
				}
			}
			bool flag2 = !this.UIEntityDistanceContainer.ContainsKey(0);
			if (flag2)
			{
				this.UIEntityDistanceContainer.Add(0, Object.Instantiate(gameObject, this.UIcanvas.transform) as GameObject);
			}
			gameObject = new GameObject("UItext");
			gameObject.transform.SetParent(this.UIcanvas.transform);
			text = gameObject.AddComponent<Text>();
			//this.UIfont = this.modAssetBundle.LoadAsset<Font>("rounded-mgenplus-1c-medium");
			text.font = this.UIfont;
			text.fontSize = 20;
			text.color = Color.white;
			text.alignment = 0;
			component = gameObject.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(500f, 200f);
			component.pivot = new Vector2(0f, 1f);
			gameObject.SetActive(false);
			for (int j = 40; j < 60; j++)
			{
				bool flag3 = !this.UItextContainer.ContainsKey(j);
				if (flag3)
				{
					this.UItextContainer.Add(j, Object.Instantiate(gameObject, this.UIcanvas.transform) as GameObject);
				}
			}
			bool flag4 = !this.UIEntitytextContainer.ContainsKey(0);
			if (flag4)
			{
				this.UIEntitytextContainer.Add(0, Object.Instantiate(gameObject, this.UIcanvas.transform) as GameObject);
			}
			GameObject gameObject2 = new GameObject("UIimage");
			gameObject2.transform.SetParent(this.UIcanvas.transform);
			Image image = gameObject2.AddComponent<Image>();
			image.type = 0;
			image.raycastTarget = false;
			RectTransform component2 = gameObject2.GetComponent<RectTransform>();
			component2.sizeDelta = new Vector2(1080f, 1080f);
			component2.pivot = new Vector2(0.5f, 0.5f);
			gameObject2.SetActive(false);
			for (int k = 0; k < 140; k++)
			{
				bool flag5 = !this.UIimageContainer.ContainsKey(k);
				if (flag5)
				{
					this.UIimageContainer.Add(k, Object.Instantiate(gameObject2, this.UIcanvas.transform) as GameObject);
				}
			}
			bool flag6 = !this.UIEntityimageContainer.ContainsKey(0);
			if (flag6)
			{
				this.UIEntityimageContainer.Add(0, Object.Instantiate(gameObject2, this.UIcanvas.transform) as GameObject);
			}
			GameObject gameObject3 = new GameObject("UImaskedimageObject");
			gameObject3.transform.SetParent(this.UIcanvas.transform);
			Image image2 = gameObject3.AddComponent<Image>();
			image2.type = 0;
			image2.raycastTarget = false;
			RectTransform rectTransform = image2.rectTransform;
			rectTransform.sizeDelta = new Vector2(1080f, 1080f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			gameObject3.SetActive(false);
			for (int l = 0; l < 100; l++)
			{
				bool flag7 = !this.UImaskedimageContainer.ContainsKey(l);
				if (flag7)
				{
					this.UImaskedimageContainer.Add(l, Object.Instantiate(gameObject3, this.UIcanvas.transform) as GameObject);
				}
			}
		}

		// Token: 0x060000DE RID: 222 RVA: 0x00012218 File Offset: 0x00010418
		private void AddScript(Block block)
		{
		}

		// Token: 0x060000DF RID: 223 RVA: 0x000122A0 File Offset: 0x000104A0
		private void AddEntityScript(Entity entity)
		{
			LevelEntity internalObject = entity.InternalObject;
			GenericEntity behaviour = internalObject.behaviour;
			bool flag = this.EntityScriptAttachContainer.ContainsKey(behaviour.prefab.ID);
			if (flag)
			{
				Type type = this.EntityScriptAttachContainer[behaviour.prefab.ID];
				try
				{
					bool flag2 = internalObject.GetComponent(type) == null;
					if (flag2)
					{
						internalObject.gameObject.AddComponent(type);
					}
				}
				catch
				{
					ModConsole.Log("AddScript Error");
				}
			}
		}

		// Token: 0x040001ED RID: 493
		private bool SimulationStartState = false;

		// Token: 0x040001EE RID: 494
		private bool CameraGetting = false;

		// Token: 0x040001EF RID: 495
		private bool CoreScriptEnable = false;

		// Token: 0x040001F2 RID: 498
		public Dictionary<string, GameObject> ExplosionContainer = new Dictionary<string, GameObject>();

		// Token: 0x040001F3 RID: 499
		public Dictionary<string, List<Transform>> ExplosionEffectContainer = new Dictionary<string, List<Transform>>();

		// Token: 0x040001F4 RID: 500
		public Dictionary<string, GameObject> ShotFlashContainer = new Dictionary<string, GameObject>();

		// Token: 0x040001F5 RID: 501
		public Dictionary<string, GameObject> TrailContainer = new Dictionary<string, GameObject>();

		// Token: 0x040001F6 RID: 502
		public Dictionary<string, List<Transform>> TrailEffectContainer = new Dictionary<string, List<Transform>>();

		// Token: 0x040001F7 RID: 503
		public Dictionary<string, GameObject> BulletEffectContainer = new Dictionary<string, GameObject>();

		// Token: 0x040001F8 RID: 504
		public Dictionary<string, GameObject> ChaffContainer = new Dictionary<string, GameObject>();

		// Token: 0x040001F9 RID: 505
		public Dictionary<string, ModAudioClip> ExplodeSoundContainer = new Dictionary<string, ModAudioClip>();

		// Token: 0x040001FA RID: 506
		public Dictionary<string, ModAudioClip> ProjectileSoundContainer = new Dictionary<string, ModAudioClip>();

		// Token: 0x040001FB RID: 507
		public Dictionary<int, string> AdShootingModuleProjectiles = new Dictionary<int, string>();

		// Token: 0x040001FC RID: 508
		public Dictionary<int, int> ProjectilePoolMaching = new Dictionary<int, int>();

		// Token: 0x040001FD RID: 509
		public Dictionary<int, int> ProjectileSkinMaching = new Dictionary<int, int>();

		// Token: 0x040001FE RID: 510
		public GameObject projectileManager;

		// Token: 0x040001FF RID: 511
		public GameObject PMEffectPool;

		// Token: 0x04000200 RID: 512
		public AdProjectileManager PMcomponent;

		// Token: 0x04000201 RID: 513
		public Dictionary<string, ModAudioClip> AdLoopAudioSourceContainer = new Dictionary<string, ModAudioClip>();

		// Token: 0x04000202 RID: 514
		public Dictionary<string, ModAudioClip> AdSeAudioSourceContainer = new Dictionary<string, ModAudioClip>();

		// Token: 0x04000203 RID: 515
		public GameObject AdAudioManager;

		// Token: 0x04000204 RID: 516
		public GameObject UIcanvas;

		// Token: 0x04000205 RID: 517
		public Font UIfont;

		// Token: 0x04000206 RID: 518
		public Dictionary<int, GameObject> UItextContainer = new Dictionary<int, GameObject>();

		// Token: 0x04000207 RID: 519
		public Dictionary<int, GameObject> UIimageContainer = new Dictionary<int, GameObject>();

		// Token: 0x04000208 RID: 520
		public Dictionary<int, GameObject> UImaskedimageContainer = new Dictionary<int, GameObject>();

		// Token: 0x04000209 RID: 521

		// Token: 0x0400020A RID: 522
		public Dictionary<int, GameObject> UIEntitytextContainer = new Dictionary<int, GameObject>();

		// Token: 0x0400020B RID: 523
		public Dictionary<int, GameObject> UIEntityDistanceContainer = new Dictionary<int, GameObject>();

		// Token: 0x0400020C RID: 524
		public Dictionary<int, GameObject> UIEntityimageContainer = new Dictionary<int, GameObject>();

		// Token: 0x0400020D RID: 525
		public Dictionary<int, Type> EntityScriptAttachContainer = new Dictionary<int, Type> { 
		{
			7000,
			typeof(AdLevelBlockBehaviour)
		} };

		// Token: 0x0400020E RID: 526
		public static List<int> EntityScriptAttachList = new List<int> { 7000 };

		// Token: 0x0400020F RID: 527
		public Dictionary<int, AdShootingModule.BeaconData> BeaconContainer = new Dictionary<int, AdShootingModule.BeaconData>();

		// Token: 0x04000210 RID: 528
		public Dictionary<long, AdShootingModule.EntityData> EntityMarkerContainer = new Dictionary<long, AdShootingModule.EntityData>();

		// Token: 0x04000211 RID: 529
		public int EntityCount = 0;

		// Token: 0x04000212 RID: 530
		public Dictionary<int, List<int>> TaregtIdListContainer = new Dictionary<int, List<int>>();

		// Token: 0x04000213 RID: 531
		public Dictionary<int, List<int>> MissileToTargetListContainer = new Dictionary<int, List<int>>();

		// Token: 0x04000214 RID: 532
		public Dictionary<int, Transform> MissilePositionContainer = new Dictionary<int, Transform>();

		// Token: 0x04000215 RID: 533
		public Dictionary<int, bool> PlayerRespawnFlagContainer = new Dictionary<int, bool>();

		// Token: 0x04000216 RID: 534
		public int MissileNum = 0;

		// Token: 0x04000217 RID: 535
		public Dictionary<int, float> CoreSpeedContainer = new Dictionary<int, float>();

		// Token: 0x04000218 RID: 536
		public Dictionary<int, float> CoreAccelerationContainer = new Dictionary<int, float>();

		// Token: 0x04000219 RID: 537
		public int ProjectilePoolCounter = 0;

		// Token: 0x0400021A RID: 538
		public Dictionary<string, AdSkinLoader.AdSkinDataPack> ProjectileSkinPackContainer = new Dictionary<string, AdSkinLoader.AdSkinDataPack>();

		// Token: 0x0400021B RID: 539
		public KeyMachingTable ProjectileSkinTable = new KeyMachingTable();

		// Token: 0x0400021C RID: 540
		//public ModAssetBundle modAssetBundle;

		// Token: 0x0400021D RID: 541
		public int ExplosionEffectCount = 0;

		// Token: 0x0400021E RID: 542
		public Dictionary<string, int> ExplosionEffectCountContainer = new Dictionary<string, int>();

		// Token: 0x0400021F RID: 543
		public int TrailEffectCount = 0;

		// Token: 0x04000220 RID: 544
		public int MAX_Effect_POOL_SIZE = 300;

		// Token: 0x0200002A RID: 42
		public class BeaconData
		{
			// Token: 0x04000221 RID: 545
			public Vector3 Posi;

			// Token: 0x04000222 RID: 546
			public string Name;

			// Token: 0x04000223 RID: 547
			public MPTeam Team;
		}

		// Token: 0x0200002B RID: 43
		public class EntityData
		{
			// Token: 0x04000224 RID: 548
			public Transform transform;

			// Token: 0x04000225 RID: 549
			public string Name;
		}
	}
}
