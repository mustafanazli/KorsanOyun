using UnityEngine;

public class KovaSistemi : MonoBehaviour
{
    [Header("Kova Ayarlarý")]
    public GameObject kovaModeli; // Kameranýn altýndaki kova objesi
    public GameObject kovaIciSu; // Kovanýn içindeki mavi su objesi (Plane)
    public float suKapasitesi = 15f; // Tek seferde ambardan ne kadar su alacak
    public float etkilesimMesafesi = 3f;

    [Header("Referanslar")]
    public Camera oyuncuKamerasi;
    public GemiSuSistemi gemiSuSistemi; // Gemideki ana su sistemi
    public ParticleSystem suFirlatmaEfekti; // (Opsiyonel) Suyu atarken çýkacak efekt

    private bool kovaEldeMi = false;
    private bool kovaDoluMu = false;

    void Start()
    {
        // Oyun baþladýðýnda kova gizli ve boþ olmalý
        if (kovaModeli != null) kovaModeli.SetActive(false);
        if (kovaIciSu != null) kovaIciSu.SetActive(false);
    }

    void Update()
    {
        // B tuþu ile kovayý eline al / býrak
        if (Input.GetKeyDown(KeyCode.B))
        {
            KovayiKusanVeyaBirak();
        }

        // Kova eldeyse sol týk ile iþlem yap (Sea of Thieves tarzý)
        if (kovaEldeMi && Input.GetMouseButtonDown(0))
        {
            KovayiKullan();
        }
    }

    void KovayiKusanVeyaBirak()
    {
        kovaEldeMi = !kovaEldeMi;
        kovaModeli.SetActive(kovaEldeMi);

        // Eðer kovayý elimizden býrakýyorsak, içindeki suyu da dökülmüþ sayalým
        if (!kovaEldeMi && kovaDoluMu)
        {
            kovaDoluMu = false;
            if (kovaIciSu != null) kovaIciSu.SetActive(false);
        }
    }

    void KovayiKullan()
    {
        Ray ray = oyuncuKamerasi.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Kameranýn ortasýndan ileriye ýþýn yolluyoruz
        if (Physics.Raycast(ray, out hit, etkilesimMesafesi))
        {
            // Senaryo 1: Suya bakýyoruz ve kovamýz BOÞ -> Suyu Doldur
            if (hit.collider.CompareTag("Su") && !kovaDoluMu)
            {
                if (gemiSuSistemi != null && gemiSuSistemi.suSeviyesi > 0)
                {
                    gemiSuSistemi.SuyuBosalt(suKapasitesi); // Gemiden suyu eksilt
                    kovaDoluMu = true;
                    if (kovaIciSu != null) kovaIciSu.SetActive(true); // Kovadaki suyu göster
                }
            }
            // Senaryo 2: Kovamýz DOLU ve suya BAKMIYORUZ -> Suyu Dýþarý At
            else if (kovaDoluMu && !hit.collider.CompareTag("Su"))
            {
                SuyuDisariAt();
            }
        }
        else
        {
            // Senaryo 3: Hiçbir yere çarpmadýk (örneðin doðrudan gökyüzüne bakýyoruz) ve kovamýz DOLU -> Suyu Dýþarý At
            if (kovaDoluMu)
            {
                SuyuDisariAt();
            }
        }
    }

    void SuyuDisariAt()
    {
        kovaDoluMu = false;
        if (kovaIciSu != null) kovaIciSu.SetActive(false); // Kovadaki suyu gizle

        if (suFirlatmaEfekti != null)
        {
            suFirlatmaEfekti.Play();
        }

        Debug.Log("Su dýþarý fýrlatýldý! Þlap!");
    }
}