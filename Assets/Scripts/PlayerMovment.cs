using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovment : MonoBehaviour
{
    public float speed = 5.0f;
    public float maxSpeed = 15f;
    public float lookSensitivity = 1f;
    public float clampRange = 45f;
    public GameObject cam;

    private Vector2 moveDirection = Vector2.zero;
    private Vector2 lookDirection = Vector2.zero;
    private float verticalLook = 0f;

    private Rigidbody rb;
    private PlayerInputActions playerControls;
    private InputAction move;
    private InputAction look;
    private InputAction fire;

    private void Awake()
    {
        playerControls = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        move = playerControls.Player.Move;
        look = playerControls.Player.Look;
        fire = playerControls.Player.Fire;

        move.Enable();
        look.Enable();
        fire.Enable();
    }
    private void OnDisable()
    {
        move.Disable();
        look.Disable();
        fire.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = move.ReadValue<Vector2>();
        lookDirection = look.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        transform.Rotate(0, lookDirection.x * lookSensitivity,0);
        verticalLook -= lookDirection.y * lookSensitivity;
        verticalLook = Mathf.Clamp(verticalLook, -clampRange, clampRange);
        cam.transform.localRotation = Quaternion.Euler(verticalLook, 0, 0);

        Vector3 temp= ((transform.right * moveDirection.x + transform.forward * moveDirection.y)).normalized * speed;
        temp *= Time.deltaTime;
        temp.y = rb.velocity.y;
        rb.velocity = temp;

    }
}
