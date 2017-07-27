using UnityEngine;
using System.Collections;

namespace WSBB.StateMachineBase {
	public enum ScuttleDir {FORWARD, BACK, LEFT, RIGHT}

    //[System.Serializable]
	public class BBallerStateMachine : StateMachine {
		BallerAI parentAI;
		BBaller parentBaller;

		public static State Pass = new State(false, "1", "Pass", "Pass_copy", 0.15f);
		public static State Clown = new State(false, "2", "Clown", "", 0.15f);
		public static State Shoot = new State(false, "3", "Shoot", "Shot_copy", 0.15f);
		public static State Rebound = new State(false, "4", "Rebound", "", 0.15f);
		public static State Steal = new State(false, "5", "Steal", "Steal_copy", 0.15f);
		public static State Block = new State(false, "6", "Block", "BlockShot_copy", 0.15f);
		public static State Intercept = new State(false, "7", "Intercept", "", 0.15f);
		public static State RemainOpen = new State(false, "8", "RemainOpen", "", 0.15f);
		public static State Receive = new State(false, "9", "Receive", "PassRecieve", 0.15f);
		public static State StagLeap = new State(false, "10", "StagLeap", "", 0.15f);
		public static State Idle_Ball = new State(false, "11", "Idle_Ball", "Stand_Ball", 0.15f);
		public static State Idle_NoBall = new State(false, "12", "Idle_NoBall", "Stand_NoBall", 0.15f);
		public static State Guard = new State(false, "12", "Guard", "", 0.15f);
		public static State Dunk = new State(false, "13", "Dunk", "", 0.15f);
		public static State Move = new State(false, "14", "Move", "", 0.15f);
        public static State Bamboozled = new State(false, "15", "Bamboozled", "", 0.15f);

		public BBallerStateMachine(BallerAI owner) : base(owner) {
			parentAI = owner;
            parentBaller = owner.parent;
            Owner = owner;
        }

		new public void Initialize() {
			base.Initialize();

			this.AddState(Pass, false);
            Pass.DoPlayAnimationEventHandler += new State.stateEvent(PassAnimation);

			this.AddState(Clown, false);
            Clown.DoPlayAnimationEventHandler += new State.stateEvent(ClownAnimation);

			this.AddState(Shoot, false);
            Shoot.DoPlayAnimationEventHandler += new State.stateEvent(ShootAnimation);

			this.AddState(Rebound, false);
            Rebound.DoPlayAnimationEventHandler += new State.stateEvent(ReboundAnimation);

			this.AddState(Steal, false);
            Steal.DoPlayAnimationEventHandler += new State.stateEvent(StealAnimation);

			this.AddState(Block, false);
            Block.DoPlayAnimationEventHandler += new State.stateEvent(BlockAnimation);

			this.AddState(Intercept, false);
            Intercept.DoPlayAnimationEventHandler += new State.stateEvent(InterceptAnimation);

			this.AddState(RemainOpen, false);
            RemainOpen.DoPlayAnimationEventHandler += new State.stateEvent(RemainOpenAnimation);

			this.AddState(Receive, false);
            Receive.DoPlayAnimationEventHandler += new State.stateEvent(ReceiveAnimation);

			this.AddState(StagLeap, false);
            StagLeap.DoPlayAnimationEventHandler += new State.stateEvent(StagLeapAnimation);

			this.AddState(Idle_Ball, false);
            Idle_Ball.DoPlayAnimationEventHandler += new State.stateEvent(IdleBallAnimation);

			this.AddState(Idle_NoBall, true);
            Idle_NoBall.DoPlayAnimationEventHandler += new State.stateEvent(IdleNoBallAnimation);

			this.AddState(Guard, false);
			Guard.DoPlayAnimationEventHandler += new State.stateEvent(GuardAnimation);
			//Guard.DoUpdateEventHandler += new State.stateEvent(ExecuteGuard);
			
			this.AddState(Dunk, false);
			Dunk.DoPlayAnimationEventHandler += new State.stateEvent(DunkAnimation);

			this.AddState(Move, false);
			Move.DoPlayAnimationEventHandler += new State.stateEvent(MoveAnimation);
			Move.DoUpdateEventHandler += new State.stateEvent(ExecuteMove);

            this.AddState(Bamboozled, false);
            Bamboozled.DoPlayAnimationEventHandler += new State.stateEvent(BamboozledAnimation);
		}

