using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverEvent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    public static HoverEvent event_Instance;
    public bool ishover = false;
    public bool isClick = false;
    public bool isEnter = false;
    void Start()
    {
        event_Instance = this;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        ishover = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       ishover = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
       isClick = true;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
       isEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isEnter = false;
    }
  
}
