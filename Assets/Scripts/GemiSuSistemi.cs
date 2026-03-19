using UnityEngine;

public class GemiSuSistemi : MonoBehaviour
{
    [Header("Su Ayarlarý")]
    public float suSeviyesi = 0f;
    public float maxSuSeviyesi = 100f;
    public float suDolmaHizi = 5f;
    public bool firtinadayiz = false;

    [Header("Görsel Su Ayarlarý")]
    public Transform icSuObjesi;
    public float minSuYuksekligi = -2f;
    public float maxSuYuksekligi = 1.5f;

    [Header("Hava Durumu Ayarlarý")]
    public Light gunesIsigi; // Oyundaki Directional Light (Güneþ)
    public ParticleSystem yagmurEfekti; // Yaptýðýmýz yaðmur
    public Color normalGokyuzuRengi = Color.white; // Güneþli havadaki ýþýk rengi
    public Color firtinaGokyuzuRengi = new Color(0.2f, 0.2f, 0.3f); // Fýrtýnadaki koyu gri/mavi renk
    public float normalIsikGucu = 1f;
    public float firtinaIsikGucu = 0.2f;
    public float havaGecisHizi = 0.5f; // Havanýn kararma/açýlma hýzý

    void Start()
    {
        if (icSuObjesi != null) icSuObjesi.gameObject.SetActive(false);
    }

    void Update()
    {
        // 1. Su Dolma Mantýðý
        if (firtinadayiz && suSeviyesi < maxSuSeviyesi)
        {
            suSeviyesi += suDolmaHizi * Time.deltaTime;
            SuSeviyesiniGuncelle();
            if (suSeviyesi >= maxSuSeviyesi) GemiBatti();
        }

        // 2. Sinematik Hava Durumu Geçiþi (Sürekli yavaþça güncellenir)
        if (gunesIsigi != null)
        {
            if (firtinadayiz)
            {
                // Iþýðý yavaþça kýs ve rengini grileþtir
                gunesIsigi.intensity = Mathf.Lerp(gunesIsigi.intensity, firtinaIsikGucu, Time.deltaTime * havaGecisHizi);
                gunesIsigi.color = Color.Lerp(gunesIsigi.color, firtinaGokyuzuRengi, Time.deltaTime * havaGecisHizi);
            }
            else
            {
                // Iþýðý yavaþça eski haline getir
                gunesIsigi.intensity = Mathf.Lerp(gunesIsigi.intensity, normalIsikGucu, Time.deltaTime * havaGecisHizi);
                gunesIsigi.color = Color.Lerp(gunesIsigi.color, normalGokyuzuRengi, Time.deltaTime * havaGecisHizi);
            }
        }
    }

    public void SuyuBosalt(float miktar)
    {
        suSeviyesi -= miktar;
        if (suSeviyesi <= 0) suSeviyesi = 0;
        SuSeviyesiniGuncelle();
    }

    void SuSeviyesiniGuncelle()
    {
        if (icSuObjesi != null)
        {
            if (suSeviyesi > 0) icSuObjesi.gameObject.SetActive(true);
            else icSuObjesi.gameObject.SetActive(false);

            float dolulukOrani = suSeviyesi / maxSuSeviyesi;
            float yeniY = Mathf.Lerp(minSuYuksekligi, maxSuYuksekligi, dolulukOrani);
            icSuObjesi.localPosition = new Vector3(icSuObjesi.localPosition.x, yeniY, icSuObjesi.localPosition.z);
        }
    }

    void GemiBatti()
    {
        Debug.Log("GEMÝ BATTI! KAPTAN GEMÝSÝYLE BÝRLÝKTE SULARA GÖMÜLDÜ!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Firtina"))
        {
            firtinadayiz = true;
            if (yagmurEfekti != null) yagmurEfekti.Play(); // Yaðmuru baþlat
            Debug.Log("Fýrtýna alanýna girildi!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Firtina"))
        {
            firtinadayiz = false;
            if (yagmurEfekti != null) yagmurEfekti.Stop(); // Yaðmuru durdur
            Debug.Log("Fýrtýna alanýndan çýkýldý!");
        }
    }
}