using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	/*
	 * 	SFX and Music Player/Rotator Script
	 * 	placed on main camera with 2 audio sources
	 * 	0 used for music, 1 used for sfx
	 */

	// music to play through AudioSource 0
	public List<AudioClip> songs;
	public static bool fadeMusicOut;

	// SFX to play through AudioSource 1
	private List<AudioClip> sfxClips;
	public AudioClip sfxWings;
	public AudioClip sfxInkShort;
	public AudioClip sfxInkLong1;
	public AudioClip sfxInkLong2;
	public AudioClip sfxPage;
	public AudioClip sfxTapping;
	// tell update to play a sfx
	private List<bool> playCues;
	public static bool playSfx_Wings;
	public static bool playSfx_InkShort;
	public static bool playSfx_InkLong1;
	public static bool playSfx_InkLong2;
	public static bool playSfx_Page;
	public static bool playSfx_Tapping;


	void Start () {

		// list of bools to toggle sfx on
		// currently the loop that checks this in Update always gets false
		playCues = new List<bool> ();
		playCues.Add (playSfx_Wings);
		playCues.Add (playSfx_InkShort);
		playCues.Add (playSfx_InkLong1);
		playCues.Add (playSfx_InkLong2);
		playCues.Add (playSfx_Page);
		playCues.Add (playSfx_Tapping);

		// list of sfx clips - indices MUST match associated bools!
		sfxClips = new List<AudioClip> ();
		sfxClips.Add (sfxWings);
		sfxClips.Add (sfxInkShort);
		sfxClips.Add (sfxInkLong1);
		sfxClips.Add (sfxInkLong2);
		sfxClips.Add (sfxPage);
		sfxClips.Add (sfxTapping);

		// initialize all audio triggers to false
		for (int i=0; i<playCues.Count; i++) {
			playCues[i] = false;
		}
		fadeMusicOut = false;

	}


	void Update () {

		// pick one of the songs and play music if not already playing
		if (!GetComponents<AudioSource>()[0].isPlaying) {
			PlaySound (songs[Random.Range (0,songs.Count-1)], 0);
		}

//		// check for any activated sfx through static bools
//		for (int i=0; i<playCues.Count; i++) {
//			// play any sfx that were toggled
//			if (playCues[i] == true) {
//				PlaySound (sfxClips[i], 1);
//				// toggle off
//				playCues[i] = false;
//			}
//		}

		// slowly fade music out
		if (fadeMusicOut && GetComponent<AudioSource>().volume > 0.01f) {
			GetComponent<AudioSource>().volume = Mathf.Lerp (GetComponent<AudioSource>().volume, 0f, Time.deltaTime);
		}

		// play sound effects if not already playing
//		if (!GetComponents<AudioSource>()[1].isPlaying) {
		// go through all bools and play any sounds that triggered
		if (playSfx_Wings) {
			PlaySound (sfxWings, 1);
			playSfx_Wings = false;
		} else if (playSfx_InkShort) {
			PlaySound (sfxInkShort, 1);
			playSfx_InkShort = false;
		} else if (playSfx_InkLong1) {
			PlaySound (sfxInkLong1, 1);
			playSfx_InkLong1 = false;
		} else if (playSfx_InkLong2) {
			PlaySound (sfxInkLong2, 1);
			playSfx_InkLong2 = false;
		} else if (playSfx_Page) {
			PlaySound (sfxPage, 1);
			playSfx_Page = false;
		} else if (playSfx_Tapping) {
			PlaySound (sfxTapping, 1);
			playSfx_Tapping = false;
		}
//		}

	}


	/**
	 * 	Bundle the audio source select, clip load and play actions
	 */
	private void PlaySound (AudioClip thisSound, int channel) {
		GetComponents<AudioSource>()[channel].clip = thisSound;
		GetComponents<AudioSource>()[channel].Play ();
	}

}
