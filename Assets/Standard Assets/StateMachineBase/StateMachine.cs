using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace WSBB.StateMachineBase {
	public class StateMachine {
		protected string description;
		public string Description { get; set; }

		protected List<State> states; 

		public State CurrentState { get; set; }
		public State DefaultState { get; set; }

		public ISMObject Owner { get; set; }

		public bool Enabled { get; set; }
		public bool Start { get; set; }
		public bool InDebugMode { get; set; }
		public bool IsPaused { get; set; }
		public bool Initialized { get; set; }

		public delegate void CustomEvent(string eventName, object parameter);
		public event CustomEvent onReceiveEvent;

		public StateMachine(ISMObject owner) {

			this.Owner = owner;
		}
		
		public void Initialize(){
			if (states == null) {
				states = new List<State> ();
			}
			CurrentState = null;
			DefaultState = null;
		}

		private void Awake () {
			Enabled = (states.Count > 0);
			if (Enabled) {
				Initialize();
                //DefaultState = states.Find(state => state.IsDefaultState == true);
                //CurrentState = DefaultState;
                Debug.Log("Awaken!!!!");
				Initialized = true;
				if (Start) {
					EnableStateMachine();
				}
			}
		}
		
		private void OnEnable(){
			if (states == null) {
				states = new List<State> ();
			}
		}

		public void AddState(State newState, bool isDefault) {
			if(newState == null) {
				return;
			}
			states.Add(newState);
			if (isDefault) {
				//Debug.Log ("setting default state to " + newState.StateName);
				DefaultState = newState;
                CurrentState = newState;
			}
		}

		public virtual void Update (ISMObject owner) {
			if (IsPaused) {
				return;
			}
			if (CurrentState != null) {
				CurrentState.Update (Owner);
			} else {
				CurrentState = DefaultState;
			}
		}		
		
		public void SetState(State mState){
			if (mState != null) {
				if (CurrentState != null)
					CurrentState.Exit (Owner);
				//if (mState != CurrentState)
				if (mState != CurrentState) {
					//CurrentState = states.Find(x=> x.Id == mState.Id);
					CurrentState = mState;
					CurrentState.Enter (Owner);
					//Debug.Log ("entering " + mState.StateName + " state!!");
					CurrentState.PlayAnimation (Owner);
				}
			}
		}
		
		public void SetDefaultState(){
			if (DefaultState != null) {
				CurrentState.Exit(Owner);
				CurrentState = DefaultState;
				CurrentState.Enter(Owner);
			}
		}
		
		public void EnableStateMachine(){
			if (!Initialized) {
				Awake();			
			}
			IsPaused = false;
		}
		
		public void DisableStateMachine(bool pause){
			if(pause) {
				IsPaused = true;
			} else {
				Enabled=false;	
			}
		}

		public void DeleteStates(){
			if (states != null) {
				foreach (State state in states) {
					state.DeleteState ();		
				}
				states.Clear ();
			}
		}
	}
}
