using UnityEngine;
using System.Collections;

public class GameSounds : MonoBehaviour {
	public static float effectVolume;
	public static float gameMusicVolume;
	//public static List<MediaPlayerPlaylist> playlists;
	public static AudioSource backgroundMusic;
	
	void Awake() {
		//MediaPlayerBinding.useApplicationMusicPlayer(true);
		//playlists = MediaPlayerBinding.getPlaylists();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
