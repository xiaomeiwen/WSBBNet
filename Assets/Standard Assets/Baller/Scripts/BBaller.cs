using UnityEngine;
using System;
using System.Collections;
using WSBB.StateMachineBase;

namespace WSBB
{
	public enum MoveType {NULL, WALK, JOG, RUN}
	public enum GuardDirection {NULL, FORWARD, BACK, RIGHT, LEFT}

	/* Format: TYPE [Success:Fail] */
	public enum MojoUpdates { 
		BLOCK, 		/* Block : - */ 
		SHOOT, 		/* Scores : Miss */ 
		FREETHROW, 	/* Scores : Miss */
		STEAL, 		/* Steal : - */
		DUNK,		/* Scores : Miss */
		REBOUND,	/* CatchTheBallBackOnRebound : - */
        CLOWN       /* Clown : - */
	}

    public class BBaller : MonoBehaviour
    {
        #region VariableDefinitions
        #region BallerVars
        public int uid;
        public string firstName;
        public string nickName;
        public string lastName;
        public int speedPercentage;
        public int agilityPercentage;
        public int powerPercentage;

        /// <summary>
        /// Boolean to denote if the Baller should start with an attempt to try to Dunk.
        /// </summary>
        public bool debugBooleanStartDunk;

        public int MyIndex = 0;

        //Not set in the Editor; Derived from the boolean pureAI in Team.cs
        [SerializeField]
        private bool isTeamAI = false;

        [SerializeField]
        private bool hasBall;
        public bool HasBall
        {
            get { return hasBall; }
            set
            {
                hasBall = value;
                BallerEvents.BallPossessionUpdate();
            }
        }

        public bool midAction;

        [SerializeField]
        public bool fatigueActive = false;
        [SerializeField]
        public float fatigueRate = 0f;

        private int ballerIndex = 0;
        public int BallerIndex
        {
            get { return ballerIndex; }
        }

        public float turnPercent = 1.0f;
        public Vector3 faceDirection;
        public Vector3 MoveDirect;
        public Vector3 lastForward;

        public Transform myShotPoint;
        public Transform myChestPoint;
        [HideInInspector]
        public Transform MyTransform;

        public CharacterController CharCollid;
        public Defense_PosFinder DefensePos;

        public Team MyTeam = null;
        public BBaller_skin SkinAndData;

        private Team_Manager teamManager;
        public PlayerStat playerStat;
        public PlayerStat ballerStat
        {
            get { return playerStat; }
        }

        public Team_Manager TeamManager
        {
            get { return teamManager; }
        }

        #endregion

        #region MovementRandomizingValues
        [SerializeField]
        private float standTime = 0.0f;
        [SerializeField]
        private float minStandTime = 1f;
        [SerializeField]
        private float maxStandTime = 2.5f;
        #endregion

        #region MovementAndGuardVars
        //!!!: Playtest to figure out a good range of the speedVaryingFactor variable, and then use the SpeedStat of the player to replicate something similar.
        public float speedVaryingFactor = 0;

        private MoveType moveState = MoveType.NULL;
        public MoveType MoveState
        {
            get { return moveState; }
            set
            {
                if (moveState != value)
                {
                    moveState = value;
                    switch (moveState)
                    {
                        case MoveType.WALK:
                            ActualMoveSpeed = walkSpeed;
                            ballerAI.ChangeState(BBallerStateMachine.Move);
                            break;

                        case MoveType.JOG:
                            ActualMoveSpeed = jogSpeed;
                            ballerAI.ChangeState(BBallerStateMachine.Move);
                            break;

                        case MoveType.RUN:
                            ActualMoveSpeed = runSpeed;
                            ballerAI.ChangeState(BBallerStateMachine.Move);
                            break;

                        case MoveType.NULL:
                        default:
                            ActualMoveSpeed = 0;
                            if (hasBall)
                            {
                                ballerAI.ChangeState(BBallerStateMachine.Idle_Ball);
                            }
                            else
                            {
                                ballerAI.ChangeState(BBallerStateMachine.Idle_NoBall);
                            }
                            break;
                    }
                }
            }
        }

        public Vector3 MoveToLoc;

        public bool IsMoveForward;
        //Actual(Base) Moving Speeds
        public float runSpeed;
        public float jogSpeed;
        public float walkSpeed;
        /// <summary>
        /// The Actual Move Speed. For the Stat-factored version, use AdjustedMoveSpeed.
        /// </summary>
        public float ActualMoveSpeed;
        /// <summary>
        /// Gets the adjusted MoveSpeed by factoring in PlayerStat[Speed]/PlayerLevel and Fatigue.
        /// </summary>
        public float AdjustedMoveSpeed
        {
            get
            {
                if (playerStat != null)
                {
                    return ( //(ActualMoveSpeed + mojoLevel * speedVaryingFactor) * 
                            (ActualMoveSpeed + mojoLevel * speedVaryingFactor) *
                        //(1f + (playerStat.LatestStatList[PlayerStats.SPEED]/100f)) * 
                            (1.0f - fatigue));
                }
                else
                {
                    return ActualMoveSpeed;
                }
            }
        }

        /// <summary>
        /// Represents the degree to which the On-screen Joystick is being pushed. 
        /// Greater degree will represent a Running Speed, Smaller will  represent Jogging, or even Smaller, simply Walking, etc
        /// </summary>
        public float SpeedPercent;
        [SerializeField]
        public float standThreshold;
        [SerializeField]
        public float walkThreshold;
        [SerializeField]
        public float jogThreshold;


        [SerializeField]
        public float guardInPosRange;
        [SerializeField]
        public float guardStandRange;
        [SerializeField]
        public float guardShuffleRange;

        /// <summary>
        /// Used by AI
        /// </summary>
        public bool guardInPos;

        /// <summary>
        /// Used for Scuttling
        /// </summary>
        public float guardSpeed;
        /// <summary>
        /// The guard dir. Use GuardDir instead of this to Set the direction.
        /// </summary>
        private GuardDirection guardDir = GuardDirection.NULL;
        public GuardDirection GuardDir
        {
            get { return guardDir; }
            set
            {
                guardDir = value;
                if (value != GuardDirection.NULL)
                    ballerAI.ChangeState(BBallerStateMachine.Guard);
                else if (hasBall)
                    Debug.LogError("While Guarding I have a ball. This is unexpected behavior.");
                else
                    ballerAI.ChangeState(BBallerStateMachine.Idle_NoBall);
            }
        }

        /// <summary>
        /// Doughnut Radii.
        /// </summary>

        public float DoughnutInnerRadius = 4.0f;
        public float DoughnutOuterRadius = 8.0f;
        #endregion

        #region DunkVars
        public bool dunkLerp;
        public float dunkTime;
        public float dunkTimeTotal;
        public Vector3 dunkStartPos;
        #endregion

        #region StagLeapVars
        public bool stagLerp;
        public float stagTime;
        public float stagTimeTotal;
        public Vector3 stagStartPos;
        #endregion

        #region InterceptVars
        public bool attemptingIntercept;
        #endregion

        #region ClownVars
        public bool attemptingClown;
        #endregion

        #region BlockVars
        public bool attemptingBlock;
        //private float blockModeMaxTime = 2.0f;
        //public float blockModeTimer = 2.0f;
        #endregion

        #region MOJOVars
        public int mojoBuildSpeed = 5;

        /// <summary>
        /// The mojo. Assuming it can reach a maximum value of 100.
        /// </summary>
        public float mojo;
        public float mojoLevel;
        #endregion

        #region FatigueVars
        [SerializeField]
        private float fatigue = 0f;
        public float Fatigue
        {
            get { return fatigue; }
        }
        #endregion

        #region StealVars
        public float stealRadius;
        public float stealCooldown;

        bool onStealTimer = false;
        #endregion

        #region ShootVars
        public float ShootDistMax;
        #endregion
        //public bool inPos; 		//Probably no use
        #endregion

        public BallerAI ballerAI;

        public bool ChangeState(State newState)
        {
            return ballerAI.ChangeState(newState);
        }

        public Animation animator;
        public string currentAnim;


        public void PlayAnimation(string animToPlay, float fadeTime, bool isLooping)
        {
            if (animator == null) return;
            //Awake ();

            if (currentAnim != animToPlay)
            {
                currentAnim = animToPlay;

                //Transition to appropriate idle state when action is completed
                if (!isLooping)
                {
                    if (hasBall)
                    {
                        StartCoroutine(AnimateAction_Ball(animToPlay, fadeTime));
                    }
                    else
                    {
                        StartCoroutine(AnimateAction_NoBall(animToPlay, fadeTime));
                    }
                }
                else
                {
                    animator.CrossFade(animToPlay, fadeTime);
                }
            }
            /////////////////////////////////////
            else
            {
                if (!animator.isPlaying)
                {
                    animator.Play(animToPlay);
                }
            }

        }

        IEnumerator AnimateAction_NoBall(string animation, float fadeTime)
        {
            animator.CrossFade(animation, fadeTime);
            yield return new WaitForSeconds(animator.GetClip(animation).length);
            ballerAI.ChangeState(BBallerStateMachine.Idle_NoBall);
            ballerAI.midAction = false;
        }

