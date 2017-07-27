using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	public class OptionSoundScreen : MonoBehaviour {
		public GUISkin displaySkin;
		public GUISkin displaySkinLarge;
		
		public Rect effectVolumeSliderRect;
		public Rect musicVolumeSliderRect;
		public Rect effectVolumeLabelRect;
		public Rect musicVolumeLabelRect;
		public String effectVolumeLabel;
		public String musicVolumeLabel;
		private float oldMusicLevel;
		private float oldEffectLevel;
		public BackButton backButton;
		
		void Awake() {
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				displaySkin = displaySkinLarge;
				effectVolumeSliderRect = ScreenHelpers.scaleRectToScreen(effectVolumeSliderRect);
				musicVolumeSliderRect = ScreenHelpers.scaleRectToScreen(musicVolumeSliderRect);
				effectVolumeLabelRect = ScreenHelpers.scaleRectToScreen(effectVolumeLabelRect);
				musicVolumeLabelRect = ScreenHelpers.scaleRectToScreen(musicVolumeLabelRect);
			}
			oldEffectLevel = GameSounds.effectVolume;
			oldMusicLevel = GameSounds.gameMusicVolume;
			backButton.Awake();
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
		
		public void ScreenUpdate() {
			backButton.ScreenUpdate();
		}
		
		public void ScreenOnGUI() {
			GUI.skin = displaySkin;
			GameSounds.effectVolume = GUI.HorizontalSlider(effectVolumeSliderRect, GameSounds.effectVolume, 0.0f, 10.0f);
			GameSounds.gameMusicVolume = GUI.HorizontalSlider(musicVolumeSliderRect, GameSounds.gameMusicVolume, 0.0f, 10.0f);
			GUI.Label(effectVolumeLabelRect, effectVolumeLabel);
			GUI.Label(musicVolumeLabelRect, musicVolumeLabel);
			backButton.ScreenOnGUI();
			if(GameSounds.effectVolume != oldEffectLevel) {
				oldEffectLevel = GameSounds.effectVolume;
				GameData.save(SaveTarget.SoundVolume);
			}
			if(GameSounds.gameMusicVolume != oldMusicLevel) {
				oldMusicLevel = GameSounds.gameMusicVolume;
				GameData.save(SaveTarget.MusicVolume);
			}	
		}
	}
}