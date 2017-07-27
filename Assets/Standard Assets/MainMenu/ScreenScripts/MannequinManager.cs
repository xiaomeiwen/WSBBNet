using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	[System.Serializable] public class AnimPos
	{
		public String anim;
		public Transform trans;
	}

	public class MannequinManager : MonoBehaviour {
		private bool transitionEnd;
		
		public Transform basePoint;
		
		public AnimPos [] uniformPoints;
		public AnimPos [] benchPoints;
		public AnimPos itemPoint;
		
		public Renderer [] playerRenders;
		public Renderer [] ballRenders;

		public MainMenuState mainMenu;
		public Animation [] mannequins;
		public BBaller_skin [] skins;
		
		void Awake() {
			transitionEnd = false;
		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
			int i;
			if(mainMenu.currentState == MenuStates.Transition) {
				switch(mainMenu.nextState) {
					case MenuStates.Uniform:
						for(i = 0; i < 2; i++) {
							mannequins[i].transform.position = uniformPoints[i].trans.position;
							mannequins[i].transform.rotation = uniformPoints[i].trans.rotation;
							skins[i].refreshUVPos();
							playerRenders[i].enabled = true;
							ballRenders[i].enabled = false;
							mannequins[i].Play();
						}
						break;
					case MenuStates.TeamSelect:
						for(i = 0; i < 6; i++) {
							skins[i].awayTeam = false;
							mannequins[i].transform.position = benchPoints[i].trans.position;
							mannequins[i].transform.rotation = benchPoints[i].trans.rotation;
							mannequins[i].Play(benchPoints[i].anim);
							skins[i].refreshUVPos();
							if(GameData.instance.teamBallers[i] == -1) {
								playerRenders[i].enabled = false;
								ballRenders[i].enabled = false;
							} else {
								playerRenders[i].enabled = true;
								ballRenders[i].enabled = true;
							}
						}
						break;
					case MenuStates.ItemSelect:
						skins[0].awayTeam = false;
						mannequins[0].transform.position = itemPoint.trans.position;
						mannequins[0].transform.rotation = itemPoint.trans.rotation;
						mannequins[0].Play(itemPoint.anim);
						skins[0].refreshUVPos();
						playerRenders[0].enabled = true;
						ballRenders[0].enabled = true;
						break;
					default:
						transitionEnd = true;
						break;
				}
			} else {
				if(transitionEnd) {
					for(i = 0; i < 6; i++) {
						mannequins[i].transform.position = basePoint.position;
						mannequins[i].transform.rotation = basePoint.rotation;
						mannequins[i].Stop();
					}
					transitionEnd = false;
				}
			}
		}
	}
}