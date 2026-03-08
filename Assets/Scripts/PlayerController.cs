using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Transform playerCamera;
    public Animator animator;

    [Header("Hareket Ayarlarý")]
    public float speed = 5f;
    public float climbSpeed = 4f;
    public float mouseSensitivity = 300f;

    [Header("Zýplama ve Normal Fizik")]
    public float jumpHeight = 2f;
    public float gravity = -19.62f;

    [Header("Yüzme Ayarlarý")]
    public float swimSpeed = 2.5f;
    public float swimUpSpeed = 3f;
    public float waterGravity = -1.5f;

    [Header("Hata Ayýklama (Durumlar)")]
    public bool isSwimming = false;
    public bool isClimbing = false;
    public bool wasGrounded; // Zemin kontrolünü Inspector'dan görebilmen için açtým

    private float xRotation = 0f;
    private Vector3 velocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // --- KAMERA KONTROLÜ ---
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // --- HAREKET GÝRDÝLERÝ ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 1. YÜZME MEKANÝÐÝ
        if (isSwimming)
        {
            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * swimSpeed * Time.deltaTime);

            if (Input.GetButton("Jump"))
            {
                velocity.y = swimUpSpeed;
            }
            else
            {
                velocity.y += waterGravity * Time.deltaTime;
                velocity.y = Mathf.Max(velocity.y, -4f);
            }

            controller.Move(velocity * Time.deltaTime);
        }
        // 2. TIRMANMA MEKANÝÐÝ
        else if (isClimbing)
        {
            velocity.y = 0f;
            Vector3 climbMove = transform.up * z;
            Vector3 horizontalMove = transform.right * x;
            controller.Move((climbMove + horizontalMove) * climbSpeed * Time.deltaTime);
        }
        // 3. NORMAL YÜRÜME MEKANÝÐÝ
        else
        {
            // --- GELÝÞMÝÞ ZEMÝN KONTROLÜ (Gemi Ýçin Çözüm) ---
            // Karakterin merkezinden aþaðýya, boyunun yarýsý + 0.2 metre ekstra mesafeye görünmez lazer atar.
            // Suyu (Trigger) görmezden gelir, sadece katý cisimleri (Gemiyi) zemin sayar.
            bool isRaycastGrounded = Physics.Raycast(controller.bounds.center, Vector3.down, controller.bounds.extents.y + 0.2f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            // Unity'nin kendi sensörü VEYA bizim lazerimiz yeri görüyorsa zýplamaya izin ver.
            wasGrounded = controller.isGrounded || isRaycastGrounded;

            if (wasGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }

            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * speed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && wasGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * 2f * Mathf.Abs(gravity));
                if (animator != null) animator.SetTrigger("Jump");
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }

        // --- ANÝMASYON ---
        if (animator != null)
        {
            float currentSpeed = new Vector2(x, z).magnitude;
            animator.SetFloat("Speed", currentSpeed);
        }
    }

    // --- TETÝKLEYÝCÝ ALANLARA GÝRÝÞ ÇIKIÞ ---
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Merdiven"))
        {
            isClimbing = true;
            velocity = Vector3.zero;
        }
        if (other.CompareTag("Su"))
        {
            isSwimming = true;
            velocity = Vector3.zero;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Merdiven")) isClimbing = false;
        if (other.CompareTag("Su")) isSwimming = false;

    }
    public void FizikleriSifirla()
    {
        isSwimming = false;
        isClimbing = false;
        velocity = Vector3.zero; // Üstünde biriken zýplama/düþme ivmesini sýfýrla
    }
}