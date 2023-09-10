using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Modding.Modules;
using Modding.Serialization;

namespace skpCustomModule
{
	// Token: 0x02000061 RID: 97
	[XmlRoot("AdShootingProp")]
	[Reloadable]
	public class AdShootingProp : BlockModule
	{
		// Token: 0x04000484 RID: 1156
		public int ProjectileId;

		// Token: 0x04000485 RID: 1157
		[XmlElement("ProjectileStart")]
		[Reloadable]
		public TransformValues ProjectileStart;

		// Token: 0x04000486 RID: 1158
		[XmlElement("ShowPlaceholderProjectile")]
		[DefaultValue(false)]
		public bool ShowPlaceholderProjectile;

		// Token: 0x04000487 RID: 1159
		[XmlElement("PlaceholderProjectileUseCollider")]
		[DefaultValue(false)]
		public bool PlaceholderProjectileUseCollider;

		// Token: 0x04000488 RID: 1160
		[XmlElement("DefaultAmmo")]
		[Reloadable]
		public int DefaultAmmo;

		// Token: 0x04000489 RID: 1161
		[XmlElement("AmmoType")]
		[DefaultValue(0)]
		[Reloadable]
		public AmmoType AmmoType;

		// Token: 0x0400048A RID: 1162
		[XmlElement("SupportsExplosionGodTool")]
		[DefaultValue(true)]
		[Reloadable]
		public bool SupportsExplosionGodTool = true;

		// Token: 0x0400048B RID: 1163
		[XmlElement("ProjectilesExplode")]
		[DefaultValue(false)]
		[Reloadable]
		public bool ProjectilesExplode;

		// Token: 0x0400048C RID: 1164
		[XmlElement("ExplodeRadius")]
		[DefaultValue(3f)]
		[Reloadable]
		public float ExplodeRadius;

		// Token: 0x0400048D RID: 1165
		[XmlElement("ExplodePower")]
		[DefaultValue(10f)]
		[Reloadable]
		public float ExplodePower;

		// Token: 0x0400048E RID: 1166
		[XmlElement("ExplodeUpPower")]
		[DefaultValue(0f)]
		[Reloadable]
		public float ExplodeUpPower;

		// Token: 0x0400048F RID: 1167
		[XmlElement("AssetBundleName")]
		[RequireToValidate]
		[CanBeEmpty]
		public ResourceReference AssetBundleName;

		// Token: 0x04000490 RID: 1168
		[XmlElement("useDefaultAsset")]
		[DefaultValue(false)]
		[Reloadable]
		public bool useDefaultAsset;

		// Token: 0x04000491 RID: 1169
		[XmlElement("ExplodeEffect")]
		[DefaultValue(null)]
		[CanBeEmpty]
		public string ExplodeEffect;

		// Token: 0x04000492 RID: 1170
		[XmlElement("ShotFlashPosition")]
		[DefaultValue(null)]
		[CanBeEmpty]
		public TransformValues ShotFlashPosition;

		// Token: 0x04000493 RID: 1171
		[XmlElement("PurgeVector")]
		[DefaultValue(null)]
		public ModVector3 PurgeVector;

		// Token: 0x04000494 RID: 1172
		[XmlElement("PurgePower")]
		[DefaultValue(0f)]
		public float PurgePower;

		// Token: 0x04000495 RID: 1173
		[XmlElement("DelayTime")]
		[DefaultValue(0f)]
		public float DelayTime;

		// Token: 0x04000496 RID: 1174
		[XmlElement("ShotFlashEffect")]
		[DefaultValue(null)]
		[CanBeEmpty]
		public string ShotFlashEffect;

		// Token: 0x04000497 RID: 1175
		[XmlElement("TrailEffect")]
		[DefaultValue(null)]
		[CanBeEmpty]
		public string TrailEffect;

		// Token: 0x04000498 RID: 1176
		[XmlElement("BulletEffect")]
		[DefaultValue(null)]
		[CanBeEmpty]
		public string BulletEffect;

		// Token: 0x04000499 RID: 1177
		[XmlElement("ChaffEffect")]
		[DefaultValue(null)]
		[CanBeEmpty]
		public string ChaffEffect;

		// Token: 0x0400049A RID: 1178
		[XmlElement("useBooster")]
		[DefaultValue(false)]
		[Reloadable]
		public bool useBooster;

		// Token: 0x0400049B RID: 1179
		[XmlElement("useJamReducer")]
		[DefaultValue(false)]
		[Reloadable]
		public bool useJamReducer;