        IEnumerator AnimateAction_Ball(string animation, float fadeTime)
        {
            animator.CrossFade(animation, fadeTime);
            yield return new WaitForSeconds(animator.GetClip(animation).length);
            ballerAI.ChangeState(BBallerStateMachine.Idle_Ball);
            ballerAI.midAction = false;
        }

        public void Cleanup()
        {
            //unsubscribeEvents();
            ballerAI.Cleanup();
        }

        public State getCurrentState()
        {
            if (null == ballerAI)
            {
                return null;
            }
            return ballerAI.getCurrentState();
        }

        #region Methods

        #region EventSubscribers
        //private void subscribeEvents() {
        //    BallerEvents.onBallPossessionUpdate += ballerAI.onBallPossessionChange;
        //    //BallerEvents.onMojoUpdate += this.checkMojoThreshold;
        //}

        //private void unsubscribeEvents() {
        //    BallerEvents.onBallPossessionUpdate -= ballerAI.onBallPossessionChange;
        //    //BallerEvents.onMojoUpdate -= this.checkMojoThreshold;
        //}
        #endregion

        #region Initializers
        void Awake()
        {
            if (!int.TryParse(this.name[this.name.Length - 1].ToString(), out ballerIndex))
            {
                Debug.LogError("Last Letter of BBaller GameObject name should be an index");
            }

            Component[] component = GetComponentsInChildren<Animation>();
            if (component.Length < 1)
            {
                Debug.LogError("No animations found for Basketballer");
            }

            animator = (Animation)component[0];

            initPlayerVars();
            //subscribeEvents();
        }

        void OnDestroy()
        {
           // Cleanup(); -- Uncomment later -- Adam
        }

        public void initPlayerVars()
        {
            //Transform
            MyTransform = this.transform;

            // Other Components
            CharCollid = GetComponent<CharacterController>();
            //DefensePos  = GetComponentInChildren<Defense_PosFinder>();
            SkinAndData = GetComponent<BBaller_skin>();
            playerStat = GetComponent<PlayerStat>();
			playerStat.setupInitialStat();
			/*
			//load "default" stats - power, speed, agility
            BBaller d_baller = XMLDataParser.playerDictionary[uid];
            firstName = d_baller.firstName;
            nickName = d_baller.nickName;
            lastName = d_baller.lastName;
            speedPercentage = d_baller.speedPercentage;
            powerPercentage = d_baller.powerPercentage;
            agilityPercentage = d_baller.agilityPercentage;
            */
            //StateMachine
            ballerAI = GetComponent<BallerAI>();
            BBallerStateMachine stateMachine = new BBallerStateMachine(ballerAI);
            ballerAI.Initialize(stateMachine);
            stateMachine.Initialize();

            //Initial State
            ballerAI.ChangeState(BBallerStateMachine.Idle_NoBall);
            // Assign Team
            StopCoroutine("initPolledVars");
            StartCoroutine("initPolledVars");

            BallerEvents.BallPossessionUpdate();
        }

        private IEnumerator initPolledVars()
        {
            while (MyTeam == null)
            {
                if (teamManager == null)
                    teamManager = GameObject.Find("Team_Manager").GetComponent<Team_Manager>();

                if (teamManager != null)
                {
                    MyTeam = (isTeamAI) ? teamManager.TeamAI : teamManager.TeamPlayer;
                }

                if (MyTeam != null)
                    break;

                yield return null;
            }

            //QuarterLength;
            while (GameData.instance == null)
            {
                yield return null;
            }
            setFatigueRate();

        }

        private void setFatigueRate()
        {
            float maxFatigue = 0.5f; //A maximum of 50% Fatigue by the end of the Quarter
            float quarterLength = GameData.quarterLength * 60f; 		// minutes to Seconds
            fatigueRate = maxFatigue / quarterLength;
            fatigueRate = (float)Math.Round(fatigueRate, 5, MidpointRounding.ToEven);
        }



        // Use this for initialization
        void Start()
        {
            //Defense_Pos detaches itself from it's parent. Not sure why. Probably for absolute world positions.
            if (DefensePos == null)
            {
                Defense_PosFinder[] allDefPos = GameObject.FindObjectsOfType<Defense_PosFinder>();
                foreach (Defense_PosFinder defPos in allDefPos)
                {
                    //Before detaching, parent bballer script is attached.
                    if (defPos.bballer == this)
                    {
                        DefensePos = defPos;
                        break;
                    }
                }
            }

            //MyIndex = MyTeam.getBallerTeamIndex(this);
        }
        #endregion

        public void Update()
        {
            if(dunkLerp) {
                //   debugBooleanStartDunk = false;
                //    mojoLevel = 100;
                //    SkinAndData.ballerData.dunk = GameAnims.OneHandDunk;
                //    ballerAI.Dunk();
                continueDunk();
            }

            if (GameAdmin.GamePause)
                return;

           // ballerAI.Update(); // comment out for now since wsbbai is a monobehavior and its update is alraedy being called internally 6/25

            //Turn in Direction
            //if(faceDirection != Vector3.zero) 
            {		//??? Probably do not need this condition
                if (turnPercent < 1)
                {
                    //Debug.Log("turn percent < 1 -- bballer.cs update");
                    MyTransform.forward = Vector3.Lerp(lastForward, faceDirection, turnPercent);
                    turnPercent += Time.deltaTime * 4;
                    if (turnPercent > 1)
                    {
                        faceDirection = MyTransform.forward;
                    }
                }
                else
                {
                    //Debug.Log("time is: " + Time.deltaTime + "");
                    MyTransform.forward = Vector3.Lerp(MyTransform.forward, faceDirection, 15 * Time.deltaTime);
                }
            }

            //Move baller in Direction
            {
                //Run(faceDirection, 1.0f);
            }

            //FatigueCalculation
            if (fatigueActive)
            {
                //Fatigue Accumulation
                fatigue += fatigueRate * Time.deltaTime;
            }
            else if (fatigue > 0f)
            {
                //Fatigue Recovery
                fatigue -= 2f * fatigueRate * Time.deltaTime;
            }
            else if (fatigue < 0f)
            {
                //Resetting Fatigue to zero
                fatigue = 0f;
            }

            //Keep the player on the ground
            MyTransform.position.Set(MyTransform.position.x,
                0, MyTransform.position.z);
            MyTransform.eulerAngles.Set(0, MyTransform.eulerAngles.y, 0);

            //do we need this? v
            //skinAndData.ballerData.ItemUpdate();

            // Update ball position if i have the ball //
            if (hasBall)
                WSBB.Ball.currentPosition = gameObject.transform.position;
        }
        #endregion

        #region MojoUpdates
        //==========
        // Mojo Update
        //==========

        //		private void checkMojoThreshold(BBaller eventBaller)
        //		{
        //			if (eventBaller != this)
        //				return;
        //
        //			if(mojoLevel >= MyTeam.MojoThreshold)
        //			{
        //				mojoLevel = MyTeam.MojoThreshold;
        //
        //			}
        //
        //		}

        public void UpdateMojo(MojoUpdates updateType)
        {
            UpdateMojo(updateType, true);
        }

        public void UpdateMojo(MojoUpdates updateType, bool isSuccessful)
        {
            int mojoIncrementValue = getMojoValue(updateType, isSuccessful);

            if (mojoIncrementValue == 0)
                Debug.LogError("UpdateMojo() has been called when it shouldn't be.");
            else
            {
                mojoLevel += mojoIncrementValue;
                //BallerEvents.MojoUpdated(this);
            }
        }
        #endregion

        #region StateSpecificMethods


        #region AIBallerPositioners

        #region ForOffenseTeam

        public void BeBetweenDefensemanAndBasket()
        {
            MoveOffenseTeamAI(MyTeam.positionPicker[MyIndex].lastPos, MoveState);
            RemainBetweenBallerAndBasket(MyTeam.otherTeam.bballers[MyIndex].MyTransform.position,
                                         MyTeam.basketTarget.position);

        }

        #endregion

        #region ForDefenseTeam
        public void RemainBetweenOffensemanAndBasket()
        {
            MoveDefenseTeamAI(MyTeam.positionPicker[MyIndex].lastPos);
            RemainBetweenBallerAndBasket(MyTeam.otherTeam.bballers[MyIndex].MyTransform.position,
                                           MyTeam.otherTeam.basketTarget.position);

        }

