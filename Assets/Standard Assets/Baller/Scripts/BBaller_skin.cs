using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	public enum TeamDataSource {localPlayer, multiPlayer, computer, localPlayerStart, computerStart};

	public class BBaller_skin : MonoBehaviour {
		public bool awayTeam;
		public TeamDataSource teamDataSource;
		[HideInInspector]public Transform myTransform;
		public BBallerData ballerData;
		public int dataIndex;
		public Renderer myRenderer;
		public Renderer ballRenderer;
		public bool updateSelf;
		public bool inGame;
		private bool delayLoad;
		
		void Awake() {
			myTransform = this.transform;
			//myRenderer = this.transform.Find("Basketballer/DD_Mesh").GetComponent<Renderer>();
			//ballRenderer = this.transform.Find("Basketballer/Ball_Mesh").GetComponent<Renderer>();

			if(GameData.instance == null) {
				delayLoad = true;
				return;
			}
			delayLoad = false;
			fetchData();
			//refreshUVPos();
		}

		// Use this for initialization
		void Start () {
		}
		
		// Update is called once per frame
		void Update () {
			if(delayLoad) {
				if(GameData.instance != null && GameData.present) {
					fetchData();
					//refreshUVPos();
					delayLoad = false;
				}
			}
		}

		public void fetchData() {
			if(GameData.instance == null) {
				delayLoad = true;
				return;
			}
			//Debug.Log("Fetching Data: " + dataIndex);
			Vector2 itemValue;
			int playerIndex;
			if(teamDataSource == TeamDataSource.localPlayerStart) {
				if(GameData.instance.teamStarters[dataIndex]!= -1) {
					if(GameData.instance.teamBallers[GameData.instance.teamStarters[dataIndex]]!= -1) {
						playerIndex = GameData.instance.teamBallers[GameData.instance.teamStarters[dataIndex]];
						ballerData = GameBallers.instance.ballers[playerIndex].cloneBallerData();
						itemValue = SharkTankRessurection.GameItems.instance.playerAssignedItems[GameData.instance.teamStarters[dataIndex]];
						ballerData.item = SharkTankRessurection.GameItems.instance.itemSets[(int)itemValue.x].items[(int)itemValue.y].CloneItemDataForGame(true);
					}
					awayTeam = GameData.instance.playerHomeTeam;
				}
			} else if(teamDataSource == TeamDataSource.localPlayer) {
				if(GameData.instance.teamBallers[dataIndex] != -1) {
					ballerData = GameBallers.instance.ballers[GameData.instance.teamBallers[dataIndex]].cloneBallerData();
					itemValue = SharkTankRessurection.GameItems.instance.playerAssignedItems[dataIndex];
					ballerData.item = SharkTankRessurection.GameItems.instance.itemSets[(int)itemValue.x].items[(int)itemValue.y].CloneItemDataForGame(true);
				}
				awayTeam = GameData.instance.playerHomeTeam;
			} else if(teamDataSource == TeamDataSource.computer) {
				if(GameData.instance.computerBallers[dataIndex] != -1) {
					ballerData = GameBallers.instance.ballers[GameData.instance.computerBallers[dataIndex]].cloneBallerData();
					itemValue = SharkTankRessurection.GameItems.instance.playerAssignedItems[dataIndex];
					ballerData.item = SharkTankRessurection.GameItems.instance.itemSets[(int)itemValue.x].items[(int)itemValue.y].CloneItemDataForGame(true);
				}
				awayTeam = GameData.instance.computerHomeTeam;
			} else if(teamDataSource == TeamDataSource.computerStart) {
				if(GameData.instance.computerStarters[dataIndex] != -1) {
					if(GameData.instance.computerBallers[GameData.instance.computerStarters[dataIndex]] != -1) {
						playerIndex = GameData.instance.computerBallers[GameData.instance.computerStarters[dataIndex]];
						ballerData = GameBallers.instance.ballers[GameData.instance.computerBallers[dataIndex]].cloneBallerData();
						itemValue = SharkTankRessurection.GameItems.instance.playerAssignedItems[GameData.instance.computerStarters[dataIndex]];
						ballerData.item = SharkTankRessurection.GameItems.instance.itemSets[(int)itemValue.x].items[(int)itemValue.y].CloneItemDataForGame(true);
					}
					awayTeam = GameData.instance.computerHomeTeam;
				}
			}
			
			ballerData.myBaller = this;
		}
		
		public void fetchAndRefresh() {
			fetchData();
			//refreshUVPos();
		}
		
		public void refreshUVPos() {
			if(GameUniforms.instance == null)
				return;
			
//			resetSkinUV();
//			resetHeadUV();
//			resetJerseyUV();
//			resetShortUV();
//			resetShoeUV();
		}
		
		private void resetSkinUV() {
			Material myMaterial = getMaterialByName("Skin");
			switch(ballerData.skinColor) {
				case SkinTone.White:
					myMaterial.mainTextureOffset = new Vector2(0, 0);
					break;
				case SkinTone.Hispanic:
					myMaterial.mainTextureOffset = new Vector2(0.75f, 0);
					break;
				case SkinTone.Asian:
					myMaterial.mainTextureOffset = new Vector2(0.5f, 0);
					break;
				case SkinTone.Black:
					myMaterial.mainTextureOffset = new Vector2(0.25f, 0);
					break;
			}
		}
		
		private void resetHeadUV() {
			Material myMaterial = getMaterialByName("Head");
			float skinXAdjust = 0.0f;
			Vector2 headPos = new Vector2(0.0f, 0.0f);
			switch(ballerData.skinColor) {
				case SkinTone.White:
					skinXAdjust  = 0;
					break;
				case SkinTone.Hispanic:
					skinXAdjust  = 0.75f;
					break;
				case SkinTone.Asian:
					skinXAdjust  = 0.5f;
					break;
				case SkinTone.Black:
					skinXAdjust  = 0.25f;
					break;
			}
			switch(ballerData.headSkin) {
				case 0:
					headPos = new Vector2(0 + skinXAdjust, 0);
					break;
				case 1:
					headPos = new Vector2(0.125f + skinXAdjust, 0);
					break;
				case 2:
					headPos = new Vector2(0.125f + skinXAdjust, 0.75f);
					break;
			}
			
			myMaterial.mainTextureOffset = headPos;
		}
		
		private void resetJerseyUV() {
			Material myMaterial = getMaterialByName("Jersey");
			int uniformItemTexture;
			Vector2 uniformItemUVpos;
			int jerseyValue = getUniformValue(GameData.instance.teamJersey, 
			                                        GameData.instance.teamJerseyAway, 
			                                        GameData.instance.computerJersey);
			
			if(awayTeam) {
				uniformItemTexture = GameUniforms.instance.awayJerseys[jerseyValue].textureIndex;
				uniformItemUVpos = GameUniforms.instance.awayJerseys[jerseyValue].UVpos;
			} else {
				uniformItemTexture = GameUniforms.instance.homeJerseys[jerseyValue].textureIndex;
				uniformItemUVpos = GameUniforms.instance.homeJerseys[jerseyValue].UVpos;
			}
			
			myMaterial.mainTexture = GameUniforms.instance.textures[uniformItemTexture];
			myMaterial.mainTextureOffset = uniformItemUVpos;
		}

		private void resetShortUV() {
			Material myMaterial = getMaterialByName("Shorts");
			int uniformItemTexture;
			Vector2 uniformItemUVpos;
			int shortsValue = getUniformValue(GameData.instance.teamShorts, 
			                                        GameData.instance.teamShortsAway, 
			                                        GameData.instance.computerShorts);
			
			if(awayTeam) {
				uniformItemTexture = GameUniforms.instance.awayShorts[shortsValue].textureIndex;
				uniformItemUVpos = GameUniforms.instance.awayShorts[shortsValue].UVpos;
			} else {
				uniformItemTexture = GameUniforms.instance.homeShorts[shortsValue].textureIndex;
				uniformItemUVpos = GameUniforms.instance.homeShorts[shortsValue].UVpos;
			}
			
			myMaterial.mainTexture = GameUniforms.instance.textures[uniformItemTexture];
			myMaterial.mainTextureOffset = uniformItemUVpos;
		}
		
		private void resetShoeUV() {
			Material myMaterial = getMaterialByName("Shoes");
			int uniformItemTexture;
			Vector2 uniformItemUVpos;
			int shoeValue = getUniformValue(GameData.instance.teamShoes, 
			                                      GameData.instance.teamShoesAway, 
			                                      GameData.instance.computerShoes);
			
			if(awayTeam) {
				uniformItemTexture = GameUniforms.instance.awayShoes[shoeValue].textureIndex;
				uniformItemUVpos = GameUniforms.instance.awayShoes[shoeValue].UVpos;
			} else {
				uniformItemTexture = GameUniforms.instance.homeShoes[shoeValue].textureIndex;
				uniformItemUVpos = GameUniforms.instance.homeShoes[shoeValue].UVpos;
			}
			
			myMaterial.mainTexture = GameUniforms.instance.textures[uniformItemTexture];
			myMaterial.mainTextureOffset = uniformItemUVpos;
		}
		
		private Material getMaterialByName(String materialName) {
			Material myMaterial = null;
			for(int i = 0; i < myRenderer.materials.Length; i++) {
				Material currentMat;
				currentMat = myRenderer.materials[i];
				if(currentMat.name.IndexOf(materialName) != -1) {
					myMaterial = currentMat;
				}
			}
			return myMaterial;
		}
		
		private int getUniformValue(int teamHomeValue, int teamAwayValue, int computerValue) {
			int endValue = teamHomeValue;
			switch(teamDataSource) {
				case TeamDataSource.localPlayer:
					if(awayTeam)
						endValue  = teamAwayValue;
					else
						endValue = teamHomeValue;
					break;
				case TeamDataSource.computer:
					endValue = computerValue;
					break;	
			}
			return endValue; 
		}
	}
}