using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace skpCustomModule
{
	// Token: 0x02000053 RID: 83
	public class AdExplosionEffect : SimBehaviour, IExplosionEffect
	{
		// Token: 0x060001CC RID: 460 RVA: 0x0001F0D4 File Offset: 0x0001D2D4
		protected override void Start()
		{
			base.Start();
			this.power *= 2f;
			this.upPower *= 0.25f;
			this.SFX2 = base.transform.GetComponent<RandomSoundPitch>();
			bool flag = this.BlockName != null;
			if (flag)
			{
				this.sound = AdCustomModuleMod.mod2.ExplodeSoundContainer[this.BlockName];
			}
		}

		// Token: 0x060001CD RID: 461 RVA: 0x00002BC0 File Offset: 0x00000DC0
		public void OnEnable()
		{
			this.hasExploded = false;
		}

		// Token: 0x060001CE RID: 462 RVA: 0x0001F150 File Offset: 0x0001D350
		public void FixedUpdate()
		{
			bool flag = !this.hasExploded;
			if (flag)
			{
				this.hasExploded = true;
				this.Explode();
			}
		}

		// Token: 0x060001CF RID: 463 RVA: 0x0001F17C File Offset: 0x0001D37C
		public bool OnExplode(float power, float upPower, float torquePower, Vector3 explosionPos, float radius, int mask)
		{
			bool flag = !base.isSimulating || !base.SimPhysics;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				bool flag2 = (mask & 32) != 0;
				if (flag2)
				{
					this.Explode();
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		// Token: 0x060001D0 RID: 464 RVA: 0x00002BCA File Offset: 0x00000DCA
		private void Explode()
		{
			base.StartCoroutine(this.ExplodeMessage());
		}

		// Token: 0x060001D1 RID: 465 RVA: 0x00002BDA File Offset: 0x00000DDA
		public IEnumerator ExplodeMessage()
		{
			bool flag = this.BlockName != null;
			if (flag)
			{
				this.SFX2.Play(this.sound, 0.5f);
			}
			this.ExplosionForce();
			this.hasExploded = true;
			yield break;
		}

		// Token: 0x060001D2 RID: 466 RVA: 0x0001F1C4 File Offset: 0x0001D3C4
		private void ExplosionForce()
		{
			bool flag = !base.SimPhysics;
			if (!flag)
			{
				this.explosionPos = base.transform.position;
				this.hitColliders = Physics.OverlapSphere(this.explosionPos, this.radius);
				Collider[] array = this.hitColliders;
				foreach (Collider collider in array)
				{
					bool flag2 = collider.attachedRigidbody && !this.prevRigidbodies.Contains(collider.attachedRigidbody) && collider.attachedRigidbody.gameObject.layer != 20 && collider.attachedRigidbody.gameObject.layer != 22 && collider.attachedRigidbody.tag != "KeepConstraintsAlways";
					if (flag2)
					{
						this.colAttachedRigidbody = collider.attachedRigidbody;
						this.colAttachedRigidbody.WakeUp();
						this.colAttachedRigidbody.constraints = 0;
						this.colAttachedRigidbody.AddExplosionForce(this.power, this.explosionPos, this.radius, this.upPower);
						this.prevRigidbodies.Add(this.colAttachedRigidbody);
						int num = 237;
						IExplosionEffect[] interfaces = (IExplosionEffect[])ReferenceMaster.GetInterfaces<IExplosionEffect>(this.colAttachedRigidbody.gameObject);
						IExplosionEffect[] array3 = interfaces;
						foreach (IExplosionEffect explosionEffect in array3)
						{
							if (explosionEffect != null)
							{
								explosionEffect.OnExplode(this.power, this.upPower, 0f, this.explosionPos, this.radius, num);
							}
						}
					}
				}
			}
		}

		// Token: 0x060001D3 RID: 467 RVA: 0x0001F36C File Offset: 0x0001D56C
		private void DustCraterQuad()
		{
			bool flag = !StatMaster.levelSimulating;
			if (!flag)
			{
				bool flag2 = this.dustCraterQuad == null;
				if (!flag2)
				{
					Vector3 position = base.transform.position;
					bool flag3 = StatMaster.ShowExplosionDecals && position.y < 5f;
					if (flag3)
					{
						this.dustCraterQuad.GetComponent<Renderer>().enabled = true;
						this.dustCraterQuad.parent = ReferenceMaster.physicsGoalInstance;
						this.dustCraterQuad.position = new Vector3(position.x, SingleInstanceFindOnly<AddPiece>.Instance.floorHeight + 0.05f, position.z);
						this.dustCraterQuad.forward = Vector3.up;
						Transform transform = this.dustCraterQuad;
						Vector3 localEulerAngles = this.dustCraterQuad.localEulerAngles;
						float x = localEulerAngles.x;
						Vector3 localEulerAngles2 = this.dustCraterQuad.localEulerAngles;
						transform.localEulerAngles = new Vector3(x, localEulerAngles2.y, Random.Range(0f, 360f));
					}
					else
					{
						this.dustCraterQuad.GetComponent<Renderer>().enabled = false;
					}
				}
			}
		}

		// Token: 0x040003F9 RID: 1017
		public float radius = 5f;

		// Token: 0x040003FA RID: 1018
		public float power = 10f;

		// Token: 0x040003FB RID: 1019
		public float upPower = 3f;

		// Token: 0x040003FC RID: 1020
		public string BlockName;

		// Token: 0x040003FD RID: 1021
		public List<AudioClip> soundlist = new List<AudioClip>();

		// Token: 0x040003FE RID: 1022
		public AudioClip sound = new AudioClip();

		// Token: 0x040003FF RID: 1023
		public RandomSoundController SFX;

		// Token: 0x04000400 RID: 1024
		public RandomSoundPitch SFX2;

		// Token: 0x04000401 RID: 1025
		public Transform dustCraterQuad;

		// Token: 0x04000402 RID: 1026
		public bool hasExploded;

		// Token: 0x04000403 RID: 1027
		private Vector3 explosionPos;

		// Token: 0x04000404 RID: 1028
		private Collider[] hitColliders;

		// Token: 0x04000405 RID: 1029
		private Rigidbody colAttachedRigidbody;

		// Token: 0x04000406 RID: 1030
		private List<Rigidbody> prevRigidbodies = new List<Rigidbody>();
	}
}
