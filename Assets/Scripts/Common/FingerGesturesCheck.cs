using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FingerGesturesCheck : MonoBehaviour
{
    private void Awake()
    {
        FingerGestures.GlobalTouchFilter = FingerGesturesGlobalFilter;
    }

    bool FingerGesturesGlobalFilter(int fingerIndex, Vector2 position)
    {
        return !ClickIsOverUI.instance.IsPointerOverUIObject(position);
    }
}
