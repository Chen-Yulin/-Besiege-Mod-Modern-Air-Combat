using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace skpCustomModule
{
	// Token: 0x02000043 RID: 67
	public class RandomSoundPitch : MonoBehaviour
	{
		// Token: 0x17000035 RID: 53
		// (get) Token: 0x0600015C RID: 348 RVA: 0x000028A8 File Offset: 0x00000AA8
		private bool SourceActive
		{
			get
			{
				return this.audioSource.gameObject != null && this.audioSource != null && this.audioSource.gameObject.activeInHierarchy;
			}
		}

		// Token: 0x0600015D RID: 349 RVA: 0x000028DE File Offset: 0x00000ADE
		protected void Awake()
		{
			this.audioSource = base.GetComponent<AudioSource>();
			this.startPitch = this.audioSource.pitch;
		}

		// Token: 0x0600015E RID: 350 RVA: 0x000028FE File Offset: 0x00000AFE
		protected void SetPitch(float value)
		{
			this.audioSource.pitch = value;
		}

		// Token: 0x0600015F RID: 351 RVA: 0x0000290E File Offset: 0x00000B0E
		protected void SetClip(AudioClip clip)
		{
			this.audioSource.clip = clip;
		}

		// Token: 0x06000160 RID: 352 RVA: 0x000170DC File Offset: 0x000152DC
		public void Stop()
		{
			bool flag = this.SourceActive && this.audioSource.isPlaying;
			if (flag)
			{
				this.audioSource.Stop();
			}
		}

		// Token: 0x06000161 RID: 353 RVA: 0x00017114 File Offset: 0x00015314
		public void Play(float volume)
		{
			this.audioSource.volume = volume;
			bool flag = this.randomPitch2;
			if (flag)
			{
				bool flag2 = this.timeWarp;
				if (flag2)
				{
					this.SetPitch((this.startPitch + Random.Range(0f - this.pitchRange, this.pitchRange)) * Time.timeScale);
				}
				else
				{
					this.SetPitch(this.startPitch + Random.Range(0f - this.pitchRange, this.pitchRange));
				}
			}
			this.audioSource.Play();
		}

		// Token: 0x06000162 RID: 354 RVA: 0x000171A8 File Offset: 0x000153A8
		public void Play(AudioClip clip, float volume)
		{
			this.audioSource.volume = volume;
			bool flag = this.randomPitch2;
			if (flag)
			{
				bool flag2 = this.timeWarp;
				if (flag2)
				{
					this.SetPitch((this.startPitch + Random.Range(0f - this.pitchRange, this.pitchRange)) * Time.timeScale);
				}
				else
				{
					this.SetPitch(this.startPitch + Random.Range(0f - this.pitchRange, this.pitchRange));
				}
			}
			this.audioSource.PlayOneShot(clip, volume);
		}

		// Token: 0x040002AD RID: 685
		public bool randomPitch;

		// Token: 0x040002AE RID: 686
		public bool randomPitch2 = true;

		// Token: 0x040002AF RID: 687
		public bool usePlayOneShot = true;

		// Token: 0x040002B0 RID: 688
		public float pitchRange = 0.1f;

		// Token: 0x040002B1 RID: 689
		public bool timeWarp;

		// Token: 0x040002B2 RID: 690
		public bool randomVolume;

		// Token: 0x040002B3 RID: 691
		public float minVol = 0.1f;

		// Token: 0x040002B4 RID: 692
		public float maxVol = 1f;

		// Token: 0x040002B5 RID: 693
		public bool playOnStart;

		// Token: 0x040002B6 RID: 694
		public float delay;

		// Token: 0x040002B7 RID: 695
		public bool forcePlay;

		// Token: 0x040002B8 RID: 696
		public AudioSource audioSource;

		// Token: 0x040002B9 RID: 697
		private float startPitch;

		// Token: 0x040002BA RID: 698
		private bool networkSounds;

		// Token: 0x040002BB RID: 699
		private NetworkBlock netBlock;

		// Token: 0x040002BC RID: 700
		private bool started;
	}
}
