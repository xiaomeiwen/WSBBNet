using UnityEngine;
using System;
using System.Collections;
using WSBB.StateMachineBase;

namespace WSBB {
    //enum OffenseActions {Idle, Stand, Pass, Shoot, DrivePass, DriveShoot, DriveToRecieve}

    public class BallerAI : MonoBehaviour, ISMObject {
        //Variables for Logic
        public bool midAction;

        public BBaller parent;
        private BBaller opponent;
        public BBaller GetOpponent() { return opponent; }

        public WSBBAI decisionAI;

        public WSBBAI.SuggestionName CurrentSuggestion;
        public WSBBAI.SuggestionName PreviousSuggestion;

        public State lastBallerState;
        private State lastOpponentState;

        public int myIndex;

        //temporary for debugging
        public float toGuardPosMag;
        public float toOppMag;

        //public float reactionTime;
        //public float minReactTime;
        //public float maxReactTime;

        //Variables for Movement
        //public Vector3 targetPos;

        //public float leftArchValue;
        //public float rightArchValue;
        public float areaPref;
        public float preference;

        public MoveType moveSpeed;

        public bool moveLocSet = false;
        public Vector3 MoveToLoc;

        //Variables for Standing/Waiting
        //public float standTime;
        //public float maxStand;
        //public float minStand;

        //public int movesBetweenStand;
        //public int maxMoves;
        //public int minMoves;

        private float stealDelay = 3.0f;
        private float stealTimer = 0.0f;

        public BBallerStateMachine stateMachine;

        #region ISMObject
        public void Initialize(WSBB.StateMachineBase.StateMachine stateMachine) {
            //Debug.Log("Initializing baller state machine");
            this.stateMachine = (BBallerStateMachine)stateMachine;
            //decisionAI = new WSBBAI(parent);
            //decisionAI = FindObjectOfType<WSBBAI>();
            decisionAI = gameObject.GetComponent<WSBBAI>();
            CurrentSuggestion = WSBBAI.SuggestionName.IDLE;
        }

        public void Update() {

            if (GameAdmin.GamePause /*|| parent.firstName != "ai1"*/)
                return;

            PreviousSuggestion = CurrentSuggestion;

            /* Update state machine */
            this.stateMachine.Update(this);

            //First see if a player is controlling this baller. 
            if (!parent.MyTeam.pureAI && parent == parent.MyTeam.getPlayerBaller()) {
                //Debug.Log("I'm under player control");

                if (parent.stagLerp)
                    ContinueAction();

                return;
            }

            if (midAction)
            {
                ContinueAction();
            }
            else
            {
                decisionAI.UpdateSuggestion(parent);
                CurrentSuggestion = decisionAI.GetCurrentPlayerSuggestion();

                //if(parent.firstName == "p3")
                 // Debug.Log(parent.firstName + ": " + CurrentSuggestion.ToString());

                //How much is position considered?  May want to ignore suggestions until baller is in ideal position.
                //Current plan to have Team choose positions

                switch (CurrentSuggestion)
                {
                    case WSBBAI.SuggestionName.BLOCK:
                        parent.BlockShot();
                        midAction = true;
                        break;
                    case WSBBAI.SuggestionName.CLOWN:
                        //Function implemented but contains no animation so exclude this for now
                        //midAction = true;
                        break;
                    case WSBBAI.SuggestionName.DUNK:
                        if (parent.CanDunk())
                        {
                            parent.Dunk();
                            midAction = true;
                        }
                        else
                        {
                            MoveTowardTargetBasket();
                        }
                        break;
                    case WSBBAI.SuggestionName.GUARD:
                        AIGuard();
                        break;
                    case WSBBAI.SuggestionName.IDLE:
                        //Need to discuss - seems as though there is no purpose for this (offense should remain open or move as default, defense should be guarding as default)
                        break;
                    case WSBBAI.SuggestionName.INTERCEPT:
                        //Need Function - might be implemented; needs testing
                        //midAction = true;
                        break;
                    case WSBBAI.SuggestionName.MOVE:
                        // Used on Offense to move to different locations on the court
                        AIMove();
                        break;
                    case WSBBAI.SuggestionName.PASS:
                        //parent.PassTheBall(); //Need to Discuss - Recipient not clearly set
                        parent.Pass();
                        midAction = true;
                        break;
                    case WSBBAI.SuggestionName.REBOUND:
                        //Need Function but seems as though it might not be needed
                        midAction = true;
                        break;
                    case WSBBAI.SuggestionName.RECEIVE:
                        //Receive();
                        //midAction = true;
                        break;
                    case WSBBAI.SuggestionName.REMAIN_OPEN:
                        parent.RemainOpen(opponent.MyTransform.position);
                        break;
                    case WSBBAI.SuggestionName.SHOOT:
                        parent.Shoot();
                        //midAction = true;
                        break;
                    case WSBBAI.SuggestionName.STAG_LEAP:
                        if (parent.CanStagLeap()) { 
                            parent.StagLeap();
                            midAction = true;
                        }
                        else
                        {
                            //Debug.Log(parent.name + " cant stag leap; moving closer to the basket");
                            //MoveTowardTargetBasket();
                            AIMove();
                        }
                        break;
                    case WSBBAI.SuggestionName.STEAL:
                        // Steal functionality will see if the baller can steal and then do so, otherwise it will simply continue to guard
                        parent.Steal(); 
                        break;
                    default:
                        Debug.Log("Invalid Suggestion Type: " + CurrentSuggestion.ToString());
                        break;
                }

                // Readjust y pos to 0 -- Not sure if this is needed
                Vector3 myPosition = parent.transform.position;
                if (myPosition.y != 0)
                    parent.transform.position.Set(myPosition.x, 0.0f, myPosition.z);

                //Readjust pitch and roll to be 0 -- Not sure if this is needed
                Quaternion myRotation = parent.transform.rotation;
                if (myRotation.y != 0.0f || myRotation.z != 0.0f)
                    parent.transform.rotation.Set(myRotation.x, 0.0f, 0.0f, myRotation.w);


                //midAction = false; //*** get rid of later ***//

                //Get current opponent based on proximity
                if (parent.MyTeam.isOffense)
                {
                    //opponent = parent.MyTeam.otherTeam.getClosestDefenseman(parent);

                }
                else
                {
                    //opponent = parent.MyTeam.otherTeam.getClosestOffenseman(parent);
                }
                opponent = parent.MyTeam.otherTeam.bballers[parent.MyIndex];

                lastBallerState = stateMachine.CurrentState;
                lastOpponentState = opponent.getCurrentState();
            }
        }

