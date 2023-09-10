using System;
using System.Collections;
using System.Collections.Generic;
using Besiege;
using MultithreadCoroutines;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000063 RID: 99
	public class AdSkinLoader : SingleInstance<AdSkinLoader>
	{
		// Token: 0x1700004E RID: 78
		// (get) Token: 0x06000248 RID: 584 RVA: 0x00002E1B File Offset: 0x0000101B
		public override string Name
		{
			get
			{
				return "AdSkinLoader";
			}
		}

		// Token: 0x02000064 RID: 100
		public abstract class AdModResource
		{
			// Token: 0x1700004F RID: 79
			// (get) Token: 0x0600024A RID: 586 RVA: 0x00002E2B File Offset: 0x0000102B
			// (set) Token: 0x0600024B RID: 587 RVA: 0x00002E33 File Offset: 0x00001033
			public AdSkinLoader.AdModResource.ResourceType Type { get; protected set; }

			// Token: 0x17000050 RID: 80
			// (get) Token: 0x0600024C RID: 588
			public abstract bool Loaded { get; }

			// Token: 0x17000051 RID: 81
			// (get) Token: 0x0600024D RID: 589
			public abstract bool HasError { get; }

			// Token: 0x17000052 RID: 82
			// (get) Token: 0x0600024E RID: 590
			public abstract string Error { get; }

			// Token: 0x0600024F RID: 591 RVA: 0x00022B18 File Offset: 0x00020D18
			public static AdSkinLoader.AdModMesh LoadMesh(string name, string path)
			{
				AdSkinLoader.AdModMesh adModMesh = new AdSkinLoader.AdModMesh();
				adModMesh.path = path;
				adModMesh.name = name;
				adModMesh.Type = AdSkinLoader.AdModResource.ResourceType.Mesh;
				ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(SingleInstance<AdShootingModule>.Instance, adModMesh.Load());
				return adModMesh;
			}

			// Token: 0x06000250 RID: 592 RVA: 0x00022B58 File Offset: 0x00020D58
			public static AdSkinLoader.AdModTexture LoadTexture(string name, string path)
			{
				AdSkinLoader.AdModTexture adModTexture = new AdSkinLoader.AdModTexture();
				adModTexture.path = path;
				adModTexture.name = name;
				adModTexture.Type = AdSkinLoader.AdModResource.ResourceType.Texture;
				SingleInstance<AdShootingModule>.Instance.StartCoroutine(adModTexture.Load());
				return adModTexture;
			}

			// Token: 0x06000251 RID: 593 RVA: 0x00022B98 File Offset: 0x00020D98
			public static AdSkinLoader.AdModMesh defualtMesh(Mesh mesh)
			{
				AdSkinLoader.AdModMesh adModMesh = new AdSkinLoader.AdModMesh();
				adModMesh.Set(mesh);
				return adModMesh;
			}

			// Token: 0x06000252 RID: 594 RVA: 0x00022BBC File Offset: 0x00020DBC
			public static AdSkinLoader.AdModTexture defualtTexture(Texture2D texture)
			{
				AdSkinLoader.AdModTexture adModTexture = new AdSkinLoader.AdModTexture();
				adModTexture.Set(texture);
				return adModTexture;
			}

			// Token: 0x06000253 RID: 595
			internal abstract IEnumerator Load();

			// Token: 0x06000254 RID: 596
			internal abstract void ApplyToObject(GameObject go);

			// Token: 0x040004CD RID: 1229
			public string name;

			// Token: 0x040004CE RID: 1230
			public string path;

			// Token: 0x040004CF RID: 1231
			public bool Readable;

			// Token: 0x02000065 RID: 101
			public enum ResourceType
			{
				// Token: 0x040004D1 RID: 1233
				Texture,
				// Token: 0x040004D2 RID: 1234
				Mesh,
				// Token: 0x040004D3 RID: 1235
				AudioClip,
				// Token: 0x040004D4 RID: 1236
				AssetBundle
			}
		}

		// Token: 0x02000066 RID: 102
		public class AdModMesh : AdSkinLoader.AdModResource
		{
			// Token: 0x17000053 RID: 83
			// (get) Token: 0x06000256 RID: 598 RVA: 0x00002E3C File Offset: 0x0000103C
			public override bool HasError
			{
				get
				{
					return this.hasError;
				}
			}

			// Token: 0x17000054 RID: 84
			// (get) Token: 0x06000257 RID: 599 RVA: 0x00002E44 File Offset: 0x00001044
			public override string Error
			{
				get
				{
					return this.error;
				}
			}

			// Token: 0x17000055 RID: 85
			// (get) Token: 0x06000258 RID: 600 RVA: 0x00002E4C File Offset: 0x0000104C
			public override bool Loaded
			{
				get
				{
					return this.loaded;
				}
			}

			// Token: 0x17000056 RID: 86
			// (get) Token: 0x06000259 RID: 601 RVA: 0x00002E54 File Offset: 0x00001054
			// (set) Token: 0x0600025A RID: 602 RVA: 0x00002E5C File Offset: 0x0000105C
			public Mesh Mesh { get; private set; }

			// Token: 0x0600025B RID: 603 RVA: 0x00002E65 File Offset: 0x00001065
			internal AdModMesh()
			{
			}

			// Token: 0x0600025C RID: 604 RVA: 0x00002E7A File Offset: 0x0000107A
			internal override IEnumerator Load()
			{
				AssetImporter.meshData meshData;
				try
				{
					meshData = new AssetImporter.meshData();
					AssetImporter.LoadMeshData(ref meshData, this.path);
				}
				catch (Exception ex2)
				{
					Exception ex = ex2;
					Exception e2 = ex;
					this.hasError = true;
					this.error = e2.ToString();
					this.loaded = true;
					yield break;
				}
				yield return Ninja.JumpToUnity;
				try
				{
					Mesh mesh = new Mesh();
					meshData.PassNewDataToMesh(ref mesh);
					mesh.name = this.name;
					this.Mesh = mesh;
					this.loaded = true;
					mesh = null;
					yield break;
				}
				catch (Exception ex2)
				{
					Exception e3 = ex2;
					this.hasError = true;
					this.error = e3.ToString();
					this.loaded = true;
					yield break;
				}
				yield break;
			}

			// Token: 0x0600025D RID: 605 RVA: 0x00022BE0 File Offset: 0x00020DE0
			internal override void ApplyToObject(GameObject go)
			{
				MeshFilter component = go.GetComponent<MeshFilter>();
				bool flag = component == null;
				if (flag)
				{
					Debug.Log("AdModMesh.SetOnObject used with an object that has no MeshFilter!");
				}
				else
				{
					component.mesh = this.Mesh;
				}
			}

			// Token: 0x0600025E RID: 606 RVA: 0x00022C20 File Offset: 0x00020E20
			public void GetMeshFromObject(GameObject go)
			{
				MeshFilter component = go.GetComponent<MeshFilter>();
				bool flag = component == null;
				if (flag)
				{
					Debug.Log("AdModMesh.GetMeshFromObject used with an object that has no MeshFilter!");
				}
				else
				{
					this.Mesh = component.mesh;
				}
			}

			// Token: 0x0600025F RID: 607 RVA: 0x00002E89 File Offset: 0x00001089
			public void Set(Mesh mesh)
			{
				this.Mesh = new Mesh();
				this.Mesh = mesh;
			}

			// Token: 0x06000260 RID: 608 RVA: 0x00022C60 File Offset: 0x00020E60
			public static implicit operator Mesh(AdSkinLoader.AdModMesh mesh)
			{
				return mesh.Mesh;
			}

			// Token: 0x040004D5 RID: 1237
			private string error = string.Empty;

			// Token: 0x040004D6 RID: 1238
			private bool hasError;

			// Token: 0x040004D7 RID: 1239
			private bool loaded;
		}

		// Token: 0x02000068 RID: 104
		public class AdModTexture : AdSkinLoader.AdModResource
		{
			// Token: 0x17000059 RID: 89
			// (get) Token: 0x06000267 RID: 615 RVA: 0x00002EB8 File Offset: 0x000010B8
			public override bool HasError
			{
				get
				{
					return this.hasError;
				}
			}

			// Token: 0x1700005A RID: 90
			// (get) Token: 0x06000268 RID: 616 RVA: 0x00002EC0 File Offset: 0x000010C0
			public override string Error
			{
				get
				{
					return this.error;
				}
			}

			// Token: 0x1700005B RID: 91
			// (get) Token: 0x06000269 RID: 617 RVA: 0x00002EC8 File Offset: 0x000010C8
			public override bool Loaded
			{
				get
				{
					return this.loaded;
				}
			}

			// Token: 0x1700005C RID: 92
			// (get) Token: 0x0600026A RID: 618 RVA: 0x00002ED0 File Offset: 0x000010D0
			// (set) Token: 0x0600026B RID: 619 RVA: 0x00002ED8 File Offset: 0x000010D8
			public Texture2D Texture { get; private set; }

			// Token: 0x0600026C RID: 620 RVA: 0x00002EE1 File Offset: 0x000010E1
			internal AdModTexture()
			{
			}

			// Token: 0x0600026D RID: 621 RVA: 0x00002EF6 File Offset: 0x000010F6
			internal override IEnumerator Load()
			{
				AssetImporter.LoadingObject lObj = AssetImporter.StartImport.Texture(this.path, null, !this.Readable);
				yield return lObj.routine;
				this.Texture = lObj.tex;
				this.loaded = true;
				this.hasError = !string.IsNullOrEmpty(lObj.texError);
				this.error = lObj.texError;
				yield break;
			}

			// Token: 0x0600026E RID: 622 RVA: 0x00022DEC File Offset: 0x00020FEC
			internal override void ApplyToObject(GameObject go)
			{
				Renderer component = go.GetComponent<Renderer>();
				bool flag = component == null;
				if (flag)
				{
					Debug.Log("AdModTexture.SetOnObject used with an object that has no Renderer!");
				}
				else
				{
					component.material.mainTexture = this.Texture;
				}
			}

			// Token: 0x0600026F RID: 623 RVA: 0x00022E30 File Offset: 0x00021030
			public void GetTextureFromObject(GameObject go)
			{
				Renderer component = go.GetComponent<Renderer>();
				bool flag = component == null;
				if (flag)
				{
					Debug.Log("AdModTexture.GetTextureFromObject used with an object that has no Renderer!");
				}
				else
				{
					this.Texture = (Texture2D)component.material.mainTexture;
				}
			}

			// Token: 0x06000270 RID: 624 RVA: 0x00002F05 File Offset: 0x00001105
			public void Set(Texture2D texture)
			{
				this.Texture = texture;
			}

			// Token: 0x06000271 RID: 625 RVA: 0x00022E78 File Offset: 0x00021078
			public static implicit operator Texture2D(AdSkinLoader.AdModTexture texture)
			{
				return (texture != null) ? texture.Texture : null;
			}

			// Token: 0x06000272 RID: 626 RVA: 0x00022E98 File Offset: 0x00021098
			public static explicit operator AdSkinLoader.AdModTexture(Texture2D texture)
			{
				bool flag = texture == null;
				AdSkinLoader.AdModTexture result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = new AdSkinLoader.AdModTexture
					{
						Texture = texture,
						loaded = true,
						hasError = false,
						error = string.Empty
					};
				}
				return result;
			}

			// Token: 0x040004E1 RID: 1249
			private bool hasError;

			// Token: 0x040004E2 RID: 1250
			private string error = string.Empty;

			// Token: 0x040004E3 RID: 1251
			private bool loaded;
		}

		// Token: 0x0200006A RID: 106
		public class AdSkinDataPack
		{
			// Token: 0x040004E9 RID: 1257
			public string skinName;

			// Token: 0x040004EA RID: 1258
			public Dictionary<string, AdSkinLoader.AdSkinDataPack.AdSkinData> ProjectileSkinContainer = new Dictionary<string, AdSkinLoader.AdSkinDataPack.AdSkinData>();

			// Token: 0x0200006B RID: 107
			public class AdSkinData
			{
				// Token: 0x0600027A RID: 634 RVA: 0x00002F3C File Offset: 0x0000113C
				internal AdSkinData()
				{
				}

				// Token: 0x0600027B RID: 635 RVA: 0x00022FB0 File Offset: 0x000211B0
				public void AdSkinSet(string name, string objpath = "none", string texpath = "none")
				{
					this.BlockName = name;
					this.objPath = objpath;
					this.texPath = texpath;
					this.isLoading = true;
					bool flag = !(this.objPath == "none");
					if (flag)
					{
						this.mesh = AdSkinLoader.AdModResource.LoadMesh(this.BlockName, this.objPath);
						this.objLoading = true;
					}
					bool flag2 = !(this.texPath == "none");
					if (flag2)
					{
						this.texture = AdSkinLoader.AdModResource.LoadTexture(this.BlockName, this.texPath);
						this.texLoading = true;
					}
					this.doneLoading = true;
				}

				// Token: 0x0600027C RID: 636 RVA: 0x00002F46 File Offset: 0x00001146
				public void AdDefualtSkinSet(Mesh dmesh, Texture2D dtexture)
				{
					this.mesh = AdSkinLoader.AdModResource.defualtMesh(dmesh);
					this.texture = AdSkinLoader.AdModResource.defualtTexture(dtexture);
					this.objLoading = true;
					this.texLoading = true;
					this.doneLoading = true;
				}

				// Token: 0x040004EB RID: 1259
				public string path;

				// Token: 0x040004EC RID: 1260
				public string objPath;

				// Token: 0x040004ED RID: 1261
				public string texPath;

				// Token: 0x040004EE RID: 1262
				public AdSkinLoader.AdModMesh mesh;

				// Token: 0x040004EF RID: 1263
				public AdSkinLoader.AdModTexture texture;

				// Token: 0x040004F0 RID: 1264
				public bool isDefult;

				// Token: 0x040004F1 RID: 1265
				public bool isLoading;

				// Token: 0x040004F2 RID: 1266
				public bool doneLoading;

				// Token: 0x040004F3 RID: 1267
				public bool objLoading;

				// Token: 0x040004F4 RID: 1268
				public bool objLoaded;

				// Token: 0x040004F5 RID: 1269
				public bool meshEnable;

				// Token: 0x040004F6 RID: 1270
				public bool texLoading;

				// Token: 0x040004F7 RID: 1271
				public bool texLoaded;

				// Token: 0x040004F8 RID: 1272
				public bool texEnable;

				// Token: 0x040004F9 RID: 1273
				public string BlockName;
			}
		}
	}
}
