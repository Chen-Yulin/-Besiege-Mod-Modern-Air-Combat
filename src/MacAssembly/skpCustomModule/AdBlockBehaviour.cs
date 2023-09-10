using System;
using Modding;
using Modding.Modules;
using Modding.Serialization;
using UnityEngine;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;

namespace skpCustomModule
{
	// Token: 0x02000019 RID: 25
	public class AdBlockBehaviour : BlockModuleBehaviour<AdBlockModule>
	{
		// Token: 0x06000094 RID: 148 RVA: 0x0000E9C4 File Offset: 0x0000CBC4
		private void OnJointPropChanged()
		{
			MotionType[] motionValue = this.MotionValue;
			MotionType[] amotionValue = this.AmotionValue;
			float jointStr = base.Module.Blockstateinfo.JointStr;
			float jointForceStr = base.Module.Blockstateinfo.JointForceStr;
			this.SoftJLSpring = default(SoftJointLimitSpring);
			this.SoftJLSpring.spring = this.ZeroisInfinity(base.Module.Springinfo.LimitSpringStr);
			this.SoftJLSpring.damper = base.Module.Springinfo.LimitDamperStr;
			this.SoftALSpring = default(SoftJointLimitSpring);
			this.SoftALSpring.spring = this.ZeroisInfinity(base.Module.Rotateinfo.LimitSpringStr);
			this.SoftALSpring.damper = base.Module.Rotateinfo.LimitDamperStr;
			this.SoftJL = default(SoftJointLimit);
			this.SoftJL.limit = base.Module.Springinfo.LimitDist;
			this.SoftAL = default(SoftJointLimit);
			this.SoftAL.limit = base.Module.Rotateinfo.LimitAng;
			this.JointComp.breakForce = jointStr;
			this.JointComp.breakTorque = jointStr;
			this.JointD = default(JointDrive);
			bool flag = this.useSlider;
			if (flag)
			{
				this.JointD.positionSpring = this.SpringSlider.Value * 100f;
				this.JointD.positionDamper = this.DamperSlider.Value * 100f;
			}
			else
			{
				this.JointD.positionSpring = base.Module.Springinfo.SpringStr;
				this.JointD.positionDamper = base.Module.Springinfo.DamperStr;
			}
			this.JointD.maximumForce = 3.4E+38f;
			this.AJointD = default(JointDrive);
			this.AJointD = this.JointComp.angularXDrive;
			this.AJointDYZ = default(JointDrive);
			this.AJointDYZ = this.JointComp.angularYZDrive;
			bool flag2 = this.useSlider_Rot;
			if (flag2)
			{
				this.AJointD.positionSpring = this.SpringSlider_Rot.Value * 100f;
				this.AJointD.positionDamper = this.DamperSlider_Rot.Value * 100f;
				this.AJointDYZ.positionSpring = this.SpringSlider_Rot.Value * 100f;
				this.AJointDYZ.positionDamper = this.DamperSlider_Rot.Value * 100f;
			}
			else
			{
				this.AJointD.positionSpring = base.Module.Rotateinfo.SpringStr;
				this.AJointD.positionDamper = base.Module.Rotateinfo.DamperStr;
				this.AJointDYZ.positionSpring = base.Module.Rotateinfo.SpringStr;
				this.AJointDYZ.positionDamper = base.Module.Rotateinfo.DamperStr;
			}
			switch (motionValue[0])
			{
			case MotionType.Locked:
				this.JointComp.xMotion = 0;
				break;
			case MotionType.Limited:
				this.JointComp.xMotion = (ConfigurableJointMotion)1;
				break;
			case MotionType.Free:
				this.JointComp.xMotion = (ConfigurableJointMotion)2;
				break;
			}
			switch (motionValue[1])
			{
			case MotionType.Locked:
				this.JointComp.yMotion = 0;
				break;
			case MotionType.Limited:
				this.JointComp.yMotion = (ConfigurableJointMotion)1;
				break;
			case MotionType.Free:
				this.JointComp.yMotion = (ConfigurableJointMotion)2;
				break;
			}
			switch (motionValue[2])
			{
			case MotionType.Locked:
				this.JointComp.zMotion = 0;
				break;
			case MotionType.Limited:
				this.JointComp.zMotion = (ConfigurableJointMotion)1;
				break;
			case MotionType.Free:
				this.JointComp.zMotion = (ConfigurableJointMotion)2;
				break;
			}
			switch (amotionValue[0])
			{
			case MotionType.Locked:
				this.JointComp.angularXMotion = 0;
				break;
			case MotionType.Limited:
				this.JointComp.angularXMotion = (ConfigurableJointMotion)1;
				break;
			case MotionType.Free:
				this.JointComp.angularXMotion = (ConfigurableJointMotion)2;
				break;
			}
			switch (amotionValue[1])
			{
			case MotionType.Locked:
				this.JointComp.angularYMotion = 0;
				break;
			case MotionType.Limited:
				this.JointComp.angularYMotion = (ConfigurableJointMotion)1;
				break;
			case MotionType.Free:
				this.JointComp.angularYMotion = (ConfigurableJointMotion)2;
				break;
			}
			switch (amotionValue[2])
			{
			case MotionType.Locked:
				this.JointComp.angularZMotion = 0;
				break;
			case MotionType.Limited:
				this.JointComp.angularZMotion = (ConfigurableJointMotion)1;
				break;
			case MotionType.Free:
				this.JointComp.angularZMotion = (ConfigurableJointMotion)2;
				break;
			}
			this.JointComp.linearLimit = this.SoftJL;
			this.JointComp.lowAngularXLimit = this.SoftAL;
			this.JointComp.highAngularXLimit = this.SoftAL;
			this.JointComp.angularYLimit = this.SoftAL;
			this.JointComp.angularZLimit = this.SoftAL;
			this.JointComp.linearLimitSpring = this.SoftJLSpring;
			this.JointComp.angularXLimitSpring = this.SoftALSpring;
			this.JointComp.angularYZLimitSpring = this.SoftALSpring;
			this.JointComp.xDrive = this.JointD;
			this.JointComp.yDrive = this.JointD;
			this.JointComp.zDrive = this.JointD;
			this.JointComp.angularXDrive = this.AJointD;
			this.JointComp.angularYZDrive = this.AJointDYZ;
			this.JointComp.projectionMode = base.Module.Blockstateinfo.JointProjectionType;
			this.JointComp.projectionDistance = base.Module.Blockstateinfo.projectionDist;
			this.JointComp.projectionAngle = base.Module.Blockstateinfo.projectionAng;
			this.JointComp.enableCollision = base.Module.Blockstateinfo.InterferenceCollision;
		}

