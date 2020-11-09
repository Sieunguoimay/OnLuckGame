using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OpenKeyboardOnPointerClickSupport : InputField
{
    TouchScreenKeyboard keyboard;
    
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
    
        if (keyboard == null || !keyboard.active)
        {
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.ASCIICapable, false, false, true);
        }
    }

}
