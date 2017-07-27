using UnityEngine;
using System.Collections;
using WSBB.StateMachineBase;

namespace WSBB {
	public class Ball : MonoBehaviour, ISMObject {

		public BallStateMachine stateMachine;
		public static Vector3 currentPosition;
		public Transform myTransform;
		public Transform startTrans;
		public Transform endTrans;
		public bool useTrans;
		public bool useEndTrans;
		public Vector3 start;
		public Vector3 end;
		public float percent;
		public float shotHeight;
		public float idealTimeInAir;
		public float timeInAir;
		public bool run;
		public Team teamToScoreFor;
		public int pointsToScore;
		public BBaller targetBaller;
		public bool shot;
		public bool rebound;
		public bool pass;
		public bool alertReciever = false;

		#region ISMObject functions
		public void Initialize(StateMachineBase.StateMachine stateMachine) {}
		
		// Update is called once per frame
		public void Update () {

            if (GameAdmin.GamePause)
                return;

			currentPosition = myTransform.position;
			if(run) {
				if(useTrans) {
					start = startTrans.position;
					end = endTrans.position;
				} else if(useEndTrans) {
					end = endTrans.position;
				}


                timeInAir += Time.deltaTime;
                percent = timeInAir / idealTimeInAir;
				if(alertReciever && percent > .25 && pass) {
					//Debug.Log("Reciever Alerted");
					targetBaller.PrepareToRecieveBall(myTransform.position);
					alertReciever = false;
				}
				if(percent > 1) {
					ReachDestination();
				}
				
				float ballArchPercent ;
				ballArchPercent = (float)((-4.0 * percent * percent) + (4.0 * percent));

                myTransform.position = Vector3.Lerp(start, end, percent);
				myTransform.position = new Vector3(myTransform.position.x,myTransform.position.y+ (shotHeight * ballArchPercent),myTransform.position.z);
//				myTransform.position.Set(myTransform.position.x,
//				                         myTransform.position.y
//				                         + (shotHeight * ballArchPercent),
//				                         myTransform.position.z);
				//Debug.Log(start +":" + end + ":" + shotHeight + ":" + ballArchPercent);
				timeInAir += Time.deltaTime;
			}
		}

		public bool ChangeState(State newState) {
			return true;
		}
		public void PlayAnimation(string animationName, float fadeTime) {}
		public void Cleanup() {}
		#endregion

		void Awake() {
            myTransform = this.transform;
            myTransform.position = Vector3.zero;
            currentPosition = myTransform.position;
			stateMachine = new BallStateMachine(this);
			stateMachine.Initialize();
		}

		// Use this for initialization
		void Start () {
			
		}

		public void SetUpVector(Vector3 newStart, Vector3 newEnd, float archHeight) {
			start = newStart;
			end = newEnd;
			useTrans = false;
			useEndTrans = false;
			shotHeight = archHeight;			
		}
		
		public void SetUpTrans(Transform newStart, Transform newEnd, float archHeight) {
			startTrans = newStart;
			start = newStart.position;
			endTrans = newEnd;
			useTrans = false;
			useEndTrans = true;
			shotHeight = archHeight;
		}
		
		public void Shoot() {
			run = true;
			shot = true;
			pass = false;
			percent = 0.0f;
			timeInAir = 0.0f;
            idealTimeInAir = 1.8f; // sounds ok? -- Adam

		}
		
		public void Pass() {
			run = true;
			pass = true;
			shot = false;
			rebound = false;
			percent = 0.0f;	
			timeInAir = 0.0f;
            idealTimeInAir = 0.7f;
        }

        public void ReachDestination() {
            //Debug.Log("Ball has reached destination");
			if(shot) {
				if(rebound) {
					SetUpTrans(myTransform, targetBaller.MyTransform, 2);
					Pass();
					//UpdateMojo for Rebound Success
					targetBaller.UpdateMojo(MojoUpdates.REBOUND);
					Scoreboard.shotTime = 45;
					return;
				} else {
					teamToScoreFor.Score(pointsToScore);
                    teamToScoreFor.resetDefense();
                    teamToScoreFor.otherTeam.resetOffense();
					Scoreboard.shotTime = 45;
					GameAdmin.instance.state = PauseState.ScoreInfo;
					GameAdmin.GamePause = true;
				}
			} else if(pass) {
				targetBaller.RecieveBall();
			}
			Destroy(this.gameObject);
		}
	}
}