using UnityEngine;
using System.Collections;
using System;

namespace WSBB {
	public class playAnimation : MonoBehaviour {
		public Animation animation;
		public Animator animator;

		void Awake() {
			animation = GetComponent<Animation> ();
			animator = GetComponent<Animator> ();
			animation.wrapMode = WrapMode.Loop;
		}
		void Start() {
			animation.Play ();
//			animator.CrossFade (GameAnims.Walk_Ball, 0.15f);
		}
	}
}