        void ContinueAction()
        {
            switch (CurrentSuggestion)
            {
                case WSBBAI.SuggestionName.DUNK:
                  //  if (parent.dunkLerp)
                        parent.continueDunk();
                    break;

                case WSBBAI.SuggestionName.STAG_LEAP:
                   // if (parent.dunkLerp)
                        parent.continueStagLeap();
                    break;
            }

        }

        public bool ChangeState(State newState) {
            //TODO: stateMachine is still fucking null

            bool returnResult = true;
            //make sure both states are both valid before attempting to 
            //call their methods
            //assert (true && true); //assert that the ScriptableObject.CreateInstance<State>s are valid player states


			// Old functions
//            if (newState == stateMachine.CurrentState) {
//                //Debug.Log("Baller State Change requested. But ScriptableObject.CreateInstance<State> (" + newState.StateName + ") " +
//                //	        "is same as Previous State (" + stateMachine.CurrentState.StateName + "). " +
//                //	        "Terminating Request ...");
//                returnResult = false;
//            } else {
//                //Debug.Log ("Process: Changing state to " + newState.StateName );
//                stateMachine.SetState(newState);
//                returnResult = true;
//            }
//			if (stateMachine.CurrentState != null)
//				stateMachine.CurrentState.Exit (stateMachine.Owner);


			// Switch the state every time manully, won't check if they are in the same state
			stateMachine.CurrentState = newState;
			stateMachine.CurrentState.Enter (stateMachine.Owner);
			//Debug.Log ("entering " + mState.StateName + " state!!");
			stateMachine.CurrentState.PlayAnimation (stateMachine.Owner);
			//stateMachine.SetState(newState);
            return returnResult;
        }

        public Animation animator;
        public string currentAnim;
        public void PlayAnimation(string animToPlay, float fadeTime) {
            if (animator == null) return;
            //Awake ();

            if (currentAnim != animToPlay)
            {
                currentAnim = animToPlay;
                parent.animator.CrossFade(animToPlay, fadeTime);
            }
        }

        public void Cleanup() {
            stateMachine.DeleteStates();
        }

        public State getCurrentState() {
            if (null == stateMachine) {
                return null;
            }
            return stateMachine.CurrentState;
        }
        #endregion


        //=============
        //Personal Note
        // Players position is determined by their place in the starter array
        // 0 = Point
        // 1 = Wing
        // 2 = Center
        //=============
        public int MyIndex() {
            return myIndex;
        }

        void Awake() {
            //stateMachine = new BBallerStateMachine(this);
            //stateMachine.Initialize();
            parent = gameObject.GetComponent<BBaller>();
            parent.initPlayerVars();
        }

        // Use this for initialization
        void Start() {
            //myIndex = parent.MyTeam.getBallerTeamIndex(parent);
            myIndex--;
            opponent = parent.MyTeam.otherTeam.bballers[myIndex];
            midAction = false;
        }

        //court side transitions for now
        public void AIMove()
        {
            ///////////// Jagged Movement Fix? ////////////////
            if (parent.MyTeam.isOffense)
            {
                //parent.MoveOffenseTeamAI(parent.MyTeam.positionPicker[myIndex].lastPos, MoveType.JOG); //was originally MoveType.RUN
                Vector3 lastPosition = parent.MyTeam.positionPicker[myIndex].lastPos;

                if (MoveToLoc == Vector3.zero || !moveLocSet)
                {
                    //Debug.Log(parent.firstName + " Picking a new position to MOVE to");
                    MoveToLoc = parent.MyTeam.positionPicker[myIndex + 1].PickPointRect(false);
                    parent.MoveToLoc = MoveToLoc;
                    parent.MoveToLoc.y = 0;
                    parent.MoveOffenseTeamAI(MoveToLoc, MoveType.JOG);
                    moveLocSet = true;
                }
                else
                    parent.MoveOffenseTeamAI(MoveToLoc, MoveType.JOG);

            }

            //// Old Movement functionality ( was creating jagged movement) -  // Adam
            #region OldCode
            /* if(parent.MyTeam.isOffense)
             {
                 //parent.MoveOffenseTeamAI(parent.MyTeam.positionPicker[myIndex].lastPos, MoveType.JOG); //was originally MoveType.RUN
                     Vector3 lastPosition = parent.MyTeam.positionPicker[myIndex].lastPos;

                 if (lastPosition == Vector3.zero)
                 {
                     //if(parent.MyTeam.pureAI)
                     //Debug.Log("my index: " + myIndex);
                     Debug.Log(parent.firstName + " Picking a new position to MOVE to");
                     parent.MoveOffenseTeamAI(parent.MyTeam.positionPicker[myIndex + 1].PickPointRect(false), MoveType.JOG);
                     moveLocSet = true;
                 }
                 else
                     parent.MoveOffenseTeamAI(lastPosition, MoveType.JOG);

             }*/
            /* else
             {
                 parent.MoveDefenseTeamAI(parent.MyTeam.positionPicker[myIndex].lastPos);
                 //parent.MoveDefenseTeamAI(parent.MyTeam.positionPicker[myIndex].RandomArchPos());
             }*/
            #endregion

        }

        void AIMoveTo(Vector3 targetLocation)
        {
            if (parent.MyTeam.isOffense)
            {
                parent.MoveOffenseTeamAI(targetLocation, MoveType.JOG);

            }
        }

