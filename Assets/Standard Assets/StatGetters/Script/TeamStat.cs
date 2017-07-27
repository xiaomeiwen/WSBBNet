using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


namespace WSBB {

	/// <summary>
	/// Team stat.
	/// </summary>
	public class TeamStat : MonoBehaviour {

		// VariableDeclarations

		public Text debugTextBox;
		private bool needCalculation = false;
		private bool hasPrimary = false;

		//!!! Probably don't need these two variables.
		//Assuming there is a "Team" GameObject; this script can be attached to that GameObject.
		private Team myTeam;
		//private BBaller[] players = new DefaultPlayer[3];

		//TODO: [UTHRA] I added an "IsActivated" boolean to denote if this card is active or not. Run it by Uthra.
		// List of Cards associated with this Team
		public List<Powerup> powerUpCards = new List<Powerup>();
		public List<Powerup> dunkCards = new List<Powerup>();

		private int numOfStats = System.Enum.GetNames(typeof(PlayerStats)).Length;

		//  Data Structure to handle the two kinds of Secondary Bonuses provided by the PowerUp Cards
		public struct SecondaryBonus
		{
			public float DiscreteBoost;
			public float PercentageBoost;
			public float PerLevelBoost;
		}

		//SecondaryBonus[] secBonus;
		private Dictionary<PlayerStats, SecondaryBonus> secondaryBonusList = new Dictionary<PlayerStats, SecondaryBonus> ();
		public Dictionary<PlayerStats, SecondaryBonus> SecondaryBonusList
		{
			get{ return secondaryBonusList; }
		}

		//DefaultCard
		#region DefaultCardSetups
		private Powerup defaultDunkCard = new Powerup();

		private void initDefaultCards()
		{
			defaultDunkCard.Name = "Default Dunk";
			defaultDunkCard.Description = "DefaultDunkCard: +5 to Speed, Agility, Power";
			defaultDunkCard.TimeScope = "5";	//TODO: Change back to 60 sec
			defaultDunkCard.CoolDown = 60f;		// [=0] Deactivate after timeScope
			defaultDunkCard.Cost = 0f;
			defaultDunkCard.PlayerMinLevel = 0;
			defaultDunkCard.IsActivated = false;
			defaultDunkCard.IsOnCooldown = false;
			defaultDunkCard.PlayerScope = "T";
			defaultDunkCard.SetLevelOfCard("BroNze");
			defaultDunkCard.AttributesAffectedString = "Speed,Power,Agility";
			defaultDunkCard.BuffBasesString = "5,5,5";
			defaultDunkCard.BuffIncrementsString = "0";
		}
		#endregion

		//Codes, Variables, methods, etc to help Debug
		#region DebugCode
		//DebugCards
		private Powerup debugPowerCard_1 = new Powerup();
		private Powerup debugPowerCard_2 = new Powerup();
		private Powerup debugDunkCard_3 = new Powerup();

