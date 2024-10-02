using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    #region Variables

    [Tooltip("Target object for the camera.")]
    [SerializeField] private Transform target;

    // Stores the distance between player's car and camera
    private float distanceToPlayer;

    // Stores mouse sensitivity
    [SerializeField] private MouseSensitivity mouseSensitivity;

    // Stores camera rotation values
    private CameraRotation camRotation;

    // Stores minimum and maximum camera angle for clamp
    [SerializeField] private CameraAngle camAngle;

    #endregion

    #region Methods

    #region Unity Methods

    private void Awake()
    {
        // Initializes distance of camera from player
        distanceToPlayer = Vector3.Distance(transform.position, target.position);
    }
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Change camera Pitch/Yaw values depending of how players move their mouse
        camRotation.yaw += (Input.GetAxis("Mouse X") * mouseSensitivity.horizontalSensitivity) * Time.deltaTime;
        camRotation.pitch += (Input.GetAxis("Mouse Y") * mouseSensitivity.verticalSensitivity) * Time.deltaTime;

        // Clamp Pitch (X axis rotation) to not let it be unmanaged
        camRotation.pitch = Mathf.Clamp(camRotation.pitch, camAngle.minAngle, camAngle.maxAngle);
    }

    private void LateUpdate()
    {
        //transform.eulerAngles = new Vector3(camRotation.pitch, camRotation.yaw, 0);
        //transform.position = transform.position - transform.forward * distanceToPlayer;
        transform.RotateAround(target.position, Vector3.up, (Input.GetAxis("Mouse X") * mouseSensitivity.horizontalSensitivity) * Time.deltaTime);
        transform.RotateAround(target.position, Vector3.right, (Input.GetAxis("Mouse Y") * mouseSensitivity.verticalSensitivity) * Time.deltaTime);
        transform.LookAt(target);
    }

    #endregion

    #endregion
}