        public void PassAnimation(ISMObject parent)
        {
            BallerAI ballerAI = (BallerAI)parent;
            BBaller baller = (BBaller)ballerAI.parent;
            baller.PlayAnimation(GameAnims.Pass, 0.15f, false);
        }

        public void ClownAnimation(ISMObject parent)
        {
            BallerAI ballerAI = (BallerAI)parent;
            BBaller baller = (BBaller)ballerAI.parent;
            baller.PlayAnimation(GameAnims.Clown1, 0.15f, false);
        }

        public void BamboozledAnimation(ISMObject parent)
        {
            BallerAI ballerAI = (BallerAI)parent;
            BBaller baller = (BBaller)ballerAI.parent;
            baller.PlayAnimation(GameAnims.Bamboozled1, 0.15f, false);
        }

        public void ShootAnimation(ISMObject parent)
        {
            BallerAI ballerAI = (BallerAI)parent;
            BBaller baller = (BBaller)ballerAI.parent;
            baller.PlayAnimation(GameAnims.Shot, 0.15f, false);
            //baller.LaunchBallToBasket();                        //We want this to happen at the end of the animation (once the baller has reached the peak height of his jump)
        }

        public void ReboundAnimation(ISMObject parent)
        {
            BallerAI ballerAI = (BallerAI)parent;
            BBaller baller = (BBaller)ballerAI.parent;
            baller.PlayAnimation(GameAnims.Rebound1, 0.15f, false);
        }

        public void StealAnimation(ISMObject parent)
        {
            BallerAI ballerAI = (BallerAI)parent;
            BBaller baller = (BBaller)ballerAI.parent;
            baller.PlayAnimation(GameAnims.Steal, 0.15f, false);
        }

        public void BlockAnimation(ISMObject parent)
        {
            BallerAI ballerAI = (BallerAI)parent;
            BBaller baller = (BBaller)ballerAI.parent;
            baller.PlayAnimation(GameAnims.BlockShot, 0.15f, false);
        }

        public void InterceptAnimation(ISMObject parent)
        {
            BallerAI ballerAI = (BallerAI)parent;
            BBaller baller = (BBaller)ballerAI.parent;
            baller.PlayAnimation(GameAnims.ReceivePass, 0.15f, false);
        }

        public void RemainOpenAnimation(ISMObject parent)
        {
            BallerAI ballerAI = (BallerAI)parent;
            BBaller baller = (BBaller)ballerAI.parent;
            baller.PlayAnimation(GameAnims.Remain_Open, 0.15f, true);
        }

        public void ReceiveAnimation(ISMObject parent)
        {
            BallerAI ballerAI = (BallerAI)parent;
            BBaller baller = (BBaller)ballerAI.parent;
            baller.PlayAnimation(GameAnims.ReceivePass, 0.15f, false);
        }

        public void StagLeapAnimation(ISMObject parent)
        {
            BallerAI ballerAI = (BallerAI)parent;
            BBaller baller = (BBaller)ballerAI.parent;
            baller.PlayAnimation(GameAnims.Stag_Leap, 0.15f, false);
        }

        public void IdleBallAnimation(ISMObject parent)
        {
            BallerAI ballerAI = (BallerAI)parent;
            BBaller baller = (BBaller)ballerAI.parent;
            baller.PlayAnimation(GameAnims.Stand_Ball, 0.15f, true);
        }