        public void RemainCloseToBasket_Defense()
        {
            MoveDefenseTeamAI(MyTeam.positionPicker[MyIndex].lastPos);

            if (!ballerAI.midAction)
            {
                if (ballerAI.stateMachine.CurrentState == BBallerStateMachine.Idle_NoBall)
                {
                    if (standTime < 0)
                    {
                        //MyTeam.positionPicker[2].PickPointRect(areaPref);
                        MyTeam.positionPicker[MyIndex].lastPos = PositionInRangeOf(MyTeam.otherTeam.basketTarget.position, 3.0f);

                        // Intended Position of this player
                        Vector3 position = MyTeam.positionPicker[MyIndex].lastPos;

                        // A point 10% of the way towards center point from either side
                        Vector3 defensiveBasketPosition = Vector3.Lerp(MyTeam.otherTeam.basketTarget.position, Vector3.zero, 0.1f);


                        Vector3 lhs = position - defensiveBasketPosition;
                        Vector3 rhs = position - MyTeam.otherTeam.getBallerWithBall().MyTransform.position;

                        if (Vector3.Dot(lhs, rhs) < 0.0f)
                        {
                            MyTeam.positionPicker[MyIndex].lastPos = defensiveBasketPosition - lhs;
                        }

                        MyTeam.positionPicker[MyIndex].lastPos.y = MyTeam.otherTeam.getBallerWithBall().MyTransform.position.y;
                        getCourtSafePosition(ref MyTeam.positionPicker[MyIndex].lastPos);
                        standTime = UnityEngine.Random.Range(minStandTime, maxStandTime);
                        MoveState = MoveType.JOG;
                    }
                    standTime -= Time.deltaTime;
                }
            }
        }
        #endregion

        #region CommonForBothTeams

        public void RemainBetweenBallerAndBasket(Vector3 opponentPos, Vector3 basketPos)
        {

            if (!ballerAI.midAction)
            {
                if (ballerAI.stateMachine.CurrentState == BBallerStateMachine.Idle_NoBall)
                {
                    if (standTime < 0)
                    {
                        //MyTeam.positionPicker[2].PickPointRect(areaPref);
                        MyTeam.positionPicker[MyIndex].lastPos = PositionBetween(opponentPos, basketPos);
                        standTime = UnityEngine.Random.Range(minStandTime, maxStandTime);
                        MoveState = MoveType.JOG;
                    }
                    standTime -= Time.deltaTime;
                }
            }

        }
        #endregion


        #endregion

        #region MovementMethods
        //==========
        // Movement Methods
        //==========

        public void Run(Vector3 direct, float speedPercent)
        {
            if (!ballerAI.midAction)
            {
                //IsMoveForward = true;
                //faceDirection = Vector3.Lerp(faceDirection, -direct, Time.deltaTime * 5);
                faceDirection = -direct;
                MyTransform.forward = -direct;
                
                this.SpeedPercent = speedPercent;
                if (speedPercent < standThreshold)
                {
                    ChangeState(hasBall ? BBallerStateMachine.Idle_Ball : BBallerStateMachine.Idle_NoBall);
                    MoveState = MoveType.NULL;
                }
                else if (speedPercent < walkThreshold)
                {
                    ChangeState(BBallerStateMachine.Move);
                    MoveState = MoveType.WALK;
                    PlayAnimation(hasBall ?  GameAnims.Walk_Ball : GameAnims.Walk_NoBall, 0.15f, true);
                }
                else if (speedPercent < jogThreshold)
                {
                    ChangeState(BBallerStateMachine.Move);
                    MoveState = MoveType.JOG;
                    PlayAnimation(hasBall ? GameAnims.Jog_Ball : GameAnims.Jog_NoBall, 0.15f, true);
                }
                else /*Run*/
                {
                    ChangeState(BBallerStateMachine.Move);
                    MoveState = MoveType.RUN;
                    PlayAnimation(hasBall ? GameAnims.Run_Ball : GameAnims.Run_NoBall, 0.15f, true);
                }
                //keeping in walk for testing purposes
                //ChangeState(BBallerStateMachine.Move);
                //MoveState = MoveType.WALK;
                Move();//????
            }
        }


        public void Move()
        {

            if (GetComponentInParent<BBaller>() != MyTeam.getPlayerBaller())
            {
                UnityEngine.AI.NavMeshAgent agent = GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
                agent.Resume();
                agent.speed = ActualMoveSpeed;
                agent.SetDestination(MoveToLoc);

                return;
            }

            CharCollid.Move(MyTransform.forward *
                                /* moveSpeed * */
                                AdjustedMoveSpeed *
                                0.03f
                                );
            return;

            //Executed in StateMachine.Move
            if (IsMoveForward)
            {
                CharCollid.Move(MyTransform.forward *
                    /* moveSpeed * */
                                AdjustedMoveSpeed *
                                Time.deltaTime
                                );
            }
            else
            {
                CharCollid.Move(MoveDirect *
                    /* moveSpeed * */
                                AdjustedMoveSpeed *
                                Time.deltaTime
                                );
            }
        }

       /* public void MoveTo(Vector3 location)
        {
            float distance = (MyTransform.position - location).magnitude;
            if (distance  > DoughnutOuterRadius)
            {
                CharCollid.Move(MyTransform.forward *
                                AdjustedMoveSpeed *
                                Time.deltaTime
                                );
            }
        }
        */

        //Discuss implementation, simple back shuffle to get away from guard
        public void RemainOpen(Vector3 guardPos)
        {
            if (!ballerAI.midAction)
            {
                Vector3 faceDir;
                var diffVector = guardPos - MyTransform.position;
                if (guardInPos)
                {
                    if (diffVector.sqrMagnitude > Mathf.Pow(guardInPosRange, 2f))
                    {
                        guardInPos = false;

                        ChangeState(BBallerStateMachine.Idle_NoBall);
                    }
                    else
                    {
                        //MoveState = BallerState.Scuttle;
                        ChangeState(BBallerStateMachine.RemainOpen);
                        ActualMoveSpeed = guardSpeed;
                        IsMoveForward = false;
                        diffVector.Normalize();
                        MoveDirect = diffVector;
                        faceDir = MyTeam.otherTeam.getBallerWithBall().MyTransform.position - MyTransform.position;
                        faceDir.y = 0;
                        faceDir.Normalize();
                        faceDirection = faceDir;
                        if (Vector3.Dot(MyTransform.right * -1, MoveDirect) > .5f)
                            GuardDir = GuardDirection.LEFT;
                        else if (Vector3.Dot(MyTransform.right, MoveDirect) > .5f)
                            GuardDir = GuardDirection.RIGHT;
                        else if (Vector3.Dot(MyTransform.forward, MoveDirect) > 0f)
                            GuardDir = GuardDirection.FORWARD;
                        else
                            GuardDir = GuardDirection.BACK;
                    }
                }
            }
        }


        public void MoveOffenseTeamAI(Vector3 offensePos, MoveType moveState)
        {
            if (!ballerAI.midAction)
            {
                //if(offensePos == null) {		//Vector3 should not be equated to null
                //TODO: See if this change introduces regression or not.
                if (offensePos == Vector3.zero)
                {
                    Debug.Log("offensePos == Vector3.zero");
                    if (hasBall)
                    {
                        ChangeState(BBallerStateMachine.Idle_Ball);
                    }
                    else
                    {
                        ChangeState(BBallerStateMachine.Idle_NoBall);
                    }
                    return;
                }
                Vector3 diffVector = offensePos - MyTransform.position;
                if (!guardInPos)
                {
					ChangeState(BBallerStateMachine.Move);
                    if (diffVector.sqrMagnitude < Mathf.Pow(guardInPosRange, 2))
                    {
                        guardInPos = true;
                        standTime = UnityEngine.Random.Range(minStandTime, maxStandTime);
                    }
                    else
                    {
                        switch (moveState)
                        {
                            case MoveType.RUN:
                                MoveState = MoveType.RUN;
                                ActualMoveSpeed = runSpeed;
                                break;
                            case MoveType.JOG:
                                MoveState = MoveType.JOG;
                                ActualMoveSpeed = jogSpeed;
                                break;
                            case MoveType.WALK:
                                MoveState = MoveType.WALK;
                                ActualMoveSpeed = walkSpeed;
                                break;
                            default:
                                MoveState = MoveType.NULL;
                                ActualMoveSpeed = runSpeed;
                                break;
                        }
                        IsMoveForward = true;
                        diffVector.Normalize();
                        faceDirection = diffVector;
                        Move();
                    }
                }
                else
                {
                    if (diffVector.sqrMagnitude > Mathf.Pow(guardStandRange, 2))
                    {
                        guardInPos = false;
                    }
                    else
                    {
                        standTime -= Time.deltaTime;
                        if(standTime <= 0.00f)
                        {
                            MyTeam.positionPicker[MyIndex].lastPos = Vector3.zero;  // Might not be needed anymore -- Adam
                            ballerAI.moveLocSet = false;    // Have the baller move to a different location
                        }
                    }
                    

                    Vector3 faceDir;
                    if (HasBall)
                    {
                        ChangeState(BBallerStateMachine.Idle_Ball);
                        faceDir = MyTeam.basketTarget.position - MyTransform.position;
                        faceDir.y = 0;
                        faceDir.Normalize();
                    }
                    else
                    {
                        ChangeState(BBallerStateMachine.Idle_NoBall);
                        faceDir = MyTeam.getPlayerBaller().MyTransform.position - MyTransform.position;
                        faceDir.y = 0;
                        faceDir.Normalize();
                    }
                    faceDirection = faceDir;
                }
                //Move();
            }
            //Move();
        }

