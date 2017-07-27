using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/*
 * The proposed functionality is: PlayerStat = BaseStats(PlayerStat.cs) + PowerupCardStat(PlayerStat.cs) + TeamStat(From TeamStat.cs)
 */

namespace WSBB {
	// PlayerStats: {PrimaryStats, DerivedStats, OtherStats} [First PlayerStats should be the Primary ones, etc]
	public enum PlayerStats{SPEED, AGILITY, POWER, BLOCK, SHOOT, STEAL, PASS, DRIVE, HANDLING, GUARD, MOJO, FOULODDS, FATIGUE};
	
	/// <summary>
	/// Associated with every Baller GameObject for their independent PlayerStats.
	/// </summary>
	public class PlayerStat : MonoBehaviour {

		public Text debugTextBox;

		private int derivedStatStartIndex = 3;

		private int numOfStats = 0;

		private BBaller bballer = null;
		private TeamStat teamStat;
		private BBallerData ballerData = null;
		private Powerup myPowerupCard = null;
		private Team myTeam = null;

        public string statFileName;


		//Stat Dictionaries
		//Dictionary<PlayerStats, float> FinalStatList 		= new Dictionary<PlayerStats, float>();
		Dictionary<PlayerStats, float> BaseStatList  		= new Dictionary<PlayerStats, float>();
		//Dictionary<PlayerStats, float> DunkStatList		= new Dictionary<PlayerStats, float>();
		public Dictionary<PlayerStats, float> LatestStatList
		{
			get{ return BaseStatList; }
		}
		Dictionary<PlayerStats, TeamStat.SecondaryBonus> PowerupStatList	= new Dictionary<PlayerStats, TeamStat.SecondaryBonus>();



		void Awake()
		{
            //TODO: get access to the parent TeamStat; verify the following line of code 
            bballer = GetComponent<BBaller>();
            numOfStats = System.Enum.GetNames(typeof(PlayerStats)).Length;

			subscribeEvents();

			//ballerData = GetComponent<BBaller_skin>().ballerData; //ZACH: i don't like this.

			initializeDictionaries ();

			//StartCoroutine("initializeTeam");
		}

		void OnDestroy()
		{
			unsubscribeEvents();
		}

		#region EventSubscribers
		private void subscribeEvents()
		{
			BallerEvents.onPrimaryStatChange 		+= this.primaryBaseStatUpdate;
			BallerEvents.onDerivedStatChange 		+= this.calculateDerivedStats;
			BallerEvents.onDemandDebugTextUpdate 	+= this.printToDebugText;
		}

		private void unsubscribeEvents()
		{
			BallerEvents.onPrimaryStatChange 		-= this.primaryBaseStatUpdate;
			BallerEvents.onDerivedStatChange 		-= this.calculateDerivedStats;
			BallerEvents.onDemandDebugTextUpdate 	-= this.printToDebugText;
		}
		#endregion


		private IEnumerator initializeTeam()
		{

			while (bballer.MyTeam == null)
				yield return null;

			myTeam = bballer.MyTeam;
			teamStat = bballer.MyTeam.GetComponent<TeamStat>();
			//setupInitialStat (); //ZACH: bballer will set up initial stat, so stats are only coming from one place

			//Check if GameData has been instantiated and then set Powerup Card here;
			while (GameData.instance == null)
			{
				yield return null;
			}

//			myPowerupCard = teamStat.GetPowerupCard (this.gameObject);
			SetPowerupCard( teamStat.GetPowerupCard (this.gameObject) );

			yield break;
		}

		private void initializeDictionaries ()
		{
			TeamStat.SecondaryBonus secBonus = new TeamStat.SecondaryBonus();

			for (int i=0; i<numOfStats; ++i) 
			{
				//FinalStatList.Add((PlayerStats)i, 0f);
				BaseStatList.Add((PlayerStats)i, 0f); 
				//DunkStatList.Add ((PlayerStats)i, 0f);
				PowerupStatList.Add ((PlayerStats)i, secBonus); 
				//SecondaryBonusList.Add((PlayerStats)i, new TeamStat.SecondaryBonus());		//!!! Verify this works
			}
		}

		private void resetDictionary(ref Dictionary<PlayerStats, float> statList)
		{
			for (int i=0; i<numOfStats; ++i) 
			{ 
				statList.Add ((PlayerStats)i, 0f);
			}

		}

