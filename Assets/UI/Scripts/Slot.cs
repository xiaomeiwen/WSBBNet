using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler {

	//-----change
	public bool isOccu = false;


	public GameObject card {
		get {
			if (transform.childCount > 0) {
				return transform.GetChild (0).gameObject;
			}
			return null;
		}
	}

	//-----change
	public bool isOccupied(){
		return isOccu;
	}

	public void slotOccupied(){
		isOccu = true;
	}
		

	#region IDropHandler implementation
	public void OnDrop (PointerEventData eventData)
	{
		if (!card) {
			DragCards.cardBeingDragged.transform.SetParent (transform);
			isOccu = true;
		}
	}
	#endregion
}