        public void MoveDefenseTeamAI(Vector3 guardPos)
        {
            if (!ballerAI.midAction)
            {
                Vector3 faceDir;
                var diffVector = guardPos - MyTransform.position;

                if (!guardInPos)
                {
                    if (diffVector.sqrMagnitude < Mathf.Pow(guardInPosRange, 2f))
                    {
                        guardInPos = true;

                        ChangeState(BBallerStateMachine.Idle_NoBall);
                    }
                    else if (diffVector.sqrMagnitude < Mathf.Pow(guardShuffleRange, 2f))
                    {
                        //MoveState = BallerState.Scuttle;
                        ChangeState(BBallerStateMachine.Guard);
                        ActualMoveSpeed = guardSpeed;
                        IsMoveForward = false;
                        diffVector.Normalize();
                        MoveDirect = diffVector;
                        faceDir = MyTeam.otherTeam.getBallerWithBall().MyTransform.position - MyTransform.position;
                        faceDir.y = 0;
                        faceDir.Normalize();
                        faceDirection = faceDir;
                        if (Vector3.Dot(MyTransform.right * -1, MoveDirect) > .5f)
                            GuardDir = GuardDirection.LEFT;
                        else if (Vector3.Dot(MyTransform.right, MoveDirect) > .5f)
                            GuardDir = GuardDirection.RIGHT;
                        else if (Vector3.Dot(MyTransform.forward, MoveDirect) > 0f)
                            GuardDir = GuardDirection.FORWARD;
                        else
                            GuardDir = GuardDirection.BACK;
                    }
                    else
                    {
                        ChangeState(BBallerStateMachine.Move);
                        MoveState = MoveType.JOG;
                        //ActualMoveSpeed = jogSpeed;
                        IsMoveForward = true;
                        diffVector.Normalize();
                        faceDirection = diffVector;
                    }
                }
                else
                {
                    if (diffVector.sqrMagnitude > Mathf.Pow(guardStandRange, 2f))
                    {
                        guardInPos = false;
                    }
                    ChangeState(BBallerStateMachine.Idle_NoBall);
                    faceDir = MyTeam.otherTeam.getBallerWithBall().MyTransform.position - MyTransform.position;
                    faceDir.y = 0;
                    faceDir.Normalize();
                    faceDirection = faceDir;
                }
            }
            Move();
        }




        //public void AIOffense(Vector3 offensePos, MoveType moveState)
        //{
        //    if (!midAction)
        //    {
        //        //if(offensePos == null) {		//Vector3 should not be equated to null
        //        //TODO: See if this change introduces regression or not.
        //        if (offensePos == Vector3.zero)
        //        {
        //            if (hasBall)
        //            {
        //                ChangeState(BBallerStateMachine.Idle_Ball);
        //            }
        //            else
        //            {
        //                ChangeState(BBallerStateMachine.Idle_NoBall);
        //            }
        //            return;
        //        }
        //        var diffVector = offensePos - MyTransform.position;
        //        if (!guardInPos)
        //        {
        //            if (diffVector.sqrMagnitude < Mathf.Pow(guardInPosRange, 2))
        //            {
        //                guardInPos = true;
        //            }
        //            else
        //            {
        //                switch (moveState)
        //                {
        //                    case MoveType.RUN:
        //                        MoveState = MoveType.RUN;
        //                        //							ActualMoveSpeed = runSpeed;
        //                        break;
        //                    case MoveType.JOG:
        //                        MoveState = MoveType.JOG;
        //                        //							ActualMoveSpeed = jogSpeed;
        //                        break;
        //                    case MoveType.WALK:
        //                        MoveState = MoveType.WALK;
        //                        //							ActualMoveSpeed = walkSpeed;
        //                        break;
        //                    default:
        //                        MoveState = MoveType.NULL;
        //                        //							ActualMoveSpeed = runSpeed;
        //                        break;
        //                }
        //                IsMoveForward = true;
        //                diffVector.Normalize();
        //                faceDirection = diffVector;
        //            }
        //        }
        //        else
        //        {
        //            if (diffVector.sqrMagnitude > Mathf.Pow(guardStandRange, 2))
        //            {
        //                guardInPos = false;
        //            }
        //            //ChangeState(BallerStates.Idle.Instance);

        //            Vector3 faceDir;
        //            if (HasBall)
        //            {
        //                ChangeState(BBallerStateMachine.Idle_Ball);
        //                faceDir = MyTeam.basketTarget.position - MyTransform.position;
        //                faceDir.y = 0;
        //                faceDir.Normalize();
        //            }
        //            else
        //            {
        //                ChangeState(BBallerStateMachine.Idle_NoBall);
        //                faceDir = MyTeam.getPlayerBaller().MyTransform.position - MyTransform.position;
        //                faceDir.y = 0;
        //                faceDir.Normalize();
        //            }
        //            faceDirection = faceDir;
        //        }
        //    }
        //}



       // public void AIGuard(Vector3 guardPos, bool opponentHasBall)
        //{
        //    if (!midAction)
        //    {
        //        Vector3 faceDir;
        //        var diffVector = guardPos - MyTransform.position;

        //        if (!guardInPos)
        //        {
        //            if (diffVector.sqrMagnitude < Mathf.Pow(guardInPosRange, 2f))
        //            {
        //                guardInPos = true;

        //                ChangeState(BBallerStateMachine.Idle_NoBall);
        //            }
        //            else if (diffVector.sqrMagnitude < Mathf.Pow(guardShuffleRange, 2f))
        //            {
        //                //MoveState = BallerState.Scuttle;
        //                ChangeState(BBallerStateMachine.Guard);
        //                ActualMoveSpeed = guardSpeed;
        //                IsMoveForward = false;
        //                diffVector.Normalize();
        //                MoveDirect = diffVector;
        //                faceDir = MyTeam.otherTeam.getBallerWithBall().MyTransform.position - MyTransform.position;
        //                faceDir.y = 0;
        //                faceDir.Normalize();
        //                faceDirection = faceDir;
        //                if (Vector3.Dot(MyTransform.right * -1, MoveDirect) > .5f)
        //                    GuardDir = GuardDirection.LEFT;
        //                else if (Vector3.Dot(MyTransform.right, MoveDirect) > .5f)
        //                    GuardDir = GuardDirection.RIGHT;
        //                else if (Vector3.Dot(MyTransform.forward, MoveDirect) > 0f)
        //                    GuardDir = GuardDirection.FORWARD;
        //                else
        //                    GuardDir = GuardDirection.BACK;
        //            }
        //            else
        //            {
        //                ChangeState(BBallerStateMachine.Move);
        //                MoveState = MoveType.JOG;
        //                //ActualMoveSpeed = jogSpeed;
        //                IsMoveForward = true;
        //                diffVector.Normalize();
        //                faceDirection = diffVector;
        //            }
        //        }
        //        else
        //        {
        //            if (diffVector.sqrMagnitude > Mathf.Pow(guardStandRange, 2f))
        //            {
        //                guardInPos = false;
        //            }
        //            ChangeState(BBallerStateMachine.Idle_NoBall);
        //            faceDir = MyTeam.otherTeam.getBallerWithBall().MyTransform.position - MyTransform.position;
        //            faceDir.y = 0;
        //            faceDir.Normalize();
        //            faceDirection = faceDir;
        //        }
        //    }
        //}
        #endregion

        #region ShootingMethods
        public void Shoot()
        {
            float distSqrd = GetDistSqrdToTargetBasket();

            if (!ballerAI.midAction && HasBall && distSqrd < ShootDistMax * ShootDistMax)
            {
                Debug.Log("Take Shot");
                ChangeState(BBallerStateMachine.Shoot);
                ballerAI.midAction = true;
                ActualMoveSpeed = 0;
                faceDirection = MyTeam.basketTarget.position - MyTransform.position;
                faceDirection.y = 0;
                faceDirection.Normalize();
                lastForward = MyTransform.forward;
                turnPercent = 0;
                ///////////////////////////
                StartCoroutine(ShootDuringAnimation(0.4f));
                //LaunchBallToBasket(); //Should be called at the end of animation, calling here for now instead until animations are fixed -- Adam
            }
            else
            {
                Debug.Log("Too far to shoot, moving forward - " + firstName);
                ballerAI.midAction = midAction = false;
                ChangeState(BBallerStateMachine.Move);
                ballerAI.MoveTowardTargetBasket();
            }
        }

        IEnumerator ShootDuringAnimation(float time)
        {
            yield return new WaitForSeconds(time);
            LaunchBallToBasket();
        }

        float GetDistSqrdToTargetBasket()
        {
            Vector3 toBasket = MyTransform.position - MyTeam.basketTarget.transform.position;

            return Vector3.SqrMagnitude(toBasket);
        }

