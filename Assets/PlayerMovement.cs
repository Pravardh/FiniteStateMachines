using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 8.0f;
    // Start is called before the first frame update
    CharacterController cc;
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float dirz = Input.GetAxis("Horizontal");   
        float diry = Input.GetAxis("Vertical");

        cc.Move(transform.forward * diry * speed * Time.deltaTime);
        cc.Move(transform.right * dirz * speed * Time.deltaTime);

        cc.Move(transform.up * 1.0f * -9.81f);
    }
}