        public void MoveTowardTargetBasket()
        {
            Vector3 basketPosition = parent.MyTeam.basketTarget.position;
            Vector3 toBasket = basketPosition - parent.transform.position;
            toBasket.Normalize();

            parent.IsMoveForward = true;
            parent.faceDirection = toBasket;
            parent.MoveState = MoveType.JOG;
            parent.ActualMoveSpeed = parent.jogSpeed;
            // Debug.Log("Move toward basket!");
            MoveToLoc = basketPosition;
            MoveToLoc.y = 0;
            parent.MoveToLoc = basketPosition;
            parent.MoveToLoc.y = 0;
            parent.Move();
        }

        void Receive()
        {
            GameObject ballGO = (GameObject)Instantiate(parent.MyTeam.BallPrefab, Vector3.zero, Quaternion.identity);
            Ball ball = (Ball)ballGO.GetComponent("Ball");

            if (ball.targetBaller == parent)
            {
                parent.RecieveBall();
            }
        }

        #region old code to keep

        //        public void PointGuardAI() {
        //            AIOffense(parent.MyTeam.positionPicker[0].lastPos, moveSpeed);
        //            if(!midAction) {
        //                if(stateMachine.CurrentState == BBallerStateMachine.Idle_NoBall) 
        ////				if(parent.MoveState == MoveType.NULL || 
        ////				   parent.GuardDir == GuardDirection.NULL || 
        ////				   parent.CurrentState == BallerStates.Idle.Instance) 
        ////				if(parent.AnimSystem.currentAnim == GameAnims.Stand_Ball ||
        ////				   parent.AnimSystem.currentAnim == GameAnims.Stand_NoBall	)
        //                {
        //                    if(movesBetweenStand > 0) {
        //                        movesBetweenStand --;
        //                        ArchStep();
        //                    } else if(standTime < 0) {
        //                        standTime = UnityEngine.Random.Range(minStand, maxStand);
        //                        movesBetweenStand = UnityEngine.Random.Range(minMoves,
        //                                                (int)(maxMoves + 0.999f));
        //                        ArchStep();
        //                    }
        //                    standTime -= Time.deltaTime;
        //                }
        //            }
        //        }

        //        public void ArchStep() {
        //            if(parent.MyTeam.positionPicker[0].StepArchPos(preference)) {
        //                if(preference != leftArchValue) {
        //                    if(UnityEngine.Random.value > .5) {
        //                        moveSpeed = MoveType.JOG;
        //                    } else {
        //                        moveSpeed = MoveType.WALK;
        //                    }
        //                }
        //                preference = leftArchValue;
        //            } else {
        //                if(preference != rightArchValue) {
        //                    if(UnityEngine.Random.value > .5) {
        //                        moveSpeed = MoveType.JOG;
        //                    } else {
        //                        moveSpeed = MoveType.WALK;
        //                    }
        //                }
        //                preference = rightArchValue;
        //            }
        //        }

        //        public void WingAI() {
        //            AIOffense(parent.MyTeam.positionPicker[1].lastPos, moveSpeed);
        //            if(!midAction) {
        //                if(stateMachine.CurrentState == BBallerStateMachine.Idle_NoBall) 
        ////				if(parent.MoveState == MoveType.NULL || 
        ////				   parent.GuardDir == GuardDirection.NULL || 
        ////				   parent.CurrentState == BallerStates.Idle.Instance)  
        ////				if(parent.AnimSystem.currentAnim == GameAnims.Stand_Ball ||
        ////				   parent.AnimSystem.currentAnim == GameAnims.Stand_NoBall	)
        //                {
        //                    if(movesBetweenStand > 0) {
        //                        movesBetweenStand --;
        //                        parent.MyTeam.positionPicker[1].PickPointRect(areaPref);
        //                    } else if(standTime < 0) {
        //                        parent.MyTeam.positionPicker[1].PickPointRect(areaPref);
        //                        standTime = UnityEngine.Random.Range(minStand, maxStand);
        //                        movesBetweenStand = UnityEngine.Random.Range(minMoves,
        //                                                        (int) (maxMoves + 0.999f));
        //                    }
        //                    standTime -= Time.deltaTime;
        //                }
        //            }
        //        }

        //        public void CenterAI() {
        //            AIOffense(parent.MyTeam.positionPicker[2].lastPos, moveSpeed);
        //            if(!midAction) {
        //                if(stateMachine.CurrentState == BBallerStateMachine.Idle_NoBall) 
        ////				if(parent.MoveState == MoveType.NULL || 
        ////				   parent.GuardDir == GuardDirection.NULL || 
        ////				   parent.CurrentState == BallerStates.Idle.Instance) 
        ////				if(parent.AnimSystem.currentAnim == GameAnims.Stand_Ball ||
        ////				   parent.AnimSystem.currentAnim == GameAnims.Stand_NoBall	)
        //                {
        //                    if(movesBetweenStand > 0) {
        //                        movesBetweenStand --;
        //                        parent.MyTeam.positionPicker[2].PickPointRect(areaPref);
        //                    } else if(standTime < 0) {
        //                        parent.MyTeam.positionPicker[2].PickPointRect(areaPref);
        //                        standTime = UnityEngine.Random.Range(minStand, maxStand);
        //                        movesBetweenStand = UnityEngine.Random.Range(minMoves,
        //                                                         (int)(maxMoves + .999));
        //                    }
        //                    standTime -= Time.deltaTime;
        //                }
        //            }
        //        }

        //        public void DefenseUpdate() {
        //            float xPosAbsSum = Mathf.Abs(opponent.MyTransform.position.x) + Mathf.Abs(parent.MyTeam.defenseTrans[myIndex].position.x);
        //            float xPosDifSum = Mathf.Abs(opponent.MyTransform.position.x + parent.MyTeam.defenseTrans[myIndex].position.x);
        //            if(xPosAbsSum != xPosDifSum) {
        //                AIGuard(parent.MyTeam.defenseTrans[myIndex].position, false);
        //            }