        public void LaunchBallToBasket() //Should be called at the end of animation, calling here for now instead until animations are fixed -- Adam
        {
            if (ballerAI.midAction && HasBall)
            {
                //checkToSee if there is a blocker Nearby and do Block Calculations
                BBaller blockingBaller = MyTeam.otherTeam.getNearestBlockingBaller(MyTransform.position);
                if (blockingBaller != null && blockingBaller.attemptingBlock)
                {
                    Debug.Log("Shooter Calculating Block");
                    float blockCalc = CalcBlockChance(blockingBaller, this);
                    Debug.Log("Block Calc");
                    if ((UnityEngine.Random.value * 100) < blockCalc)
                    {
                        //Debug.Log("BlockSuccesful");
                        this.HasBall = false;
                        //this.SkinAndData.ballerData.item.TakeEvent(ItemEvent.shotFail);
                        blockingBaller.HasBall = true;
                        MyTeam.otherTeam.setAsPlayer(blockingBaller);
                        ChangeState(BBallerStateMachine.Idle_Ball);
                        blockingBaller.ballerAI.midAction = false;
                        blockingBaller.attemptingBlock = false;
                        return;
                    }
                    else
                    {
                        ChangeState(BBallerStateMachine.Idle_Ball);
                        blockingBaller.ballerAI.midAction = false;
                        blockingBaller.attemptingBlock = false;
                    }
                }


                //Prepare ball and make shot calculations
                Debug.Log("Ball Thrown");
                GameObject ballGO = (GameObject)Instantiate(MyTeam.BallPrefab, Vector3.zero, Quaternion.identity);
                Ball ball = (Ball)ballGO.GetComponent("Ball");
                ball.myTransform.position = myShotPoint.position; //Set ball's position to be where the shot starts
                ball.SetUpVector(myShotPoint.position, MyTeam.basketTarget.position, 6);
                ball.teamToScoreFor = MyTeam;
                ball.pointsToScore = 2;
                float shotChance = CalcShotChance();
                //Debug.Log("Shot Chance: " + shotChance);
                float randomValue = UnityEngine.Random.value * 100;
                //Debug.Log("Shot Random Value: " + randomValue);
                if (shotChance < 0 || randomValue > shotChance)  //This definitely needs tweaking, will get to it soon -- Adam
                {
                    //Debug.Log("Shot Is Going To Miss");
                    //this.skinAndData.ballerData.item.TakeEvent(ItemEvent.shotFail);
                    ball.rebound = true;
                    int baller;
                    Team reboundTeam;
                    //if (UnityEngine.Random.value >= .5)
                    if(true)                                //only rebound the ball to player's team for now until more states are in place for ai team to run an offense -- Adam
                    {
                        reboundTeam = MyTeam;
                    }
                    else
                    {
                        reboundTeam = MyTeam.otherTeam;
                    }
                    baller = UnityEngine.Random.Range(0, reboundTeam.bballers.Length);

                    if (baller == MyTeam.bballers.Length)//Should never go in here.
                    {
                        baller = 0;
                    }
                    ball.targetBaller = reboundTeam.bballers[baller];

                    //Handle Mojo
                    UpdateMojo(MojoUpdates.SHOOT, false);

                }
                else
                {
                    //Debug.Log("Shot Is Going To Be Made");

                    //Handle Mojo
                    UpdateMojo(MojoUpdates.SHOOT, true);

                    //this.SkinAndData.ballerData.item.TakeEvent(ItemEvent.shotMade);	
                }
                ball.Shoot();
                HasBall = false;
                ballerAI.midAction = false;                    //put this at the end of the animation?  --Adam
                MyTeam.cameraArm.springTension *= .2f;
            }
        }

        public float CalcShotChance()
        {
            Vector3 vector = MyTeam.basketTarget.position - myShotPoint.position;
            vector.y = 0;
            //float shotBase = SkinAndData.ballerData.getShoot();
            float shotBase = playerStat.LatestStatList[PlayerStats.SHOOT];
            //float distanceToHoop = vector.magnitude * .3048f;                     //why multiply by .3048?  -- Adam
            float distanceToHoop = vector.magnitude;
            float distanceValue = Mathf.Pow(distanceToHoop, (1 + (shotBase / 140)));
            //return shotBase - distanceValue;
            return 85.0f - distanceValue - shotBase;  //subtract from 85 since that is just over the max distance (hoop-hoop)  -- Adam
        }

        #endregion

        #region DunkMethods
        //==========
        //Dunk Functions
        //==========

        public String GetDunkName()
        {
            //return GameDunks.instance.dunks[GameData.instance.teamDunk].animationToPlay;   //not finding the correct animation, need to revisit later -- Adam
            return "DunkClassic"; //temp hack  --Adam
        }

        public bool CanDunk()
        {
            //if (mojoLevel < 100)      Ignore mojo until implemented -- Adam
                //return false;
            Vector3 vector = MyTransform.position - MyTeam.basketTarget.position;
            float distanceToHoopSqr = vector.sqrMagnitude * .3048f;
            //Debug.Log(distanceToHoopSqr + ":" + Mathf.Pow(GameData.instance.dunkRange, 2));

            if (distanceToHoopSqr > Mathf.Pow(GameData.instance.dunkRange, 2))
                return false;

            return true;
        }

        public void Dunk()
        {
            if (!CanDunk())
                return;
            ChangeState(BBallerStateMachine.Dunk);
            ballerAI.midAction = true;
            ActualMoveSpeed = 0;
            faceDirection = MyTeam.basketTarget.position - MyTransform.position;
            faceDirection.y = 0;
            faceDirection.Normalize();
            lastForward = MyTransform.forward;
            turnPercent = 0;
            DunkStartLerp(1);
        }

        public void DunkStartLerp(int lerpTime)
        {
            //Debug.Log(GetDunkName() + "--" + animator[GetDunkName()].clip.frameRate);
            AnimationClip animClip = animator[GetDunkName()].clip;
            dunkTimeTotal = lerpTime / animClip.frameRate;
            dunkTime = 0;
            dunkLerp = true;
            dunkStartPos = MyTransform.position;
        }

        public void continueDunk()
        {
            dunkTime += Time.deltaTime;
            float percent = dunkTime / 1.0f;
            Vector3 dunkSpot = MyTeam.basketTarget.position;
            dunkSpot.y = 0;
            MyTransform.position = Vector3.Lerp(dunkStartPos, dunkSpot, percent);
            if((MyTransform.position - dunkSpot).magnitude < 2.0f)
            {
                ChangeState(BBallerStateMachine.Idle_NoBall);
                endDunk();
            }
        }
        
        public void scoreDunk()
        {
            Debug.Log("FAN FARE");

            if (mojoLevel >= MyTeam.MojoThreshold)
            {
                mojoLevel = MyTeam.MojoThreshold;
                BallerEvents.DunkCompletedWithMojo(MyTeam);

            }
            else
            {
                BallerEvents.DunkCompleted(MyTeam);
            }

            UpdateMojo(MojoUpdates.DUNK, true);

        }
        
        public void endDunk()
        {
            if (stagLerp) // we actually did a stag leap!
                endStagLeap();
            else {
                Debug.Log("End dunk");
                Debug.Log(StackTraceUtility.ExtractStackTrace().ToString());
                ballerAI.midAction = false;
                ballerAI.midAction = false;
                MyTeam.Score(2);
                Scoreboard.shotTime = 45;
                GameAdmin.instance.state = PauseState.ScoreInfo;
                GameAdmin.GamePause = true;
                dunkLerp = false;
                ChangeState(BBallerStateMachine.Idle_NoBall);
                
            }
        }

        #endregion

        #region StagLeapMethods

        public bool CanStagLeap()
        {
            //if (mojoLevel < 100)      Ignore mojo until implemented -- Adam
            //return false;
            Vector3 vector = MyTransform.position - MyTeam.basketTarget.position;
            //float myDistanceToHoopSqr = vector.sqrMagnitude * .3048f;
            float myDistanceToHoopSqr = vector.sqrMagnitude;
            float maxThresh = Mathf.Pow(30/*GameData.instance.stagLeapRange*/, 2);
            float minThresh = Mathf.Pow(15, 2);

            BBaller dribbler = MyTeam.getBallerWithBall();


            if (myDistanceToHoopSqr > maxThresh || myDistanceToHoopSqr < minThresh)
            {
                //Debug.Log("StagLeap: DistToHoopSqrd = " + myDistanceToHoopSqr + " ::vs::  thresh = " + thresh);
                ChangeState(BBallerStateMachine.Move);
                ballerAI.midAction = false;
                return false;
            }

           // Debug.Log("StagLeap: DistToHoopSqrd = " + distanceToHoopSqr);
            return true;
        }

        public void StagLeap()
        {
            ChangeState(BBallerStateMachine.StagLeap);
            GetComponentInParent<UnityEngine.AI.NavMeshAgent>().Stop();
            ballerAI.midAction = true;
            ActualMoveSpeed = 0;
            faceDirection = MyTeam.basketTarget.position - MyTransform.position;
            faceDirection.y = 0;
            faceDirection.Normalize();
            lastForward = MyTransform.forward;
            turnPercent = 0;
            StagStartLerp(2.33f);
        }