		public void setupInitialStat()
		{
			//TODO: [Uthra] Get Info from XML Statsheet OR Player Info and assign data to each of the three Primary stats.
//			for (int i=0; i<secondaryStatStartIndex; ++i) {		//i<3 because first three are primary stats.
//				BaseStatList[(PlayerStats)i] = 0; //[UthraGetPrimaryStat]		!!!
//			}
            //string fileName = System.IO.File.ReadAllText(Application.dataPath + "/" + statFileName + ".csv");

            primaryBaseStatUpdate(bballer.MyTeam);

		}

		//TODO: After verification of it's working, limit it's access-modifier to private
		public void SetPowerupCard(Powerup card)
		{
			if (card.TimeScope != "Q" )
			{
				Debug.LogError("PlayerStat.cs: PowerUp Card expected.");
				return;
			}

			myPowerupCard = card;

			// Ideally, AttributesAffected.Length should be 1.
			for (int i=0; i<myPowerupCard.AttributesAffected.Length; ++i)
			{
				TeamStat.SecondaryBonus secBonus = new TeamStat.SecondaryBonus();
				secBonus.DiscreteBoost = myPowerupCard.BuffBases[i];
				secBonus.PerLevelBoost = myPowerupCard.BuffIncrements[i];
				
				PowerupStatList[myPowerupCard.AttributesAffected[i]] = secBonus;
			}
			
			if (myPowerupCard.HasPrimary)
				//primaryBaseStatUpdate();
				BallerEvents.PrimaryStatChanged(bballer.MyTeam);
			else
				//calculateDerivedStats();
				BallerEvents.DerivedStatChanged(bballer.MyTeam);
		}


		private void primaryBaseStatUpdate(Team eventTeam)
		{
			if(eventTeam != bballer.MyTeam)
				return;

			if (bballer != null)
			{
                //////// THIS IS INSANE AND TERRIBLE //////////////////////////////////////////////////sorry. ZACH

                /*BaseStatList[PlayerStats.SPEED] = ballerData.baseSpeed + 
					PowerupStatList[PlayerStats.SPEED].DiscreteBoost +
						( PowerupStatList[PlayerStats.SPEED].PerLevelBoost * bballer.mojoLevel ) +
						teamStat.SecondaryBonusList[PlayerStats.SPEED].DiscreteBoost + 
						( teamStat.SecondaryBonusList[PlayerStats.SPEED].PerLevelBoost * bballer.mojoLevel );
				BaseStatList[PlayerStats.AGILITY] = ballerData.baseSkill + 
					PowerupStatList[PlayerStats.AGILITY].DiscreteBoost +
						( PowerupStatList[PlayerStats.AGILITY].PerLevelBoost * bballer.mojoLevel ) +
						teamStat.SecondaryBonusList[PlayerStats.AGILITY].DiscreteBoost + 
						( teamStat.SecondaryBonusList[PlayerStats.AGILITY].PerLevelBoost * bballer.mojoLevel );
                BaseStatList[PlayerStats.POWER] = ballerData.basePower + 
					PowerupStatList[PlayerStats.POWER].DiscreteBoost +
						( PowerupStatList[PlayerStats.POWER].PerLevelBoost * bballer.mojoLevel ) +
						teamStat.SecondaryBonusList[PlayerStats.POWER].DiscreteBoost + 
						( teamStat.SecondaryBonusList[PlayerStats.POWER].PerLevelBoost * bballer.mojoLevel );*/
                BaseStatList[PlayerStats.SPEED] = bballer.speedPercentage;
                BaseStatList[PlayerStats.AGILITY] = bballer.agilityPercentage;
                BaseStatList[PlayerStats.POWER] = bballer.powerPercentage;
                //Debug.Log(bballer.name + " is setting up stats.");
                calculateDerivedStats(eventTeam);
				
			}
			
			
		}


