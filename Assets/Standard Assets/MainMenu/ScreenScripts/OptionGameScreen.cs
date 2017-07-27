using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	public class OptionGameScreen : MonoBehaviour {
		public int lengthChoiceXOffset;
		public int [] lengthChoiceValues;
		
		public String quarterLengthLabel;
		
		public Rect lengthChoiceArea;
		public Rect quarterLengthRect;

		public GUISkin baseSkin;
		public GUISkin baseSkinLarge;
		public GUISkin lengthChoiceSkin;
		public GUISkin lengthChoiceSkinLarge;
		
		public BackButton backButton;
		
		public void Awake() {
			if(Screen.height != ScreenHelpers.idealScreenHeight) {
				baseSkin = baseSkinLarge;
				lengthChoiceSkin = lengthChoiceSkinLarge;
				lengthChoiceArea = ScreenHelpers.scaleRectToScreen(lengthChoiceArea);
				quarterLengthRect = ScreenHelpers.scaleRectToScreen(quarterLengthRect);
				lengthChoiceXOffset *= (Screen.height / ScreenHelpers.idealScreenHeight);
			}
			backButton.Awake();
		}

		// Use this for initialization
		public void Start () {
		
		}
		
		// Update is called once per frame
		public void Update () {
		
		}
		
		public void ScreenUpdate() {
			backButton.ScreenUpdate();
		}
		
		public void ScreenOnGUI() {
			GUI.skin = baseSkin;
			GUI.Label(quarterLengthRect, quarterLengthLabel);
			GUI.skin = lengthChoiceSkin;
			for(int i = 0; i < lengthChoiceValues.Length; i++) {
				Rect tempRect = lengthChoiceArea;
				tempRect.x += i * (tempRect.width + lengthChoiceXOffset);
				if(GameData.quarterLength == lengthChoiceValues[i]) {
					GUI.Box(tempRect, "" + lengthChoiceValues[i]);
				} else if(GUI.Button(tempRect, "" + lengthChoiceValues[i])) {
					GameData.quarterLength = lengthChoiceValues[i];
					GameData.save(SaveTarget.QuarterLength);
				}
			}
			backButton.ScreenOnGUI();
		}
	}
}