        public void StagStartLerp(float lerpTime)
        {
            AnimationClip animClip = animator[GameAnims.Stag_Leap].clip;
            //dunkTimeTotal = lerpTime / animClip.frameRate;
            dunkTimeTotal = animClip.length;
            dunkTime = 0;
            stagLerp = true;
            dunkStartPos = MyTransform.position;
        }

        public void continueStagLeap()
        {
            //Debug.Log("continue Stag Leap");
            dunkTime += Time.deltaTime;
            //float percent = dunkTime / 1.0f;
            float percent = dunkTime / dunkTimeTotal;
            Vector3 passedBasket = faceDirection * 2; // Make baller Lerp towards passed the basket so that the animation lines up
            Vector3 dunkSpot = MyTeam.basketTarget.position + passedBasket; 
            //dunkSpot.y = 0;
            dunkSpot.y = MyTeam.basketTarget.position.y / 2;    // Actually make the baller lerp up to the basket until animation changes to move upwards instead
            MyTransform.position = Vector3.Lerp(dunkStartPos, dunkSpot, percent);
            if ( (MyTransform.position - dunkSpot).sqrMagnitude < (0.5f * 0.5f) )
            {
                Debug.Log(firstName + ": baller at hoop");
                endStagLeap();
            }
        }
        
        public void endStagLeap()
        {
            ballerAI.midAction = false;
            stagLerp = false;
            if (hasBall)    // Baller received the ball while in air and scored!
            {
                Debug.Log(firstName + "Stag Leap Success!");
                MyTeam.Score(2);
                Scoreboard.shotTime = 45;
                GameAdmin.instance.state = PauseState.ScoreInfo;
                GameAdmin.GamePause = true;
            }
            else
            {
                Debug.Log(firstName + "Stag Leap Failure! Did not have ball at end of stag leap!");
                MyTransform.position = new Vector3(MyTransform.position.x, 0, MyTransform.position.z);
                //GetComponentInParent<NavMeshAgent>().Resume();
            }
        }

        #endregion

        #region PassMethods
        //==========
        // Pass Functions
        //==========

        private BBaller getClosestTeammate()
        {
            int closestIndex = -1;
            float closestDistance = -1;
            for (int i = 0; i < MyTeam.bballers.Length; i++)
            {
                if (i != MyIndex)
                {
                    float distance = (MyTransform.position - MyTeam.bballers[i].MyTransform.position).magnitude;
                    if (distance < closestDistance || closestDistance == -1)
                    {
                        closestDistance = distance;
                        closestIndex = i;
                    }
                }
            }
            if (closestIndex == -1)
            {
                Debug.Log("Error (Pass): Could not find the closest teammate to pass to");
                return null;
            }
            else
                return MyTeam.bballers[closestIndex];
        }

        private BBaller getBestTeammateForPass()
        {
            int closestIndex = -1;
            float closestDistance = -1;
            for (int i = 0; i < MyTeam.bballers.Length; i++)
            {
                if (i != MyIndex)
                {
                    if (MyTeam.bballers[i].stagLerp){

                        return MyTeam.bballers[i];
                    }

                    float distance = (MyTransform.position - MyTeam.bballers[i].MyTransform.position).magnitude;
                    if (distance < closestDistance || closestDistance == -1)
                    {
                        closestDistance = distance;
                        closestIndex = i;
                    }
                }
            }
            if (closestIndex == -1)
            {
                Debug.Log("Error (Pass): Could not find the closest teammate to pass to");
                return null;
            }
            else
                return MyTeam.bballers[closestIndex];
        }



        public void Pass()
        {
            if (!ballerAI.midAction && HasBall)    // && !MyTeam.amICurrentBaller(this))
            {
                //Debug.Log("Passing Ball");
                ChangeState(BBallerStateMachine.Pass);
                ballerAI.midAction = true;
                BBaller closestTeammate = getBestTeammateForPass();
                Vector3 destination = closestTeammate.MyTransform.position;

                ActualMoveSpeed = 0;
                faceDirection = destination - MyTransform.position;
                faceDirection.y = 0;
                faceDirection.Normalize();
                lastForward = MyTransform.forward;
                turnPercent = 0;
                PassTheBall(closestTeammate);

            }
        }

        // public void PassTheBall()
        public void PassTheBall(BBaller recipient)
        {
            /*if ((UnityEngine.Random.value * 100) < CalcPassChance())
            {
                Debug.Log("Ball Pass Would Succeed");
                //this.skinAndData.ballerData.item.TakeEvent(ItemEvent.passMade);
            }
            else
            {
                Debug.Log("Ball Pass Would Fail");
                //this.skinAndData.ballerData.item.TakeEvent(ItemEvent.passFail);
            }
            */
            BBaller blockingBaller = MyTeam.otherTeam.getNearestBlockingBaller(MyTransform.position);
            if (blockingBaller != null && blockingBaller.attemptingIntercept)
            {
                //this.skinAndData.ballerData.item.TakeEvent(ItemEvent.passFail);
                //TODO: finish intercept calculations and methods (similar to block)
                if (false)
                {
                    switchPossession(blockingBaller);
                    return;
                }

            }


            //Debug.Log("Ball Thrown");
            GameObject ballGO = (GameObject)Instantiate(MyTeam.BallPrefab, Vector3.zero, Quaternion.identity);
            Ball ball = (Ball)ballGO.GetComponent("Ball");
            //ball.SetUpTrans(myShotPoint, MyTeam.getCurrentBaller().transform, 1);
            //ball.targetBaller = MyTeam.getCurrentBaller();

            ball.myTransform.position = myShotPoint.position;
            ball.SetUpVector(ball.myTransform.position, recipient.MyTransform.position, 1.1f);
            ball.targetBaller = recipient;

            float passChance = CalcPassChance();
            float passValue = UnityEngine.Random.value * 100;
			if (false)//passValue < passChance)
            //TODO: handle mojo updates?
            {
                //pass is not successful
                Debug.Log("UNsuccessful pass");
                BBaller recipientGuard = MyTeam.otherTeam.getNearestBlockingBaller(recipient.MyTransform.position);
                if (recipientGuard != null)
                {
                    //recipient guard will get the ball
                    switchPossession(recipientGuard);
                    return;
                }
                else
                {
                    Debug.Log("no recipient guard");
                    MyTeam.otherTeam.getNearestBaller(MyTransform.position);
                }
            }
            else
            {
                //pass is successful
                Debug.Log("sucessful pass");
            }

            ball.Pass();
            ball.alertReciever = true;
            HasBall = false;
            ballerAI.midAction = false;
			//recipient.HasBall = true;
			//this.ChangeState(BBallerStateMachine.Move);
            MyTeam.cameraArm.springTension *= .2f;
        }

        public void PrepareToRecieveBall(Vector3 ballPos)
        {
            //ballerAI.midAction = true;
            lastForward = MyTransform.forward;
            turnPercent = 0;    // purpose?
        }

        public void RecieveBall()
        {
            hasBall = true;

            // Stop Navigation if ball belongs to player's team
            if (MyTeam.cameraArm.playerTeam == MyTeam)
            {
               // Debug.Log("Set pass recipient as player");
                MyTeam.setAsPlayer(this);
                if (!stagLerp)
                {
                    UnityEngine.AI.NavMeshAgent agent = GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
                    agent.Stop();
                }
            }
            MyTeam.setAsCurrent(this);

            // Unless we are stag leaping, end the current action
            if (!stagLerp)
            {
                this.ChangeState(BBallerStateMachine.Move);
                ballerAI.midAction = false;
            }
            else
            {
                Debug.Log(firstName + " received ball during stag leap!");
            }

        }

        public float CalcPassChance()
        {
            BBaller nearestDefender = MyTeam.otherTeam.getNearestBaller(MyTransform.position);
            //float passBase = skinAndData.ballerData.getPass();
            float passBase = playerStat.LatestStatList[PlayerStats.PASS];
            float guardBase = nearestDefender.playerStat.LatestStatList[PlayerStats.GUARD];
            float distanceValue = 1 - (MyTransform.position - nearestDefender.MyTransform.position).magnitude * .3084f;
            if (distanceValue > 1)
            {
                return 1.0f;
            }
            return 100 - (distanceValue * 50) + passBase - (guardBase / 2);
        }
        #endregion

        #region ClownMethods
        //==========
        // Block Functions
        //==========
        public void Clown()
        {

            if (!ballerAI.midAction && HasBall)
            {
                ballerAI.midAction = true;
                ChangeState(BBallerStateMachine.Clown);

                faceDirection = MyTeam.basketTarget.position - MyTransform.position;
                faceDirection.y = 0;
                faceDirection.Normalize();
                lastForward = MyTransform.forward;
                turnPercent = 0.25f;
                Debug.Log("Attempting Clown");
                attemptingClown = true;
                FinishClown();
            }
        }

