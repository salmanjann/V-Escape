using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    private float sensX;
    private float sensY;

    public Transform orientation;

    private float x_rotation;
    private float y_rotation;

    private float mouseX;
    private float mouseY;

    // Start is called before the first frame update
    void Start()
    {
        sensX = 400f;
        sensY = 400f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        y_rotation += mouseX;
        x_rotation -= mouseY;
        x_rotation = Mathf.Clamp(x_rotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(x_rotation, y_rotation, 0);
        orientation.rotation = Quaternion.Euler(0, y_rotation, 0);
    }
}
