using UnityEngine;
using System.Collections;

public class TeamPlayerModelManagerCS : MonoBehaviour
{

		public GameObject slot1;
		public GameObject slot2;
		public GameObject slot3;
		GameObject left;
		GameObject mid;
		GameObject right;
		Vector3 destinyLeft;
		Vector3 destinyMid;
		Vector3 destinyRight;
		Vector3 destinyScaleLeft;
		Vector3 destinyScaleMid;
		Vector3 destinyScaleRight;

		float speed;
		float slideTime;
		float slideScaleTime;
		float minSpeed;
		float minScaleSpeed;

		// Use this for initialization
		void Start ()
		{
	
				speed = 1f;
				slideTime = 0.5f;
				slideScaleTime = 0.5f;
				minSpeed = 0.5f;
				minScaleSpeed = 0.2f;


				//init
				left = slot1;
				mid = slot2;
				right = slot3;

				//sort
				sortLeftToRight (ref left, ref mid, ref right);

				destinyLeft = new Vector3 (left.transform.position.x, left.transform.position.y, left.transform.position.z);
				destinyMid = new Vector3 (mid.transform.position.x, mid.transform.position.y, mid.transform.position.z);
				destinyRight = new Vector3 (right.transform.position.x, right.transform.position.y, right.transform.position.z);

				destinyScaleLeft = new Vector3 (left.transform.localScale.x, left.transform.localScale.y, left.transform.localScale.z);
				destinyScaleMid = new Vector3 (mid.transform.localScale.x, mid.transform.localScale.y, mid.transform.localScale.z);
				destinyScaleRight = new Vector3 (right.transform.localScale.x, right.transform.localScale.y, right.transform.localScale.z);


				

		}
	
