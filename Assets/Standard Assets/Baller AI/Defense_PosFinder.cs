using UnityEngine;
using System.Collections;

namespace WSBB {
	public class Defense_PosFinder : MonoBehaviour {
		[HideInInspector]public Transform myTransform;
		[HideInInspector]public Transform defensePos;
		[HideInInspector]public Transform defensePosLeft;
		[HideInInspector]public Transform defensePosRight;

		[HideInInspector]public BBaller bballer = null;
		public float adjustSpeed;

		private BBaller opponentBaller = null;

		[HideInInspector]public bool EnableDebugMsgs = true;

		void Awake() {
			myTransform = this.transform;
			defensePos = this.transform.Find("DefensePos");
			defensePosLeft = this.transform.Find("DefensePos_Left");
			defensePosRight = this.transform.Find("DefensePos_Right");

			bballer = GetComponentInParent<BBaller>();


			//IEnumerator init = initAssignments();
			StopCoroutine("initAssignments");
			//StartCoroutine(init);
			StartCoroutine("initAssignments");

			myTransform.parent = null;
		}

		private IEnumerator initAssignments()
		{
			while (bballer.MyTeam == null)
			{
				yield return null;
			}

			if (bballer != null)
			{
				int index = bballer.MyTeam.getBallerTeamIndex(bballer);
                //Debug.Log(index);
                if(index != -1) //TEMP FIX... Need to find out why players are being assigned id of -1
				    opponentBaller = bballer.MyTeam.otherTeam.bballers[index];
			}
			else if(EnableDebugMsgs)
				Debug.LogError("Defense_PosFinder.cs: Valid BBaller not found in Parent");

			//yield break;

		}

		// Use this for initialization
		void Start () {
		
		}
		
		// Update is called once per frame
		void Update () {
			Vector3 forVec;

			//Resolve this better
			if (bballer == null)
				return;

			if(bballer.HasBall) {
				Vector3 hoopPos = bballer.MyTeam.basketTarget.position;
				hoopPos.y = 0;
				forVec = myTransform.forward;
				myTransform.LookAt(hoopPos);
				myTransform.forward = Vector3.Slerp(forVec, myTransform.forward, adjustSpeed * Time.deltaTime);
			} else {
				if(bballer.MyTeam.HasBall()) {
					BBaller ballerWithBall = bballer.MyTeam.getBallerWithBall();
					Vector3 ballerPos = ballerWithBall.MyTransform.position;
					ballerPos.y = 0;
					forVec = myTransform.forward;
					myTransform.LookAt(ballerPos);
					myTransform.forward = Vector3.Slerp(forVec, myTransform.forward, adjustSpeed * Time.deltaTime);
				}
			}
			myTransform.position = bballer.MyTransform.position;
		}
		
		public Transform getBestDefensePos() {
			if(bballer.HasBall) {
				return defensePos;
			} else {
				Vector3 lineToHoop = bballer.MyTeam.basketTarget.position - myTransform.position;
				lineToHoop.y = 0;
				if(Vector3.Dot(lineToHoop, myTransform.right) > 0) {
					return defensePosRight;
				} else {
					return defensePosLeft;
				}
			}
		}
		
		public void OnDrawGizmos() {
			if(bballer!=null && (bballer.MyTeam).HasBall()) {
				Gizmos.color = Color.white;
				Gizmos.DrawLine(myTransform.position, defensePos.position);
				Gizmos.DrawLine(myTransform.position, defensePosLeft.position);
				Gizmos.DrawLine(myTransform.position, defensePosRight.position);
				Gizmos.DrawWireSphere(getBestDefensePos().position, 1);
			}
		}
	}
}