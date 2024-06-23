using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovment : MonoBehaviour
{
    public float speed = 5.0f;
    public float maxSpeed = 15f;
    public float jumpForce = 200f;
    public float interactRange = 5f;
    public float lookSensitivity = 1f;
    public float clampRange = 45f;
    public GameObject cam;
    //public GameObject jumpPoint;

    private Vector2 moveDirection = Vector2.zero;
    private Vector2 lookDirection = Vector2.zero;
    private float verticalLook = 0f;

    private Rigidbody rb;
    private PlayerInputActions playerControls;
    private InputAction move;
    private InputAction look;
    private InputAction jump;
    private bool hitSomething = false;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        move = playerControls.Player.Move;
        look = playerControls.Player.Look;
        jump = playerControls.Player.Jump;

        move.Enable();
        look.Enable();
        jump.Enable();
    }
    private void OnDisable()
    {
        move.Disable();
        look.Disable();
        jump.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
        lookDirection = look.ReadValue<Vector2>();
        RaycastHit hit;
        if (Keyboard.current.eKey.isPressed)
        {
            Debug.Log("fired raycast");
            hitSomething = Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, interactRange);
            if (hitSomething)
            {
                Debug.Log("hit something");
                string temp = hit.transform.gameObject.name;
                if(temp == "Left")
                {
                    hit.transform.parent.GetComponent<BoatControls>().left();
                }
                else if(temp == "Right")
                {
                    hit.transform.parent.GetComponent<BoatControls>().right();
                }
                else if (temp == "Straight")
                {
                    hit.transform.parent.GetComponent<BoatControls>().straight();
                }
                else if (temp == "Forward")
                {
                    hit.transform.parent.GetComponent<BoatControls>().accelerate();
                }
                else if (temp == "Backwards")
                {
                    hit.transform.parent.GetComponent<BoatControls>().decelerate();
                }
                else if (temp == "Anchor")
                {
                    hit.transform.parent.GetComponent<BoatControls>().anchor();
                }
            }
        }

        if (playerControls.Player.Jump.triggered)
        {
            Debug.Log("Jump");
            hitSomething = Physics.Raycast(transform.position, Vector3.down, out _, 1.5f);
            Debug.Log(hitSomething);
            if(hitSomething)
            { 
                rb.AddForce(Vector3.up * jumpForce);
            }
        }
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, lookDirection.x * lookSensitivity,0);
        verticalLook -= lookDirection.y * lookSensitivity;
        verticalLook = Mathf.Clamp(verticalLook, -clampRange, clampRange);
        cam.transform.localRotation = Quaternion.Euler(verticalLook, 0, 0);

        Vector3 temp= ((transform.right * moveDirection.x + transform.forward * moveDirection.y)).normalized * speed;
        temp.y = rb.velocity.y;
        rb.velocity = temp;

    }
}
