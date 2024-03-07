using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Chromecore
{
	public class AudioPlayer : MonoBehaviour
	{
        // singleton
		public static AudioPlayer instance;

		private void Awake()
		{
			if (instance != null)
			{
				Destroy(gameObject);
				return;
			}
			instance = this;

			int numMusicPlayers = FindObjectsOfType<AudioPlayer>().Length;

			if (numMusicPlayers > 1)
			{
				Destroy(gameObject);
			}
			else
			{
				DontDestroyOnLoad(gameObject);
			}
		}

		// audio clips
		public SoundAudioClip[] musicAudioClipArray;
		public SoundAudioClip[] soundAudioClipArray;

		[System.Serializable]
		public class SoundAudioClip
		{
			public Sound sound;
			public AudioClip audioClip;
			public float volume = 1f;
			public float pitch = 1f;
		}

		[SerializeField] AudioSource musicAudioSource = null;
		[SerializeField] List<AudioSource> soundAudioSources = new List<AudioSource>();

		bool mute = false;
		bool changingMusic = false;

		float musicVolume = 1;
		float soundVolume = 1;

		public void PlaySound(Sound sound)
		{
			if (soundAudioSources.Count > 0)
			{
				AudioSource audioSource = soundAudioSources[0];
				soundAudioSources.Remove(soundAudioSources[0]);
				audioSource.pitch = GetPitchSound(sound);
				audioSource.volume = mute ? 0f : GetVolume(sound) * soundVolume;
				audioSource.PlayOneShot(GetAudioClip(sound));
				StartCoroutine(PutAudioSourceBack(audioSource, GetLength(sound)));
			}
		}

		IEnumerator PutAudioSourceBack(AudioSource audioSource, float length)
		{
			yield return new WaitForSeconds(length);
			soundAudioSources.Add(audioSource);
		}

		public void PlayMusic(Sound sound)
		{
			if (musicAudioSource.clip == null)
			{
				musicAudioSource.clip = GetAudioClip(sound);
				musicAudioSource.Play();
			}
			else
			{
				if ((GetAudioClip(sound) != musicAudioSource.clip) && !changingMusic)
				{
                    StartCoroutine(ChangeMusic(sound));
                }
			}
			musicAudioSource.volume = mute ? 0f : GetVolume(sound) * musicVolume;
		}

		IEnumerator ChangeMusic(Sound sound)
		{
			changingMusic = true;
			yield return new WaitForSeconds(1f);
			musicAudioSource.Stop();
			musicAudioSource.clip = GetAudioClip(sound);
			musicAudioSource.Play();
			changingMusic = false;
		}

		static AudioClip GetAudioClip(Sound sound)
		{
			foreach (SoundAudioClip soundAudioClip in instance.soundAudioClipArray)
			{
				if (soundAudioClip.sound == sound)
				{
					return soundAudioClip.audioClip;
				}
			}
			foreach (SoundAudioClip soundAudioClip in instance.musicAudioClipArray)
			{
				if (soundAudioClip.sound == sound)
				{
					return soundAudioClip.audioClip;
				}
			}
			Debug.LogError("Sound " + sound + " not found!");
			return null;
		}

		static float GetVolume(Sound sound)
		{
			foreach (SoundAudioClip soundAudioClip in instance.soundAudioClipArray)
			{
				if (soundAudioClip.sound == sound)
				{
					return soundAudioClip.volume;
				}
			}
			foreach (SoundAudioClip soundAudioClip in instance.musicAudioClipArray)
			{
				if (soundAudioClip.sound == sound)
				{
					return soundAudioClip.volume;
				}
			}
			Debug.LogError("Sound " + sound + " volume not found!");
			return 1f;
		}

		static float GetPitchSound(Sound sound)
		{
			foreach (SoundAudioClip soundAudioClip in instance.soundAudioClipArray)
			{
				if (soundAudioClip.sound == sound)
				{
					return soundAudioClip.pitch + Random.Range(-0.2f, 0.2f);
				}
			}
			Debug.LogError("Sound " + sound + " pitch not found!");
			return 1f;
		}

		static float GetLength(Sound sound)
		{
			foreach (SoundAudioClip soundAudioClip in instance.soundAudioClipArray)
			{
				if (soundAudioClip.sound == sound)
				{
					return soundAudioClip.audioClip.length;
				}
			}
			Debug.LogError("Sound " + sound + " length not found!");
			return 1f;
		}

		public void Mute()
		{
			mute = true;
			musicAudioSource.volume = 0f;
			foreach (AudioSource audioSource in soundAudioSources)
			{
				audioSource.volume = 0f;
			}
		}

		public void UnMute()
		{
			mute = false;
			musicAudioSource.volume = 0.4f * musicVolume;
			foreach (AudioSource audioSource in soundAudioSources)
			{
				audioSource.volume = 0.4f * soundVolume;
			}
		}

		public void SetMusicVolume(float volume)
		{
			musicVolume = volume;
			musicAudioSource.volume = 0.4f * volume;
		}

		public void SetSoundVolume(float volume)
		{
			soundVolume = volume;
			foreach (AudioSource audioSource in soundAudioSources)
			{
				audioSource.volume = 0.4f * soundVolume;
			}
		}

		public float GetMusicVolume()
		{
			return musicVolume;
		}

		public float GetSoundVolume()
		{
			return soundVolume;
		}
    }

	public enum Sound
	{
		mainMusic,
		shoot,
        shootBig,
	}
}