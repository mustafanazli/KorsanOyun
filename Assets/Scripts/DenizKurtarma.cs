using UnityEngine;

public class DenizdenKurtarma : MonoBehaviour
{
    [Header("Ayarlar")]
    public float beklemeSuresi = 5f; // Kaç saniye suda kalýnca yazý çýksýn?

    [Header("Baðlantýlar")]
    public PlayerController oyuncuKontrol; // Yüzme durumunu buradan çekeceðiz
    public Transform gemiDogmaNoktasi;     // Gemide ýþýnlanacaðýmýz yer
    public GameObject kurtarmaYazisi;      // Ekrana çýkacak "U'ya bas" UI yazýsý

    private float denizdeGecenSure = 0f;
    private bool yaziAcikMi = false;

    void Start()
    {
        // Oyun baþýnda yazý kapalý olsun
        if (kurtarmaYazisi != null) kurtarmaYazisi.SetActive(false);
    }

    void Update()
    {
        // SADECE oyuncu yüzüyorsa süreyi saymaya baþla
        if (oyuncuKontrol.isSwimming)
        {
            denizdeGecenSure += Time.deltaTime;

            // Süre dolduysa ve yazý henüz açýlmadýysa yazýyý ekranda göster
            if (denizdeGecenSure >= beklemeSuresi && !yaziAcikMi)
            {
                yaziAcikMi = true;
                if (kurtarmaYazisi != null) kurtarmaYazisi.SetActive(true);
            }

            // Yazý ekrandayken U tuþuna basýlýrsa gemiye ýþýnlan
            if (yaziAcikMi && Input.GetKeyDown(KeyCode.U))
            {
                GemiyeDon();
            }
        }
        else
        {
            // Suda deðilsek (karadaysak veya gemideysek) kronometreyi ve yazýyý sýfýrla
            if (denizdeGecenSure > 0 || yaziAcikMi)
            {
                denizdeGecenSure = 0f;
                yaziAcikMi = false;
                if (kurtarmaYazisi != null) kurtarmaYazisi.SetActive(false);
            }
        }
    }

    void GemiyeDon()
    {
        // Karakterin fiziðini geçici kapat
        oyuncuKontrol.controller.enabled = false;

        // Karakteri gemideki güvenli noktaya ýþýnla
        transform.position = gemiDogmaNoktasi.position;

        // SÝHÝRLÝ DOKUNUÞ: Unity'nin suda kaldýk sanmasýný engelle!
        oyuncuKontrol.FizikleriSifirla();

        // Fiziði geri aç
        oyuncuKontrol.controller.enabled = true;

        // Deðerleri sýfýrla
        denizdeGecenSure = 0f;
        yaziAcikMi = false;
        if (kurtarmaYazisi != null) kurtarmaYazisi.SetActive(false);

        Debug.Log("Kaptan gemiye geri döndü ve fizikler sýfýrlandý!");
    }
}