		private void fillUpDebugCards()
		{
			debugPowerCard_1.Name = "DebugCard_1";
			debugPowerCard_1.Description = "First DebugCard: Powerup";
			debugPowerCard_1.TimeScope = "Q";
			debugPowerCard_1.CoolDown = 0f;
			debugPowerCard_1.Cost = 0f;
			debugPowerCard_1.PlayerMinLevel = 0;
			debugPowerCard_1.IsActivated = false;
			debugPowerCard_1.IsOnCooldown = false;
			debugPowerCard_1.PlayerScope = "I";
			debugPowerCard_1.SetLevelOfCard("BroNze");
			debugPowerCard_1.AttributesAffectedString = "Speed";
			debugPowerCard_1.BuffBasesString = "10";
			debugPowerCard_1.BuffIncrementsString = "0.5";

			debugPowerCard_2.Name = "DebugCard_2";
			debugPowerCard_2.Description = "Second DebugCard: Powerup";
			debugPowerCard_2.TimeScope = "Q";
			debugPowerCard_2.CoolDown = 0f;
			debugPowerCard_2.Cost = 0f;
			debugPowerCard_2.PlayerMinLevel = 0;
			debugPowerCard_2.IsActivated = false;
			debugPowerCard_2.IsOnCooldown = false;
			debugPowerCard_2.PlayerScope = "I";
			debugPowerCard_2.SetLevelOfCard("silver");
			debugPowerCard_2.AttributesAffectedString = "Block";
			debugPowerCard_2.BuffBasesString = "10";
			debugPowerCard_2.BuffIncrementsString = "0.5";

			debugDunkCard_3.Name = "DebugCard_3";
			debugDunkCard_3.Description = "Third DebugCard: DUNK";
			debugDunkCard_3.TimeScope = "10";
			debugDunkCard_3.CoolDown = 5f;
			debugDunkCard_3.Cost = 0f;
			debugDunkCard_3.PlayerMinLevel = 0;
			debugDunkCard_3.IsActivated = false;
			debugDunkCard_3.IsOnCooldown = false;
			debugDunkCard_3.PlayerScope = "T";
			debugDunkCard_3.SetLevelOfCard("BroNze");
			debugDunkCard_3.AttributesAffectedString = "Speed,Power,Agility";
			debugDunkCard_3.BuffBasesString = "5,5,5";
			debugDunkCard_3.BuffIncrementsString = "0";

		}

		private void printToDebugText()
		{
			string text = "SecondaryBonuses:\n";
			for (int i=0; i<numOfStats; ++i)
			{
				text += ((PlayerStats)i).ToString() + ": " 
					+ secondaryBonusList[(PlayerStats)i].DiscreteBoost.ToString() + "\n\n" ;
			}
			
			debugTextBox.text = text;
		}

		#endregion

		// Methods 
		void Awake()
		{
			//secBonus = new SecondaryBonus[numOfStats];
			numOfStats = System.Enum.GetNames(typeof(PlayerStats)).Length;

			//Subscribe to Events
			subscribeEvents ();

			//initializing the Dictionaries with all the possible PlayerStats.
			setupDictionaries();

			initDefaultCards();
			dunkCards.Add(defaultDunkCard);

			//StopCoroutines
			StopCoroutine("initializeCards");
			StopCoroutine("setupTeamAccess");

			//StartCoroutines
			StartCoroutine("initializeCards");
			StartCoroutine("setupTeamAccess");

			//CalculateSecondaryBonuses ();
		}

		void OnDestroy()
		{
			unsubscribeEvents ();
		}

		#region EventSubscribers
		private void subscribeEvents()
		{
			BallerEvents.onDemandCardStatRecalculation += this.CalculateSecondaryBonuses;
			BallerEvents.onDunkCompletionWithMojo += this.dunkWithMojoCompleted;
		}

		private void unsubscribeEvents()
		{
			BallerEvents.onDemandCardStatRecalculation -= this.CalculateSecondaryBonuses;
			BallerEvents.onDunkCompletionWithMojo -= this.dunkWithMojoCompleted;
		}
		#endregion

		#region Initializers
		private void setupDictionaries()
		{
			//InitializeSecondaryBonusList
			for (int i=0; i<numOfStats; ++i) 
			{
				secondaryBonusList.Add((PlayerStats)i, new SecondaryBonus());		//!!! Verify this works
			}
		}

		private IEnumerator initializeCards()
		{
			if(GameData.instance == null)
				yield return null;

			//TODO: Get all the Cards associated with this team.
			if(GameData.instance.DebugModeActive)
			{
				fillUpDebugCards();
				
				powerUpCards.Add(debugPowerCard_1);
				powerUpCards.Add(debugPowerCard_2);
				dunkCards.Add(debugDunkCard_3);
			}
			
			//powerUpCard.Add( |OtherCards| );		
			//TODO: Needs a reference to collection of the cards. ??? Is this done?

			//InitActivation ();

			yield break;
		}

