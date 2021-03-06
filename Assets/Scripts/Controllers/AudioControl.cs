using UnityEngine;
using System.Collections;

public class AudioControl : MonoBehaviour
{


	public static void PlayAudioFile(string file) {
		var clip = (AudioClip) Resources.Load("Sounds/" + file);
		Camera.main.audio.clip = clip;
		Camera.main.audio.audio.Play();
	}
	
	public static void PlayAudioFileAt(string file, Vector3 position) {
		var clip = (AudioClip) Resources.Load("Sounds/" + file);
		AudioSource.PlayClipAtPoint(clip, position);
	}

	public void PlayAudio(AudioClip clip) {
		Camera.main.audio.PlayOneShot(clip);
	}
	
	public void PlayNewTurnSound() {
		PlayAudio(Assets.Instance.NewTurnSound);
	}
	
	public void PlayErrorSound() {
		PlayAudio(Assets.Instance.ErrorSound);
	}
	
	public void PlayEndTurnSound() {
		PlayAudio(Assets.Instance.EndTurnSound);
	}
	
}