		public void calculateDerivedStats(Team eventTeam)
		{
			if (eventTeam != bballer.MyTeam)
				return;
			//Calculation of Derived PlayerStats
			for (int i = derivedStatStartIndex; i<numOfStats; ++i) 
			{

				//DM: Commenting out teamStat.SecondaryBonusList effects here as they only deal with Dunk Cards (affecting Primary Stats).

				switch (i)				// Check for Derived Stat type and call corresponding function
				{
				case (int)PlayerStats.BLOCK:		
					BaseStatList[PlayerStats.BLOCK] = getBlockBase() + 
						PowerupStatList[PlayerStats.BLOCK].DiscreteBoost +
							( PowerupStatList[PlayerStats.BLOCK].PerLevelBoost * bballer.mojoLevel ); 
//							teamStat.SecondaryBonusList[PlayerStats.BLOCK].DiscreteBoost + 
//							( teamStat.SecondaryBonusList[PlayerStats.BLOCK].PerLevelBoost * bballer.mojoLevel );
					break;
					
				case (int)PlayerStats.SHOOT:
					BaseStatList[PlayerStats.SHOOT] = getShootBase() + 
						PowerupStatList[PlayerStats.SHOOT].DiscreteBoost +
							( PowerupStatList[PlayerStats.SHOOT].PerLevelBoost * bballer.mojoLevel ); 
//						teamStat.SecondaryBonusList[PlayerStats.SHOOT].DiscreteBoost + 
//							( teamStat.SecondaryBonusList[PlayerStats.SHOOT].PerLevelBoost * bballer.mojoLevel );
					break;
					
				case (int)PlayerStats.STEAL:
					BaseStatList[PlayerStats.STEAL] = getStealBase() + 
						PowerupStatList[PlayerStats.STEAL].DiscreteBoost +
							( PowerupStatList[PlayerStats.STEAL].PerLevelBoost * bballer.mojoLevel ); 
//						teamStat.SecondaryBonusList[PlayerStats.STEAL].DiscreteBoost + 
//							( teamStat.SecondaryBonusList[PlayerStats.STEAL].PerLevelBoost * bballer.mojoLevel );
					break;
					
				case (int)PlayerStats.PASS:
					BaseStatList[PlayerStats.PASS] = getPassBase() + 
						PowerupStatList[PlayerStats.PASS].DiscreteBoost +
							( PowerupStatList[PlayerStats.PASS].PerLevelBoost * bballer.mojoLevel ); 
//						teamStat.SecondaryBonusList[PlayerStats.PASS].DiscreteBoost + 
//							( teamStat.SecondaryBonusList[PlayerStats.PASS].PerLevelBoost * bballer.mojoLevel );
					break;
					
				case (int)PlayerStats.DRIVE:
					BaseStatList[PlayerStats.DRIVE] = getDriveBase() + 
						PowerupStatList[PlayerStats.DRIVE].DiscreteBoost +
							( PowerupStatList[PlayerStats.DRIVE].PerLevelBoost * bballer.mojoLevel ); 
//						teamStat.SecondaryBonusList[PlayerStats.DRIVE].DiscreteBoost + 
//							( teamStat.SecondaryBonusList[PlayerStats.DRIVE].PerLevelBoost * bballer.mojoLevel );
					break;
					
				case (int)PlayerStats.HANDLING:
					BaseStatList[PlayerStats.HANDLING] = getHandlingBase() + 
						PowerupStatList[PlayerStats.HANDLING].DiscreteBoost +
							( PowerupStatList[PlayerStats.HANDLING].PerLevelBoost * bballer.mojoLevel ); 
//						teamStat.SecondaryBonusList[PlayerStats.HANDLING].DiscreteBoost + 
//							( teamStat.SecondaryBonusList[PlayerStats.HANDLING].PerLevelBoost * bballer.mojoLevel );
					break;
					
				case (int)PlayerStats.GUARD:
					BaseStatList[PlayerStats.GUARD] = getGuardBase() + 
						PowerupStatList[PlayerStats.GUARD].DiscreteBoost +
							( PowerupStatList[PlayerStats.GUARD].PerLevelBoost * bballer.mojoLevel ); 
//						teamStat.SecondaryBonusList[PlayerStats.GUARD].DiscreteBoost + 
//							( teamStat.SecondaryBonusList[PlayerStats.GUARD].PerLevelBoost * bballer.mojoLevel );
					break;
					
				case (int)PlayerStats.MOJO:
					BaseStatList[PlayerStats.MOJO] = getMojoBase() + 
						PowerupStatList[PlayerStats.MOJO].DiscreteBoost +
							( PowerupStatList[PlayerStats.MOJO].PerLevelBoost * bballer.mojoLevel ); 
//						teamStat.SecondaryBonusList[PlayerStats.MOJO].DiscreteBoost + 
//							( teamStat.SecondaryBonusList[PlayerStats.MOJO].PerLevelBoost * bballer.mojoLevel );
					break;
					
				case (int)PlayerStats.FOULODDS:
					BaseStatList[PlayerStats.FOULODDS] = getFoulOddsBase() + 
						PowerupStatList[PlayerStats.FOULODDS].DiscreteBoost +
							( PowerupStatList[PlayerStats.FOULODDS].PerLevelBoost * bballer.mojoLevel ); 
//						teamStat.SecondaryBonusList[PlayerStats.FOULODDS].DiscreteBoost + 
//							( teamStat.SecondaryBonusList[PlayerStats.FOULODDS].PerLevelBoost * bballer.mojoLevel );
					break;
					
				case (int)PlayerStats.FATIGUE:
					BaseStatList[PlayerStats.FATIGUE] = getFatigueBase() + 
						PowerupStatList[PlayerStats.FATIGUE].DiscreteBoost +
							( PowerupStatList[PlayerStats.FATIGUE].PerLevelBoost * bballer.mojoLevel ); 
//						teamStat.SecondaryBonusList[PlayerStats.FATIGUE].DiscreteBoost + 
//							( teamStat.SecondaryBonusList[PlayerStats.FATIGUE].PerLevelBoost * bballer.mojoLevel );
					break;
					
				default:
					BaseStatList[(PlayerStats)i] = 0;
					break;
					
				}
                //Debug.Log(bballer.name);
                //Debug.Log((PlayerStats)i + ": " + BaseStatList[(PlayerStats)i]);
			}

			if(GameData.instance != null)
				BallerEvents.DemandDebugTextUpdate(GameData.instance.DebugModeActive);
		}

