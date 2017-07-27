using UnityEngine;
using System.Collections;

public class ChangeUVTest : MonoBehaviour {

	public GameObject playerMesh;

	Mesh mesh;
	Vector2[] uvs;

	// Use this for initialization
	void Start () {

		mesh = playerMesh.GetComponent<MeshFilter>().mesh;
		uvs = mesh.uv;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnClick()
	{
		if (name == "Home") {

						for (int i = 0; i < playerMesh.GetComponent<SkinnedMeshRenderer> ().materials.Length; i++) {
								playerMesh.GetComponent<SkinnedMeshRenderer> ().materials [i].mainTextureOffset = new Vector2(0,0);
						}
				}
		else if(name == "Away")
		{
			uvs [0] = new Vector2 (0.25f, 0f);
			uvs [1] = new Vector2 (0.5f, 0f);
			uvs [2] = new Vector2 (0.25f, 1.0f);
			uvs [3] = new Vector2 (0.5f, 1.0f);
			//mesh.uv = uvs;
			for (int i = 0; i < playerMesh.GetComponent<SkinnedMeshRenderer> ().materials.Length; i++) 
				playerMesh.GetComponent<SkinnedMeshRenderer> ().materials [i].mainTextureOffset = uvs [0];
		}
	}

}
