using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace XMLSpace
{
	public class SelectTeam : MonoBehaviour {
	    public static string selectedTeamName;
	    GameObject teamType;
//	    Team clickedTeam;
		public TeamLoader clickedTeamLoader;
		public GameObject defaultButton;
	    GameObject playerSpeed, playerAgility, playerPower;
		WSBB.XMLDataParser b_XMLDataParser;
//	    Dictionary<GameObject,bool> teamDict = new Dictionary<GameObject,bool>();
//	    bool testing = true;

	    // new variables
		public GameObject speed_image = new GameObject();
		public GameObject agility_image = new GameObject();
		public GameObject power_image = new GameObject();
//	    public GameObject[] defaultWidthObject = new GameObject[3];
//	    float[] defaultWidth = new float[3];
//	    float[] agilityDefaultXPos = new float[3];
//	    float[] powerDefaultXPos = new float[3];
//	    float[] speedDefaultXPos = new float[3]; 
		float speedDefaultXPos;
		float agilityDefaultXPos;
		float powerDefaultXPos;

		// Use this for initialization
	    void Awake()
	    {
			speedDefaultXPos = speed_image.GetComponent<RectTransform> ().localPosition.x;
			agilityDefaultXPos = agility_image.GetComponent<RectTransform> ().localPosition.x;
			powerDefaultXPos = power_image.GetComponent<RectTransform> ().localPosition.x;

//			Debug.Log ("speedPos: " + speedDefaultXPos + "agilityPos: " + agilityDefaultXPos + "powerPos: " + powerDefaultXPos);
//	        for (int i = 0; i < 3; ++i)
//	        {
//				agilityDefaultXPos[i] = agility_image[i].GetComponent<RectTransform>().localPosition.x;
//	            powerDefaultXPos[i] = power_image[i].GetComponent<RectTransform>().localPosition.x;
//	            speedDefaultXPos[i] = speed_image[i].GetComponent<RectTransform>().localPosition.x;
//	            defaultWidth[i] = defaultWidthObject[i].GetComponent<RectTransform>().rect.width;
//	        }


	    }
	    void Start () {
			buttonClicking (defaultButton);
		}
	   
		// Update is called once per frame
		void Update () {
	    }

		public void buttonClicking(GameObject myButton)
	    {
//			Debug.Log (b_XMLDataParser.d_team.bballers[0].firstName);

	        //Formula.resetBars(agilityButtonTabs, speedButtonTabs, powerButtonTabs, agilityDefaultXPos, speedDefaultXPos, powerDefaultXPos);
			selectedTeamName = myButton.GetComponent<Button>().name;
//			Debug.Log ("team: " + buttonName);
//			teamType = GameObject.Find(buttonName);
//			clickedTeamLoader = (TeamLoader)teamType.GetComponent(typeof(TeamLoader));
//			clickedTeam = clickedTeamLoader.MyTeam;
			clickedTeamLoader = GameObject.Find(selectedTeamName).GetComponent<TeamLoader>();
			Debug.Log ("team: " + selectedTeamName);

	        int totalSpeed = 0;
			int totalAgility = 0;
	        int totalPower = 0;
			totalSpeed = clickedTeamLoader.player1_t_speedPercentage + clickedTeamLoader.player2_t_speedPercentage + clickedTeamLoader.player3_t_speedPercentage;
			totalAgility = clickedTeamLoader.player1_t_agilityPercentage + clickedTeamLoader.player2_t_agilityPercentage + clickedTeamLoader.player3_t_agilityPercentage;
			totalPower = clickedTeamLoader.player1_t_powerPercentage + clickedTeamLoader.player2_t_powerPercentage + clickedTeamLoader.player3_t_powerPercentage;

//			Debug.Log ("speed: " + totalSpeed + " agility: " + totalAgility + " power: " + totalPower);

			Formula.changeTeamBarLength(speed_image.GetComponent<RectTransform>(), agility_image.GetComponent<RectTransform>(), power_image.GetComponent<RectTransform>(),
									totalSpeed, totalAgility, totalPower);
//	        for (int i = 0; i < 3; i++ )
//	        {
//	            Formula.resetTotalBars(speed_image[i].GetComponent<RectTransform>(), agility_image[i].GetComponent<RectTransform>(), power_image[i].GetComponent<RectTransform>(),
//	                defaultWidth[i], powerDefaultXPos[i], agilityDefaultXPos[i]);
//
//	            Formula.changeAllBarLength(speed_image[i].GetComponent<RectTransform>(), agility_image[i].GetComponent<RectTransform>(), power_image[i].GetComponent<RectTransform>(),
//					totalSpeed, totalAgility, totalPower);
//	        }
	    }
	}
}