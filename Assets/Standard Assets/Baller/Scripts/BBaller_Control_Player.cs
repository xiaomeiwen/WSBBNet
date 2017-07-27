using UnityEngine;
using System.Collections;

namespace WSBB{
	public class BBaller_Control_Player : MonoBehaviour {

		public Team team;

		//********************************
		//Joystick Variables
		//********************************
		public Texture2D joystickImage;
		public Texture2D joystickBoarder;
		public Vector2 stickPosOffset;
		public Rect stickBoundry;
		public Rect stickInnerBoundry;
		public Rect stickBackground;
		public Vector3 directionVector;
		public float joystickTilt;
		public GUISkin joystickSkin;
		
		public Texture2D offscreenLeft;
		public Texture2D offscreenRight;
		public Rect offscreenArrowRect;
		
		private Vector2 touchPos;
		private int range;
		public bool joystickEnabled;
		private int touchIndex;
		private Vector2 touchOrigin;

		//********************************
		//Buttons
		//********************************
		public Texture2D jumpImage;
		public Texture2D passImage;
		public Texture2D passImageFade;
		public Texture2D shootImage;
		public Texture2D dunkImage;
		public Texture2D targetImage;
		public Rect jumpArea;
		public Rect passArea;
		public Rect shootArea;
		public Rect targetArea;
		//********************************

		private Transform myTransform;

		void Awake()
		{
			myTransform = transform;

			if(Screen.height != ScreenHelpers.idealScreenHeight)
			{
				stickBoundry = ScreenHelpers.scaleRectToScreen(stickBoundry);	
				stickInnerBoundry = ScreenHelpers.scaleRectToScreen(stickInnerBoundry);	
				stickBackground = ScreenHelpers.scaleRectToScreen(stickBackground);
				jumpArea = ScreenHelpers.scaleRectToScreen(jumpArea);	
				passArea = ScreenHelpers.scaleRectToScreen(passArea);	
				shootArea = ScreenHelpers.scaleRectToScreen(shootArea);	
				targetArea = ScreenHelpers.scaleRectToScreen(targetArea);		
				offscreenArrowRect = ScreenHelpers.scaleRectToScreen(offscreenArrowRect);
			}
			range = (int)(stickBoundry.width / 2f);	
		}
		
		// Update is called once per frame
		void Update () 
		{
            //Debug.Log("Control_Player Update called");
            if (GameAdmin.GamePause)
                return;

            if (team.pureAI != true)
            {
                JoystickUpdate();
                ButtonUpdate();
                team.getPlayerBaller().Run(directionVector, joystickTilt);
            }
            else
                Debug.Log("Error: Controller is set to pure AI Team!");
			myTransform.position = team.getPlayerBaller().MyTransform.position;
			myTransform.position = new Vector3(myTransform.position.x, 8f, myTransform.position.z);
		}

		void OnGUI()
		{
            if (GameAdmin.GamePause)
                return;
			JoystickGUI();
			ButtonGUI();
			
			var tempPos = transform.position;
			tempPos.y = 3;
			Vector3 viewPos = Camera.main.WorldToViewportPoint(tempPos);
			Rect tempRect = new Rect();
			//Debug.Log(viewPos);
			if(viewPos.x < 0f)
			{
				tempRect.x = 0f;
				tempRect.y = (Screen.height - (viewPos.y * Screen.height)) - (offscreenArrowRect.height / 2.0f);
				tempRect.width = offscreenArrowRect.width;
				tempRect.height = offscreenArrowRect.height;
				GUI.Label(tempRect, offscreenLeft);
				//Debug.Log("Draw Arrow Left:" + tempRect);
			}
			else if(viewPos.x > 1f)
			{
				tempRect.x = Screen.width - offscreenArrowRect.width;
				tempRect.y = (Screen.height - (viewPos.y * Screen.height)) - (offscreenArrowRect.height / 2.0f);
				tempRect.width = offscreenArrowRect.width;
				tempRect.height = offscreenArrowRect.height;
				GUI.Label(tempRect, offscreenRight);
				//Debug.Log("Draw Arrow Right:" + tempRect);
			}
		}


