using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DragCards : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {
	public static GameObject cardBeingDragged;
	Vector3 startPosition;
	float posX, posY;
	Transform startParent;
	public GameObject panelSelectedCard;// = GameObject.Find("PanelSelectedCard");
	bool isDroped;

	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		cardBeingDragged = gameObject;
//		transform.localScale = new Vector3 (0.4f, 0.4f, 1.0f);
//		Debug.Log("card: " + transform.position);
		startPosition = Camera.main.WorldToScreenPoint(transform.position);
//		Debug.Log("cardworld: " + startPosition);
		posX = Input.mousePosition.x - startPosition.x;
		posY = Input.mousePosition.y - startPosition.y;
		startParent = transform.parent;
		GetComponent<CanvasGroup> ().blocksRaycasts = false;
//		Debug.Log (Camera.main.WorldToScreenPoint(panelSelectedCard.transform.position));
//		Debug.Log (panelSelectedCard.GetComponent<RectTransform>().rect.width);
//		Debug.Log (panelSelectedCard.GetComponent<RectTransform>().rect.height);
		isDroped = false;
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{	
		
		Vector3 curPos = new Vector3(Input.mousePosition.x - posX, Input.mousePosition.y - posY, startPosition.z);
		Vector3 worldPos = Camera.main.ScreenToWorldPoint (curPos);
		if (worldPos.x > 250) {
			transform.localScale = new Vector3 (0.4f, 0.4f, 1.0f);
		} else {
			transform.localScale = new Vector3 (1f, 1f, 1.0f);

		}
			
			
		transform.position = worldPos;
		GameObject cards = GameObject.Find ("Cards");
		transform.SetParent (cards.transform);
//		BoxCollider2D cardsSelectedPanel = panelSelectedCard.GetComponent<BoxCollider2D> ();
//		Debug.Log(transform.GetComponent<BoxCollider2D>().IsTouching(cardsSelectedPanel));
//		Debug.Log("cardsSelectedPanel " + cardsSelectedPanel.isTrigger);



	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		cardBeingDragged = null;
		GetComponent<CanvasGroup> ().blocksRaycasts = true;
		Vector3 lastStayPos = Camera.main.ScreenToWorldPoint (startPosition);;	
		float slotXOffset = 3.5f; //GameObject.Find ("1SelectedCard1").GetComponent<RectTransform>().rect.width;
		float slotYOffset = 4.8f; //GameObject.Find ("1SelectedCard1").GetComponent<RectTransform>().rect.height;
		for (int i = 1; i < 5; i++) {
			for (int j = 1; j < 4; j++) {
				GameObject slots = GameObject.Find (i + "SelectedCard" + j);
				//bool test = slots.GetComponent (Slot).isOccupied ();



				Vector3 slotPos = slots.transform.position;
//				Debug.Log((slotPos.x - slotXOffset) + ", " + (slotPos.x + slotXOffset) + ", " + (slotPos.y - slotYOffset) + ", " + (slotPos.y + slotYOffset));
//				slots.transform.childCount == 0 && 
				if (transform.position.x >= slotPos.x - slotXOffset && transform.position.x <= slotPos.x + slotXOffset
				    && transform.position.y >= slotPos.y - slotYOffset && transform.position.y <= slotPos.y + slotYOffset) {
					transform.position = slotPos;
					transform.SetParent (slots.transform);
					isDroped = true;

					//------changed
					bool test = slots.GetComponent<Slot> ().isOccupied ();
					if (test == false) {
						slots.GetComponent<Slot> ().slotOccupied ();

					}

					Debug.Log ("slot occupied:"+test);

					print (transform.position + "slot pos:" + slotPos);
				} else {
				}
			}
		}
		if (!isDroped) {
			transform.position = lastStayPos;
		}
		print (isDroped);
	}

	#endregion



//	private bool _mouseState;
//	private GameObject target;
//	public Vector3 screenSpace;
//	public Vector3 offset;
//
//	public void dragCards ()
//	{
//		 Debug.Log(_mouseState);
//		if (Input.GetMouseButtonDown (0)) {
//
//			RaycastHit hitInfo;
//			target = GetClickedObject (out hitInfo);
//			Debug.Log ("target" + target);
//			if (target != null) {
//				_mouseState = true;
//				screenSpace = Camera.main.WorldToScreenPoint (target.transform.position);
//				offset = target.transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenSpace.z));
//			}
//		}
//		if (Input.GetMouseButtonUp (0)) {
//			_mouseState = false;
//		}
//		if (_mouseState) {
//			//keep track of the mouse position
//			Vector3 curScreenSpace = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenSpace.z);
//
//			//convert the screen mouse position to world point and adjust with offset
//			Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenSpace) + offset;
//
//			//update the position of the object in the world
//			target.transform.position = curPosition;
//		}
//	}
//
//
//	GameObject GetClickedObject (out RaycastHit hit)
//	{
//		GameObject target = null;
//		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
//		if (Physics.Raycast (ray.origin, ray.direction * 10, out hit)) {
//			target = hit.collider.gameObject;
//		}
//
//		return target;
//	}
//	Vector3 dist;
//	float posX;
//	float posY;

//	void OnMouseDown() {
//		dist = Camera.main.WorldToScreenPoint (transform.position);
//		posX = Input.mousePosition.x - dist.x;
//		posY = Input.mousePosition.y - dist.y;
//	}
//
//	void OnMouseDrag() {
//		Vector3 curPos = new Vector3 (Input.mousePosition.x - posX, Input.mousePosition.y - posY, dist.z);
//		Vector3 worldPos = Camera.main.ScreenToWorldPoint (curPos);
//		transform.position = worldPos;
//	}
}