        //            AIGuard(opponent.getDefensePos().position, opponent.HasBall);	
        //            /*
        //                if(opponent.state == BallerState.Stand)
        //                {
        //                    if(lastOpponentState != BallerState.Stand)
        //                    {
        //                        campDelay = 0;
        //                        campStealDelay = Random.Range(minStealDelay, maxStealDelay);
        //                    }
        //                    campDelay += Time.deltaTime;

        //                }
        //                if(opponent.state == BallerState.Shot)
        //                {
        //                    parent.BlockShot();
        //                }
        //                if(campDelay >= campStealDelay)
        //                {
        //                    campDelay = 0;
        //                    campStealDelay = Random.Range(minStealDelay, maxStealDelay);
        //                    parent.Steal();
        //                }
        //            */
        //        }

        //#region StateSpecificMethods


        //#region MovementMethods
        ////==========
        //// Movement Methods
        ////==========

        //public void Run(Vector3 direct, float speedPercent) {
        //    if(!midAction) {
        //        parent.IsMoveForward = true;
        //        parent.faceDirection = Vector3.Lerp(parent.faceDirection, -direct, Time.deltaTime * 5);
        //        parent.SpeedPercent = speedPercent;
        //        if(speedPercent < parent.standThreshold) {
        //            //ChangeState(BallerStates.Idle.Instance);
        //            parent.MoveState = MoveType.NULL;
        //        } else if(speedPercent < parent.walkThreshold) {
        //            ChangeState(BBallerStateMachine.Move);
        //            parent.MoveState = MoveType.WALK;
        //        } else if(speedPercent < parent.jogThreshold) {
        //            ChangeState(BBallerStateMachine.Move);
        //            parent.MoveState = MoveType.JOG;
        //        } else /*Run*/ {
        //            ChangeState(BBallerStateMachine.Move);
        //            parent.MoveState = MoveType.RUN;
        //        }
        //    }
        //}

        ////public void AIOffense(Vector3 offensePos, MoveType moveState) {
        ////    if(!midAction) {
        ////        //if(offensePos == null) {		//Vector3 should not be equated to null
        ////        //TODO: See if this change introduces regression or not.
        ////        if(offensePos == Vector3.zero) {
        ////            ChangeState(BBallerStateMachine.Idle_Ball);
        ////            return;
        ////        }
        ////        var diffVector = offensePos - parent.MyTransform.position;
        ////        if(!parent.guardInPos) {
        ////            if(diffVector.sqrMagnitude < Mathf.Pow(parent.guardInPosRange, 2)) {
        ////                parent.guardInPos = true;
        ////            } else {
        ////                switch(moveState) {
        ////                case MoveType.RUN:
        ////                    parent.MoveState = MoveType.RUN;
        ////                    //							ActualMoveSpeed = runSpeed;
        ////                    break;
        ////                case MoveType.JOG:
        ////                    parent.MoveState = MoveType.JOG;
        ////                    //							ActualMoveSpeed = jogSpeed;
        ////                    break;
        ////                case MoveType.WALK:
        ////                    parent.MoveState = MoveType.WALK;
        ////                    //							ActualMoveSpeed = walkSpeed;
        ////                    break;
        ////                default:
        ////                    parent.MoveState = MoveType.NULL;
        ////                    //							ActualMoveSpeed = runSpeed;
        ////                    break;
        ////                }
        ////                parent.IsMoveForward = true;	
        ////                diffVector.Normalize();
        ////                parent.faceDirection = diffVector;
        ////            }
        ////        } else {
        ////            if(diffVector.sqrMagnitude  > Mathf.Pow(parent.guardStandRange, 2)) {
        ////                parent.guardInPos = false;
        ////            }
        ////            ChangeState(BBallerStateMachine.Idle_NoBall);

        ////            Vector3 faceDir;
        ////            if(parent.HasBall) {
        ////                faceDir = parent.MyTeam.basketTarget.position - parent.MyTransform.position;
        ////                faceDir.y = 0;
        ////                faceDir.Normalize();
        ////            } else {
        ////                faceDir = parent.MyTeam.getPlayerBaller().MyTransform.position - parent.MyTransform.position;
        ////                faceDir.y = 0;
        ////                faceDir.Normalize();
        ////            }
        ////            parent.faceDirection = faceDir;
        ////        }
        ////    }
        ////}
        #endregion