		// Token: 0x0400049C RID: 1180
		[XmlElement("useTimefuse")]
		[DefaultValue(false)]
		[Reloadable]
		public bool useTimefuse;

		// Token: 0x0400049D RID: 1181
		[XmlElement("useDelayTimer")]
		[DefaultValue(false)]
		[Reloadable]
		public bool useDelayTimer;

		// Token: 0x0400049E RID: 1182
		[XmlElement("useThrustDelayTimer")]
		[DefaultValue(false)]
		[Reloadable]
		public bool useThrustDelayTimer;

		// Token: 0x0400049F RID: 1183
		[XmlElement("useDelay")]
		[DefaultValue(false)]
		[Reloadable]
		public bool useDelay;

		// Token: 0x040004A0 RID: 1184
		[XmlElement("useBeacon")]
		[DefaultValue(false)]
		[Reloadable]
		public bool useBeacon;

		// Token: 0x040004A1 RID: 1185
		[XmlElement("useBurstShot")]
		[DefaultValue(false)]
		[Reloadable]
		public bool useBurstShot;

		// Token: 0x040004A2 RID: 1186
		[XmlElement("useFreezingAttack")]
		[DefaultValue(false)]
		[Reloadable]
		public bool useFreezingAttack;

		// Token: 0x040004A3 RID: 1187
		[XmlElement("GuidType")]
		[DefaultValue(null)]
		[CanBeEmpty]
		public string Guidtype;

		// Token: 0x040004A4 RID: 1188
		[XmlElement("isChaff")]
		[DefaultValue(false)]
		[Reloadable]
		public bool isChaff;

		// Token: 0x040004A5 RID: 1189
		[XmlElement("GuidRatio")]
		[DefaultValue(0.5f)]
		[Reloadable]
		public float GuidRatio;

		// Token: 0x040004A6 RID: 1190
		[XmlElement("useExplodeRotation")]
		[DefaultValue(false)]
		[Reloadable]
		public bool useExplodeRotation;

		// Token: 0x040004A7 RID: 1191
		[XmlElement("ProjectilesDespawnImmediately")]
		[DefaultValue(false)]
		[Reloadable]
		public bool ProjectilesDespawnImmediately;

		// Token: 0x040004A8 RID: 1192
		[XmlElement("FireKey")]
		[RequireToValidate]
		public MKeyReference FireKey;

		// Token: 0x040004A9 RID: 1193
		[XmlElement("PowerSlider")]
		[RequireToValidate]
		public MSliderReference PowerSlider;

		// Token: 0x040004AA RID: 1194
		[XmlElement("RateOfFireSlider")]
		[RequireToValidate]
		public MSliderReference RateOfFireSlider;

		// Token: 0x040004AB RID: 1195
		[XmlElement("TimefuseSlider")]
		[DefaultValue(null)]
		[RequireToValidate]
		public MSliderReference TimefuseSlider;

		// Token: 0x040004AC RID: 1196
		[XmlElement("DelayTimerSlider")]
		[DefaultValue(null)]
		[RequireToValidate]
		public MSliderReference DelayTimerSlider;

		// Token: 0x040004AD RID: 1197
		[XmlElement("HoldToShootToggle")]
		[RequireToValidate]
		public MToggleReference HoldToShootToggle;

		// Token: 0x040004AE RID: 1198
		[XmlElement("ThrustDelayTimerSlider")]
		[DefaultValue(null)]
		[RequireToValidate]
		public MSliderReference ThrustDelayTimerSlider;

		// Token: 0x040004AF RID: 1199
		[XmlElement("RecoilMultiplier")]
		[DefaultValue(0.1f)]
		[Reloadable]
		public float RecoilMultiplier = 0.1f;

		// Token: 0x040004B0 RID: 1200
		[XmlElement("RandomInterval")]
		[DefaultValue(0.05f)]
		[Reloadable]
		public float RandomInterval = 0.05f;

		// Token: 0x040004B1 RID: 1201
		[XmlElement("RandomDiffusion")]
		[DefaultValue(0.01f)]
		[Reloadable]
		public float RandomDiffusion = 0.01f;

		// Token: 0x040004B2 RID: 1202
		[XmlElement("RandomFuseInterval")]
		[DefaultValue(0.05f)]
		[Reloadable]
		public float RandomFuseInterval = 0.05f;

		// Token: 0x040004B3 RID: 1203
		[XmlElement("FuseDelayTime")]
		[DefaultValue(0f)]
		[Reloadable]
		public float FuseDelayTime = 0f;

