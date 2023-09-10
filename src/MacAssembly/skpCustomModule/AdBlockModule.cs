using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Modding.Modules;
using Modding.Serialization;
using UnityEngine;

namespace skpCustomModule
{
	// Token: 0x02000023 RID: 35
	[XmlRoot("AdBlockProp")]
	[Reloadable]
	public class AdBlockModule : BlockModule
	{
		// Token: 0x040001A1 RID: 417
		[XmlElement("BlockState")]
		[RequireToValidate]
		[Reloadable]
		public AdBlockModule.AdBlockState Blockstateinfo;

		// Token: 0x040001A2 RID: 418
		[XmlElement("SpringState")]
		[RequireToValidate]
		[Reloadable]
		public AdBlockModule.SpringMotion Springinfo;

		// Token: 0x040001A3 RID: 419
		[XmlElement("RotateState")]
		[RequireToValidate]
		[Reloadable]
		public AdBlockModule.RotateMotion Rotateinfo;

		// Token: 0x02000024 RID: 36
		[Serializable]
		public class AdBlockState : Element
		{
			// Token: 0x040001A4 RID: 420
			[XmlElement("JointStr")]
			[DefaultValue(15000f)]
			public float JointStr = 15000f;

			// Token: 0x040001A5 RID: 421
			[XmlElement("FrictionStr")]
			[DefaultValue(0.6f)]
			public float FrictionStr = 0.6f;

			// Token: 0x040001A6 RID: 422
			[XmlElement("BounceStr")]
			[DefaultValue(0f)]
			public float BounceStr = 0f;

			// Token: 0x040001A7 RID: 423
			[XmlElement("JointForceStr")]
			[DefaultValue(3.4E+38f)]
			public float JointForceStr = 3.4E+38f;

			// Token: 0x040001A8 RID: 424
			[XmlElement("CollisionType")]
			[DefaultValue(CollisionType.Discrete)]
			public CollisionType CollisionTypeS;

			// Token: 0x040001A9 RID: 425
			[XmlElement("InterferenceCollision")]
			[DefaultValue(false)]
			public bool InterferenceCollision;

			// Token: 0x040001AA RID: 426
			[XmlElement("FrictionCombineType")]
			[DefaultValue(CombineType.Average)]
			public CombineType FriCombType;

			// Token: 0x040001AB RID: 427
			[XmlElement("BounceCombineType")]
			[DefaultValue(CombineType.Average)]
			public CombineType BounceComType;

			// Token: 0x040001AC RID: 428
			[XmlElement("JointProjectionMode")]
			[DefaultValue(0)]
			public JointProjectionMode JointProjectionType;

			// Token: 0x040001AD RID: 429
			[XmlElement("ProjectionDistance")]
			[DefaultValue(0f)]
			public float projectionDist = 0f;

			// Token: 0x040001AE RID: 430
			[XmlElement("ProjectionAngle")]
			[DefaultValue(0f)]
			public float projectionAng = 0f;

			// Token: 0x040001AF RID: 431
			[XmlElement("Bouyancy")]
			[DefaultValue(1f)]
			public float bouyancy = 1f;

			// Token: 0x040001B0 RID: 432
			[XmlElement("BlockColor")]
			[DefaultValue(null)]
			public Color3 BlockColor;

			// Token: 0x040001B1 RID: 433
			[XmlIgnore]
			public bool BlockColorSpecified;

			// Token: 0x040001B2 RID: 434
			[XmlElement("SubObjectPosition")]
			[DefaultValue(null)]
			public TransformValues SubObjectPosition;

			// Token: 0x040001B3 RID: 435
			[XmlElement("useSubObject")]
			[DefaultValue(false)]
			public bool useSubObject;

			// Token: 0x040001B4 RID: 436
			[XmlElement("SubMesh")]
			[DefaultValue(null)]
			public MeshReference SubMesh;

			// Token: 0x040001B5 RID: 437
			[XmlElement("SubTexture")]
			[DefaultValue(null)]
			public ResourceReference SubTexture;

			// Token: 0x040001B6 RID: 438
			[XmlElement("SubMass")]
			[DefaultValue(1f)]
			public float SubMass;

			// Token: 0x040001B7 RID: 439
			[XmlElement("SubDrag")]
			[DefaultValue(0f)]
			public float SubDrag;

			// Token: 0x040001B8 RID: 440
			[XmlElement("SubAngularDrag")]
			[DefaultValue(5f)]
			public float SubAngularDrag = 5f;

			// Token: 0x040001B9 RID: 441
			[XmlElement("SubObjectCollisionType")]
			[DefaultValue(CollisionType.Discrete)]
			public CollisionType SubObjectCollisionType;

			// Token: 0x040001BA RID: 442
			[XmlElement("ColliderToAddPoint")]
			[DefaultValue(false)]
			public bool ColliderToAddPoint = false;

			// Token: 0x040001BB RID: 443
			[XmlArray("Colliders")]
			[CanBeEmpty]
			[XmlArrayItem("BoxCollider", typeof(AdBoxModCollider))]
			public ModCollider[] Colliders;

