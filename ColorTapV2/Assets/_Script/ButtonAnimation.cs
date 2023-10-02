using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimation : MonoBehaviour
{
    ButtonController _ButtonController;
    

    private void Awake()
    {
        _ButtonController = GetComponent<ButtonController>();
    }
}