		// Update is called once per frame
		void Update ()
		{


				// The step size is equal to speed times frame time.
				var step = speed * Time.deltaTime;
		
				// Move our position a step closer to the target.
				speed = (destinyLeft.sqrMagnitude - left.transform.position.sqrMagnitude) / slideTime;
				if (speed < 0)
						speed = -speed;
//				if (speed < minSpeed)
//						speed = minSpeed;

				step = speed * Time.deltaTime;
				left.transform.position = Vector3.MoveTowards (left.transform.position, destinyLeft, step);

				speed = (destinyMid.sqrMagnitude - mid.transform.position.sqrMagnitude) / slideTime;
				if (speed < 0)
						speed = -speed;
//				if (speed < minSpeed)
//					speed = minSpeed;
				step = speed * Time.deltaTime;
				mid.transform.position = Vector3.MoveTowards (mid.transform.position, destinyMid, step);

				speed = (destinyRight.sqrMagnitude - right.transform.position.sqrMagnitude) / slideTime;
				if (speed < 0)
						speed = -speed;
//				if (speed < minSpeed)
//					speed = minSpeed;
				step = speed * Time.deltaTime;
				right.transform.position = Vector3.MoveTowards (right.transform.position, destinyRight, step);

				//scale
				Vector3 tempDelta = new Vector3 (0f, 0f, 0f);

		tempDelta.x = (destinyScaleLeft.x - left.transform.localScale.x) / slideScaleTime * Time.deltaTime;
		tempDelta.y = (destinyScaleLeft.y - left.transform.localScale.y) / slideScaleTime * Time.deltaTime;
		tempDelta.z = (destinyScaleLeft.z - left.transform.localScale.z) / slideScaleTime * Time.deltaTime;
//				if (tempDelta.x < minScaleSpeed && tempDelta.x > 0)
//					tempDelta.x = minScaleSpeed;
//				if (tempDelta.y < minScaleSpeed && tempDelta.x > 0)
//					tempDelta.y = minScaleSpeed;
//				if (tempDelta.z < minScaleSpeed && tempDelta.x > 0)
//					tempDelta.z = minScaleSpeed;
//
//				if (tempDelta.x > -minScaleSpeed && tempDelta.x < 0)
//					tempDelta.x = -minScaleSpeed;
//				if (tempDelta.y > -minScaleSpeed && tempDelta.x < 0)
//					tempDelta.y = -minScaleSpeed;
//				if (tempDelta.z > -minScaleSpeed && tempDelta.x < 0)
//					tempDelta.z = -minScaleSpeed;
				left.transform.localScale += tempDelta;

		tempDelta.x = (destinyScaleRight.x - right.transform.localScale.x) / slideScaleTime * Time.deltaTime;
		tempDelta.y = (destinyScaleRight.y - right.transform.localScale.y) / slideScaleTime * Time.deltaTime;
		tempDelta.z = (destinyScaleRight.z - right.transform.localScale.z) / slideScaleTime * Time.deltaTime;
//				if (tempDelta.x < minScaleSpeed && tempDelta.x > 0)
//					tempDelta.x = minScaleSpeed;
//				if (tempDelta.y < minScaleSpeed && tempDelta.x > 0)
//					tempDelta.y = minScaleSpeed;
//				if (tempDelta.z < minScaleSpeed && tempDelta.x > 0)
//					tempDelta.z = minScaleSpeed;
//				
//				if (tempDelta.x > -minScaleSpeed && tempDelta.x < 0)
//					tempDelta.x = -minScaleSpeed;
//				if (tempDelta.y > -minScaleSpeed && tempDelta.x < 0)
//					tempDelta.y = -minScaleSpeed;
//				if (tempDelta.z > -minScaleSpeed && tempDelta.x < 0)
//					tempDelta.z = -minScaleSpeed;
				right.transform.localScale += tempDelta;

		tempDelta.x = (destinyScaleMid.x - mid.transform.localScale.x) / slideScaleTime * Time.deltaTime;
		tempDelta.y = (destinyScaleMid.y - mid.transform.localScale.y) / slideScaleTime * Time.deltaTime;
		tempDelta.z = (destinyScaleMid.z - mid.transform.localScale.z) / slideScaleTime * Time.deltaTime;
//				if (tempDelta.x < minScaleSpeed && tempDelta.x > 0)
//					tempDelta.x = minScaleSpeed;
//				if (tempDelta.y < minScaleSpeed && tempDelta.x > 0)
//					tempDelta.y = minScaleSpeed;
//				if (tempDelta.z < minScaleSpeed && tempDelta.x > 0)
//					tempDelta.z = minScaleSpeed;
//				
//				if (tempDelta.x > -minScaleSpeed && tempDelta.x < 0)
//					tempDelta.x = -minScaleSpeed;
//				if (tempDelta.y > -minScaleSpeed && tempDelta.x < 0)
//					tempDelta.y = -minScaleSpeed;
//				if (tempDelta.z > -minScaleSpeed && tempDelta.x < 0)
//					tempDelta.z = -minScaleSpeed;
				mid.transform.localScale += tempDelta;




//		mid.transform.localScale.x += (destinyScaleMid.x - mid.transform.localScale.x) / slideTime;
//		mid.transform.localScale.y += (destinyScaleMid.y - mid.transform.localScale.y) / slideTime;
//		mid.transform.localScale.z += (destinyScaleMid.z - mid.transform.localScale.z) / slideTime;
//
//		right.transform.localScale.x += (destinyScaleRight.x - right.transform.localScale.x) / slideTime;
//		right.transform.localScale.y += (destinyScaleRight.y - right.transform.localScale.y) / slideTime;
//		right.transform.localScale.z += (destinyScaleRight.z - right.transform.localScale.z) / slideTime;


	
		}

		void sortLeftToRight (ref GameObject left, ref GameObject mid, ref GameObject right)
		{
				GameObject temp;
		
				if (left.transform.position.x > mid.transform.position.x) {
						temp = left;
						left = mid;
						mid = temp;
				}
		
				if (mid.transform.position.x > right.transform.position.x) {
						temp = mid;
						mid = right;
						right = temp;
				} 
		
				if (left.transform.position.x > mid.transform.position.x) {
						temp = left;
						left = mid;
						mid = temp;
				}
		}

		void OnClick ()
		{
				//Debug.Log ("wuliya");
				//turnAntiClockwise ();
				setDestinyClockWise ();
		}