        public void FinishClown()
        {
            if (!attemptingClown)
            {
                return;
            }
            BBaller nearestDefender = MyTeam.otherTeam.getNearestBaller(MyTransform.position);
            if (HasBall)
            //&& otherBaller.ballerAI.stateMachine.CurrentState == BBallerStateMachine.Guard)
            {
                Debug.Log("Calculating Clown");
                float clownChance = CalcClownChance(this, nearestDefender);
                Debug.Log("Clown chance: " + clownChance);
                if ((UnityEngine.Random.value * 100) < clownChance)
                {
                    PlayAnimation(GameAnims.Clown1, 0.15f, true);
                    ballerAI.midAction = false;
                    Debug.Log("Clown Attempt: Successful");

                    //UpdateMojo(MojoUpdates.CLOWN);
                }
                else
                {
                    Debug.Log("Clown Attempt: Failed");
                    ballerAI.midAction = false;
                    switchPossession(nearestDefender);

                }
            }
            else
            {
                Debug.Log("Can't Clown, opponent has finished action");
                this.attemptingClown = false;
            }
        }

        public float CalcClownChance(BBaller clowner, BBaller defender)
        {
            Debug.Log("CALCULATING CLOWN CHANCE");
            //Vector3 adjustedShooterPos = passer.MyTransform.position + (passer.MyTransform.forward * .25f);
            //float distanceValue = 1 - ((defender.MyTransform.position - adjustedShooterPos).magnitude * .3084f);
            float distanceValue = ((clowner.MyTransform.position - defender.MyTransform.position).magnitude);
            Debug.Log(distanceValue);
            if (distanceValue < 0)
                return 0;
            float stealBase = defender.playerStat.LatestStatList[PlayerStats.STEAL];
            float ballHandleBase = clowner.playerStat.LatestStatList[PlayerStats.AGILITY];
            Debug.Log("Distance Value: " + distanceValue);
            Debug.Log("Steal Base: " + stealBase);
            Debug.Log("Ball Handle Base: " + ballHandleBase);
            return (((distanceValue) * 100)) - ballHandleBase;
        }

        #endregion

        #region StealMethods
        //==========
        // Steal Functions
        //==========

        public BBaller CanSteal()
        {
            if (onStealTimer)
                return null;

            BBaller otherBaller = null;
            foreach (BBaller testBaller in MyTeam.otherTeam.bballers) //scan other team for ball carrier
            {
                if (testBaller.HasBall)
                {
                    otherBaller = testBaller;
                    Vector3 disVector = MyTransform.position - otherBaller.MyTransform.position;
                    if (disVector.sqrMagnitude < Mathf.Pow(stealRadius, 2) && CheckStealPositioning(otherBaller)) //check if close enough to steal
                    {
                        return otherBaller;
                    }else
                    {
                        break;
                    }
                }
            }

            return null;
        }

        public void Steal()
        {
            //BBaller otherBaller = CanSteal();
            //if (otherBaller != null)
            if(ballerAI.GetOpponent().HasBall)
            {
                Debug.Log("Attempt steal - " + firstName);
                ballerAI.midAction = true;
                ChangeState(BBallerStateMachine.Steal);
               // faceDirection = otherBaller.MyTransform.position - MyTransform.position;
                faceDirection.y = 0;
                faceDirection.Normalize();
                lastForward = MyTransform.forward;
                turnPercent = 0.25f;
                StartCoroutine(WaitForStealAnim());
                //FinishSteal();
           }
           else
           {
               // Debug.Log("Steal: Other baller = null - " + firstName);
                ChangeState(BBallerStateMachine.Guard);
                ballerAI.AIGuard();
                ballerAI.midAction = false;
            }
        }

        IEnumerator WaitForStealAnim()
        {
            yield return new WaitForSeconds(0.7f);
            FinishSteal();

        }

        public void FinishSteal()
        {
			BBaller otherBaller = CanSteal();
            if (otherBaller == null ||
               otherBaller.ballerAI.stateMachine.CurrentState == BBallerStateMachine.Shoot ||
               otherBaller.ballerAI.stateMachine.CurrentState == BBallerStateMachine.Pass)
            {
                ballerAI.midAction = false;
            }
            else
            {
                float stealChance = CalcStealChance(otherBaller);
                float stealValue = UnityEngine.Random.value * 100;
                //Debug.Log("chance = " + stealChance);
                //Debug.Log("value = " + stealValue);
                if (stealValue < stealChance)
                {
                    //this.skinAndData.ballerData.item.TakeEvent(ItemEvent.stealMade);

                    // Swap values so that this teams switch offense/defense
                    otherBaller.HasBall = false;
                    this.HasBall = true;
                    ballerAI.midAction = false;
                    MyTeam.setAsPlayer(this);
                    MyTeam.isOffense = true;
                    MyTeam.otherTeam.isOffense = false;
                    Scoreboard.shotTime = 45.0f;
                    //Debug.Log("Steal Attempt: Successful");
                    UpdateMojo(MojoUpdates.STEAL, true);
                }
                else
                {
                    //Debug.Log("Steal Attempt: Failed");
                    //UpdateMojo(MojoUpdates.STEAL, false); //TODO: apparently we shouldn't update mojo since steal/false value is 0

                    //this.skinAndData.ballerData.item.TakeEvent(ItemEvent.stealFail);
                }

            }

            ballerAI.midAction = false;

            // Disable steal for a time so it cannot be spammed
            onStealTimer = true;
            StartCoroutine(StealTimer(stealCooldown));
        }

        public float CalcStealChance(BBaller otherBaller)
        {
            float distanceValue = (otherBaller.MyTransform.position - MyTransform.position).sqrMagnitude;
            float stealBase = playerStat.LatestStatList[PlayerStats.STEAL];
            if (stealBase == 0.0f) stealBase = 1; //arbitrary, since PlayerStats are not set up
            float ballHandleBase = otherBaller.playerStat.LatestStatList[PlayerStats.HANDLING];
            if (ballHandleBase == 0.0f) ballHandleBase = 1; //arbitrary, since PlayerStats are not set up
            return (100 - distanceValue) * (stealBase / ballHandleBase);
        }

        bool CheckStealPositioning(BBaller ballerToStealFrom)
        {
            // Check to see if ballers are facing opposite directions
            Vector3 victimForward = ballerToStealFrom.transform.forward;
            victimForward.Normalize();

            Vector3 myForward = transform.forward;
            myForward.Normalize();

            float facingDotProd = Vector3.Dot(victimForward, myForward);
            if (facingDotProd > 0)
                return false;

            // Check to see if I am in front of other baller?
            //Vector3 VictimToBasket = ballerToStealFrom.MyTeam.basketTarget.position - transform.position;
            //Vector3 MeToBasket = 
            //float inFrontDotProd = Vector3.Dot()


            return true;
        }

        IEnumerator StealTimer(float time)
        {
            yield return new WaitForSeconds(time);

            // Reenable Steal for this baller
            onStealTimer = false;
        }

        #endregion

        #region BlockingMethods
        //==========
        // Block Functions
        //==========
        public void BlockShot()
        {
            BBaller otherBaller = CanSteal();
            if (otherBaller != null)
            {
                ballerAI.midAction = true;
                ChangeState(BBallerStateMachine.Block);
                faceDirection = otherBaller.MyTransform.position - MyTransform.position;
                faceDirection.y = 0;
                faceDirection.Normalize();
                lastForward = MyTransform.forward;
                turnPercent = 0.25f;
                Debug.Log("Attempting Block");
                attemptingBlock = true;
                StartCoroutine(BlockTimer(1.3f));
            }
            else
            {
                // If no ballers within range, just default to guard
                Debug.Log("BlockShot: Other baller = null - " + firstName);
                ChangeState(BBallerStateMachine.Guard);
                ballerAI.AIGuard();
                ballerAI.midAction = false;
            }
        }

        IEnumerator BlockTimer(float time)
        {
            yield return new WaitForSeconds(time);
            if (attemptingBlock)
            {
                FinishBlock();
            }
        }

        public void FinishBlock()
        {
            if (!attemptingBlock)
            {
                return;
            }
            BBaller otherBaller = CanSteal();
            if (otherBaller != null && otherBaller.HasBall && otherBaller.ballerAI.stateMachine.CurrentState == BBallerStateMachine.Shoot)
            {
                Debug.Log("Defender Calculating Block");
                float blockChance = CalcBlockChance(this, otherBaller);
                Debug.Log("Block chance: " + blockChance);
                if ((UnityEngine.Random.value * 100) < blockChance)
                {
                    //Debug.Log("BlockSuccesful");
                    //this.skinAndData.ballerData.item.TakeEvent(ItemEvent.blockMade);
                    otherBaller.HasBall = false;
                    this.HasBall = true;
                    MyTeam.setAsPlayer(this);

                    Debug.Log("Block Attempt: Successful");
                    UpdateMojo(MojoUpdates.BLOCK);
                    ballerAI.midAction = false;
                    ChangeState(BBallerStateMachine.Idle_NoBall);
                    attemptingBlock = false;
                }
                else
                {
                    //Debug.Log("Block Attempt: Failed");
                    ballerAI.midAction = false;
                    ChangeState(BBallerStateMachine.Idle_NoBall);
                    attemptingBlock = false;
                }
            }
            else
            {
                //Debug.Log("Can't Block, opponent has finished action");
                ballerAI.midAction = false;
                ChangeState(BBallerStateMachine.Idle_NoBall);
                attemptingBlock = false;
            }
        }