		private IEnumerator setupTeamAccess()
		{
			while (myTeam == null)
			{
				myTeam = GetComponent<Team>();
				yield return null;
			}
		}

		
		//TODO: Activate Quarter length Cards. Verify. - Probably needs deleting
		[System.Obsolete]private void InitActivation ()
		{
			for (int i=0; i<powerUpCards.Count; ++i) {
				if (powerUpCards[i].TimeScope == "Q")		//Quarter Long cards
				{
					ActivateCard(powerUpCards[i]);
					hasPrimary = (powerUpCards[i].HasPrimary) ? true : hasPrimary;
				}

			}
			
			//CalculateSecondaryBonuses();
			BallerEvents.DemandCardStatRecalculation (myTeam, hasPrimary);
			
		}
		
		#endregion

		//Checking for MojoLevels
		// Is there a better way to do this?
		void Update()
		{
			//TODO: Get rid of this for actual gameplay
			//if(GameData.instance.DebugModeActive)
				//dunkWithMojoCompleted(myTeam);

//			bool needCalculation = false;
			if(GameData.instance.DebugModeActive && debugTextBox!=null)
			{
				printToDebugText();
			}
		}

		//Should be called only when a verified Post-MojoThresholdReached Dunk takes place
		private void dunkWithMojoCompleted(Team eventTeam)
		{
			if (eventTeam != myTeam)
				return;


			foreach(Powerup card in dunkCards)
			{
				if (!card.IsActivated)
				{
					ActivateCard(card);
				}
			}


		}

		private bool hasMojoReached(ref int mojo)
		{
			bool mojoReached = false;
			for (int i=0; i < 3; ++i)
			{
				if(myTeam.bballers[i].mojo >= 100)
				{
					mojo = (int)myTeam.bballers[i].mojo;
					myTeam.bballers[i].mojo = 0;
					mojoReached = true;
					break;
				}
			}

			return mojoReached;
		}


		//Verify which card is Active and Handle() it
		private void CalculateSecondaryBonuses(Team team, bool hasPrimary)
		{
			if (team != myTeam)
				return;

			//Debug.Log ("TeamStat.cs: Resetting SecBonusList.");
			ResetSecBonusList ();

			//Debug.Log ("TeamStat.cs: Recalculating SecBonusList.");
			for (int i=0; i<dunkCards.Count; ++i) {
				if (dunkCards[i].IsActivated && !dunkCards[i].IsOnCooldown)
				{
					HandleCard(i);
				}
			}

			if (hasPrimary)
				BallerEvents.PrimaryStatChanged(team);
			else
				BallerEvents.DerivedStatChanged(team);		//This should NOT be called now.

		}

		//Reset all BonusStat Elements;
		private void ResetSecBonusList ()
		{
			SecondaryBonus secBonus = new SecondaryBonus();

			secBonus.DiscreteBoost = secBonus.PerLevelBoost = secBonus.PercentageBoost = 0f;

			for (int i=0; i<numOfStats; ++i) {

				secondaryBonusList[(PlayerStats)i] = secBonus;
			}

		}

		//Addition of Card stats to the SecondaryBonusList
		private void HandleCard(int cardIndex)
		{
			//TODO: [Uthra] Implement a Card Identifier based on the enum PlayerStats; Currently this deals with just one StatModifer per card

			if (dunkCards [cardIndex].AttributesAffected.Length == 0) 
			{
				Debug.LogError ("No Attributes in this List. Needs attributes mentioned in the XML.");
			}

			for (int i=0; i<dunkCards[cardIndex].AttributesAffected.Length; ++i) 
			{
				int statType = (int)dunkCards [cardIndex].AttributesAffected[i];

				if (statType >= numOfStats || statType < 0)		//if erroneous 
				{
					Debug.LogError("StatType is Unidentified. Needs correction.");
					return;
				}

				SecondaryBonus secBonus = secondaryBonusList [(PlayerStats)statType];

				if(dunkCards[cardIndex].BuffBases.Length-1 >= i)
					secBonus.DiscreteBoost += dunkCards [cardIndex].BuffBases[i];
		
				if(dunkCards[cardIndex].BuffIncrements.Length-1 >= i)
					secBonus.PerLevelBoost += dunkCards [cardIndex].BuffIncrements[i];

				secondaryBonusList [(PlayerStats)statType] = secBonus;	
			}

			return;
		}