		// Token: 0x040004B4 RID: 1204
		[XmlElement("RateOfBurst")]
		[DefaultValue(2f)]
		[Reloadable]
		public float RateOfBurst = 2f;

		// Token: 0x040004B5 RID: 1205
		[XmlElement("BurstShotNum")]
		[DefaultValue(3)]
		public int BurstShotNum = 3;

		// Token: 0x040004B6 RID: 1206
		[XmlElement("PoolSize")]
		[DefaultValue(10)]
		public int PoolSize = 10;

		// Token: 0x040004B7 RID: 1207
		[XmlArray("Sounds")]
		[RequireToValidate]
		[DefaultValue(null)]
		[CanBeEmpty]
		[XmlArrayItem("AudioClip", typeof(ResourceReference))]
		public object[] Sounds;

		// Token: 0x040004B8 RID: 1208
		[XmlArray("HitSounds")]
		[RequireToValidate]
		[DefaultValue(null)]
		[CanBeEmpty]
		[XmlArrayItem("AudioClip", typeof(ResourceReference))]
		public object[] HitSounds;

		// Token: 0x040004B9 RID: 1209
		[XmlArray("ProjectileSounds")]
		[RequireToValidate]
		[DefaultValue(null)]
		[CanBeEmpty]
		[XmlArrayItem("AudioClip", typeof(ResourceReference))]
		public object[] ProjectileSounds;

		// Token: 0x040004BA RID: 1210
		[XmlElement("ShootingState")]
		[RequireToValidate]
		[Reloadable]
		public AdShootingProp.ShootingState Shootingstateinfo;

		// Token: 0x02000062 RID: 98
		[Serializable]
		public class ShootingState : Element
		{
			// Token: 0x040004BB RID: 1211
			[XmlElement("Projectile")]
			[DefaultValue(true)]
			public bool Projectileflag = true;

			// Token: 0x040004BC RID: 1212
			[XmlElement("Mesh")]
			[RequireToValidate]
			public MeshReference Mesh;

			// Token: 0x040004BD RID: 1213
			[XmlElement("Texture")]
			[RequireToValidate]
			public ResourceReference Texture;

			// Token: 0x040004BE RID: 1214
			[XmlArray("Colliders")]
			[RequireToValidate]
			[XmlArrayItem("BoxCollider", typeof(BoxModCollider))]
			[XmlArrayItem("SphereCollider", typeof(SphereModCollider))]
			[XmlArrayItem("CapsuleCollider", typeof(CapsuleModCollider))]
			public ModCollider[] Colliders;

			// Token: 0x040004BF RID: 1215
			[XmlElement("Mass")]
			public float Mass;

			// Token: 0x040004C0 RID: 1216
			[XmlElement("IgnoreGravity")]
			[DefaultValue(false)]
			public bool IgnoreGravity;

			// Token: 0x040004C1 RID: 1217
			[XmlElement("Drag")]
			[DefaultValue(0f)]
			public float Drag;

			// Token: 0x040004C2 RID: 1218
			[XmlElement("AngularDrag")]
			[DefaultValue(5f)]
			public float AngularDrag = 5f;

			// Token: 0x040004C3 RID: 1219
			[XmlElement("Buoyancy")]
			[DefaultValue(0f)]
			public float Buoyancy = 0f;

			// Token: 0x040004C4 RID: 1220
			[XmlElement("FrictionStr")]
			[DefaultValue(0.6f)]
			public float FrictionStr = 0.6f;

			// Token: 0x040004C5 RID: 1221
			[XmlElement("BounceStr")]
			[DefaultValue(0f)]
			public float BounceStr = 0f;

			// Token: 0x040004C6 RID: 1222
			[XmlElement("FrictionCombineType")]
			[DefaultValue(CombineType.Average)]
			public CombineType FriCombType;

			// Token: 0x040004C7 RID: 1223
			[XmlElement("BounceCombineType")]
			[DefaultValue(CombineType.Average)]
			public CombineType BounceComType;

			// Token: 0x040004C8 RID: 1224
			[XmlElement("EntityDamage")]
			[DefaultValue(100f)]
			public float EntityDamage = 100f;

			// Token: 0x040004C9 RID: 1225
			[XmlElement("BlockDamage")]
			[DefaultValue(1f)]
			public float BlockDamage = 1f;

			// Token: 0x040004CA RID: 1226
			[XmlElement("CollisionTypeS")]
			[DefaultValue(CollisionType.Discrete)]
			public CollisionType CollisionTypeS;

			// Token: 0x040004CB RID: 1227
			[XmlElement("Attaches")]
			public bool Attaches;
		}
	}
}
