using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float crouchHeight = 1f; // Высота при приседании
    [SerializeField] private float normalHeight = 2f; // Обычная высота
    [SerializeField] private float jumpForce = 10f; // Сила прыжка
    [SerializeField] private float gravityMultiplier = 2f;
    [SerializeField] private float pickupDistance = 2f;

    [Header("Mouse Settings")]
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private Camera playerCamera; // Ссылка на камеру игрока
    private float xRotation = 0f;  // Ограничение вращения камеры по оси X

    private float currentSpeed;
    private Rigidbody rb;
    private CapsuleCollider capsuleCollider;

    private Vector3 cameraInitialPosition; // Начальная позиция камеры относительно капсулы
    private bool isGrounded; // Проверка, на земле ли игрок


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        currentSpeed = walkSpeed;

        // Сохраняем начальное положение камеры
        cameraInitialPosition = playerCamera.transform.localPosition;

        // Блокируем вращение капсулы
        rb.freezeRotation = true;

        // Скрываем курсор
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        RotatePlayer(); // Повороты управляются в Update
        HandleCrouch(); // Проверка на приседание
        HandleJump(); // Проверка на прыжок
    }

    void FixedUpdate()
    {
        Move();
        ApplyGravity();// Физическое перемещение через FixedUpdate для стабильности
    }

    void Move()
    {
        // Получаем ввод по осям
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Направление движения относительно взгляда игрока
        Vector3 moveDirection = transform.forward * moveVertical + transform.right * moveHorizontal;
        moveDirection.Normalize();

        // Проверяем бег
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = runSpeed;
        }
        else if (Input.GetKey(KeyCode.LeftControl)) // Проверяем приседание
        {
            currentSpeed = crouchSpeed;
        }
        else
        {
            currentSpeed = walkSpeed;
        }

        // Устанавливаем скорость Rigidbody для мгновенной остановки
        rb.velocity = moveDirection * currentSpeed + new Vector3(0, rb.velocity.y, 0);

        // Если нет ввода, обнуляем скорость
        if (moveDirection.magnitude == 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0); // Оставляем вертикальную скорость
        }
    }
    

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) // При начале приседания
        {
            capsuleCollider.height = crouchHeight; // Уменьшаем высоту капсулы
            capsuleCollider.center = new Vector3(0, -(normalHeight - crouchHeight) / 2, 0); // Сдвигаем коллайдер вниз

            // Сдвигаем камеру вниз, чтобы она оставалась на уровне "головы"
            playerCamera.transform.localPosition = new Vector3(cameraInitialPosition.x, cameraInitialPosition.y - (normalHeight - crouchHeight) / 2, cameraInitialPosition.z);
        }
        if (Input.GetKeyUp(KeyCode.LeftControl)) // Когда игрок перестает приседать
        {
            capsuleCollider.height = normalHeight; // Восстанавливаем высоту капсулы
            capsuleCollider.center = Vector3.zero; // Центр коллайдера возвращаем на место

            // Возвращаем камеру на исходное положение
            playerCamera.transform.localPosition = cameraInitialPosition;
        }
    }

    private void HandleJump()
    {
        // Проверка на прыжок
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Jump()
    {
        // Добавляем силу к Rigidbody для прыжка
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; // Игрок не на земле
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Проверка на соприкосновение с землёй
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Игрок на земле
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Проверка, когда игрок покидает землю
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false; // Игрок не на земле
        }
    }

    private void ApplyGravity()
    {
        // Применение увеличенной силы тяжести
        rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
    }

    void RotatePlayer()
    {
        // Получаем ввод мыши
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Вращаем игрока вокруг оси Y (по горизонтали)
        transform.Rotate(Vector3.up * mouseX);

        // Вращаем камеру вокруг оси X (по вертикали)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Ограничиваем угол поворота камеры

        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }


}
