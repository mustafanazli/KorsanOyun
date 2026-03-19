using UnityEngine;

public class HaritaKontrol : MonoBehaviour
{
    [Header("Arayüz Baðlantýlarý")]
    public GameObject haritaPaneli;
    public EnvanterKontrol envanterSistemi;

    [Header("Harita Kamera Ayarlarý")]
    public Transform haritaKamerasi;
    public Transform takipEdilecekObje;
    public float suruklemeHizi = 0.5f;

    [Header("Harita Sýnýrlarý (Clamp)")]
    public float minX = -500f;
    public float maxX = 500f;
    public float minZ = -500f;
    public float maxZ = 500f;

    [Header("Yakýnlaþtýrma (Zoom) Ayarlarý")]
    public float zoomHizi = 50f; // Tekerlek hassasiyeti (Daha hýzlý zoom için artýrabilirsin)
    public float minZoom = 50f;  // En fazla ne kadar yaklaþsýn (Sayý KÜÇÜLDÜKÇE yaklaþýr)
    public float maxZoom = 250f; // En fazla ne kadar uzaklaþsýn (Sayý BÜYÜDÜKÇE uzaklaþýr)

    private bool haritaAcikMi = false;
    private Vector3 sonFarePozisyonu;
    private Camera camBileseni; // Kameranýn 'Size' ayarýna ulaþmak için

    void Start()
    {
        if (haritaPaneli != null) haritaPaneli.SetActive(false);

        // Oyun baþlarken kameranýn içindeki 'Camera' özelliðini bulup hafýzaya alýyoruz
        if (haritaKamerasi != null)
        {
            camBileseni = haritaKamerasi.GetComponent<Camera>();
        }
    }

    void Update()
    {
        if (envanterSistemi != null && envanterSistemi.envanterAcikMi) return;

        if (Input.GetKeyDown(KeyCode.M))
        {
            HaritayiAcKapa();
        }

        if (haritaAcikMi)
        {
            // --- 1. HARÝTA SÜRÜKLEME MEKANÝÐÝ ---
            if (Input.GetMouseButtonDown(0))
            {
                sonFarePozisyonu = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector3 fareFarki = Input.mousePosition - sonFarePozisyonu;
                Vector3 hareket = new Vector3(-fareFarki.x, 0, -fareFarki.y) * suruklemeHizi;

                if (haritaKamerasi != null)
                {
                    haritaKamerasi.Translate(hareket, Space.World);

                    Vector3 sinirliPozisyon = haritaKamerasi.position;
                    sinirliPozisyon.x = Mathf.Clamp(sinirliPozisyon.x, minX, maxX);
                    sinirliPozisyon.z = Mathf.Clamp(sinirliPozisyon.z, minZ, maxZ);
                    haritaKamerasi.position = sinirliPozisyon;
                }
                sonFarePozisyonu = Input.mousePosition;
            }

            // --- 2. YAKINLAÞTIRMA (ZOOM) MEKANÝÐÝ ---
            if (camBileseni != null)
            {
                // Farenin tekerlek hareketini algýla (Ýleri itince artý, geri çekince eksi deðer verir)
                float scrollGirdisi = Input.GetAxis("Mouse ScrollWheel");

                if (scrollGirdisi != 0)
                {
                    // Deðerleri eksi ile çarpýyoruz çünkü ileri ittirince yaklaþmasýný istiyoruz (Size deðeri küçülmeli)
                    camBileseni.orthographicSize -= scrollGirdisi * zoomHizi;

                    // Yakýnlaþma ve uzaklaþma sýnýrlarýný kilitle (Belli bir sýnýrdan sonra zoom dursun)
                    camBileseni.orthographicSize = Mathf.Clamp(camBileseni.orthographicSize, minZoom, maxZoom);
                }
            }
        }
    }

    public void HaritayiAcKapa()
    {
        haritaAcikMi = !haritaAcikMi;
        if (haritaPaneli != null) haritaPaneli.SetActive(haritaAcikMi);

        if (haritaAcikMi)
        {
            if (haritaKamerasi != null && takipEdilecekObje != null)
            {
                haritaKamerasi.position = new Vector3(takipEdilecekObje.position.x, haritaKamerasi.position.y, takipEdilecekObje.position.z);
            }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}