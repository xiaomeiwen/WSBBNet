using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using UnityEngine.UI;
using XMLSpace;

namespace WSBB
{
	public class XMLDataParser : MonoBehaviour
	{
	   
		public static ArrayList powerUpList;
//		public ArrayList teams;
		public static Dictionary <int, BBaller> playerDictionary;
//		public static Dictionary <int, DefaultPlayer> playerDictionary;
	    GameObject[] teamButtonList;
//	    Dictionary<int,Button> sortedButtonList =  new Dictionary<int,Button>();
		public GameObject teamObject;
		TeamLoader teamObj;

		int teamNum = 1;

	    void Awake()
	    {
	        
	    }
	    void Start()
	    {
			LoadTeamData();
	    }
	    public void LoadTeamData(){
	        //Create a new XML document out of the file
	        XDocument xmlPowerupDoc = XDocument.Load(Application.dataPath+ "/Standard Assets/XmlImporter/xml/powerups.xml");
			XDocument xmlPlayerTeams = XDocument.Load(Application.dataPath + "/Standard Assets/XmlImporter/xml/teams.xml");
			XDocument xmlPlayers = XDocument.Load(Application.dataPath + "/Standard Assets/XmlImporter/xml/default_players.xml");
		    //Process nodes
	        ProcessPowerupNodes(xmlPowerupDoc);
	        
	//        teamButtonList = GameObject.FindGameObjectsWithTag("Teams");
	//        foreach (GameObject go in teamButtonList)
	//        {
	//            string buttonName = go.GetComponent<Button>().name;
	//            sortedBottonList.Add(Convert.ToInt16(Regex.Match(buttonName, @"\d+").Value), go.GetComponent<Button>());
	//        }

	        ProcessDefaultPlayers(xmlPlayerTeams, xmlPlayers);
	    }
	 
	     //Convert an XmlNodeList into objects 
	    private void ProcessPowerupNodes(XDocument xmlPowerupDoc)
	     {
			//MHS this will allow the powerups to be added to a usable game object
			//TODO make sure this list is viable across scripts and scenes 
			powerUpList = new ArrayList();
	        
			Powerup powerup;

	        foreach (XElement node in xmlPowerupDoc.Descendants("powerup"))
	        {
				// If XML has an invalid value. (Should start with a '0')
				if(node.Element("name").Value[0] == '0')
				{
					continue;
				}

	            powerup = new Powerup();
				powerup.Name = node.Element("name").Value.Trim();
		        powerup.Description = node.Element("description").Value;
		        powerup.Cost = (float)Convert.ToDouble(node.Element("cost").Value);
		        //powerup.pack = node.Element("pack").Value;
		        powerup.PlayerMinLevel = Convert.ToInt16(node.Element("playerMinLevel").Value);
		        powerup.TimeScope = node.Element("timeScope").Value;
	            //  powerup.coolDown = Convert.ToInt16(node.Element("cooldown").Value); 
	            // Cooldown not in XML check the excel to xml conversion
		        powerup.PlayerScope = node.Element("playerScope").Value;
				powerup.SetLevelOfCard(node.Element ("cardLevel").Value);
				powerup.AttributesAffectedString = node.Element("attribute").Value;
				powerup.BuffBasesString = node.Element("buffBase").Value;
				powerup.BuffIncrementsString = node.Element ("buffIncrement").Value;
				//Edit - Daniyal
				//powerup.attributeStringList = powerup.attribute.Split(new char[] {','});		//??? or just ','
				//SetAttributeList(powerup);

				powerUpList.Add(powerup);
	        }
				
//			foreach (Powerup card in powerUpList) {
//				Debug.Log (card.Name + " " + card.cardLevel + " " + card.Description + " " + card.buffBasesString);
//			}
	    }



	    private void ProcessDefaultPlayers(XDocument xmlTeamDoc, XDocument xmlDefaultPlayerDoc)
	    {
//	        teams = new ArrayList();
//	        DefaultPlayer d_player;
			BBaller d_player;
			Team d_team;
			playerDictionary =  new Dictionary<int, BBaller>();
			int[] playerID;


	        foreach (XElement node in xmlDefaultPlayerDoc.Descendants("default_player"))
	        {
				d_player = new BBaller();
	            d_player.uid = Convert.ToInt16(node.Element("UID").Value);
	            d_player.firstName = node.Element("First").Value;
	            d_player.nickName  = node.Element("Nickname").Value;
	            d_player.lastName  = node.Element("Last").Value;
				//Debug.Log (d_player.firstName + d_player.lastName);
	            d_player.speedPercentage   = Convert.ToInt16(node.Element("speed").Value);
	            d_player.agilityPercentage = Convert.ToInt16(node.Element("agility").Value);
	            d_player.powerPercentage   = Convert.ToInt16(node.Element("power").Value);
	            playerDictionary.Add(d_player.uid,d_player);
	        }

	        foreach (XElement node in xmlTeamDoc.Descendants("team"))
	        {
	            d_team = new Team();
				playerID = new int[3];
	            d_team.teamName = node.Element("name").Value;

				playerID [0] = Convert.ToInt32 (node.Element ("player1").Value);   
				playerID [1] = Convert.ToInt32 (node.Element ("player2").Value);
				playerID [2] = Convert.ToInt32 (node.Element ("player3").Value);

//				Debug.Log (d_team.teamName + playerID [0] + playerID [1] + playerID [2]); 

				d_team.bballers[0] = playerDictionary[playerID[0]];
				d_team.bballers[1] = playerDictionary[playerID[1]];
				d_team.bballers[2] = playerDictionary[playerID[2]];
//				d_team.players[0] = playerDictionary[playerID[0]];
//				d_team.players[1] = playerDictionary[playerID[1]];
//				d_team.players[2] = playerDictionary[playerID[2]];

//				Debug.Log (d_team.bballers [0].uid + d_team.bballers [0].firstName);
//				Debug.Log (d_team.players [0].uid + d_team.players [0].firstName);
//	            teams.Add(d_team);
				LoadTeam(d_team);
	            Debug.Log("Loading Team");
	        }
	    }
	    //Finds object in application, creates prefab and send the powerup as parameter. 
	    //Can be changed later to access each card prefab instead
	    private void LoadPowerup(Powerup powerup)
	    {
	        GameObject powerupGameObject = GameObject.Find("Powerup");
	        GameObject powerupObject = Instantiate(powerupGameObject) as GameObject;
	        GameObject parentCardObject = GameObject.Find("Cards");
	        powerupObject.name = powerup.Name.ToString();
	        powerupObject.transform.SetParent(parentCardObject.transform,false);
	        powerupObject.SendMessage("LoadPowerup", powerup);
	    }

