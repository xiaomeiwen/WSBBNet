using System;
using UnityEngine;


namespace WSBB
{
	public class Powerup
	{
		#region ClassVariables
		public enum CardLevel { BRONZE, SILVER, GOLD }
		public string Name;	/// Name of Card.
		public string Description;	/// Description about what this card does.
		private string timeScope = "0";
		//private float coolDown = 0;
		public float Cost = 0;
		public int PlayerMinLevel = 0;	/// Minimum Level of Player before this card can be used.
		public bool IsActivated = false;
		public bool IsOnCooldown = false;
		public string PlayerScope = "I";
		public GameObject ballerGameObject = null;

		/// The level of card. Values could be BRONZE, SILVER, GOLD. Default = BRONZE.
		public CardLevel cardLevel = CardLevel.BRONZE; 
		public WSBB.PlayerStats[] attributesAffected;
		public string attributesAffectedString="";
		public float[] buffBases;
		public string buffBasesString="";
		public float[] buffIncrements;
		public string buffIncrementsString="";

		private bool hasPrimary = false;

		private float activePeriod = 0f;
		private float cooldownPeriod = 0f;
		public float CurrentActiveTime = 0f;
		public float CurrentCooldownTime = 0f;
		#endregion

		#region Properties

		public string TimeScope
		{
			get{ return timeScope; }
			set{ 
				timeScope = value;
				if(!float.TryParse(timeScope, out activePeriod))
					activePeriod = 0f;
				CurrentActiveTime = activePeriod;
			}
		}
		public float ActivePeriod
		{
			get{ return activePeriod; }
		}
		public float CoolDown
		{
			get{ return cooldownPeriod; }
			set{
				cooldownPeriod = CurrentCooldownTime = value;
			}
		}


		public bool HasPrimary
		{
			get{ return hasPrimary; }
		}

		/// <summary>
		/// Gets the List of Attributes Affected.
		/// </summary>
		/// <value>An array of attributes affected.</value>
		public CardLevel GetLevelOfCard
		{
			get{ return cardLevel; }
		}
		/// <summary>
		/// Gets the List of Attributes Affected.
		/// </summary>
		/// <value>An array of attributes affected.</value>
		public WSBB.PlayerStats[] AttributesAffected
		{
			get{ return attributesAffected; }
		}
		public string AttributesAffectedString
		{
			get{ return attributesAffectedString; } 
			set{
				attributesAffectedString = value;
				setAttributesAffected(value);
				setHasPrimary();
			}
		}
		/// <summary>
		/// Gets the array of Buff Bases.
		/// </summary>
		/// <value>The buff bases.</value>
		public float[] BuffBases
		{
			get{ return buffBases; }
		}
		public string BuffBasesString
		{
			get{ return buffBasesString; } 
			set{
				buffBasesString = value;
				setBuffBases(value);
			}
		}
		/// <summary>
		/// Gets the Per-Level Buff increments.
		/// </summary>
		/// <value>The buff increments.</value>
		public float[] BuffIncrements
		{
			get{ return buffIncrements; }
		}
		public string BuffIncrementsString
		{
			get{ return buffIncrementsString; } 
			set{
				buffIncrementsString = value;
				setBuffIncrements(value);
			}
		}
		#endregion

		#region Constructors
		public Powerup ()
		{
			
		}
		#endregion

		#region SetterFunctions
		/// <summary>
		/// Sets the cardLevel by accepting a string input.
		/// </summary>
		/// <returns><c>true</c>, if card level was set, <c>false</c> otherwise. Defaults to CardLevel=BRONZE.</returns>
		/// <param name="level">The string for the Level. Like "Gold", "silver", "BROnze", etc.</param>
		public bool SetLevelOfCard(string level)
		{
			bool isSuccessful = false;
			level = level.ToUpper().Trim();
			int enumLength = Enum.GetNames (typeof(CardLevel)).Length;
			for (int i=0; i<enumLength; ++i) 
			{
				bool isEqual = (level == ((CardLevel)i).ToString ());
//				Debug.Log(level + " " + ((CardLevel)i).ToString() + " " + isEqual);
				if(level == ((CardLevel)i).ToString())
				{
					cardLevel = (CardLevel)i;
					isSuccessful = true;
				}
			}
			
			return isSuccessful;
		}

		private void setHasPrimary()
		{
			hasPrimary = false;

			for(int i=0; i<attributesAffected.Length; ++i)
			{
				//First three stats are supposed to be Primary PlayerStats
				if ( ((int)attributesAffected[i]) < 3)
				{
					hasPrimary = true;
					break;
				}
			}
		}

		/// <summary>
		/// Sets the attributesAffected by accepting a string input.
		/// </summary>
		private void setAttributesAffected(string attr)
		{
			attributesAffectedString = attr;

			string[] attrList;
			attrList = attr.Split (new char[] {','});
			attributesAffected = convertToStatArray (attrList);
		}
		/// <summary>
		/// Sets the buffBases by accepting a string input.
		/// </summary>
		private void setBuffBases(string attr)
		{
			string[] attrList;
			attrList = attr.Split (new char[] {','});
			buffBases = convertToFloatArray (attrList);
			
		}
		/// <summary>
		/// Sets the buffIncrements level by accepting a string input.
		/// </summary>
		private void setBuffIncrements(string attr)
		{
			string[] attrList;
			attrList = attr.Split (new char[] {','});
			buffIncrements = convertToFloatArray (attrList);
		}	
		#endregion

		#region HelperFunctions
		private float[] convertToFloatArray( string[] source ){

			float[] destination = new float[source.Length];

			for (int i = 0; i < source.Length; ++i) {
				destination[i] = (float) Convert.ToDouble (source[i]);
			}

			return destination;
		}

		private WSBB.PlayerStats[] convertToStatArray( string[] source )
		{
			int enumLength = System.Enum.GetNames(typeof(PlayerStats)).Length;

			PlayerStats[] destination = new PlayerStats[source.Length];

			for (int i = 0; i < source.Length; i++) {
				source[i] = source[i].ToUpper();
				for  (int enumIndex = 0; enumIndex < enumLength; ++enumIndex){
					if ( ((PlayerStats)enumIndex).ToString() == source[i] ){
						destination[i] = (PlayerStats)enumIndex;
					}
				}
			}

			return destination;
		}
		#endregion
	}

}