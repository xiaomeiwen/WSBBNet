using UnityEngine;
using System.Collections;

namespace WSBB.StateMachineBase
{
	public class BallStateMachine : StateMachine
	{
		public static State AirPass = new State(false, "1", "AirbornePass", "", 0.15f);
		public static State BouncePass = new State(false, "2", "BouncePass", "", 0.15f);
		public static State AirborneShot = new State(false, "3", "AirborneShot", "", 0.15f);
		public static State OnRim = new State(false, "4", "OnRim", "", 0.15f);
		public static State InBasket = new State(false, "5", "InBasket", "", 0.15f);
		public static State Loose = new State(false, "6", "Loose", "", 0.15f);

		public static State Posessed = new State(false, "6", "Posessed", "", 0.15f);
		public static State Rebound = new State(false, "6", "Loose", "", 0.15f);

		public BallStateMachine(Ball owner) : base(owner)
		{}

		public new void Initialize()
		{
			base.Initialize();
			this.AddState(AirPass, false);
			this.AddState(BouncePass, false);
			this.AddState(AirborneShot, false);
			this.AddState(OnRim, false);
			this.AddState(InBasket, false);
			this.AddState(Loose, true);

			this.AddState(Posessed, false);
			this.AddState(Rebound, false);
		}
	}
}