	    // Loading defualt team with 3 players each team, decided by the formula
	    private void LoadTeam(Team defaultTeam)
	    {
//			Debug.Log ("defaultTeam: " + defaultTeam.teamName.ToString());
//	        GameObject teamGameObject = GameObject.Find("ContentTeam");
//	        GameObject teamObject = Instantiate(teamGameObject) as GameObject;
			teamObject = GameObject.Find("Team_" + teamNum);
//	        GameObject parentTeam = GameObject.Find("Team_1");
	        teamObject.name = defaultTeam.teamName.ToString();
			teamNum++;
//	        teamObject.transform.SetParent(parentTeam.transform, false);
	        GameObject playerFront = GameObject.Find("BasketballerFront");
			GameObject playerLeft = GameObject.Find("BasketballerLeft");
			GameObject playerRight = GameObject.Find("BasketballerRight");
	        GameObject playerObj01 = Instantiate(playerFront) as GameObject;
	        GameObject playerObj02 = Instantiate(playerLeft) as GameObject;
	        GameObject playerObj03 = Instantiate(playerRight) as GameObject;
			playerObj01.name = defaultTeam.bballers[0].firstName.ToString() + " " + defaultTeam.bballers[0].lastName.ToString();
	        playerObj01.transform.SetParent(teamObject.transform, false);
			playerObj02.name = defaultTeam.bballers[1].firstName.ToString() + " " + defaultTeam.bballers[1].lastName.ToString();
	        playerObj02.transform.SetParent(teamObject.transform, false);
			playerObj03.name = defaultTeam.bballers[2].firstName.ToString() + " " + defaultTeam.bballers[2].lastName.ToString();
	        playerObj03.transform.SetParent(teamObject.transform, false);
//			playerObj01.name = defaultTeam.players[0].firstName.ToString() + " " + defaultTeam.players[0].lastName.ToString();
//			playerObj01.transform.SetParent(teamObject.transform, false);
//			playerObj02.name = defaultTeam.players[1].firstName.ToString() + " " + defaultTeam.players[1].lastName.ToString();
//			playerObj02.transform.SetParent(teamObject.transform, false);
//			playerObj03.name = defaultTeam.players[2].firstName.ToString() + " " + defaultTeam.players[2].lastName.ToString();
//			playerObj03.transform.SetParent(teamObject.transform, false);
//			Debug.Log ("playerName: " + playerObj01.name + " " + playerObj02.name + " " + playerObj03.name);

			teamObj = teamObject.GetComponent<TeamLoader>();

			teamObj.player1_t_uid = defaultTeam.bballers [0].uid;
			teamObj.player2_t_uid = defaultTeam.bballers [1].uid;
			teamObj.player3_t_uid = defaultTeam.bballers [2].uid;
			teamObj.player1_t_firstName = defaultTeam.bballers [0].firstName;
			teamObj.player2_t_firstName = defaultTeam.bballers [1].firstName;
			teamObj.player3_t_firstName = defaultTeam.bballers [2].firstName;
			teamObj.player1_t_nickName = defaultTeam.bballers [0].nickName;
			teamObj.player2_t_nickName = defaultTeam.bballers [1].nickName;
			teamObj.player3_t_nickName = defaultTeam.bballers [2].nickName;
			teamObj.player1_t_lastName = defaultTeam.bballers [0].lastName;
			teamObj.player2_t_lastName = defaultTeam.bballers [1].lastName;
			teamObj.player3_t_lastName = defaultTeam.bballers [2].lastName;
			teamObj.player1_t_speedPercentage = defaultTeam.bballers [0].speedPercentage;
			teamObj.player2_t_speedPercentage = defaultTeam.bballers [1].speedPercentage;
			teamObj.player3_t_speedPercentage = defaultTeam.bballers [2].speedPercentage;
			teamObj.player1_t_agilityPercentage = defaultTeam.bballers [0].agilityPercentage;
			teamObj.player2_t_agilityPercentage = defaultTeam.bballers [1].agilityPercentage;
			teamObj.player3_t_agilityPercentage = defaultTeam.bballers [2].agilityPercentage;
			teamObj.player1_t_powerPercentage = defaultTeam.bballers [0].powerPercentage;
			teamObj.player2_t_powerPercentage = defaultTeam.bballers [1].powerPercentage;
			teamObj.player3_t_powerPercentage = defaultTeam.bballers [2].powerPercentage;


	        teamObject.SendMessage("LoadTeam", defaultTeam);
	    }
	}
    
}