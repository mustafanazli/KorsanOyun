using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cam;
    public float gorunmeMesafesi = 3.5f; // Yazýnýn kaç metreden sonra görüneceði
    public GameObject yaziObjesi; // Canvas'ýn içindeki Text veya Canvas'ýn kendisi

    void Start()
    {
        cam = Camera.main.transform;

        // Baþlangýçta yazýyý gizle
        if (yaziObjesi != null) yaziObjesi.SetActive(false);
    }

    void LateUpdate()
    {
        // 1. Her zaman kameraya bakmasýný saðla
        transform.LookAt(transform.position + cam.forward);

        // 2. Mesafeyi ölç
        float mesafe = Vector3.Distance(transform.position, cam.position);

        // 3. Mesafeye göre aç veya kapat
        if (mesafe <= gorunmeMesafesi)
        {
            if (!yaziObjesi.activeSelf) yaziObjesi.SetActive(true);
        }
        else
        {
            if (yaziObjesi.activeSelf) yaziObjesi.SetActive(false);
        }
    }
}