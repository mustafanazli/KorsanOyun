using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TopSistemi : MonoBehaviour
{
    [Header("Top Parçalarý")]
    public Transform atisNoktasi;
    public Transform namlu;
    public Camera topKamerasi;

    [Header("Sistem Baðlantýlarý")]
    public GameObject gullePrefab;
    public EnvanterKontrol envanterSistemi;
    public ItemData gulleVerisi;

    [Header("Top Ayarlarý")]
    public float atisGucu = 1000f;

    [Header("Niþan Alma Ayarlarý")]
    public float dikeyDonusHizi = 2f;
    public float maxYukariAci = 25f;
    public float maxAsagiAci = 10f;

    // YENÝ: Yatay (Sað/Sol) dönüþ ayarlarý
    public float yatayDonusHizi = 2f;
    public float maxSagaDonus = 40f;
    public float maxSolaDonus = 40f;

    public float doldurmaSuresi = 3f;

    [Header("Arayüz (UI)")]
    public GameObject kullanimArayuzu;
    public Image doldurmaBari;

    [Header("Efektler")]
    public ParticleSystem atesEfekti;

    private bool kullaniliyor = false;
    private bool dolu = false;
    private bool dolduruluyor = false;

    // YENÝ: Baþlangýçtan itibaren ne kadar hareket ettiðimizi tutan deðiþkenler
    private float dikeyOffset = 0f;
    private float yatayOffset = 0f;
    private Vector3 baslangicRotasyonu;

    private PlayerController aktifOyuncu;
    private Camera oyuncuKamerasi;
    private InteractionManager oyuncuEtkilesim;

    void Start()
    {
        if (topKamerasi != null) topKamerasi.gameObject.SetActive(false);
        if (kullanimArayuzu != null) kullanimArayuzu.SetActive(false);
        if (doldurmaBari != null) doldurmaBari.fillAmount = 0f;

        if (namlu != null)
        {
            // Topun haritadaki ilk duruþunu hafýzaya alýyoruz
            baslangicRotasyonu = namlu.localEulerAngles;
        }
    }

    void Update()
    {
        if (!kullaniliyor) return;

        if (!dolduruluyor && namlu != null)
        {
            // Fare hareketlerini al
            float mouseY = Input.GetAxis("Mouse Y") * dikeyDonusHizi;
            float mouseX = Input.GetAxis("Mouse X") * yatayDonusHizi; // YENÝ

            // Sapma (Offset) deðerlerini güncelle
            dikeyOffset -= mouseY;
            yatayOffset += mouseX; // YENÝ

            // Sýnýrlarý belirle ki top kendi etrafýnda fýrýldak gibi dönmesin
            dikeyOffset = Mathf.Clamp(dikeyOffset, -maxYukariAci, maxAsagiAci);
            yatayOffset = Mathf.Clamp(yatayOffset, -maxSolaDonus, maxSagaDonus); // YENÝ

            // Namluyu hem X (Dikey) hem Y (Yatay) ekseninde, baþlangýç açýsýna ekleyerek döndür
            namlu.localRotation = Quaternion.Euler(
                baslangicRotasyonu.x + dikeyOffset,
                baslangicRotasyonu.y + yatayOffset,
                baslangicRotasyonu.z
            );
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TopuBirak();
        }

        if (Input.GetKeyDown(KeyCode.E) && !dolu && !dolduruluyor)
        {
            if (envanterSistemi.SeciliHotbarEsyasiniTuket(gulleVerisi))
            {
                StartCoroutine(DoldurmaRutini());
            }
            else
            {
                Debug.Log("Gülle doldurmak için eline (Hotbar) bir gülle almalýsýn!");
            }
        }

        if (Input.GetKeyDown(KeyCode.F) && dolu && !dolduruluyor)
        {
            AtesEt();
        }
    }

    public void TopuKullanmayaBasla(PlayerController oyuncu)
    {
        kullaniliyor = true;
        aktifOyuncu = oyuncu;

        aktifOyuncu.enabled = false;

        oyuncuEtkilesim = aktifOyuncu.GetComponentInChildren<InteractionManager>();
        if (oyuncuEtkilesim != null)
        {
            oyuncuEtkilesim.enabled = false;
            if (oyuncuEtkilesim.etkilesimYazisiObjesi != null)
                oyuncuEtkilesim.etkilesimYazisiObjesi.SetActive(false);
        }

        oyuncuKamerasi = aktifOyuncu.playerCamera.GetComponent<Camera>();
        if (oyuncuKamerasi != null) oyuncuKamerasi.gameObject.SetActive(false);

        if (topKamerasi != null) topKamerasi.gameObject.SetActive(true);
        if (kullanimArayuzu != null) kullanimArayuzu.SetActive(true);
    }

    private void TopuBirak()
    {
        kullaniliyor = false;

        if (aktifOyuncu != null) aktifOyuncu.enabled = true;

        if (oyuncuEtkilesim != null) oyuncuEtkilesim.enabled = true;

        if (oyuncuKamerasi != null) oyuncuKamerasi.gameObject.SetActive(true);

        if (topKamerasi != null) topKamerasi.gameObject.SetActive(false);
        if (kullanimArayuzu != null) kullanimArayuzu.SetActive(false);

        if (dolduruluyor)
        {
            StopAllCoroutines();
            dolduruluyor = false;
            if (doldurmaBari != null) doldurmaBari.fillAmount = 0f;
        }
    }

    IEnumerator DoldurmaRutini()
    {
        dolduruluyor = true;
        float gecenSure = 0f;

        while (gecenSure < doldurmaSuresi)
        {
            gecenSure += Time.deltaTime;
            if (doldurmaBari != null) doldurmaBari.fillAmount = gecenSure / doldurmaSuresi;
            yield return null;
        }

        dolu = true;
        dolduruluyor = false;
        if (doldurmaBari != null) doldurmaBari.fillAmount = 0f;
        Debug.Log("Top Atýþa Hazýr!");
    }

    private void AtesEt()
    {
        dolu = false;

        if (atesEfekti != null) atesEfekti.Play();

        GameObject gulle = Instantiate(gullePrefab, atisNoktasi.position, atisNoktasi.rotation);
        Rigidbody rb = gulle.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(atisNoktasi.forward * atisGucu);
        }

        // Geri tepme: Top ateþlenince namlu biraz yukarý kalkar
        dikeyOffset -= 5f;

        Destroy(gulle, 5f);
    }
}