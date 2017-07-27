using UnityEngine;
using System.Collections;

namespace WSBB {
	public class GameUniforms : MonoBehaviour {
		public static GameUniforms instance;
		public Texture2D [] textures;
		public Texture2D baseJerseyIcon;
		public Texture2D lockedIcon;
		public UniformItem [] homeJerseys;
		public UniformItem [] awayJerseys;
		public UniformItem [] homeShorts;
		public UniformItem [] awayShorts;
		public UniformItem [] homeShoes;
		public UniformItem [] awayShoes;
		
		void Awake() {
			GameUniforms.instance = this;
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public void saveUniformUnlock() {
			int i = 0;
			for(i = 0; i < homeJerseys.Length; i++) {
				PlayerPrefs.SetInt("HomeJerseys" + i, BoolToInt(homeJerseys[i].unlocked));
			}
			for(i = 0; i < awayJerseys.Length; i++) {
				PlayerPrefs.SetInt("AwayJerseys" + i, BoolToInt(awayJerseys[i].unlocked));
			}
			
			for(i = 0; i < homeShorts.Length; i++) {
				PlayerPrefs.SetInt("HomeShorts" + i, BoolToInt(homeShorts[i].unlocked));
			}
			for(i = 0; i < awayShorts.Length; i++) {
				PlayerPrefs.SetInt("AwayShorts" + i, BoolToInt(awayShorts[i].unlocked));
			}
			
			for(i = 0; i < homeShoes.Length; i++) {
				PlayerPrefs.SetInt("HomeShoes" + i, BoolToInt(homeShoes[i].unlocked));
			}
			for(i = 0; i < awayShoes.Length; i++) {
				PlayerPrefs.SetInt("AwayShoes" + i, BoolToInt(awayShoes[i].unlocked));
			}
		}
		
		public void loadUniformUnlock() {
			int i = 0;
			for(i = 0; i < homeJerseys.Length; i++) {
				homeJerseys[i].unlocked = IntToBool(PlayerPrefs.GetInt("HomeJerseys" + i));
			}
			for(i = 0; i < awayJerseys.Length; i++) {
				awayJerseys[i].unlocked = IntToBool(PlayerPrefs.GetInt("AwayJerseys" + i));
			}
			
			for(i = 0; i < homeShorts.Length; i++) {
				homeShorts[i].unlocked = IntToBool(PlayerPrefs.GetInt("HomeShorts" + i));
			}
			for(i = 0; i < awayShorts.Length; i++) {
				awayShorts[i].unlocked = IntToBool(PlayerPrefs.GetInt("AwayShorts" + i));
			}
			
			for(i = 0; i < homeShoes.Length; i++) {
				homeShoes[i].unlocked = IntToBool(PlayerPrefs.GetInt("HomeShoes" + i));
			}
			for(i = 0; i < awayShoes.Length; i++) {
				awayShoes[i].unlocked = IntToBool(PlayerPrefs.GetInt("AwayShoes" + i));
			}
		}
		
		private bool IntToBool(int value) {
			if(value == 0)
				return false;
			return true;
		}
		
		private int BoolToInt(bool value) {
			if(value)
				return 1;			
			return 0;
		}
	}
}