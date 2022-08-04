using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float mouseSensitivity;
    [SerializeField] float jumpHeight;
    [SerializeField] float speed;
    Rigidbody rb;
    Camera cam;

    bool jump;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    Vector2 md;

    void FixedUpdate()
    {
        var movement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * speed;
        rb.velocity = transform.forward * movement.z + new Vector3(0, rb.velocity.y, 0);

        if (jump)
        {
            rb.AddForce(new Vector3(0, jumpHeight, 0));
            jump = false;
        }

        Vector2 mouseInputs = new Vector2(Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime, Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime);

        md += mouseInputs;

        cam.transform.localRotation = Quaternion.AngleAxis(-md.y, Vector3.right);
        transform.localRotation = Quaternion.AngleAxis(md.x, Vector3.up);
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }

    }
}
