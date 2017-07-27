using UnityEngine;
using System.Collections;

namespace WSBB
{
	public class CameraArm : MonoBehaviour {
		[HideInInspector]public Transform myTransform;
		[HideInInspector]public Team playerTeam = null;
		private Team_Manager teamManager = null;

		public Vector3 myTarget;
		public float xLimit;
		public float zLimit;
		public float xRange;
		public float zRange;
		public float xCurrent;
		public float zCurrent;
		public float springTension;
		private float fullSpringTension;
		
		public void Awake ()
		{
			myTransform = this.transform;
			setupPlayerTeam();

			fullSpringTension = springTension;
		}

		private void setupPlayerTeam()
		{
			if (teamManager == null)
			{
				teamManager = GameObject.Find("Team_Manager").GetComponent<Team_Manager>();
			}

			if (teamManager != null)
			{
				playerTeam = (teamManager.homeTeam.pureAI) ? teamManager.awayTeam : teamManager.homeTeam;
			}
		}


		// Use this for initialization
		public void Start () {
		
		}
		
		// Update is called once per frame
		public void Update () {
			if(springTension != fullSpringTension)
			{
				springTension = Mathf.Lerp(springTension, fullSpringTension, 4 * Time.deltaTime);
				if(Mathf.Abs(fullSpringTension - springTension) < .1)
					springTension = fullSpringTension;
			}
			
			if(playerTeam.HasBall())
			{
				myTarget = playerTeam.getPlayerBaller().MyTransform.position;
			}
			else if(playerTeam.otherTeam.HasBall())
			{
				myTarget = playerTeam.otherTeam.getBallerWithBall().MyTransform.position;
			}
			else
			{
				myTarget = Ball.currentPosition;
				//Debug.Log("Following Ball");
			}
			
			xCurrent = myTarget.x;
			zCurrent = myTarget.z;
			if(xCurrent > xLimit)
			{
				xCurrent = xLimit;
			}
			else if(xCurrent < -xLimit)
			{
				xCurrent = -xLimit;
			}
			
			if(zCurrent > zLimit)
			{
				zCurrent = zLimit;
			}
			else if(zCurrent < -zLimit)
			{
				zCurrent = -zLimit;
			}
			
			myTransform.position = Vector3.Lerp(myTransform.position, new Vector3(xRange * (xCurrent / xLimit), 0, zRange * (zCurrent / zLimit)), Time.deltaTime * springTension);	
		}
	}
}