using UnityEngine;
using System;
using System.Collections;

[System.Serializable] public class CourtItem
{
	public String courtName;
	public String courtDesc;
	public String courtScene;
	public Texture2D courtPic;

	public bool unlocked;
	public bool isHomeTeam;

	public int requiredLevel;
	public int teamJerseyColor;
	public int teamShortsColor;
	public int teamShoesColor;
	public int teamLogo;

	public int [] teamPlayers;
	public int [] teamStarters;
}