        //float  diffSqrd = 0f;
        //float inPosSqrd = 0f;
        //Vector3 diffVec;
        //Vector3 inPosVec;
        // public void AIGuard(Vector3 guardPos, bool opponentHasBall) {
        public void AIGuard() {

            // Find the position in between the offensive player and the basket to guard at (basically in front of the offense towards the basket)
            Vector3 ballerPos = parent.MyTransform.position;
            Vector3 opponentPos = opponent.MyTransform.position;
            Vector3 diffVec = parent.MyTeam.otherTeam.basketTarget.position - opponentPos;
            diffVec.y = 0;
            diffVec.Normalize();

            Vector3 toOpponent = opponentPos - ballerPos;
            toOpponent.y = 0;
            toOppMag = toOpponent.magnitude;

            Vector3 oppVelocity = opponent.GetComponentInParent<UnityEngine.AI.NavMeshAgent>().velocity;
            oppVelocity.Normalize();
            float oppMovementDot = Vector3.Dot(diffVec, oppVelocity);

            Vector3 guardPos;

            // If offense is moving towards the basket or stag leaping, guard further away to avoid ballers running into eachother
            if (oppMovementDot > 0 || opponent.stagLerp)
                guardPos = 3 * (diffVec * parent.DoughnutInnerRadius) + opponentPos;
            else
                guardPos = (diffVec * parent.DoughnutInnerRadius ) + opponentPos;


            parent.MoveToLoc = guardPos;


            if (!midAction) {
                Vector3 faceDir;
                Vector3 toGuardPos = guardPos - parent.MyTransform.position;
                toGuardPos.y = 0;
                toGuardPosMag = toGuardPos.magnitude;
                //diffVec = guardPos;
                //inPosVec = parent.MyTransform.position;
                //diffSqrd = diffVector.sqrMagnitude;
                //inPosSqrd = Mathf.Pow(parent.guardInPosRange, 2f);
				
                if(!parent.guardInPos) {
                    // Get defender out of the way so that offensive baller can keep running through
                    //if (toOpponent.magnitude <= 3.5f)
                    //{
                    //    //parent.MoveDirect = toGuardPos;
                    //    //parent.IsMoveForward = false;
                    //    parent.faceDirection = -toGuardPos;
                    //    parent.IsMoveForward = true;
                    //    ChangeState(BBallerStateMachine.Move);
                    //    parent.MoveState = MoveType.RUN;
                    //    parent.ActualMoveSpeed = parent.runSpeed;
                    //    parent.Move();
                    //    return;
                    //}

                    // Move to guard position if not there, otherwise guard my opponent
                    if (toGuardPosMag <= 2.0f/*parent.guardInPosRange*/) {
                        
                        parent.guardInPos = true;
                        ChangeState(BBallerStateMachine.Guard);
                        GetComponentInParent<UnityEngine.AI.NavMeshAgent>().Stop();
                    } else if(toGuardPos.magnitude <= 3.5f /*parent.guardShuffleRange*/) {
                        //MoveState = BallerState.Scuttle;
                        ChangeState(BBallerStateMachine.Guard);
                        parent.ActualMoveSpeed = parent.guardSpeed;
                        parent.IsMoveForward = false;
                        toGuardPos.Normalize();
                        parent.MoveDirect = toGuardPos;
                        faceDir = parent.MyTeam.otherTeam.getBallerWithBall().MyTransform.position - parent.MyTransform.position;
                        faceDir.y = 0;
                        faceDir.Normalize();
                        parent.faceDirection = faceDir;
                        if(Vector3.Dot(parent.MyTransform.right * -1, parent.MoveDirect) > .5f)
                            parent.GuardDir = GuardDirection.LEFT;
                        else if(Vector3.Dot(parent.MyTransform.right, parent.MoveDirect) > .5f)
                            parent.GuardDir = GuardDirection.RIGHT;
                        else if(Vector3.Dot(parent.MyTransform.forward, parent.MoveDirect) > 0f)
                            parent.GuardDir = GuardDirection.FORWARD;
                        else
                            parent.GuardDir = GuardDirection.BACK;
                        parent.Move();
                    } else {
                        ChangeState(BBallerStateMachine.Move);
                        parent.MoveState = MoveType.RUN;
                        parent.ActualMoveSpeed = parent.runSpeed;
                        parent.IsMoveForward = true;
                        toGuardPos.Normalize();
                        parent.faceDirection = toGuardPos;
                        parent.Move();
                    }
                } else {
                    //Debug.Log(parent.firstName + "in guard position!");
                    if(toGuardPos.magnitude > 2.0f/*parent.guardStandRange*/) {
                        parent.guardInPos = false;
                        return;
                    }
                    ChangeState(BBallerStateMachine.Idle_NoBall);
                    faceDir = parent.MyTeam.otherTeam.getBallerWithBall().MyTransform.position - parent.MyTransform.position;
                    faceDir.y = 0;
                    faceDir.Normalize();
                    parent.faceDirection = faceDir;
                }
            }
        }

        //Not Working Correctly yet  -- Adam
        Vector3 FindGuardPosition()
        {
            Vector3 ballerPos = parent.MyTransform.position;
            Vector3 opponentPos = opponent.MyTransform.position;
            Vector3 diffVecBasket = parent.MyTeam.otherTeam.basketTarget.position - opponentPos;
            diffVecBasket.Normalize();

            Vector3 diffVecDribbler = parent.MyTeam.otherTeam.getBallerWithBall().MyTransform.position - ballerPos;
            diffVecDribbler.Normalize();

            double angleBtwn = Vector3.Angle(diffVecBasket, diffVecDribbler);
            //float cosAngle = Vector3.Dot(diffVecBasket, diffVecDribbler);
            //double angleBtwn = Math.Cos((double)cosAngle);

            if (Vector3.Cross(diffVecBasket, diffVecDribbler).z < 0)
                angleBtwn = angleBtwn * -1;

            angleBtwn = angleBtwn / 2; //Half the angle to be halfway between basket and dribbler to have defender stay in the passing lane
            Vector3 turnAngle = Vector3.RotateTowards(diffVecBasket, diffVecDribbler, (float)angleBtwn, (float)angleBtwn);

            turnAngle.Normalize();
            Vector3 guardPos = (turnAngle * parent.DoughnutInnerRadius) + opponentPos;

            return guardPos;
        }

        bool BallerBlockingGuardPos(Vector3 guardPos)
        {
            Vector3 ballerPos = parent.MyTransform.position;
            Vector3 opponentPos = opponent.MyTransform.position;
            
            Vector3 oppTobasket = parent.MyTeam.otherTeam.basketTarget.position - opponentPos;
            Vector3 toOpp = opponentPos - ballerPos;
            Vector3 meToBasket = parent.MyTeam.otherTeam.basketTarget.position - ballerPos;
            Vector3 toGuardPos = guardPos - ballerPos;

            float dotProd = Vector3.Dot(toOpp, toGuardPos);

            // If I am behind my opponent and he is in the way, then don't move directly to the guard pos
            if (oppTobasket.sqrMagnitude < meToBasket.sqrMagnitude && dotProd > 0.6f)
                return true;

            return false;
        }

        Vector3 GoAroundOpponent(Vector3 guardPos)
        {
            //if(parent.firstName == "ai1")
                //Debug.Log("Guard - going around opponent as they are in the way");
            Vector3 ballerPos = parent.MyTransform.position;
            Vector3 opponentPos = opponent.MyTransform.position;

            if(opponentPos.z > guardPos.z)
            {
                guardPos = new Vector3(opponentPos.x, 0.0f, (guardPos.z - 4.0f));
            }
            else
            {
                guardPos = new Vector3(opponentPos.x, 0.0f, (guardPos.z + 4.0f));
            }

            Vector3 toGuardPos = guardPos - ballerPos;
            parent.faceDirection = toGuardPos;
            parent.IsMoveForward = true;
            ChangeState(BBallerStateMachine.Move);
            parent.MoveState = MoveType.JOG;
            parent.ActualMoveSpeed = parent.jogSpeed;
            parent.Move();

            return guardPos;
        }

