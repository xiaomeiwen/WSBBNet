using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WSBB.StateMachineBase {
	public interface ISMObject {
		void Initialize(StateMachine stateMachine);		
		void Update();
		bool ChangeState(State newState);
		void PlayAnimation(string animationName, float fadeTime);
		void Cleanup();
	}
}