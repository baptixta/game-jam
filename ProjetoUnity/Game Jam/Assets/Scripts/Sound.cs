using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
	public string Name;
	public AudioClip Clip;
	[Range(0f, 1f)]
	public float Volume = 1;
	[Range(0.1f, 2.1f)]
	public float Pitch = 1;
	public bool Loop = false;
	public bool PlayOnAwake = false;
	[HideInInspector]
	public AudioSource Source;
}