        public float CalcBlockChance(BBaller defender, BBaller shooter)
        {
            Debug.Log("CALCULATING BLOCK CHANCE");
            float heightDifference = (defender.MyTransform.position.y - shooter.MyTransform.position.y) * .3048f;
            Vector3 adjustedShooterPos = shooter.MyTransform.position + (shooter.MyTransform.forward * .25f);
            float distanceValue = 1 - ((defender.MyTransform.position - adjustedShooterPos).magnitude * .3084f);
            if (distanceValue < 0)
                return 0;
            if (heightDifference > 1)
                heightDifference = 1;
            //float blockBase = defender.skinAndData.ballerData.getBlock();
            float blockBase = defender.playerStat.LatestStatList[PlayerStats.BLOCK];
            //float ballHandleBase = shooter.skinAndData.ballerData.getHandling();
            float ballHandleBase = shooter.playerStat.LatestStatList[PlayerStats.HANDLING];
            Debug.Log("Height Difference: " + heightDifference);
            Debug.Log("Distance Value: " + distanceValue);
            Debug.Log("Block Base: " + blockBase);
            Debug.Log("Ball Handle Base: " + ballHandleBase);
            return ((((distanceValue + heightDifference) / 2) * 100) * blockBase) - ballHandleBase;
        }
        #endregion

        #region InterceptMethods
        //==========
        // Intercept Functions
        //==========
        public void Intercept()
        {
            BBaller otherBaller = CanSteal();
            if (otherBaller != null)
            {
                ballerAI.midAction = true;
                ChangeState(BBallerStateMachine.Intercept);
                faceDirection = otherBaller.MyTransform.position - MyTransform.position;
                faceDirection.y = 0;
                faceDirection.Normalize();
                lastForward = MyTransform.forward;
                turnPercent = 0.25f;
                Debug.Log("Attempting Intercept");
                attemptingIntercept = true;
                StartCoroutine(InterceptTimer(1.3f));
            }
        }

        IEnumerator InterceptTimer(float time)
        {
            yield return new WaitForSeconds(time);
            if (attemptingIntercept)
            {
                FinishIntercept();
            }
        }

        public void FinishIntercept()
        {
            if (!attemptingIntercept)
            {
                return;
            }
            BBaller otherBaller = CanSteal();
            if (otherBaller != null && otherBaller.HasBall && otherBaller.ballerAI.stateMachine.CurrentState == BBallerStateMachine.Pass)
            {
                Debug.Log("Defender Calculating Intercept");
                float interceptChance = CalcInterceptChance(this, otherBaller);
                Debug.Log("Intercept chance: " + interceptChance);
                if ((UnityEngine.Random.value * 100) < interceptChance)
                {
                    otherBaller.HasBall = false;
                    this.HasBall = true;
                    MyTeam.setAsPlayer(this);

                    Debug.Log("Intercept Attempt: Successful");
                    UpdateMojo(MojoUpdates.STEAL);
                }
                else
                {
                    Debug.Log("Intercept Attempt: Failed");

                }
            }
            else
            {
                Debug.Log("Can't Intercept, opponent has finished action");
                this.attemptingIntercept = false;
            }
        }

        public float CalcInterceptChance(BBaller defender, BBaller passer)
        {
            Debug.Log("CALCULATING INTERCEPT CHANCE");
            Vector3 adjustedShooterPos = passer.MyTransform.position + (passer.MyTransform.forward * .25f);
            float distanceValue = 1 - ((defender.MyTransform.position - adjustedShooterPos).magnitude * .3084f);
            if (distanceValue < 0)
                return 0;
            float interceptBase = defender.playerStat.LatestStatList[PlayerStats.STEAL];
            float ballHandleBase = passer.playerStat.LatestStatList[PlayerStats.PASS];
            Debug.Log("Distance Value: " + distanceValue);
            Debug.Log("Intercept Base: " + interceptBase);
            Debug.Log("Ball Handle Base: " + ballHandleBase);
            return (((distanceValue) * 100) * interceptBase) - ballHandleBase;
        }
        //        public bool attemptingIntercept()
        //        {
        //            return ballerAI.getCurrentState().StateName == "INTERCEPT";
        //        }
        #endregion

        #region MojoUpdates
        //==========
        // Mojo Update
        //==========

        //		private void checkMojoThreshold(BBaller eventBaller)
        //		{
        //			if (eventBaller != this)
        //				return;
        //
        //			if(mojoLevel >= MyTeam.MojoThreshold)
        //			{
        //				mojoLevel = MyTeam.MojoThreshold;
        //
        //			}
        //
        //		}

        #endregion

        #region MiscellaneousHelperMethods
        //==========
        // Misc & Helper Functions
        //==========

        private void switchPossession(BBaller newDribbler)
        {
            this.HasBall = false;
            newDribbler.HasBall = true;
            MyTeam.otherTeam.setAsPlayer(newDribbler);
        }

        // Data from Formulas Excel Sheet
        private int getMojoValue(MojoUpdates updateType, bool isSuccessful)
        {
            int mojoValue = 0;

            switch (updateType)
            {
                case MojoUpdates.BLOCK:
                    mojoValue = (isSuccessful) ? 2 : 0;
                    break;

                case MojoUpdates.DUNK:
                    mojoValue = (isSuccessful) ? 2 : 1;
                    break;

                case MojoUpdates.FREETHROW:
                    mojoValue = (isSuccessful) ? 2 : 1;
                    break;

                case MojoUpdates.REBOUND:
                    mojoValue = (isSuccessful) ? 2 : 0;
                    break;

                case MojoUpdates.SHOOT:
                    mojoValue = (isSuccessful) ? 2 : 1;
                    break;

                case MojoUpdates.STEAL:
                    mojoValue = (isSuccessful) ? 2 : 0;
                    break;
            }

            return mojoValue;
        }

        public void ActionEnd()
        {
            ballerAI.midAction = false;
        }

        public Transform getDefensePos()
        {
            return DefensePos.getBestDefensePos();
        }

        private void updateFatigueActivation()
        {
            if (HasBall)
            {
                fatigueActive = true;
            }
            else
            {
                fatigueActive = false;
            }
        }

        private void onBallPossessionChange()
        {
            updateFatigueActivation();

            //CurrentState.ResetAnimation(this);
        }

        private void getCourtSafePosition(ref Vector3 position)
        {
            //Vector3 courtLimits = GameAdmin.CourtLimits;

            //if ((position.x > courtLimits.x) && (courtLimits.x > 0))
            //{
            //    position.x = courtLimits.x - 0.5f;
            //}

            //if ((position.x < courtLimits.x) && (courtLimits.x < 0))
            //{
            //    position.x = courtLimits.x + 0.5f;
            //}

            //if ((position.z > courtLimits.z) && (courtLimits.z > 0))
            //{
            //    position.z = courtLimits.z - 0.5f;
            //}

            //if ((position.z < courtLimits.z) && (courtLimits.z < 0))
            //{
            //    position.z = courtLimits.z + 0.5f;
            //}
        }

        private void getBallFacingPosition(ref Vector3 position)
        {
            Vector3 lhs = MyTransform.position - position;
            Vector3 rhs = MyTeam.HasBall() ? MyTeam.getBallerWithBall().MyTransform.position - position :
                MyTeam.otherTeam.getBallerWithBall().MyTransform.position - position;

            if (Vector3.Dot(lhs, rhs) < 0.0f)
            {

            }
        }

        /// <summary>
        /// Returns a Position between pos1 and pos2. Closer to pos2.
        /// </summary>
        /// <returns>The a Vector3 position.</returns>
        /// <param name="pos1">Pos1.</param>
        /// <param name="pos2">Pos2.</param>
        public Vector3 PositionBetween(Vector3 pos1, Vector3 pos2)
        {
            Vector3 intendedPosition = Vector3.zero;

            if (!ballerAI.midAction)
            {
                intendedPosition = Vector3.Lerp(pos1, pos2, UnityEngine.Random.Range(0.5f, 0.66f));
                intendedPosition.y = 0;
            }

            return intendedPosition;
        }

        /// <summary>
        /// Returns a Position between pos1 and pos2. Closer to pos2.
        /// </summary>
        /// <returns>The a Vector3 position.</returns>
        /// <param name="pos1">Pos1.</param>
        /// <param name="pos2">Pos2.</param>
        public Vector3 PositionInRangeOf(Vector3 pos, float distanceRange)
        {
            Vector3 intendedPosition = Vector3.zero;

            if (!ballerAI.midAction)
            {
                float x = UnityEngine.Random.Range(pos.x - distanceRange, pos.x + distanceRange);
                float z = UnityEngine.Random.Range(pos.z - distanceRange, pos.z + distanceRange);

                intendedPosition.Set(x, 0f, z);



                //intendedPosition.y = 0;
            }

            return intendedPosition;
        }

        #endregion

        #endregion
    }
		

}