using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	[System.Serializable] public class UniformItem {
		public String name;
		public Color previewColor;
		public int cost;
		public bool unlocked;
		public String unlockMessage;
		public int textureIndex;
		public Vector2 UVpos;
	}
}