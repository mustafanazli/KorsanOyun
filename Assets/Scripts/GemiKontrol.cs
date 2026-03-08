using UnityEngine;
using System.Collections.Generic;

public class GemiKontrol : MonoBehaviour
{
    [Header("Gemi Hareket Ayarlarý")]
    public float maxHiz = 30f;       // Çýkabileceði en yüksek hýz
    public float ivme = 2f;          // Hýzlanma gücü (Bunu düþürürsen gemi daha geç hýzlanýr)
    public float yavaslama = 3f;     // Tuþu býrakýnca durma süresi (Sürtünme)
    public float donusHizi = 15f;

    [Header("Baðlantýlar ve UI")]
    public bool dumenBende = false;
    public GameObject oyuncu;
    public PlayerController oyuncuKontrol;
    public GameObject dumenCikisYazisi;

    private float guncelHiz = 0f;    // Geminin anlýk gerçek hýzý

    private List<CharacterController> gemidekiOyuncular = new List<CharacterController>();
    private Vector3 oncekiPozisyon;
    private Quaternion oncekiDonus;

    void Start()
    {
        oncekiPozisyon = transform.position;
        oncekiDonus = transform.rotation;
        if (dumenCikisYazisi != null) dumenCikisYazisi.SetActive(false);
    }

    void Update()
    {
        float dikeyGirdi = 0f;
        float yatayGirdi = 0f;

        // Sadece dümen bizdeyse klavye tuþlarýný oku
        if (dumenBende)
        {
            dikeyGirdi = Input.GetAxis("Vertical");   // W ve S tuþlarý
            yatayGirdi = Input.GetAxis("Horizontal"); // A ve D tuþlarý

            // Dümenden çýkýþ
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DumeniTutVeyaBirak();
            }
        }

        // --- ÝVME VE YAVAÞLAMA MATEMATÝÐÝ (SÝHÝRLÝ KISIM) ---
        if (dikeyGirdi != 0)
        {
            // Tuþa basýyorsak ivmelenerek hýzlan (Ýleri basýyorsa artý, geri basýyorsa eksi yönde ekler)
            guncelHiz += dikeyGirdi * ivme * Time.deltaTime;

            // Hýzý maksimum sýnýrlar içinde tut (Geri gitme hýzýný maxHiz'in yarýsý yaptýk ki daha gerçekçi olsun)
            guncelHiz = Mathf.Clamp(guncelHiz, -maxHiz / 2f, maxHiz);
        }
        else
        {
            // Tuþa basmýyorsak (veya dümeni býraktýysak) hýzý yavaþ yavaþ 0'a doðru çek (Fren/Suyun Sürtünmesi)
            guncelHiz = Mathf.MoveTowards(guncelHiz, 0f, yavaslama * Time.deltaTime);
        }

        // Gemiyi hesaplanan güncel hýzla hareket ettir
        transform.Translate(Vector3.forward * guncelHiz * Time.deltaTime);

        // Dönüþ (Sadece dümen bizdeyken ve tuþa basýyorsak döner)
        if (dumenBende && yatayGirdi != 0)
        {
            transform.Rotate(Vector3.up, yatayGirdi * donusHizi * Time.deltaTime);
        }
    }

    void LateUpdate()
    {
        // MULTIPLAYER YOLCU FÝZÝÐÝ (Buraya dokunulmadý)
        Vector3 hareketFarki = transform.position - oncekiPozisyon;
        Quaternion donusFarki = transform.rotation * Quaternion.Inverse(oncekiDonus);

        foreach (CharacterController yolcu in gemidekiOyuncular)
        {
            if (yolcu != null)
            {
                yolcu.Move(hareketFarki);
                Vector3 merkezdenUzaklik = yolcu.transform.position - transform.position;
                Vector3 yeniUzaklik = donusFarki * merkezdenUzaklik;
                yolcu.Move(yeniUzaklik - merkezdenUzaklik);
                yolcu.transform.rotation = donusFarki * yolcu.transform.rotation;
            }
        }

        oncekiPozisyon = transform.position;
        oncekiDonus = transform.rotation;
    }

    public void DumeniTutVeyaBirak()
    {
        dumenBende = !dumenBende;

        if (dumenBende)
        {
            oyuncuKontrol.enabled = false;
            if (dumenCikisYazisi != null) dumenCikisYazisi.SetActive(true);
        }
        else
        {
            oyuncuKontrol.enabled = true;
            if (dumenCikisYazisi != null) dumenCikisYazisi.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        CharacterController binen = other.GetComponent<CharacterController>();
        if (binen != null && !gemidekiOyuncular.Contains(binen)) gemidekiOyuncular.Add(binen);
    }

    void OnTriggerExit(Collider other)
    {
        CharacterController atlayan = other.GetComponent<CharacterController>();
        if (atlayan != null && gemidekiOyuncular.Contains(atlayan)) gemidekiOyuncular.Remove(atlayan);
    }
}