        //#endregion
        #region more old code
        //#region ShootingMethods
        //public void Shoot() {
        //    if(!midAction && parent.HasBall) {
        //        Debug.Log("Take Shot");	
        //        ChangeState(BBallerStateMachine.Shoot);
        //        midAction = true;
        //        parent.ActualMoveSpeed = 0;
        //        parent.faceDirection = parent.MyTeam.basketTarget.position - parent.MyTransform.position;
        //        parent.faceDirection.y = 0;
        //        parent.faceDirection.Normalize();
        //        parent.lastForward = parent.MyTransform.forward;
        //        parent.turnPercent = 0;
        //    }
        //}

        //public void LaunchBallToBasket() {
        //    if(parent.midAction && parent.HasBall) {
        //        //checkToSee if there is a blocker Nearby and do Block Calculations
        //        BBaller blockingBaller = parent.MyTeam.otherTeam.getNearestBlockingBaller(parent.MyTransform.position);
        //        if(blockingBaller != null && blockingBaller.attemptingBlock) {
        //            Debug.Log("Shooter Calculating Block");
        //            float blockCalc = CalcBlockChance(blockingBaller, parent);
        //            Debug.Log("Block Calc");
        //            if((UnityEngine.Random.value * 100) < blockCalc) {
        //                //Debug.Log("BlockSuccesful");
        //                parent.HasBall = false;
        //                //this.SkinAndData.ballerData.item.TakeEvent(ItemEvent.shotFail);
        //                blockingBaller.HasBall = true;
        //                parent.MyTeam.otherTeam.setAsPlayer(blockingBaller);
        //                blockingBaller.attemptingBlock = false;
        //                return;
        //            } else {
        //                blockingBaller.attemptingBlock = false;
        //            }
        //        }


        //        //Prepare ball and make shot calculations
        //        Debug.Log("Ball Thrown");
        //        GameObject ballGO = (GameObject) Instantiate(parent.MyTeam.BallPrefab, Vector3.zero, Quaternion.identity);
        //        Ball ball = (Ball) ballGO.GetComponent("Ball");
        //        ball.SetUpVector(parent.myShotPoint.position, parent.MyTeam.basketTarget.position, 6);
        //        ball.teamToScoreFor = parent.MyTeam;
        //        ball.pointsToScore = 2;
        //        float shotChance = CalcShotChance();
        //        Debug.Log("Shot Chance: " + shotChance);
        //        float randomValue = UnityEngine.Random.value * 100;
        //        Debug.Log("Shot Random Value: " + randomValue);
        //        if(shotChance < 0 || randomValue > shotChance) {
        //            Debug.Log("Shot Is Going To Miss");
        //            //this.skinAndData.ballerData.item.TakeEvent(ItemEvent.shotFail);
        //            ball.rebound = true;
        //            int baller;
        //            Team reboundTeam;
        //            if(UnityEngine.Random.value >= .5) {
        //                reboundTeam = parent.MyTeam;
        //            } else {
        //                reboundTeam = parent.MyTeam.otherTeam;
        //            }
        //            baller = UnityEngine.Random.Range(0, reboundTeam.bballers.Length);

        //            if(baller == parent.MyTeam.bballers.Length)//Should never go in here.
        //            {
        //                baller = 0;
        //            }	
        //            ball.targetBaller = reboundTeam.bballers[baller];

        //            //Handle Mojo
        //            parent.UpdateMojo(MojoUpdates.SHOOT, false);
        //        } else {
        //            Debug.Log ("Shot Is Going To Be Made");

        //            //Handle Mojo
        //            parent.UpdateMojo(MojoUpdates.SHOOT, true);

        //            //this.SkinAndData.ballerData.item.TakeEvent(ItemEvent.shotMade);	
        //        }	
        //        ball.Shoot();
        //        parent.HasBall = false;
        //        parent.MyTeam.cameraArm.springTension *= .2f;
        //    }
        //}

        //public float CalcShotChance() {
        //    Vector3 vector = parent.myShotPoint.position - parent.MyTeam.basketTarget.position;
        //    vector.y = 0;
        //    //float shotBase = SkinAndData.ballerData.getShoot();
        //    float shotBase = parent.playerStat.LatestStatList[PlayerStats.SHOOT];
        //    float distanceToHoop = vector.magnitude * .3048f;
        //    float distanceValue = Mathf.Pow(distanceToHoop, (1 + (shotBase / 140)));
        //    return shotBase - distanceValue;
        //}

        //#endregion

        //#region DunkMethods
        ////==========
        ////Dunk Functions
        ////==========
        //public bool CanDunk() {
        //    if(parent.mojoLevel < 100)
        //        return false;
        //    Vector3 vector = parent.MyTransform.position - parent.MyTeam.basketTarget.position;
        //    float distanceToHoopSqr = vector.sqrMagnitude * .3048f;
        //    Debug.Log(distanceToHoopSqr + ":" + Mathf.Pow(GameData.instance.dunkRange, 2));

        //    if(distanceToHoopSqr > Mathf.Pow(GameData.instance.dunkRange, 2))
        //        return false;

        //    return true;
        //}

        //public void Dunk() {
        //    if(!CanDunk())
        //        return;
        //    Debug.Log("Dunking");
        //    ChangeState(BBallerStateMachine.Dunk);
        //    midAction = true;
        //    parent.ActualMoveSpeed = 0;
        //    parent.faceDirection = parent.MyTeam.basketTarget.position - parent.MyTransform.position;
        //    parent.faceDirection.y = 0;
        //    parent.faceDirection.Normalize();
        //    parent.lastForward = parent.MyTransform.forward;
        //    parent.turnPercent = 0;
        //}

