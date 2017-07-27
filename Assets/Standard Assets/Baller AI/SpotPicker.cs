using UnityEngine;
using System.Collections;

namespace WSBB {
	//===========================
	// custom class to facility point selector
	//===========================
	[System.Serializable]public class Rect3D
	{
		public Vector3 center;
		public Vector3 range;
		public bool InArea(Vector3 point, Vector3 offset)
		{
			Vector3 minPoint = getMin(offset);
			Vector3 maxPoint = getMax(offset);
			if(point.x > minPoint.x && point.x < maxPoint.x)
				if(point.x > minPoint.y && point.y < maxPoint.y)
					if(point.x > minPoint.z && point.z < maxPoint.z)
						return true;
			return false;
		}
		public bool InArea(Vector3 point)
		{
			return InArea(point, Vector3.zero);
		}
		
		public Vector3 getMin(Vector3 offset)
		{
			return center - (range / 2);
		}
		
		public Vector3 getMin()
		{
			return getMin(Vector3.zero);
		}
		
		public Vector3 getMax(Vector3 offset)
		{
			return center + (range / 2);
		}
		
		public Vector3 getMax()
		{
			return getMax(Vector3.zero);
		}
	}

	public class SpotPicker : MonoBehaviour {
		public Rect3D[] area_;
		public Rect3D[] exclusion_zones;
		
		public bool debugArch;
		
		public int lastArea;

		public float minArchRange;
		public float maxArchRange;
		public float lastArchAngle;
		public float archMaxAngle;
		public float archMinAngle;
		public float archAdjust;

		public Vector3 lastPos;

		// Use this for initialization
		void Start () {
            lastPos = new Vector3(0, 0, 0);
		}
		
		// Update is called once per frame
		void Update () {
		
		}

		public Vector3 RandomArchPos()
		{
			lastArchAngle = Random.Range(archMinAngle, archMaxAngle);
			transform.localEulerAngles.Set(transform.localEulerAngles.x,
			                               lastArchAngle,
			                               transform.localEulerAngles.z);
			lastPos = transform.position + (transform.forward * Random.Range(minArchRange, maxArchRange));
			return lastPos;
		}
		
		public bool StepArchPos(float leftPreference)
		{
			bool returnValue;
			/*Original JS Random.value >= leftPreference && lastArchAngle */
			if((Random.value >= leftPreference) && (0 != lastArchAngle))
			{
				lastArchAngle += archAdjust;
				returnValue = true;
				if(lastArchAngle > archMaxAngle)
				{
					lastArchAngle = archMaxAngle;
					returnValue = false;
				}
			}
			else
			{
				lastArchAngle -= archAdjust;
				returnValue = false;
				if(lastArchAngle < archMinAngle)
				{
					lastArchAngle = archMinAngle;
					returnValue = true;
				}
			}
			transform.localEulerAngles.Set(transform.localEulerAngles.x,
			                               lastArchAngle,
			                               transform.localEulerAngles.z);
			lastPos = transform.position + (transform.forward * Random.Range(minArchRange, maxArchRange));
			return returnValue;
		}
		
		//===============================
		// Goal of this class is to select a random point within an area while ensuring that point is not in any exclusion zone
		//===============================
		public Vector3 PickPointRect(bool ignoreAvoid)
		{
			Vector3 random_point;
			while(true) {
                //Debug.Log("area_ size = " + area_.Length);
                //int randomArea = Random.Range(0, (int)(area_.Length - .001));
                int randomArea;
                Rect3D selectedArea;
                if (area_.Length != 0)
                {
                   randomArea = Random.Range(0, area_.Length);
                    //Debug.Log("randomArea = " + randomArea);
                    if (randomArea > area_.Length - 1)
                        randomArea = area_.Length - 1;
                    selectedArea = area_[randomArea];
                }
                else
                {
                    randomArea = 0;
                    selectedArea = area_[0];
                }
                
				lastArea = randomArea;
				Vector3 min_point = selectedArea.getMin(transform.position);
				Vector3 max_point = selectedArea.getMax(transform.position);
				random_point.x = Random.Range(min_point.x, max_point.x);
				random_point.y = Random.Range(min_point.y, max_point.y);
				random_point.z = Random.Range(min_point.z, max_point.z);
				bool valid = true;
				if(!ignoreAvoid)
				{
					foreach(Rect3D zone in exclusion_zones)
					{
						if(zone.InArea(random_point, transform.position))
						{
							valid = false;
							Debug.Log("Point In Zone");
							break;
						}
					}
				}
				if(valid)
				{
					lastPos = random_point + transform.position;
                    //Debug.Log(lastPos.x + "--" + lastPos.y + "--" + lastPos.z);
					return lastPos;
				}
                else
                {
                    Debug.Log("Invalid Spot!");
                }
			}
		}
		
		public Vector3 PickPointRect(float areaStay)
		{
			Vector3 random_point;
			Rect3D selectedArea = new Rect3D();
			while(true)
			{
				if(Random.value < areaStay)
				{
					int randomArea = Random.Range(0, (int)(area_.Length - .001));
					selectedArea = area_[randomArea];
					lastArea = randomArea;
				}
				else
				{
					string name = this.name;
					if(lastArea < area_.Length)
						selectedArea = area_[lastArea];
				}
				Vector3 min_point = selectedArea.getMin(transform.position);
				Vector3 max_point = selectedArea.getMax(transform.position);
				random_point.x = Random.Range(min_point.x, max_point.x);
				random_point.y = Random.Range(min_point.y, max_point.y);
				random_point.z = Random.Range(min_point.z, max_point.z);
				bool valid = true;
				foreach(Rect3D zone in exclusion_zones)
				{
					if(zone.InArea(random_point, transform.position))
					{
						valid = false;
						Debug.Log("Point In Zone");
						break;
					}
				}
				if(valid)
				{
					lastPos = random_point + transform.position;
					return lastPos;
				}
			}
		}
		//Draws the picking zone and exclusion zones
		public void OnDrawGizmos()
		{
			if(!debugArch)
			{
				Gizmos.color = Color.white;
				foreach(Rect3D partArea in area_)
					Gizmos.DrawWireCube(partArea.center + transform.position, partArea.range);
				Gizmos.color = new Color(.5F,.5F,.5F);
				foreach(Rect3D zone in exclusion_zones)
				{
					Gizmos.DrawWireCube(zone.center + transform.position, zone.range);
				}
			}
			else
			{
				Gizmos.color = Color.white;
				Gizmos.DrawRay(transform.position, transform.forward * maxArchRange);
				Vector3 upPos = transform.position;
				upPos.y += .25F;
				Gizmos.color = new Color(1,0,0);
				Gizmos.DrawRay(upPos, transform.forward * minArchRange);
			}
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(lastPos, 1);
		}
	}
}