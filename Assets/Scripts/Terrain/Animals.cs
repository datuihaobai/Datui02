using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animals : MonoBehaviour 
{
    public enum ElementType
    {
        Fire,
        Wood,
        Sand,
    }

    public GameObject facade;
    public GameObject nestEffect;
    public ElementType elementType;
}