		//TODO: How will the card information be relayed? (Which card has been activated?) Manage it as the Parameters for following functions.
		//!!!: Make sure you call CalculateSecondaryBonuses() at the end of this call in the caller function.
		public void ActivateCard(Powerup Card)
		{
			//TODO: change secBonus[Type] according to incoming Card
			Card.IsActivated = true;

			//TODO: How exactly will the cooldown feature be implemented
			float timeout = 0f;
			bool isNum = float.TryParse (Card.TimeScope, out timeout);

			if (isNum)		//Handling if there is a timeout or if it lasts for the entire Quarter
			{
				StartCoroutine ("StartDunkCard", Card);		//XXX
			}

		}

		private void ReactivateCard(Powerup Card)
		{
			Card.IsOnCooldown = false;

			//CalculateSecondaryBonuses ();
//			needCalculation = true;
			BallerEvents.DemandCardStatRecalculation(myTeam, Card.HasPrimary);


		}

		private void CardCoolDown(Powerup Card)
		{
			Card.IsOnCooldown = true;

//			CalculateSecondaryBonuses ();
			//needCalculation = true;
			BallerEvents.DemandCardStatRecalculation(myTeam, Card.HasPrimary);
		}

		IEnumerator StartDunkCard(Powerup Card)
		{
			float activeTime = 0.0f;

			if (!float.TryParse (Card.TimeScope, out activeTime))
				yield break;

			Card.CurrentActiveTime  	= Card.ActivePeriod;
			Card.CurrentCooldownTime 	= Card.CoolDown;

			while (Card.IsActivated) {
				//Reactivation
				ReactivateCard(Card);

				while (Card.CurrentActiveTime > 0f)
				{
					Card.CurrentActiveTime -= Time.deltaTime;
					yield return null;
				}
				Card.CurrentActiveTime = Card.ActivePeriod;

				//CoolDown
				if(Card.CoolDown > 0f)
				{
					CardCoolDown(Card);

					while (Card.CurrentCooldownTime > 0f)
					{
						Card.CurrentCooldownTime -= Time.deltaTime;
						yield return null;
					}
					Card.CurrentCooldownTime = Card.CoolDown;

				}
				else 	//Card Deactivation
				{
					DeactivateCard(Card);
				}
			}
		}

		//!!! Probably not to be used anymore.
		public void DeactivateCard(Powerup Card)
		{
			//TODO: change secBonus[Type] according to incoming Card
			{
				Card.IsActivated = false;
//				CalculateSecondaryBonuses();
				BallerEvents.DemandCardStatRecalculation(myTeam, Card.HasPrimary);
			}
		}


		public Powerup GetPowerupCard(GameObject ballerGO)
		{
			Powerup returnCard = null;

			if (GameData.instance.DebugModeActive)
			{
				int rand = 0;
				if ( !int.TryParse(ballerGO.name[ballerGO.name.Length-1].ToString(), out rand) )
					return null;

				switch (rand)
				{
				case 0:	
					returnCard = debugPowerCard_1;
					break;

				case 1:
					returnCard = debugPowerCard_2;
					break;

				default:
					returnCard = debugPowerCard_2;
					break;
				}

			}
			else
			{
				//TODO: Verify on implementing ballerGO
				foreach (Powerup card in powerUpCards)
				{
					if(card.ballerGameObject == ballerGO)
					{
						returnCard = card;
						break;
					}
				}

			}

			return returnCard;
		}


	}
}