using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Click : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public delegate void OnPointerEventCallback();
    public OnPointerEventCallback onPointerDownCallback = null;
    public OnPointerEventCallback onPointerUpCallback = null;
    public void OnPointerDown(PointerEventData eventData)
    {
        // Do action
        if (onPointerDownCallback != null)
            onPointerDownCallback();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Do action
        if (onPointerUpCallback != null)
            onPointerUpCallback();
    }
}