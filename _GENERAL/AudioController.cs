using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioController : MonoBehaviour 
{
	public static AudioController instance;
	public AudioSource sfx;
	public AudioSource music;
	public AudioClip[] sfxList;
	public Dictionary<string, AudioClip> sfxTable = new Dictionary<string, AudioClip>();

	void Awake()
	{
		instance = this;
	}

	void Start()
	{
		foreach(AudioClip aud in sfxList)
		{
			if(aud != null)
			{
				sfxTable.Add(aud.name, aud);
			}
		}
		sfxList = null;
	}

	public void PlaySingle(string name)
	{
		sfx.clip = sfxTable[name];
		sfx.Play();
	}

}
