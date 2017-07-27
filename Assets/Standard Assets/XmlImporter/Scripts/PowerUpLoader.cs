using UnityEngine;
using System;
using System.Collections;
using WSBB;

namespace XMLSpace
{
	public class PowerUpLoader : MonoBehaviour {

	    public Powerup myPowerup;

	    public String p_name;
	    public String p_description;
	    public String p_attribute;
	    public string p_buffBase;
	    public string p_buffIncrement;
	    public float p_cost = 0;
	    //public String p_pack;
	    public String p_cardLevel;
	    public int p_playerMinLevel = 0;
	    public String p_timeScope = "0";
	    public float p_coolDown = 0;
	    public String p_playerScope;
		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
		
		}

	    void LoadPowerup(Powerup powerup){
	        myPowerup = powerup;

	        // just to see values in editor, can be deleted later
	        p_name = powerup.Name;
	        p_description = powerup.Description;
	        p_attribute = powerup.AttributesAffectedString;
			p_buffBase = powerup.BuffBasesString;
	        p_buffIncrement = powerup.BuffIncrementsString;
	        p_cost = powerup.Cost;
	        //p_pack = powerup.pack;
	        p_cardLevel = powerup.GetLevelOfCard.ToString();
	        p_playerMinLevel = powerup.PlayerMinLevel;
	        p_timeScope = powerup.TimeScope;
	        p_coolDown = powerup.CoolDown;
	        p_playerScope = powerup.PlayerScope;
	    }
	}
}