		//Provide Access to the LatestStat
		public Dictionary<PlayerStats, float> GetLatestStat()
		{
//			resetDictionary(ref DunkStatList);
//
//			for (int i=0; i<numOfStats; ++i) 
//			{
//				// Pending Approval from St.John
//				if(i == derivedStatStartIndex)
//				{
//					calculateDerivedStats(ref DunkStatList);
//				}
//
//				FinalStatList[(PlayerStats)i] = BaseStatList[(PlayerStats)i] 
//										+ DunkStatList[(PlayerStats)i]
//										+ teamStat.SecondaryBonusList[(PlayerStats)i].DiscreteBoost 
//										/* + (teamStat.SecondaryBonusList[(PlayerStats)i].PerLevelBoost *PlayerLevel ) */;		//TODO: Retrieve PlayerLevel
//
//				if(i < derivedStatStartIndex)
//				{
//					DunkStatList[(PlayerStats)i] = FinalStatList[(PlayerStats)i];
//				}
//			}
			
			//return FinalStatList;
			return BaseStatList;
		}




		#region DerivedStatsCalculation
		//**************	Calculation of Derived PlayerStats
		public float getBlockBase()
		{
			float power = BaseStatList [PlayerStats.POWER];
			float agility = BaseStatList [PlayerStats.AGILITY];

			float sum = power + (agility / 2.0f);

			return sum/2.0f;
		}

		public float getShootBase()
		{
			float speed = BaseStatList [PlayerStats.SPEED];
			float agility = BaseStatList [PlayerStats.AGILITY];
			
			float sum = agility + (speed / 2.0f);
			
			return sum/2.0f;
		}

		public float getDriveBase()
		{
			float power = BaseStatList [PlayerStats.POWER];
			float speed = BaseStatList [PlayerStats.SPEED];
			
			float sum = power + (speed / 2.0f);
			
			return sum/2.0f;
		}

		public float getHandlingBase()
		{
			float power = BaseStatList [PlayerStats.POWER];
			float speed = BaseStatList [PlayerStats.SPEED];
			
			float sum = speed + (power / 2.0f);
			
			return sum/2.0f;
		}

		public float getStealBase()
		{
			float power = BaseStatList [PlayerStats.POWER];
			float speed = BaseStatList [PlayerStats.SPEED];
			
			float sum = speed + (power / 2.0f);
			
			return sum/2.0f;
		}

		public float getPassBase()
		{
			float speed = BaseStatList [PlayerStats.SPEED];
			float agility = BaseStatList [PlayerStats.AGILITY];
			
			float sum = agility + (speed / 2.0f);
			
			return sum/2.0f;
		}

		public float getGuardBase()
		{
			float power = BaseStatList [PlayerStats.POWER];
			float agility = BaseStatList [PlayerStats.AGILITY];
			
			float sum = power + (agility / 2.0f);
			
			return sum/2.0f;
		}

		public float getMojoBase()
		{
			return 1.0f;
		}

		public float getFoulOddsBase()
		{
			return 100.0f;
		}

		public float getFatigueBase()
		{
			return 0.0f;
		}

		#endregion


		#region DebuggerMethods
		void printToDebugText()
		{
			if (debugTextBox == null)
				return;

			string text = bballer.SkinAndData.ballerData.ballerName+" PlayerStats: \n";
			for (int i=0; i<numOfStats; ++i)
			{
				text += ((PlayerStats)i).ToString() + ": " 
					+ BaseStatList[(PlayerStats)i].ToString() + "\n\n" ;
			}
			
			debugTextBox.text = text;


		}
		#endregion

	}


}