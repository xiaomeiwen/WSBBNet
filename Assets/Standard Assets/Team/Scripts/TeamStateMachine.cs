using UnityEngine;
using System.Collections;

namespace WSBB.StateMachineBase {
	public enum TeamState{
		// Offense
		SPREAD,
		PICK,
		RUSH,
		// Defense
		BASKET_GUARD,
		ZONE_GUARD,
		BALLER_GUARD,
		// General
		IDLE
	}

	public class TeamStateMachine : StateMachine {
		public static State Spread = new State(false, "1", "Spread", "", 0.15f);
		public static State Pick = new State(false, "2", "Pick", "", 0.15f);
		public static State Rush = new State(false, "3", "Rush", "", 0.15f);
		public static State BasketGuard = new State(false, "4", "BasketGuard", "", 0.15f);
		public static State ZoneGuard = new State(false, "5", "ZoneGuard", "", 0.15f);
		public static State BallerGuard = new State(false, "6", "BallerGuard", "", 0.15f);

		public TeamStateMachine(Team owner) : base(owner) {
		}

		new public void Initialize() {
			base.Initialize();
			this.AddState(Spread, true);
			this.AddState(Pick, false);
			this.AddState(Rush, false);
			this.AddState(BasketGuard, false);
			this.AddState(ZoneGuard, false);
			this.AddState(BallerGuard, false);

		}
	}
}