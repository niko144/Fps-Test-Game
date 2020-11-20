using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse_movement : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    public Transform playerBody;

    public float xrotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xrotation -= mouseY;
        xrotation = Mathf.Clamp(xrotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xrotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
