using UnityEngine;
using System;
using System.Collections;

namespace WSBB {
	public class BBaller_anim : MonoBehaviour {
		public BBaller baller;
		public String currentAnim;
		public Animation animator;
		public String anim1;
		public float anim1Weight;
		public String anim2;
		public float anim2Weight;


		void Awake()
		{
			baller = GetComponentInParent<BBaller>();
			animator = GetComponent<Animation>();
		}

		#region OriginalAnimationUpdateMethods
		// Update is called once per frame
		//void Update () 
//		public void SetAnimations()
//		{
//			if(GameAdmin.GamePause)
//				return;
//			
//			//String nextAnim;
//			switch(baller.CurrentState.Name)
//			{
//			case BallerStates.Idle.Instance.Name:
////				if(baller.HasBall) {
////					PlayAnimation(GameAnims.Stand_Ball, 0.15f);
////					break;
////				}
////
////				PlayAnimation(GameAnims.Stand_NoBall, 0.15f);
//				break;
//			case BallerStates.Move.Instance.Name:
//
////				if (baller.HasBall)
////				{
////					switch(baller.MoveState)
////					{
////					case MoveType.WALK:
////						PlayAnimation(GameAnims.Walk_Ball, 0.15f);
////						break;
////
////					case MoveType.JOG:
////						PlayAnimation(GameAnims.Jog_Ball, 0.15f);
////						break;
////
////					case MoveType.RUN:
////						PlayAnimation(GameAnims.Run_Ball, 0.15f);
////						break;
////					}
////				}
////				else
////				{
////					switch(baller.MoveState)
////					{
////					case MoveType.WALK:
////						PlayAnimation(GameAnims.Walk_NoBall, 0.15f);
////						break;
////						
////					case MoveType.JOG:
////						PlayAnimation(GameAnims.Jog_NoBall, 0.15f);
////						break;
////						
////					case MoveType.RUN:
////						PlayAnimation(GameAnims.Run_NoBall, 0.15f);
////						break;
////					}
////				}
//				break;
//
//			case BallerStates.Guard.Instance.Name:
////				switch(baller.GuardDir) 
////				{
////				case GuardDirection.FORWARD:
////					PlayAnimation(GameAnims.GuardForward, 0.15f);
////					break;
////				case GuardDirection.BACK:
////					PlayAnimation(GameAnims.GuardBack, 0.15f);
////					break;
////				case GuardDirection.LEFT:
////					PlayAnimation(GameAnims.GuardLeft, 0.15f);
////					break;
////				case GuardDirection.RIGHT:
////					PlayAnimation(GameAnims.GuardRight, 0.15f);
////					break;
////				}
//				break;
//			case BallerStates.Shoot.Instance.Name:
////				PlayAnimation(GameAnims.Shot, 0.15f);
//				break;
//			case BallerStates.Pass.Instance:
////				PlayAnimation(GameAnims.Pass, 0.15f);
//				break;
//			case BallerStates.Receive.Instance:
////				PlayAnimation(GameAnims.ReceivePass, 0.15f);
//				break;
//			case BallerStates.Steal.Instance:
////				PlayAnimation(GameAnims.Steal, 0.15f);
//				break;
//			case BallerStates.Block.Instance:
////				PlayAnimation(GameAnims.BlockShot, 0.15f);
//				break;
//
//				//Probably not used yet. So replacing with STATE = GUARD, GUARDTYPE = <something>
////				case BallerState.Guard_Idle:
////					PlayAnimation(GameAnims.Stand_NoBall, 0.15f);
////					break;
////				case BallerState.Guard_Move:
////					PlayBlend(0.15f);
////					break;
//			case BallerStates.Dunk.Instance.Name:
//				Debug.Log(baller.GetDunkName());
//				PlayAnimation(baller.GetDunkName(), 0.15f);
//				break;
//			}		
//		}
		#endregion

		public void animWalkStep() {
			Debug.Log("Step");
		}
		
		public void animTakeShot() {
			baller.ballerAI.parent.LaunchBallToBasket();
		}
		
		public void animPassBall() {
            //baller.ballerAI.parent.PassTheBall();	
		}
		
		public void animSteal() {
            baller.ballerAI.parent.FinishSteal();
		}
		
		public void animEndAction() {
            baller.ballerAI.parent.ActionEnd();
		}
		
		public void animBlock() {
            baller.ballerAI.parent.FinishBlock();
		}
		
		public void startDunkLerp(int lerpTime) {
            Debug.Log("startDunkLerp - BBaller_anim");
            baller.ballerAI.parent.DunkStartLerp(lerpTime);	
		}
		
		public void scoreDunk() {
            baller.ballerAI.parent.scoreDunk();
		}
		
		public void EndDunk() {
            baller.ballerAI.parent.endDunk();
		}

		public void PlayAnimation(String animToPlay, float fadeTime) {

			if(animator == null)
				Awake ();

			if(currentAnim != animToPlay) {
				currentAnim = animToPlay;
				animator.CrossFade(animToPlay, fadeTime);
			}
		}

		//Used by GUARD_MOVE (Legacy state). But Guard_Move was never implemented itself
		public void PlayBlend(float fadeTime) {
			currentAnim = "BlendAnim";
			//Debug.Log(anim1 + ":" + anim2);
			animator.CrossFade(anim1, fadeTime);
			animator.Blend(anim2, anim2Weight, fadeTime / 4);
		}
	}
}