using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class InputPanel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    Vector2 lastPosition;
    Player player;
    Canvas canvas;
    void OnEnable()
    {
        player = FindObjectOfType<Player>(true);
        canvas = GetComponentInParent<Canvas>();
        //var eventSystem = FindObjectOfType<EventSystem>();
        //EventSystem.current.currentInputModule.
    }


    void LateUpdate()
    {
        player.InputX = 0f;
    }
  

    public void OnBeginDrag(PointerEventData eventData)
    {
        lastPosition = eventData.position;
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        player.InputX = (eventData.position - lastPosition).x / canvas.pixelRect.width;
        lastPosition = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        player.InputX = 0;
    }
}
