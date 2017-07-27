using UnityEngine;
using System.Collections;

namespace WSBB {
	public class DataLoader : MonoBehaviour {
		public bool forceLoad;

		public bool DebugMode = false;		//DebugMode
		
		void Awake () {
			if(!GameData.instance) {
				if(DebugMode)
					PlayerPrefs.SetInt("DebugMode", 1);
				else
					PlayerPrefs.SetInt("DebugMode", 0);

				Application.LoadLevelAdditive("DataLevel"); 
				
				Destroy(this.gameObject);
			}
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}