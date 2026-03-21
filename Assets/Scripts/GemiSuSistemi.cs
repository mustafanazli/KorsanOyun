using UnityEngine;

public class GemiSuSistemi : MonoBehaviour
{
    [Header("Su Ayarlarý")]
    public float suSeviyesi = 0f;
    public float maxSuSeviyesi = 100f;
    public float suDolmaHizi = 1f; // Fýrtýnanýn temel su verme hýzý
    public bool firtinadayiz = false;

    [Header("Hasar Ayarlarý")]
    public int aktifDelikSayisi = 0; // Þu an gemide kaç delik var?
    public float delikBasinaSuHizi = 8f; // Her bir delik saniyede ne kadar su ekler?

    // ÝÞTE EKSÝK OLAN KISIM BURASIYDI:
    public GameObject delikPrefab; // Assets'teki Mavi Küp Þablonumuz
    public Transform[] delikNoktalari; // Gemiye dizdiðimiz Nokta_1, Nokta_2'ler
    public float delikAcilmaSuresi = 10f; // Fýrtýnada kaç saniyede bir delik açýlsýn?
    private float delikZamanlayici = 0f;

    [Header("Görsel Su Ayarlarý")]
    public Transform icSuObjesi;
    public float minSuYuksekligi = -2f;
    public float maxSuYuksekligi = 1.5f;

    [Header("Hava Durumu Ayarlarý")]
    public Light gunesIsigi;
    public ParticleSystem yagmurEfekti;
    public Color normalGokyuzuRengi = Color.white;
    public Color firtinaGokyuzuRengi = new Color(0.2f, 0.2f, 0.3f);
    public float normalIsikGucu = 1f;
    public float firtinaIsikGucu = 0.2f;
    public float havaGecisHizi = 0.5f;

    void Start()
    {
        if (icSuObjesi != null) icSuObjesi.gameObject.SetActive(false);
    }

    void Update()
    {
        // 1. FIRTINADA RASTGELE DELÝK AÇMA MANTIÐI
        if (firtinadayiz)
        {
            delikZamanlayici += Time.deltaTime;
            if (delikZamanlayici >= delikAcilmaSuresi)
            {
                RastgeleDelikAc();
                delikZamanlayici = 0f; // Sayacý sýfýrla, yeni delik için bekle
            }
        }

        // 2. GELÝÞMÝÞ SU DOLMA MANTIÐI
        if ((firtinadayiz || aktifDelikSayisi > 0) && suSeviyesi < maxSuSeviyesi)
        {
            float anlikFirtinaHizi = firtinadayiz ? suDolmaHizi : 0;
            float toplamDolmaHizi = anlikFirtinaHizi + (aktifDelikSayisi * delikBasinaSuHizi);

            suSeviyesi += toplamDolmaHizi * Time.deltaTime;
            SuSeviyesiniGuncelle();

            if (suSeviyesi >= maxSuSeviyesi) GemiBatti();
        }

        // 3. HAVA DURUMU
        HavaDurumunuYonet();
    }

    void RastgeleDelikAc()
    {
        // Eðer nokta listemiz boþsa veya þablon yoksa iþlem yapma
        if (delikNoktalari.Length == 0 || delikPrefab == null) return;

        // Rastgele bir noktanýn numarasýný seç
        int rastgeleIndex = Random.Range(0, delikNoktalari.Length);
        Transform secilenNokta = delikNoktalari[rastgeleIndex];

        // Eðer o noktada zaten bir delik varsa pas geç (üst üste delik açýlmasýn)
        if (secilenNokta.childCount > 0) return;

        // O noktada deliði yarat ve "secilenNokta"nýn çocuðu yap
        Instantiate(delikPrefab, secilenNokta.position, secilenNokta.rotation, secilenNokta);

        aktifDelikSayisi++;
        Debug.Log("GÖVDEDE DELÝK AÇILDI! SU ALIYORUZ!");
    }

    void HavaDurumunuYonet()
    {
        if (gunesIsigi != null)
        {
            float hedefIsik = firtinadayiz ? firtinaIsikGucu : normalIsikGucu;
            Color hedefRenk = firtinadayiz ? firtinaGokyuzuRengi : normalGokyuzuRengi;

            gunesIsigi.intensity = Mathf.Lerp(gunesIsigi.intensity, hedefIsik, Time.deltaTime * havaGecisHizi);
            gunesIsigi.color = Color.Lerp(gunesIsigi.color, hedefRenk, Time.deltaTime * havaGecisHizi);
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
            if (yagmurEfekti != null) yagmurEfekti.Play();
            Debug.Log("Fýrtýna alanýna girildi!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Firtina"))
        {
            firtinadayiz = false;
            if (yagmurEfekti != null) yagmurEfekti.Stop();
            Debug.Log("Fýrtýna alanýndan çýkýldý!");
        }
    }
}