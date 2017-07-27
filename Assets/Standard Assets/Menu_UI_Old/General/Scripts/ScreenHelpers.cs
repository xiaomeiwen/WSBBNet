using UnityEngine;
using System.Collections;

namespace WSBB {
	public static class ScreenHelpers {

		public static int idealScreenHeight = 320;
		public static int idealScreenWidth = 480;

		public static bool checkTouch(Vector2 touch, Rect testArea) {
			touch.y = Screen.height - touch.y;
			if((touch.x > testArea.x) && (touch.x < (testArea.x + testArea.width))) {
				if((touch.y > testArea.y) && (touch.y < (testArea.y + testArea.height))) {
					return true;
				}
			}	
			return false;
		}
		
		public static Rect scaleRectToScreen(Rect area) {
			area.x = area.x * ((float)Screen.width / (float)idealScreenWidth);
			area.y = area.y * ((float)Screen.height / (float)idealScreenHeight);
			area.width = area.width * ((float)Screen.width / (float)idealScreenWidth);
			area.height = area.height * ((float)Screen.height / (float)idealScreenHeight);	
			return area;
		}
		
		public static Vector2 scaleVector2ToScreen(Vector2 vect) {
			vect.x *=  ((float)Screen.width / (float)idealScreenWidth);
			vect.y *= ((float)Screen.height / (float)idealScreenHeight);
			return vect;
		}
	}
}