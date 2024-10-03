using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public struct MouseSensitivity
{
    [Tooltip("Mouse sensitivity on the X axis (horizontal movements).")]
    public float horizontalSensitivity;

    [Tooltip("Mouse sensitivity on the Y axis (vertical movements).")]
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
    [Tooltip("Minimum angle that camera can reach.")]
    public float minAngle;

    [Tooltip("Maximum angle that camera can reach.")]
    public float maxAngle;
}
