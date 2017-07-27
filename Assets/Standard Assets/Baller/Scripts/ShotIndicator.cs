using UnityEngine;
using System.Collections;

namespace WSBB {
	public class ShotIndicator : MonoBehaviour {

		[HideInInspector]public BBaller myBaller;
		[HideInInspector]public Renderer myRendererRing;
		[HideInInspector]public Renderer myRendererCore;
		[HideInInspector]public Transform myCoreTransform;
		[HideInInspector]public Transform myMojoTopTrans;
		public Color noChanceColor;
		public Color lowChanceColor;
		public Color midChanceColor;
		public Color highChanceColor;
		public Color bestChanceColor;
		public Vector3 fullBarScale;
		private Material ringMaterial;
		private Material coreMaterial;
		private float startYRot;
		public bool playerTeam;

		[HideInInspector]public bool EnableDebugMsgs = true;
		private bool isInitDone = false;

		void Awake()
		{
			StopCoroutine("initVars");
			StartCoroutine("initVars");

		}

		private IEnumerator initVars()
		{
			// Assign myBaller
			myBaller = GetComponentInParent<BBaller>();

			while (myBaller.MyTeam == null)
			{
				yield return null;
			}

			// Assign Renderer Core and Ring
			MeshRenderer[] rends = null;
			rends = GetComponentsInChildren<MeshRenderer>();

			foreach (MeshRenderer rend in rends)
			{
				int baseFound = 0;
				if (int.TryParse(rend.name[rend.name.Length-1].ToString(), out baseFound) == false )
				{
					if (EnableDebugMsgs)
						Debug.LogError("ShotIndicator.cs: Child Mesh Renderers not named properly.");
				}
				else
				{
					if (baseFound == 1)
						myRendererRing = rend;
					else if (baseFound == 2)
						myRendererCore = rend;
					else
					{
						if (EnableDebugMsgs)
							Debug.LogError("ShotIndicator.cs: Child Mesh Renderers not named properly.");
					}

				}
			}


			// Assign Transforms
			myCoreTransform = this.transform;
			if(myRendererCore != null)
				myMojoTopTrans = myRendererCore.transform;
			else
			{
				if (EnableDebugMsgs)
					Debug.LogError("ShotIndicator.cs: myRenderCore is null. Check Child Mesh Renderer presence/naming.");
			}

			ringMaterial = myRendererRing.material;
			coreMaterial = myRendererCore.material;
			
			if(myBaller.MyTeam == myBaller.MyTeam.cameraArm.playerTeam)
			{
				playerTeam = true;
				myRendererCore.enabled = true;
			}
			else
			{
				playerTeam = false;
				myRendererCore.enabled = false;
			}
			startYRot = myMojoTopTrans.localEulerAngles.y;

			isInitDone = true;

			yield break;
		}


		void Update()
		{
			if (!isInitDone)
				return;

			if(playerTeam && myBaller.HasBall)
				myRendererRing.enabled = true;
			else
				myRendererRing.enabled = false;

			Vector3 myMojoTopEuler = myMojoTopTrans.localEulerAngles;
			Vector3 myCoreEuler = myCoreTransform.eulerAngles;
			Vector3 myCoreScale = myCoreTransform.localScale;

			myCoreEuler.y = 0.0f;
			if(myBaller.mojoLevel >= 100.0f)
			{
				myRendererCore.material.SetFloat("_Cutoff", 0f);
				myMojoTopEuler.y += Time.deltaTime * 90.0f;
				if(myBaller.HasBall)
				{
					myCoreScale.x = 2.5f;
					myCoreScale.z = 2.5f;
				}
				else
				{
					myCoreScale.x = fullBarScale.x;
					myCoreScale.z = fullBarScale.z;
				}
			}
			else
			{
				myRendererCore.material.SetFloat("_Cutoff", (100.0f - (myBaller.mojoLevel)) / 100.1f);
				myMojoTopEuler.y = startYRot;
				myCoreScale.x = 2.5f;
				myCoreScale.z = 2.5f;
			}

			myMojoTopTrans.localEulerAngles = myMojoTopEuler;
			myCoreTransform.eulerAngles = myCoreEuler;
			myCoreTransform.localScale = myCoreScale;
		}

		void FixedUpdate()
		{
			Color targetColor;
			if(playerTeam && myBaller.HasBall)
			{
                float shotChance = 50.0f; // myBaller.ballerAI.parent.CalcShotChance();
				Vector2 ringTextureOffset = ringMaterial.mainTextureOffset;
				
				if(shotChance <= 25.0f)
					targetColor = Color.Lerp(noChanceColor, lowChanceColor, (shotChance * 4.0f) / 100.0f);
				else if(shotChance <= 50.0f)
					targetColor = Color.Lerp(lowChanceColor, midChanceColor, ((shotChance - 25.0f) * 4.0f) / 100.0f);
				else if (shotChance <= 75.0f)
					targetColor = Color.Lerp(midChanceColor, highChanceColor, ((shotChance - 50.0f) * 4.0f) / 100.0f);
				else
					targetColor = Color.Lerp(highChanceColor, bestChanceColor, ((shotChance - 75.0f) * 4.0f) / 100.0f);
				ringMaterial.color = targetColor;
				
				if(shotChance < 0f)
					ringTextureOffset.x = 0f;
				else if (shotChance > 100f)
					ringTextureOffset.x = .5f;
				else
					ringTextureOffset.x = (shotChance / 100f) * .5f;

				ringMaterial.mainTextureOffset = ringTextureOffset;
			}		

			//Debug.Log("Indicator Shot Chance: " + shotChance);
		}


	}
}