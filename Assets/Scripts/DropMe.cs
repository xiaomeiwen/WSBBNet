using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropMe : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool bigContainer;
	public Image containerImage;
	public Image receivingImage;
	private Color normalColor;
	public Color highlightColor = Color.yellow;
    public GameObject cardReserve;

    //public GameObject item
    //{
    //    get
    //    {
    //        if(transform.childCount>0)
    //        {
    //            return transform.GetChild(0).gameObject;
    //        }
    //        return null;
    //    }
    //}
	
	public void OnEnable ()
	{
        if (!bigContainer)
        {
            if (containerImage != null)
                normalColor = containerImage.color;
        }
	}
	
	public void OnDrop(PointerEventData data)
	{
        if (!bigContainer)
        {
            containerImage.color = normalColor;

            DragMe d = data.pointerDrag.GetComponent<DragMe>();
            if (d != null)
            {
                d.startParent = this.transform;
            }
        }
        //if (!item)
        //{
        //    DragMe.itemBeingDragged.transform.SetParent(transform);
        //}
        //if (receivingImage == null)
        //    return;
		
        //Sprite dropSprite = GetDropSprite (data);
        //if (dropSprite != null)
        //    receivingImage.overrideSprite = dropSprite;
	}

	public void OnPointerEnter(PointerEventData data)
	{
        //if (containerImage == null)
        //    return;
		
        //Sprite dropSprite = GetDropSprite (data);
        //if (dropSprite != null)
        if (!bigContainer)
        {
            containerImage.color = highlightColor;
        }
        else
        {
        }
	}

	public void OnPointerExit(PointerEventData data)
	{
        //if (containerImage == null)
        //    return;
		
		containerImage.color = normalColor;
        DragMe d = data.pointerDrag.GetComponent<DragMe>();
        if (d != null)
        {
            d.startParent = cardReserve.transform;
        }
	}
	
    //private Sprite GetDropSprite(PointerEventData data)
    //{
    //    var originalObj = data.pointerDrag;
    //    if (originalObj == null)
    //        return null;

    //    var srcImage = originalObj.GetComponent<Image>();
    //    if (srcImage == null)
    //        return null;
		
    //    return srcImage.sprite;
    //}
}