			// Token: 0x040001BC RID: 444
			[XmlElement("DisableAddPoint")]
			[DefaultValue(false)]
			public bool DisableAddPoint = false;

			// Token: 0x040001BD RID: 445
			[XmlElement("DisableDefaultCollider")]
			[DefaultValue(false)]
			public bool DisableDefaultCollider = false;

			// Token: 0x040001BE RID: 446
			[XmlElement("useMeshCollider")]
			[DefaultValue(false)]
			public bool useMeshCollider = false;

			// Token: 0x040001BF RID: 447
			[XmlElement("MeshColliderObj")]
			[DefaultValue(null)]
			public MeshReference MeshColliderObj;
		}

		// Token: 0x02000025 RID: 37
		[Serializable]
		public class SpringMotion : Element
		{
			// Token: 0x040001C0 RID: 448
			[XmlElement("MotionX")]
			[DefaultValue(MotionType.Locked)]
			public MotionType XMotionType;

			// Token: 0x040001C1 RID: 449
			[XmlElement("MotionY")]
			[DefaultValue(MotionType.Locked)]
			public MotionType YMotipnType;

			// Token: 0x040001C2 RID: 450
			[XmlElement("MotionZ")]
			[DefaultValue(MotionType.Locked)]
			public MotionType ZMotionType;

			// Token: 0x040001C3 RID: 451
			[XmlElement("AnchorShiftX")]
			[DefaultValue(0f)]
			public float AnchorShiftX = 0f;

			// Token: 0x040001C4 RID: 452
			[XmlElement("AnchorShiftY")]
			[DefaultValue(0f)]
			public float AnchorShiftY = 0f;

			// Token: 0x040001C5 RID: 453
			[XmlElement("AnchorShiftZ")]
			[DefaultValue(0f)]
			public float AnchorShiftZ = 0f;

			// Token: 0x040001C6 RID: 454
			[XmlElement("SpringStr")]
			[DefaultValue(0f)]
			public float SpringStr = 0f;

			// Token: 0x040001C7 RID: 455
			[XmlElement("DamperStr")]
			[DefaultValue(0f)]
			public float DamperStr = 0f;

			// Token: 0x040001C8 RID: 456
			[XmlElement("LimitSpringStr")]
			[DefaultValue(0f)]
			public float LimitSpringStr = 0f;

			// Token: 0x040001C9 RID: 457
			[XmlElement("LimitDamperStr")]
			[DefaultValue(0f)]
			public float LimitDamperStr = 0f;

			// Token: 0x040001CA RID: 458
			[XmlElement("LimitDistance")]
			[DefaultValue(0f)]
			public float LimitDist = 0f;

			// Token: 0x040001CB RID: 459
			[XmlElement("useSlider")]
			[DefaultValue(false)]
			public bool useSlider;

			// Token: 0x040001CC RID: 460
			[XmlElement("SpringSlider")]
			[DefaultValue(null)]
			[RequireToValidate]
			public MSliderReference SpringSlider;

			// Token: 0x040001CD RID: 461
			[XmlElement("DamperSlider")]
			[DefaultValue(null)]
			[RequireToValidate]
			public MSliderReference DamperSlider;
		}

		// Token: 0x02000026 RID: 38
		[Serializable]
		public class RotateMotion : Element
		{
			// Token: 0x040001CE RID: 462
			[XmlElement("AngularMotionX")]
			[DefaultValue(MotionType.Locked)]
			public MotionType XAngMotionType;

			// Token: 0x040001CF RID: 463
			[XmlElement("AngularMotionY")]
			[DefaultValue(MotionType.Locked)]
			public MotionType YAngMotipnType;

			// Token: 0x040001D0 RID: 464
			[XmlElement("AngularMotionZ")]
			[DefaultValue(MotionType.Locked)]
			public MotionType ZAngMotionType;

			// Token: 0x040001D1 RID: 465
			[XmlElement("SpringStr")]
			[DefaultValue(0f)]
			public float SpringStr = 0f;

			// Token: 0x040001D2 RID: 466
			[XmlElement("DamperStr")]
			[DefaultValue(0f)]
			public float DamperStr = 0f;

			// Token: 0x040001D3 RID: 467
			[XmlElement("LimitSpringStr")]
			[DefaultValue(0f)]
			public float LimitSpringStr = 0f;

			// Token: 0x040001D4 RID: 468
			[XmlElement("LimitDamperStr")]
			[DefaultValue(0f)]
			public float LimitDamperStr = 0f;

			// Token: 0x040001D5 RID: 469
			[XmlElement("LimitAngular")]
			[DefaultValue(0f)]
			public float LimitAng = 0f;

			// Token: 0x040001D6 RID: 470
			[XmlElement("useSlider")]
			[DefaultValue(false)]
			public bool useSlider;

			// Token: 0x040001D7 RID: 471
			[XmlElement("SpringSlider")]
			[DefaultValue(null)]
			[RequireToValidate]
			public MSliderReference SpringSlider;

			// Token: 0x040001D8 RID: 472
			[XmlElement("DamperSlider")]
			[DefaultValue(null)]
			[RequireToValidate]
			public MSliderReference DamperSlider;
		}
	}
}
