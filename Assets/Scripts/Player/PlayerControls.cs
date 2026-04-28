using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5.0f;
    
    [Header("Key Bindings")]
    public KeyCode moveUp = KeyCode.W;
    public KeyCode moveDown = KeyCode.S;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;

    [Header("References")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Camera mainCamera;

    private Vector2 _movementInput;
    private Vector2 _mouseWorldPosition;

    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        GetInput();

        _mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }

    void FixedUpdate()
    {
        ApplyMovement();
        AimAtMouse();
    }

    private void GetInput()
    {
        _movementInput.x = 0;
        _movementInput.y = 0;

        if (Input.GetKey(moveRight)) _movementInput.x = 1;
        else if (Input.GetKey(moveLeft)) _movementInput.x = -1;

        if (Input.GetKey(moveUp)) _movementInput.y = 1;
        else if (Input.GetKey(moveDown)) _movementInput.y = -1;

        // Normalize to prevent faster diagonal movement
        if (_movementInput.magnitude > 1)
        {
            _movementInput.Normalize();
        }
    }

    private void ApplyMovement()
    {
        rb.linearVelocity = _movementInput * moveSpeed;
    }

    private void AimAtMouse()
    {
        Vector2 lookDirection = _mouseWorldPosition - rb.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        rb.rotation = angle;
    }
}
