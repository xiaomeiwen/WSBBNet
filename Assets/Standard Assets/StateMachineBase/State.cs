using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WSBB.StateMachineBase {
	public class State {
		//protected bool isDefaultState;
		public bool IsDefaultState { get; set;}

		public delegate void stateEvent(ISMObject parent);

		public event stateEvent DoEnterEventHandler;
		public event stateEvent DoUpdateEventHandler;
		public event stateEvent DoExitEventHandler;
		public event stateEvent DoPlayAnimationEventHandler;

		protected List<State> transitions;
		public void addTransition(State toState) {
			transitions.Add(toState);
			Debug.Log ("Process: Added transition to " + toState.StateName + " from " + StateName + " State instance");
		}
		
		//protected string id;
		public string Id { get; set; }
		
		//protected string stateName;
		public string StateName { get; set; }

		//protected string stateAnimation;
		public string StateAnimation { get; set; }

		//protected float animationFadeTime;
		public float AnimationFadeTime { get; set; }

		public State(bool isDefaultState, string id, string stateName, string stateAnimation, float animationFadeTime) {
			this.IsDefaultState = isDefaultState;
			this.Id = id;
			this.StateName = stateName;
			this.StateAnimation = stateAnimation;
			this.AnimationFadeTime = animationFadeTime;
			//Debug.Log ("Process: Constructing a " + stateName + " State instance");
		}

		public bool isTransitionValid(State toState) {
			if(!transitions.Contains(toState)) {
				return false;
			}

			/* Perform any additional validations */
			
			Debug.Log ("Process: Exiting: " + StateName + ".isTransitionValid()");
			return true;
		}

		public void Enter(ISMObject parent){
			if (DoEnterEventHandler != null) {
				DoEnterEventHandler(parent);
			}
			//Debug.Log ("Process: Exiting: " + StateName + ".DoEnter()");
		}

		public void Exit(ISMObject parent){
			if (DoExitEventHandler != null) {
				DoExitEventHandler(parent);
			}
			//Debug.Log ("Process: Exiting: " + StateName + ".DoExit()");
		}

		public void Update(ISMObject parent){
			if (DoUpdateEventHandler != null) {
				DoUpdateEventHandler(parent);
			}
			//Debug.Log ("Process: Exiting: " + StateName + ".DoUpdate()");
		}

		public void PlayAnimation(ISMObject parent){
			if (DoPlayAnimationEventHandler != null) {
				DoPlayAnimationEventHandler (parent);
			} else {
				parent.PlayAnimation (StateAnimation, AnimationFadeTime);
			}
			//Debug.Log ("Process: Exiting: " + StateName + ".DoPlayAnimation()");
		}

		public void DeleteState(){
			/* Destroys the state. DO NOT USE if only making a transition */
			//DestroyImmediate(this, true);
		}
	}
}