        public void IdleNoBallAnimation(ISMObject parent)
        {
            BallerAI ballerAI = (BallerAI)parent;
            BBaller baller = (BBaller)ballerAI.parent;
            baller.PlayAnimation(GameAnims.Stand_NoBall, 0.15f, true);
        }

		public void GuardAnimation(ISMObject parent) {
			BallerAI ballerAI = (BallerAI)parent;
			BBaller baller = (BBaller)ballerAI.parent;
			switch(baller.GuardDir)  {
				case GuardDirection.FORWARD:
					baller.PlayAnimation(GameAnims.GuardForward, 0.15f, true);
					break;
				case GuardDirection.BACK:
					baller.PlayAnimation(GameAnims.GuardBack, 0.15f, true);
					break;
				case GuardDirection.LEFT:
					baller.PlayAnimation(GameAnims.GuardLeft, 0.15f, true);
					break;
				case GuardDirection.RIGHT:
					baller.PlayAnimation(GameAnims.GuardRight, 0.15f,true);
					break;
				default:
					break;
			}
		}

		/* TODO : This shouldnt be here */
        //public void ExecuteGuard(ISMObject parent) {
        //    BallerAI ballerAI = (BallerAI)parent;
        //    BBaller baller = (BBaller)ballerAI.parent;
        //    if(baller.dunkLerp) {
        //        baller.dunkTime += Time.deltaTime;
        //        if(baller.dunkTime > baller.dunkTimeTotal)
        //            baller.dunkTime = baller.dunkTimeTotal;
        //        Vector3 currentPos= baller.MyTransform.position;
        //        Vector3 targetPos = Vector3.Lerp(baller.dunkStartPos, baller.MyTeam.basketTarget.position, 
        //                                         baller.dunkTime / baller.dunkTimeTotal);
        //        targetPos.y = 0;
        //        Vector3 posDifference = targetPos - currentPos;
        //        baller.MyTransform.position = targetPos;
        //        Debug.Log(targetPos);
        //    } else {
        //        Debug.LogWarning(CurrentState.StateName + " State: Undesired location reached");
        //    }
        //}

		public void DunkAnimation(ISMObject parent) {
			BallerAI ballerAI = (BallerAI)parent;
			BBaller baller = (BBaller)ballerAI.parent;
			//Debug.Log(baller.GetDunkName());
			baller.PlayAnimation(baller.GetDunkName(), 0.15f, false);
		}

        public void ExecuteMove(ISMObject parent)
        {
            BallerAI ballerAI = (BallerAI)parent;
            BBaller baller = (BBaller)ballerAI.parent;
            //baller.Move();
        }

		public void MoveAnimation(ISMObject parent) {
			BallerAI ballerAI = (BallerAI)parent;
			BBaller baller = (BBaller)ballerAI.parent;
			if (baller.HasBall) {
				switch(baller.MoveState) {
					case MoveType.WALK:
						baller.PlayAnimation(GameAnims.Walk_Ball, 0.15f, true);
						break;
						
					case MoveType.JOG:
                        baller.PlayAnimation(GameAnims.Jog_Ball, 0.15f, true);
						break;
						
					case MoveType.RUN:
                        baller.PlayAnimation(GameAnims.Run_Ball, 0.15f, true);
						break;
					default:
                        baller.PlayAnimation(GameAnims.Stand_Ball, 0.15f, true);
						break;
				}
			} else {
				switch(baller.MoveState) {
					case MoveType.WALK:
                        baller.PlayAnimation(GameAnims.Walk_NoBall, 0.15f, true);
						break;
						
					case MoveType.JOG:
                        baller.PlayAnimation(GameAnims.Jog_NoBall, 0.15f, true);
						break;
						
					case MoveType.RUN:
                        baller.PlayAnimation(GameAnims.Run_NoBall, 0.15f, true);
						break;
					default:
                        baller.PlayAnimation(GameAnims.Stand_NoBall, 0.15f, true);
						break;
				}
			}
		}
	}
}