		// Token: 0x06000095 RID: 149 RVA: 0x0000EFF0 File Offset: 0x0000D1F0
		private void OnJointAnchorChanged()
		{
			Vector3 forward = base.transform.forward;
			Vector3 right = base.transform.right;
			Vector3 up = base.transform.up;
			Vector3 vector = Vector3.zero;
			Vector3 vector2 = Vector3.zero;
			Vector3 vector3 = Vector3.zero;
			bool flag = this.JointComp.connectedBody;
			if (flag)
			{
				vector = this.JointComp.connectedBody.transform.InverseTransformVector(forward) * base.Module.Springinfo.AnchorShiftY;
				vector2 = this.JointComp.connectedBody.transform.InverseTransformVector(right) * base.Module.Springinfo.AnchorShiftX;
				vector3 = this.JointComp.connectedBody.transform.InverseTransformVector(up) * base.Module.Springinfo.AnchorShiftZ;
			}
			Vector3 connectedAnchor = this.JointComp.connectedAnchor;
			this.JointComp.autoConfigureConnectedAnchor = false;
			this.JointComp.connectedAnchor = connectedAnchor + vector + vector2 + vector3;
		}

		// Token: 0x06000096 RID: 150 RVA: 0x0000F114 File Offset: 0x0000D314
		private void OnPysicPropChanged()
		{
			float frictionStr = base.Module.Blockstateinfo.FrictionStr;
			CombineType friCombType = base.Module.Blockstateinfo.FriCombType;
			float bounceStr = base.Module.Blockstateinfo.BounceStr;
			CombineType bounceComType = base.Module.Blockstateinfo.BounceComType;
			float num = frictionStr;
			float bounciness = bounceStr;
			this.PhysMaterial = new PhysicMaterial();
			this.PhysMaterial.staticFriction = num;
			this.PhysMaterial.dynamicFriction = num;
			switch (friCombType)
			{
			case CombineType.Average:
				this.PhysMaterial.frictionCombine = 0;
				break;
			case CombineType.Minimum:
				this.PhysMaterial.frictionCombine = (PhysicMaterialCombine)2;
				break;
			case CombineType.Maximum:
				this.PhysMaterial.frictionCombine = (PhysicMaterialCombine)3;
				break;
			case CombineType.Multiply:
				this.PhysMaterial.frictionCombine = (PhysicMaterialCombine)1;
				break;
			}
			this.PhysMaterial.bounciness = bounciness;
			switch (bounceComType)
			{
			case CombineType.Average:
				this.PhysMaterial.bounceCombine = 0;
				break;
			case CombineType.Minimum:
				this.PhysMaterial.bounceCombine = (PhysicMaterialCombine)2;
				break;
			case CombineType.Maximum:
				this.PhysMaterial.bounceCombine = (PhysicMaterialCombine)3;
				break;
			case CombineType.Multiply:
				this.PhysMaterial.bounceCombine = (PhysicMaterialCombine)1;
				break;
			}
			int num2 = 0;
			foreach (Collider collider in this.ColBody)
			{
				this.ColBody[num2].material = this.PhysMaterial;
				num2++;
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x0000F29C File Offset: 0x0000D49C
		private void OnCollisionPropChange()
		{
			switch (base.Module.Blockstateinfo.CollisionTypeS)
			{
			case CollisionType.Discrete:
				this.ModrBody.collisionDetectionMode = 0;
				break;
			case CollisionType.Continuous:
				this.ModrBody.collisionDetectionMode = (CollisionDetectionMode)1;
				break;
			case CollisionType.ContinuousDynamic:
				this.ModrBody.collisionDetectionMode = (CollisionDetectionMode)2;
				break;
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x0000F300 File Offset: 0x0000D500
		public void OnMeshRenderChange()
		{
			bool blockColorSpecified = base.Module.Blockstateinfo.BlockColorSpecified;
			Color3 blockColor = base.Module.Blockstateinfo.BlockColor;
			bool flag = !blockColorSpecified;
			Color color;
			if (flag)
			{
				color = new Color(1f, 1f, 1f);
				
			}
			else
			{
				color = new Color(blockColor.r, blockColor.g, blockColor.b);
			}
			int num = 0;
			foreach (Renderer renderer in this.RenderComp)
			{
				this.RenderComp[num].sharedMaterial.SetColor("_Color", color);
				num++;
			}
		}

		// Token: 0x06000099 RID: 153 RVA: 0x0000F3B4 File Offset: 0x0000D5B4
		public float ZeroisInfinity(float f)
		{
			bool flag = f == 0f;
			if (flag)
			{
				f = float.PositiveInfinity;
			}
			return f;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000F3DC File Offset: 0x0000D5DC
		public override void SafeAwake()
		{
			this.BlockName = base.transform.name.Replace("(Clone)", "");
			try
			{
				this.MotionValue = new MotionType[]
				{
					base.Module.Springinfo.XMotionType,
					base.Module.Springinfo.YMotipnType,
					base.Module.Springinfo.ZMotionType
				};
				this.AmotionValue = new MotionType[]
				{
					base.Module.Rotateinfo.XAngMotionType,
					base.Module.Rotateinfo.YAngMotipnType,
					base.Module.Rotateinfo.ZAngMotionType
				};
				this.ColBody = base.transform.FindChild("Colliders").gameObject.GetComponentsInChildren<Collider>();
				this.JointComp = base.GetComponent<ConfigurableJoint>();
				this.ModrBody = base.GetComponent<Rigidbody>();
				this.RenderComp = base.transform.FindChild("Vis").gameObject.GetComponentsInChildren<Renderer>();
				bool flag = base.Module.Springinfo.useSlider;
				if (flag)
				{
					this.useSlider = true;
					this.SpringSlider = base.GetSlider(base.Module.Springinfo.SpringSlider);
					this.DamperSlider = base.GetSlider(base.Module.Springinfo.DamperSlider);
				}
				bool flag2 = base.Module.Rotateinfo.useSlider;
				if (flag2)
				{
					this.useSlider_Rot = true;
					this.SpringSlider_Rot = base.GetSlider(base.Module.Rotateinfo.SpringSlider);
					this.DamperSlider_Rot = base.GetSlider(base.Module.Rotateinfo.DamperSlider);
				}
				this.OnJointPropChanged();
				this.OnPysicPropChanged();
				this.OnCollisionPropChange();
				this.OnMeshRenderChange();
				bool colliderToAddPoint = base.Module.Blockstateinfo.ColliderToAddPoint;
				if (colliderToAddPoint)
				{
					foreach (Collider collider in this.ColBody)
					{
						GameObject gameObject = collider.gameObject;
						gameObject.layer = 12;
					}
				}
				bool disableDefaultCollider = base.Module.Blockstateinfo.DisableDefaultCollider;
				if (disableDefaultCollider)
				{
					foreach (Collider collider2 in this.ColBody)
					{
						GameObject gameObject2 = collider2.gameObject;
						Object.Destroy(gameObject2);
					}
				}
				ModCollider[] colliders = base.Module.Blockstateinfo.Colliders;
				bool flag3 = colliders != null;
				if (flag3)
				{
					foreach (ModCollider modCollider in colliders)
					{
						Collider collider3 = modCollider.CreateCollider(base.transform);
					}
				}
				bool useMeshCollider = base.Module.Blockstateinfo.useMeshCollider;
				if (useMeshCollider)
				{
					GameObject gameObject3 = new GameObject("ModMeshCollider");
					Transform transform = gameObject3.transform;
					transform.SetParent(base.transform.FindChild("Colliders"));
					MeshReference meshColliderObj = base.Module.Blockstateinfo.MeshColliderObj;
					ModMesh modMesh = (ModMesh)base.GetResource(base.Module.Blockstateinfo.MeshColliderObj);
					MeshCollider meshCollider = gameObject3.AddComponent<MeshCollider>();
					meshCollider.sharedMesh = modMesh;
					meshCollider.convex = true;
					meshCollider.enabled = true;
					transform.localPosition = meshColliderObj.Position;
					transform.localRotation = Quaternion.Euler(meshColliderObj.Rotation);
					transform.localScale = meshColliderObj.Scale;
				}
			}
			catch (Exception ex)
			{
				Debug.Log("ACM:Error BlockName : " + this.BlockName);
				Object.Destroy(this);
				return;
			}
			this.Init = false;
			this.anchorchange = false;
			this.InitWaitCount = 0;
			this.ObjectBehavior = base.GetComponent<BlockBehaviour>();
			bool useSubObject = base.Module.Blockstateinfo.useSubObject;
			if (useSubObject)
			{
				this.GetSubObjectPrefab();
				this.SubObjectPosition = base.Module.Blockstateinfo.SubObjectPosition;
				bool flag4 = base.transform.Find("SubTrigger");
				if (flag4)
				{
					this.SubObjectTrigger = base.transform.Find("SubTrigger").gameObject;
					TriggerSetSubVis component = this.SubObjectTrigger.GetComponent<TriggerSetSubVis>();
					component.block = this.ObjectBehavior;
					component.machine = base.Machine;
					component.SubVis = this.Prefab;
				}
				else
				{
					GameObject gameObject4 = base.transform.Find("TriggerForJoint").gameObject;
					this.SubObjectTrigger = Object.Instantiate<GameObject>(gameObject4);
					Object.Destroy(this.SubObjectTrigger.transform.GetComponent<TriggerSetJoint>());
					this.SubObjectTrigger.name = "SubTrigger";
					this.SubObjectTrigger.transform.SetParent(base.transform);
					this.SubObjectTrigger.transform.position = gameObject4.transform.position;
					this.SubObjectTrigger.transform.rotation = gameObject4.transform.rotation;
					TriggerSetSubVis triggerSetSubVis = this.SubObjectTrigger.AddComponent<TriggerSetSubVis>();
					triggerSetSubVis.block = this.ObjectBehavior;
					triggerSetSubVis.machine = base.Machine;
					triggerSetSubVis.SubVis = this.Prefab;
				}
			}
			bool flag5 = !base.Machine.InternalObject.isSimulating;
			if (flag5)
			{
				bool useSubObject2 = base.Module.Blockstateinfo.useSubObject;
				if (useSubObject2)
				{
					this.SubObject = this.Prefab.transform;
					this.SubObject.parent = base.transform;
					Transform transform2 = this.SubObject.FindChild("Vis");
					this.SubObject.position = base.transform.position;
					this.SubObject.rotation = base.transform.rotation;
					this.SubObjectPosition.SetOnTransform(this.SubObject);
					this.SubObject.gameObject.SetActive(true);
				}
			}
			else
			{
				bool flag6 = !StatMaster.isMP;
				if (flag6)
				{
					this.OwnerId = 0;
				}
				else
				{
					this.OwnerId = (int)base.Machine.InternalObject.PlayerID;
				}
			}
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00002191 File Offset: 0x00000391
		public void Awake()
		{
		}

		// Token: 0x0600009C RID: 156 RVA: 0x0000FA54 File Offset: 0x0000DC54
		public void Update()
		{
			bool disableAddPoint = base.Module.Blockstateinfo.DisableAddPoint;
			if (disableAddPoint)
			{
				bool flag = base.transform.FindChild("Adding Point") != null && !this.checkAddPoint;
				if (flag)
				{
					Object.Destroy(base.transform.FindChild("Adding Point").gameObject);
				}
				else
				{
					this.checkAddPoint = true;
				}
			}
		}

		// Token: 0x0600009D RID: 157 RVA: 0x0000FAC8 File Offset: 0x0000DCC8
		public void FixedUpdate()
		{
			bool flag = !base.Machine.InternalObject.isSimulating;
			if (!flag)
			{
				bool flag2 = !this.Init;
				if (flag2)
				{
					this.Init = true;
					UWaterBehaviourScript component = base.GetComponent<UWaterBehaviourScript>();
					bool flag3 = component != null;
					if (flag3)
					{
						component.floating = base.Module.Blockstateinfo.bouyancy;
					}
					bool flag4 = this.JointComp && (this.useSlider || this.useSlider_Rot);
					if (flag4)
					{
						this.OnJointPropChanged();
					}
				}
				bool flag5 = this.JointComp && this.InitWaitCount == 3;
				if (flag5)
				{
					this.OnJointAnchorChanged();
					this.InitWaitCount++;
				}
				else
				{
					bool flag6 = this.InitWaitCount < 3;
					if (flag6)
					{
						this.InitWaitCount++;
					}
				}
			}
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00002191 File Offset: 0x00000391
		public override void SimulateFixedUpdateClient()
		{
		}

		// Token: 0x0600009F RID: 159 RVA: 0x0000FBBC File Offset: 0x0000DDBC
		public GameObject GetSubObjectPrefab()
		{
			bool flag = this.Prefab != null;
			GameObject prefab;
			if (flag)
			{
				prefab = this.Prefab;
			}
			else
			{
				bool flag2 = base.transform.FindChild("SubVis");
				if (flag2)
				{
					this.Prefab = base.transform.FindChild("SubVis").gameObject;
					prefab = this.Prefab;
				}
				else
				{
					this.Prefab = Object.Instantiate<GameObject>(AdCustomModuleMod.mod.SubObjectTemplate);
					this.Prefab.name = "SubVis";
					Transform transform = this.Prefab.transform.FindChild("Vis").transform;
					MeshReference subMesh = base.Module.Blockstateinfo.SubMesh;
					ModMesh modMesh = (ModMesh)base.GetResource(base.Module.Blockstateinfo.SubMesh);
					ModTexture modTexture = (ModTexture)base.GetResource(base.Module.Blockstateinfo.SubTexture);
					transform.gameObject.GetComponent<MeshFilter>().mesh = modMesh;
					MeshRenderer component = transform.gameObject.GetComponent<MeshRenderer>();
					component.material.mainTexture = modTexture;
					transform.localPosition = subMesh.Position;
					transform.localRotation = Quaternion.Euler(subMesh.Rotation);
					transform.localScale = subMesh.Scale;
					this.Prefab.SetActive(false);
					prefab = this.Prefab;
				}
			}
			return prefab;
		}

		// Token: 0x0400013B RID: 315
		private MotionType[] MotionValue;

		// Token: 0x0400013C RID: 316
		private MotionType[] AmotionValue;

		// Token: 0x0400013D RID: 317
		private ConfigurableJoint JointComp;

		// Token: 0x0400013E RID: 318
		private JointDrive JointD;

		// Token: 0x0400013F RID: 319
		private JointDrive AJointD;

		// Token: 0x04000140 RID: 320
		private JointDrive AJointDYZ;

		// Token: 0x04000141 RID: 321
		private Rigidbody ModrBody;

		// Token: 0x04000142 RID: 322
		private Collider[] ColBody;

		// Token: 0x04000143 RID: 323
		private PhysicMaterial PhysMaterial;

		// Token: 0x04000144 RID: 324
		private SoftJointLimitSpring SoftJLSpring;

		// Token: 0x04000145 RID: 325
		private SoftJointLimitSpring SoftALSpring;

		// Token: 0x04000146 RID: 326
		private SoftJointLimit SoftJL;

		// Token: 0x04000147 RID: 327
		private SoftJointLimit SoftAL;

		// Token: 0x04000148 RID: 328
		private Renderer[] RenderComp;

		// Token: 0x04000149 RID: 329
		private bool Init = false;

		// Token: 0x0400014A RID: 330
		private bool anchorchange = false;

		// Token: 0x0400014B RID: 331
		private int InitWaitCount = 0;

		// Token: 0x0400014C RID: 332
		private GameObject Prefab;

		// Token: 0x0400014D RID: 333
		private AdTransformValues SubObjectPosition = new AdTransformValues();

		// Token: 0x0400014E RID: 334
		private string BlockName;

		// Token: 0x0400014F RID: 335
		private Transform SubObject;

		// Token: 0x04000150 RID: 336
		private GameObject SubObjectTrigger;

		// Token: 0x04000151 RID: 337
		private bool attachSubObject = false;

		// Token: 0x04000152 RID: 338
		private bool useSlider = false;

		// Token: 0x04000153 RID: 339
		private MSlider SpringSlider;

		// Token: 0x04000154 RID: 340
		private MSlider DamperSlider;

		// Token: 0x04000155 RID: 341
		private bool useSlider_Rot = false;

		// Token: 0x04000156 RID: 342
		private MSlider SpringSlider_Rot;

		// Token: 0x04000157 RID: 343
		private MSlider DamperSlider_Rot;

		// Token: 0x04000158 RID: 344
		private bool checkAddPoint = false;

		// Token: 0x04000159 RID: 345
		private bool checkCollider = false;

		// Token: 0x0400015A RID: 346
		private int OwnerId;

		// Token: 0x0400015B RID: 347
		public BlockBehaviour ObjectBehavior;
	}
}