        //public void DunkStartLerp(int lerpTime) {
        //    Debug.Log(parent.GetDunkName());
        //    AnimationClip animClip = animator[parent.GetDunkName()].clip;
        //    parent.dunkTimeTotal= lerpTime / animClip.frameRate;
        //    parent.dunkTime = 0;
        //    parent.dunkLerp = true;
        //    parent.dunkStartPos = parent.MyTransform.position;
        //}

        //public void scoreDunk() {
        //    Debug.Log("FAN FARE");

        //    if(parent.mojoLevel >= parent.MyTeam.MojoThreshold)
        //    {
        //        parent.mojoLevel = parent.MyTeam.MojoThreshold;
        //        BallerEvents.DunkCompletedWithMojo(parent.MyTeam);

        //    }
        //    else
        //    {
        //        BallerEvents.DunkCompleted(parent.MyTeam);
        //    }

        //    parent.UpdateMojo(MojoUpdates.DUNK, true);

        //}

        //public void endDunk() {
        //    parent.midAction = false;
        //    parent.MyTeam.Score(2);
        //    Scoreboard.shotClock = 45;
        //    GameAdmin.instance.state = PauseState.ScoreInfo;
        //    GameAdmin.GamePause = true;
        //    parent.dunkLerp = false;

        //}

        //#endregion

        //#region PassMethods
        ////==========
        //// Pass Functions
        ////==========

        //public void Pass() {
        //    if(!midAction && parent.HasBall && !parent.MyTeam.amICurrentBaller(this.parent)) {
        //        Debug.Log("Passing Ball");
        //        ChangeState(BBallerStateMachine.Pass);
        //        midAction = true;
        //        parent.ActualMoveSpeed = 0;
        //        parent.faceDirection = parent.MyTeam.getCurrentBaller().MyTransform.position - parent.MyTransform.position;
        //        parent.faceDirection.y = 0;
        //        parent.faceDirection.Normalize();
        //        parent.lastForward = parent.MyTransform.forward;
        //        parent.turnPercent = 0;
        //    }
        //}

        //public void PassTheBall() {
        //    if((UnityEngine.Random.value * 100) < CalcPassChance()) {
        //        Debug.Log("Ball Pass Would Succeed");
        //        //this.skinAndData.ballerData.item.TakeEvent(ItemEvent.passMade);
        //    } else {
        //        Debug.Log("Ball Pass Would Fail");
        //        //this.skinAndData.ballerData.item.TakeEvent(ItemEvent.passFail);
        //    }

        //    Debug.Log("Ball Thrown");
        //    GameObject ballGO = (GameObject) Instantiate(parent.MyTeam.BallPrefab, Vector3.zero, Quaternion.identity);
        //    Ball ball = (Ball) ballGO.GetComponent("Ball");
        //    ball.SetUpTrans(parent.myShotPoint, parent.MyTeam.getCurrentBaller().transform, 1);
        //    ball.targetBaller = parent.MyTeam.getCurrentBaller();
        //    ball.Pass();
        //    ball.alertReciever = true;
        //    parent.HasBall = false;
        //    parent.MyTeam.cameraArm.springTension *= .2f;
        //}
        //public float CalcPassChance()
        //{
        //    BBaller nearestDefender = parent.MyTeam.otherTeam.getNearestBaller(parent.MyTransform.position);
        //    //float passBase = skinAndData.ballerData.getPass();
        //    float passBase = parent.playerStat.LatestStatList[PlayerStats.PASS];
        //    float guardBase = nearestDefender.ballerStat.LatestStatList[PlayerStats.GUARD];
        //    float distanceValue = 1 - (parent.MyTransform.position - nearestDefender.MyTransform.position).magnitude * .3084f;
        //    if(distanceValue > 1) {
        //        return 1.0f;
        //    }
        //    return 100 - (distanceValue * 50) + passBase - (guardBase / 2);
        //}
        //#endregion

        //#region StealMethods
        ////==========
        //// Steal Functions
        ////==========

        //public BBaller CanSteal()
        //{
        //    BBaller otherBaller = null;
        //    foreach(BBaller testBaller in parent.MyTeam.otherTeam.bballers) {
        //        if(testBaller.HasBall) {
        //            otherBaller = testBaller;
        //        }
        //    }
        //    if(otherBaller != null) {
        //        Vector3 disVector = parent.MyTransform.position - otherBaller.MyTransform.position;
        //        if(disVector.sqrMagnitude < Mathf.Pow(parent.stealRadius, 2)) {
        //            return otherBaller;
        //        }
        //    }
        //    return null;
        //}

        //public void Steal() {
        //    BBaller otherBaller = CanSteal();
        //    if(otherBaller != null) {
        //        midAction = true;
        //        ChangeState(BBallerStateMachine.Steal);
        //        parent.faceDirection = otherBaller.MyTransform.position - parent.MyTransform.position;
        //        parent.faceDirection.y = 0;
        //        parent.faceDirection.Normalize();
        //        parent.lastForward = parent.MyTransform.forward;
        //        parent.turnPercent = 0.25f;
        //        Debug.Log("Attempting Steal");
        //    }	
        //}

        //public void FinishSteal() {
        //    BBaller otherBaller = CanSteal();

        //    if(otherBaller == null || 
        //       otherBaller.getCurrentState() == BBallerStateMachine.Shoot || 
        //       otherBaller.getCurrentState() == BBallerStateMachine.Pass) 
        //    {
        //        parent.midAction = false;
        //    } 
        //    else 
        //    {
        //        if((UnityEngine.Random.value * 100) < CalcStealChance(otherBaller)) 
        //        {
        //            //this.skinAndData.ballerData.item.TakeEvent(ItemEvent.stealMade);
        //            otherBaller.HasBall = false;
        //            parent.HasBall = true;
        //            parent.midAction = false;
        //            parent.MyTeam.setAsPlayer(this.parent);
        //            Scoreboard.shotClock = 45.0f;
        //            Debug.Log ("Steal Attempt: Successful");
        //            parent.UpdateMojo(MojoUpdates.STEAL, true);
        //        } 
        //        else 
        //        {
        //            Debug.Log ("Steal Attempt: Failed");
        //            parent.UpdateMojo(MojoUpdates.STEAL, false);

