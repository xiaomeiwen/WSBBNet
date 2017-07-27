using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragMe : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public bool dragOnSurfaces = true;
	
    //New Edits
    public static GameObject itemBeingDragged;
    Vector3 startPos;
    public Transform startParent = null;

    //public Transform cardReserve;


    private GameObject m_DraggingIcon;
    private RectTransform m_DraggingPlane;

	public void OnBeginDrag(PointerEventData eventData)
	{

		var canvas = FindInParents<Canvas>(gameObject);
		if (canvas == null)
			return;

        itemBeingDragged = gameObject;
        startPos = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;

        transform.SetParent(canvas.transform, false);
        transform.SetAsLastSibling();

        var image = gameObject.GetComponent<Image>();
        image.SetNativeSize();
        
        //// We have clicked something that can be dragged.
        //// What we want to do is create an icon for this.
        //m_DraggingIcon = new GameObject("icon");

        //m_DraggingIcon.transform.SetParent (canvas.transform, false);
        //m_DraggingIcon.transform.SetAsLastSibling();
		
		//var image = m_DraggingIcon.AddComponent<Image>();
		// The icon will be under the cursor.
		// We want it to be ignored by the event system.
		//CanvasGroup group = gameObject.AddComponent<CanvasGroup>();
		//group.blocksRaycasts = false;

        //image.sprite = GetComponent<Image>().sprite;
        //image.SetNativeSize();
		
        //if (dragOnSurfaces)
        //    m_DraggingPlane = transform as RectTransform;
        //else
        //    m_DraggingPlane = canvas.transform as RectTransform;
		
        //SetDraggedPosition(eventData);
	}

	public void OnDrag(PointerEventData data)
	{
        //if (m_DraggingIcon != null)
        //    SetDraggedPosition(data);

        transform.position = data.position;
	}

    //private void SetDraggedPosition(PointerEventData data)
    //{
    //    if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
    //        m_DraggingPlane = data.pointerEnter.transform as RectTransform;
		
    //    var rt = m_DraggingIcon.GetComponent<RectTransform>();
    //    Vector3 globalMousePos;
    //    if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
    //    {
    //        rt.position = globalMousePos;
    //        rt.rotation = m_DraggingPlane.rotation;
    //    }
    //}

	public void OnEndDrag(PointerEventData eventData)
	{
        //if (m_DraggingIcon != null)
        //    Destroy(m_DraggingIcon);

        itemBeingDragged = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        transform.SetParent(startParent);
        //if (transform.parent != startParent)
        //{
        //    transform.position = startPos;
        //}
	}

	static public T FindInParents<T>(GameObject go) where T : Component
	{
		if (go == null) return null;
		var comp = go.GetComponent<T>();

		if (comp != null)
			return comp;
		
		Transform t = go.transform.parent;
		while (t != null && comp == null)
		{
			comp = t.gameObject.GetComponent<T>();
			t = t.parent;
		}
		return comp;
	}
}
