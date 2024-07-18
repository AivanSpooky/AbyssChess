using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputHandler
{
    void ProcessInput(Vector3 pos, GameObject selectedObject, Action callback);
}