        //            //this.skinAndData.ballerData.item.TakeEvent(ItemEvent.stealFail);
        //        }

        //    }
        //}

        //public float CalcStealChance(BBaller otherBaller) {
        //    float distanceValue = 1 - ((otherBaller.MyTransform.position - parent.MyTransform.position).magnitude * 3.084f);
        //    if(distanceValue < 0)
        //        return 0.0f;
        //    float stealBase = parent.playerStat.LatestStatList[PlayerStats.STEAL];
        //    float ballHandleBase = otherBaller.ballerStat.LatestStatList[PlayerStats.HANDLING];
        //    return ((stealBase / 5) * distanceValue) - ballHandleBase;
        //}
        //#endregion

        //#region BlockingMethods
        ////==========
        //// Block Functions
        ////==========
        //public void BlockShot() {
        //    BBaller otherBaller = CanSteal();
        //    if(otherBaller != null) {
        //        midAction = true;
        //        ChangeState(BBallerStateMachine.Block);
        //        parent.faceDirection = otherBaller.MyTransform.position - parent.MyTransform.position;
        //        parent.faceDirection.y = 0;
        //        parent.faceDirection.Normalize();
        //        parent.lastForward = parent.MyTransform.forward;
        //        parent.turnPercent = 0.25f;
        //        //Debug.Log("Attempting Block");
        //        parent.attemptingBlock = true;
        //    }
        //}

        //public void FinishBlock() {
        //    if(!parent.attemptingBlock) {
        //        return;
        //    }
        //    BBaller otherBaller = CanSteal();
        //    if(otherBaller != null && otherBaller.HasBall && otherBaller.getCurrentState() == BBallerStateMachine.Shoot) {
        //        Debug.Log("Defender Calculating Block");
        //        float blockChance = CalcBlockChance(parent, otherBaller);
        //        Debug.Log("Block chance: " + blockChance);
        //        if((UnityEngine.Random.value * 100) < blockChance) {
        //            //Debug.Log("BlockSuccesful");
        //            //this.skinAndData.ballerData.item.TakeEvent(ItemEvent.blockMade);
        //            otherBaller.HasBall = false;
        //            parent.HasBall = true;
        //            parent.MyTeam.setAsPlayer(this.parent);

        //            Debug.Log("Block Attempt: Successful");
        //            parent.UpdateMojo(MojoUpdates.BLOCK);

        //        } else {
        //            //Debug.Log("Block Attempt: Failed");

        //            //this.skinAndData.ballerData.item.TakeEvent(ItemEvent.blockFail);
        //        }
        //    } else {
        //        //Debug.Log("Can't Block, opponent has finished action");
        //        parent.attemptingBlock = false;
        //    }
        //}

        //public float CalcBlockChance(BBaller defender, BBaller shooter) {
        //    Debug.Log("CALCULATING BLOCK CHANCE");
        //    float heightDifference = (defender.MyTransform.position.y - shooter.MyTransform.position.y) * .3048f;
        //    Vector3 adjustedShooterPos =  shooter.MyTransform.position + (shooter.MyTransform.forward * .25f);
        //    float distanceValue = 1 - ((defender.MyTransform.position - adjustedShooterPos).magnitude * .3084f);
        //    if(distanceValue < 0)
        //        return 0;
        //    if(heightDifference > 1)
        //        heightDifference = 1;
        //    //float blockBase = defender.skinAndData.ballerData.getBlock();
        //    float blockBase = defender.ballerStat.LatestStatList[PlayerStats.BLOCK];
        //    //float ballHandleBase = shooter.skinAndData.ballerData.getHandling();
        //    float ballHandleBase = shooter.ballerStat.LatestStatList[PlayerStats.HANDLING];
        //    Debug.Log("Height Difference: " + heightDifference);
        //    Debug.Log("Distance Value: " + distanceValue);
        //    Debug.Log("Block Base: " + blockBase);
        //    Debug.Log("Ball Handle Base: " + ballHandleBase);
        //    return ((((distanceValue + heightDifference) / 2) * 100) * blockBase) - ballHandleBase;
        //}
        //#endregion

        //#region MiscellaneousHelperMethods
        ////==========
        //// Misc & Helper Functions
        ////==========

        //// Data from Formulas Excel Sheet
        //public int getMojoValue(MojoUpdates updateType, bool isSuccessful)
        //{
        //    int mojoValue = 0;

        //    switch(updateType)
        //    {
        //    case MojoUpdates.BLOCK:
        //        mojoValue = (isSuccessful) ? 2 : 0 ;
        //        break;

        //    case MojoUpdates.DUNK:
        //        mojoValue = (isSuccessful) ? 2 : 1 ;
        //        break;

        //    case MojoUpdates.FREETHROW:
        //        mojoValue = (isSuccessful) ? 2 : 1 ;
        //        break;

        //    case MojoUpdates.REBOUND:
        //        mojoValue = (isSuccessful) ? 2 : 0 ;
        //        break;

        //    case MojoUpdates.SHOOT:
        //        mojoValue = (isSuccessful) ? 2 : 1 ;
        //        break;

        //    case MojoUpdates.STEAL:
        //        mojoValue = (isSuccessful) ? 2 : 0 ;
        //        break;
        //    }

        //    return mojoValue;
        //}

        //public void ActionEnd() {
        //    midAction = false;
        //}

        //private void updateFatigueActivation()
        //{
        //    if (parent.HasBall)
        //    {
        //        parent.fatigueActive = true;
        //    }
        //    else
        //    {
        //        parent.fatigueActive = false;
        //    }
        //}

        //public void onBallPossessionChange()
        //{
        //    updateFatigueActivation();

        //    /** TODO : Change state to something else */
        //}

        //#endregion

        #endregion to keep
    }
}