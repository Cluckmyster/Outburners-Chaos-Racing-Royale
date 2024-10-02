using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct MouseSensitivity
{
    public float horizontalSensitivity;
    public float verticalSensitivity;
}

public struct CameraRotation
{
    /// <summary>
    /// Works on camera X axis.
    /// </summary>
    public float pitch;

    /// <summary>
    /// Works on camera Y axis.
    /// </summary>
    public float yaw;

    /// <summary>
    /// Works on camera Z axis.
    /// </summary>
    public float roll;
}

[Serializable]
public struct CameraAngle
{
    public float minAngle;
    public float maxAngle;
}
