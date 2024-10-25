using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float crouchHeight = 1f; // ������ ��� ����������
    [SerializeField] private float normalHeight = 2f; // ������� ������
    [SerializeField] private float jumpForce = 10f; // ���� ������
    [SerializeField] private float gravityMultiplier = 2f;
    [SerializeField] private float pickupDistance = 2f;

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Camera playerCamera; // ������ �� ������ ������
    private float xRotation = 0f;  // ����������� �������� ������ �� ��� X

    private float currentSpeed;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private Vector3 cameraInitialPosition; // ��������� ������� ������ ������������ �������
    private bool isGrounded; // ��������, �� ����� �� �����


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        currentSpeed = walkSpeed;

        // ��������� ��������� ��������� ������
        cameraInitialPosition = playerCamera.transform.localPosition;

        // ��������� �������� �������
        rb.freezeRotation = true;

        // �������� ������
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        RotatePlayer(); // �������� ����������� � Update
        HandleCrouch(); // �������� �� ����������
        HandleJump(); // �������� �� ������
    }

    void FixedUpdate()
    {
        Move();
        ApplyGravity();// ���������� ����������� ����� FixedUpdate ��� ������������
    }

    void Move()
    {
        // �������� ���� �� ����
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // ����������� �������� ������������ ������� ������
        Vector3 moveDirection = transform.forward * moveVertical + transform.right * moveHorizontal;
        moveDirection.Normalize();

        // ��������� ���
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl)) // ��������� ����������
        {
            currentSpeed = crouchSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        // ������������� �������� Rigidbody ��� ���������� ���������
        rb.velocity = moveDirection * currentSpeed + new Vector3(0, rb.velocity.y, 0);

        // ���� ��� �����, �������� ��������
        if (moveDirection.magnitude == 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0); // ��������� ������������ ��������
        }
    }
    

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) // ��� ������ ����������
        {
            capsuleCollider.height = crouchHeight; // ��������� ������ �������
            capsuleCollider.center = new Vector3(0, -(normalHeight - crouchHeight) / 2, 0); // �������� ��������� ����

            // �������� ������ ����, ����� ��� ���������� �� ������ "������"
            playerCamera.transform.localPosition = new Vector3(cameraInitialPosition.x, cameraInitialPosition.y - (normalHeight - crouchHeight) / 2, cameraInitialPosition.z);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl)) // ����� ����� ��������� ���������
        {
            capsuleCollider.height = normalHeight; // ��������������� ������ �������
            capsuleCollider.center = Vector3.zero; // ����� ���������� ���������� �� �����

            // ���������� ������ �� �������� ���������
            playerCamera.transform.localPosition = cameraInitialPosition;
        }
    }

    private void HandleJump()
    {
        // �������� �� ������
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Jump()
    {
        // ��������� ���� � Rigidbody ��� ������
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; // ����� �� �� �����
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �������� �� ��������������� � �����
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // ����� �� �����
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // ��������, ����� ����� �������� �����
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false; // ����� �� �� �����
        }
    }

    private void ApplyGravity()
    {
        // ���������� ����������� ���� �������
        rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
    }

    void RotatePlayer()
    {
        // �������� ���� ����
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // ������� ������ ������ ��� Y (�� �����������)
        transform.Rotate(Vector3.up * mouseX);

        // ������� ������ ������ ��� X (�� ���������)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // ������������ ���� �������� ������

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }


}
