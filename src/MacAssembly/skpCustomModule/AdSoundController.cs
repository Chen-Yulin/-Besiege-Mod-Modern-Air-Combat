using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace skpCustomModule
{
	// Token: 0x02000044 RID: 68
	public class AdSoundController : MonoBehaviour
	{
		// Token: 0x06000164 RID: 356 RVA: 0x00017240 File Offset: 0x00015440
		public void Awake()
		{
			this.SourceObject = new GameObject("SourceObject");
			Object.DontDestroyOnLoad(this.SourceObject);
			this.audioSource = this.SourceObject.AddComponent<AudioSource>();
			this.OneShotSourceArray = new AudioSource[5];
			for (int i = 0; i < 5; i++)
			{
				this.OneShotSourceArray[i] = this.SourceObject.AddComponent<AudioSource>();
			}
			this.LoopSourceArray = new AudioSource[5];
			for (int j = 0; j < 5; j++)
			{
				this.LoopSourceArray[j] = this.SourceObject.AddComponent<AudioSource>();
				this.LoopSourceArray[j].loop = true;
			}
		}

		// Token: 0x06000165 RID: 357 RVA: 0x00002956 File Offset: 0x00000B56
		protected void SetLoopPitch(int channel, float value)
		{
			this.LoopSourceArray[channel].pitch = value;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00002968 File Offset: 0x00000B68
		protected void SetSEPitch(int channel, float value)
		{
			this.OneShotSourceArray[channel].pitch = value;
		}

		// Token: 0x06000167 RID: 359 RVA: 0x0000297A File Offset: 0x00000B7A
		public void SetLoopClip(int channel, AudioClip clip)
		{
			this.LoopSourceArray[channel].clip = clip;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0000298C File Offset: 0x00000B8C
		public void SetOneShotClip(int channel, AudioClip clip)
		{
			this.OneShotSourceArray[channel].clip = clip;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x000172EC File Offset: 0x000154EC
		public void LoopStop(int channel)
		{
			bool isPlaying = this.LoopSourceArray[channel].isPlaying;
			if (isPlaying)
			{
				this.LoopSourceArray[channel].Stop();
			}
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0001731C File Offset: 0x0001551C
		public void LoopPlay(float volume, int channel, bool timeWarp = false)
		{
			this.LoopSourceArray[channel].volume = volume;
			bool flag = !this.LoopSourceArray[channel].isPlaying;
			if (flag)
			{
				if (timeWarp)
				{
					this.SetLoopPitch(channel, (this.startPitch + Random.Range(0f - this.pitchRange, this.pitchRange)) * Time.timeScale);
				}
				this.LoopSourceArray[channel].Play();
			}
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00017390 File Offset: 0x00015590
		public void SEPlay(float volume, int channel, bool timeWarp = false)
		{
			this.OneShotSourceArray[channel].volume = volume;
			bool isPlaying = this.OneShotSourceArray[channel].isPlaying;
			if (isPlaying)
			{
				this.OneShotSourceArray[channel].Stop();
			}
			if (timeWarp)
			{
				this.SetSEPitch(channel, (this.startPitch + Random.Range(0f - this.pitchRange, this.pitchRange)) * Time.timeScale);
			}
			this.OneShotSourceArray[channel].Play();
		}

		// Token: 0x0600016C RID: 364 RVA: 0x00017410 File Offset: 0x00015610
		public void OneShotPlay(AudioClip clip, float volume, bool timeWarp = false)
		{
			this.audioSource.volume = volume;
			if (timeWarp)
			{
				this.audioSource.pitch = (this.startPitch + Random.Range(0f - this.pitchRange, this.pitchRange)) * Time.timeScale;
			}
			this.audioSource.PlayOneShot(clip, volume);
		}

		// Token: 0x040002BD RID: 701
		private const int ONE_SHOT_CHANNEL = 5;

		// Token: 0x040002BE RID: 702
		private const int LOOP_CHANNEL = 5;

		// Token: 0x040002BF RID: 703
		public bool randomPitch;

		// Token: 0x040002C0 RID: 704
		public bool randomPitch2 = true;

		// Token: 0x040002C1 RID: 705
		public bool usePlayOneShot = true;

		// Token: 0x040002C2 RID: 706
		public float pitchRange = 0f;

		// Token: 0x040002C3 RID: 707
		public bool timeWarp;

		// Token: 0x040002C4 RID: 708
		public bool randomVolume;

		// Token: 0x040002C5 RID: 709
		public float minVol = 0.1f;

		// Token: 0x040002C6 RID: 710
		public float maxVol = 1f;

		// Token: 0x040002C7 RID: 711
		public bool playOnStart;

		// Token: 0x040002C8 RID: 712
		public float delay;

		// Token: 0x040002C9 RID: 713
		public bool forcePlay;

		// Token: 0x040002CA RID: 714
		public GameObject SourceObject;

		// Token: 0x040002CB RID: 715
		public AudioSource audioSource;

		// Token: 0x040002CC RID: 716
		public AudioSource[] OneShotSourceArray;

		// Token: 0x040002CD RID: 717
		public AudioSource[] LoopSourceArray;

		// Token: 0x040002CE RID: 718
		private float startPitch = 1f;
	}
}
