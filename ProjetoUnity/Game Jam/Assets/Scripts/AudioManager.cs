using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public Sound[] Sounds;
	public static AudioManager instance;

	void Awake ()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}
		foreach (Sound sound in Sounds)
		{
			sound.Source = gameObject.AddComponent<AudioSource>();
			sound.Source.clip = sound.Clip;
			sound.Source.volume = sound.Volume;
			sound.Source.pitch = sound.Pitch;
			sound.Source.loop = sound.Loop;
			if (sound.PlayOnAwake)
			PlaySound (sound.Name);
		}
		DontDestroyOnLoad(gameObject);
	}

	public void SetPitch (string name, float pitch)
	{
		Sound SoundToUse = Array.Find (Sounds, sound => sound.Name == name);
		SoundToUse.Source.pitch = pitch;
	}

	public void SetVolume (string Name, float Volume)
	{
		Sound SoundToUse = Array.Find (Sounds, sound => sound.Name == Name);
		SoundToUse.Source.volume = Volume;
	}

	public void PlaySound (string Name)
	{
		Sound SoundToUse = Array.Find (Sounds, sound => sound.Name == Name);
		SoundToUse.Source.Play();
	}
}