using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float _x = Input.GetAxis("Mouse X");
        float _y = Input.GetAxis("Mouse Y");

        float mouseY = -1 * Mathf.Clamp(_y, -90.0f, 90.0f);

        player.transform.Rotate(new Vector3(0.0f, _x, 0.0f));
        transform.Rotate(new Vector3(mouseY, 0.0f, 0.0f));

    }
}
