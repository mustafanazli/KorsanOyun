using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController controller;
    public Transform playerCamera;
    public Animator animator; // Ýþte aradýðýmýz boþluðu açacak kod bu!

    public float speed = 5f;
    public float mouseSensitivity = 300f;

    private float xRotation = 0f;

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

        // --- HAREKET KONTROLÜ ---
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);
        controller.Move(new Vector3(0, -9.81f, 0) * Time.deltaTime);

        // --- ANÝMASYON KONTROLÜ ---
        if (animator != null)
        {
            // WASD'ye basma þiddetimize göre animatöre hýz verisi yolluyoruz
            float currentSpeed = move.magnitude;
            animator.SetFloat("Speed", currentSpeed);
        }
    }
}