		void JoystickUpdate()
		{
			foreach (Touch touch in Input.touches) 
			{
				if(!joystickEnabled)
				{
					touchPos.x = touch.position.x;
					var touchY = Screen.height - touch.position.y;
					touchPos.y = touchY;
					if(touch.position.x > stickInnerBoundry.x && touch.position.x < stickInnerBoundry.x + stickInnerBoundry.width)
					{
						if((touchY) > stickInnerBoundry.y && (touchY) < stickInnerBoundry.y + stickInnerBoundry.height)
						{
							//Debug.Log("Button Pressed");
							joystickEnabled = true;
							touchIndex = touch.fingerId;
							touchOrigin = touch.position;
						}
					}
				}
				else
				{
					stickPosOffset = Vector2.zero;
					joystickEnabled = false;
					if(touch.fingerId == touchIndex)
					{
						if(touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
						{
							joystickTilt = 0f;
							touchIndex = -1;
						}
						else
						{
							joystickEnabled = true;
							stickPosOffset = touchOrigin - touch.position;
							stickPosOffset.x = stickPosOffset.x * -1;
							
							Vector3 temp3D = new Vector3(stickPosOffset.x, stickPosOffset.y, 0f);
							temp3D.Normalize();
							if(Mathf.Pow(stickPosOffset.x, 2) + Mathf.Pow(stickPosOffset.y, 2) > 0)
							{
								directionVector.x = temp3D.x;
								directionVector.y = 0f;
								directionVector.z = -temp3D.y;
							}
							
							if(Mathf.Pow(range, 2) <= (Mathf.Pow(stickPosOffset.x, 2) + Mathf.Pow(stickPosOffset.y, 2)))
							{
								stickPosOffset.x = temp3D.x;
								stickPosOffset.y = temp3D.y;
								stickPosOffset = stickPosOffset * range;
							}
							temp3D = new Vector3(stickPosOffset.x, stickPosOffset.y, 0f);
							joystickTilt = temp3D.magnitude / range;
						}
					}
				}
			}		
		}


		void JoystickGUI()
		{
			GUI.skin = joystickSkin;
			GUI.Label(stickBackground, joystickBoarder);
			GUI.Label(
				new Rect(stickBoundry.x + stickPosOffset.x, stickBoundry.y + stickPosOffset.y, stickBoundry.width, stickBoundry.height), 
				joystickImage);
			
			//Debug Box Draws
			//GUI.Box(stickInnerBoundry, "");	
		}

		void ButtonUpdate()
		{
			foreach (Touch touch in Input.touches) 
			{
				if(touch.phase == TouchPhase.Began)
				{
					Touch tempTouch = touch;
					if(team.getPlayerBaller().HasBall)
					{
						if(ScreenHelpers.checkTouch(tempTouch.position, targetArea))
						{
							team.selectNextBaller();
						}
						else if(ScreenHelpers.checkTouch(tempTouch.position, shootArea))
						{
                            if (team.getPlayerBaller().ballerAI.parent.CanDunk())
                                team.getPlayerBaller().ballerAI.parent.Dunk();
							else
                                team.getPlayerBaller().ballerAI.parent.Shoot();
						}
						else if(ScreenHelpers.checkTouch(tempTouch.position, passArea))
						{
                            team.getPlayerBaller().ballerAI.parent.Pass();
						}
					}
					else
					{
						if(ScreenHelpers.checkTouch(tempTouch.position, jumpArea))
						{
                            //BLOCK SHOT
                            team.getCurrentBaller().BlockShot();
						}
						else if(ScreenHelpers.checkTouch(tempTouch.position, targetArea))
						{
							team.changeToNextBaller();
						}
						else if(ScreenHelpers.checkTouch(tempTouch.position, passArea))
						{
                            team.getPlayerBaller().ballerAI.parent.Steal();
						}
						
					}
				}
			}
			
			//Debug Input
			if(Input.GetKeyDown("space"))
			{
				team.selectNextBaller();
			}
		}

		void ButtonGUI()
		{
			if(team.getPlayerBaller().HasBall)
			{
				GUI.Label(jumpArea, jumpImage);
				if(team.amICurrentBaller(team.getPlayerBaller()))
				{
					GUI.Label(passArea, passImageFade);
				}
				else
				{
					GUI.Label(passArea, passImage);
				}
                if (team.getPlayerBaller().ballerAI.parent.CanDunk())
					GUI.Label(shootArea, dunkImage);
				else
					GUI.Label(shootArea, shootImage);
				GUI.Label(targetArea, targetImage);
				
				//Debug Input
				//GUI.Box(targetArea, "");
			}
			else
			{
				GUI.Label(jumpArea, jumpImage);
				GUI.Label(targetArea, targetImage);
                if (team.getPlayerBaller().ballerAI.parent.CanSteal() != null)
				{
					GUI.Label(passArea, passImage);
				}
				else
				{
					GUI.Label(passArea, passImageFade);
				}  //Need to replace image for Steal
			}
		}


	}
}