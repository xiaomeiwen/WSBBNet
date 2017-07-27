using UnityEngine;
using System.Collections;

namespace WSBB {
	public class GameBallers : MonoBehaviour {
		
		public static GameBallers instance;

		public BBallerData [] ballers;
		
		void Awake() {
			GameBallers.instance = this;
			//GameObject.DontDestroyOnLoad(this.gameObject);
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}
	}
}