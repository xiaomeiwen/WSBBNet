using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	[System.Serializable] public class DunkItem {

		public String popUpMessage;
		public String animationToPlay;

		public bool unlocked;

		public int cost;
		public int levelNeeded;

		public Texture2D selectionImage;
		public SharkTankRessurection.Item effect;
	}
}