		void setDestinyClockWise ()
		{
				float temp;

				//position
				temp = destinyLeft.x;
				destinyLeft.x = destinyRight.x;
				destinyRight.x = destinyMid.x;
				destinyMid.x = temp;

				temp = destinyLeft.y;
				destinyLeft.y = destinyRight.y;
				destinyRight.y = destinyMid.y;
				destinyMid.y = temp;

				temp = destinyLeft.z;
				destinyLeft.z = destinyRight.z;
				destinyRight.z = destinyMid.z;
				destinyMid.z = temp;

				//sacle
				temp = destinyScaleLeft.x;
				destinyScaleLeft.x = destinyScaleRight.x;
				destinyScaleRight.x = destinyScaleMid.x;
				destinyScaleMid.x = temp;
		
				temp = destinyScaleLeft.y;
				destinyScaleLeft.y = destinyScaleRight.y;
				destinyScaleRight.y = destinyScaleMid.y;
				destinyScaleMid.y = temp;
		
				temp = destinyScaleLeft.z;
				destinyScaleLeft.z = destinyScaleRight.z;
				destinyScaleRight.z = destinyScaleMid.z;
				destinyScaleMid.z = temp;




//		destinyLeft.y = destinyRight.y;
//		destinyRight.y = destinyMid.y;
//		destinyMid.y = destinyLeft.y;

//		destinyLeft.x = right.transform.position.x - left.transform.position.x;
//		destinyMid.x = left.transform.position.x - mid.transform.position.x;
//		destinyRight.x = mid.transform.position.x - right.transform.position.x;
//
//		destinyLeft.y = right.transform.position.y - left.transform.position.y;
//		destinyMid.y = left.transform.position.y - mid.transform.position.y;
//		destinyRight.y = mid.transform.position.y - right.transform.position.y;

//		startLeft.x = left.transform.position.x;
//		startMid.x = mid.transform.position.x;
//		startRight.x = right.transform.position.x;
//
//		startLeft.y = left.transform.position.y;
//		startMid.y = mid.transform.position.y;
//		startRight.y = right.transform.position.y;


		}

		void turnClockwise ()
		{
				float temp1;
				float temp2;

				//change position X
				temp1 = left.transform.position.x;
				//left.transform.position.x = right.transform.position.x;
				left.transform.Translate (new Vector3 (right.transform.position.x - left.transform.position.x, 0, 0));


				temp2 = mid.transform.position.x;
				//mid.transform.position.x = temp1;
				mid.transform.Translate (new Vector3 (temp1 - mid.transform.position.x, 0, 0));

				//right.transform.position.x = temp2;
				right.transform.Translate (new Vector3 (temp2 - right.transform.position.x, 0, 0));


				//change position y
				temp1 = left.transform.position.y;
				//left.transform.position.y = right.transform.position.y;
				left.transform.Translate (new Vector3 (0, right.transform.position.y - left.transform.position.y, 0));
		
		
				temp2 = mid.transform.position.y;
				//mid.transform.position.y = temp1;
				mid.transform.Translate (new Vector3 (0, temp1 - mid.transform.position.y, 0));
		
				//right.transform.position.y = temp2;
				right.transform.Translate (new Vector3 (0, temp2 - right.transform.position.y, 0));

				//change scale
				right.transform.localScale = mid.transform.localScale;
				mid.transform.localScale = left.transform.localScale;

				//re-sort
				sortLeftToRight (ref left, ref mid, ref right);
		}

		void turnAntiClockwise ()
		{
				float temp1;
				float temp2;
		
				//change position X
				temp1 = right.transform.position.x;
				right.transform.Translate (new Vector3 (left.transform.position.x - right.transform.position.x, 0, 0));
		
		
				temp2 = mid.transform.position.x;
				//mid.transform.position.x = temp1;
				mid.transform.Translate (new Vector3 (temp1 - mid.transform.position.x, 0, 0));
		
				//right.transform.position.x = temp2;
				left.transform.Translate (new Vector3 (temp2 - left.transform.position.x, 0, 0));
		
		
				//change position y
				temp1 = right.transform.position.y;
				right.transform.Translate (new Vector3 (0, left.transform.position.y - right.transform.position.y, 0));
		
		
				temp2 = mid.transform.position.y;
				//mid.transform.position.y = temp1;
				mid.transform.Translate (new Vector3 (0, temp1 - mid.transform.position.y, 0));
		
				//right.transform.position.y = temp2;
				left.transform.Translate (new Vector3 (0, temp2 - left.transform.position.y, 0));
		
				//change scale
				left.transform.localScale = mid.transform.localScale;
				mid.transform.localScale = right.transform.localScale;
		
				//re-sort
				sortLeftToRight (ref left, ref mid, ref right);
		}
}
