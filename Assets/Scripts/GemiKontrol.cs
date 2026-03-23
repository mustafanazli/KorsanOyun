using UnityEngine;
using System.Collections.Generic;

public class GemiKontrol : MonoBehaviour
{
    [Header("Gemi Hareket Ayarlarý")]
    public float maxHiz = 30f;
    public float ivme = 2f;
    public float yavaslama = 3f;
    public float donusHizi = 15f;

    [Header("Baðlantýlar ve UI")]
    public bool dumenBende = false;
    public GameObject dumenCikisYazisi;

    // SÝHÝR 1: Artýk bu yuvalarý gizledik çünkü kod bunlarý dümeni tutan kiþiye göre otomatik dolduracak
    private GameObject aktifOyuncu;
    private PlayerController aktifOyuncuKontrol;

    private float guncelHiz = 0f;
    private List<CharacterController> gemidekiOyuncular = new List<CharacterController>();
    private Vector3 oncekiPozisyon;
    private Quaternion oncekiDonus;

    void Start()
    {
        oncekiPozisyon = transform.position;
        oncekiDonus = transform.rotation;

        // --- UI RADARI: DÜMENDEN ÇIKIÞ YAZISINI OTOMATÝK BUL ---
        Canvas anaCanvas = FindAnyObjectByType<Canvas>();
        if (anaCanvas != null)
        {
            Transform[] tumUIObjeleri = anaCanvas.GetComponentsInChildren<Transform>(true);
            foreach (Transform obje in tumUIObjeleri)
            {
                // DÝKKAT: Hiyerarþideki isimle birebir ayný olmalý (ESC_Cikis_Yazisi gibi bir þey olabilir, sendekine göre düzelt!)
                if (obje.name == "ESC_Cikis_Yazisi") dumenCikisYazisi = obje.gameObject;
            }
        }

        if (dumenCikisYazisi != null) dumenCikisYazisi.SetActive(false);
    }

    void Update()
    {
        float dikeyGirdi = 0f;
        float yatayGirdi = 0f;

        if (dumenBende)
        {
            dikeyGirdi = Input.GetAxis("Vertical");
            yatayGirdi = Input.GetAxis("Horizontal");

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DumeniTutVeyaBirak(); // Çýkýþ yaparken mevcut oyuncuyu serbest býrakýr
            }
        }

        if (dikeyGirdi != 0)
        {
            guncelHiz += dikeyGirdi * ivme * Time.deltaTime;
            guncelHiz = Mathf.Clamp(guncelHiz, -maxHiz / 2f, maxHiz);
        }
        else
        {
            guncelHiz = Mathf.MoveTowards(guncelHiz, 0f, yavaslama * Time.deltaTime);
        }

        transform.Translate(Vector3.forward * guncelHiz * Time.deltaTime);

        if (dumenBende && yatayGirdi != 0)
        {
            transform.Rotate(Vector3.up, yatayGirdi * donusHizi * Time.deltaTime);
        }
    }

    void LateUpdate()
    {
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

    // SÝHÝR 2: Artýk dümeni kimin tuttuðunu dýþarýdan (InteractionManager'dan) parametre olarak alýyoruz
    public void DumeniTutVeyaBirak(GameObject basanOyuncu = null)
    {
        dumenBende = !dumenBende;

        if (dumenBende)
        {
            // Dümeni tutan oyuncuyu hafýzaya al
            if (basanOyuncu != null)
            {
                aktifOyuncu = basanOyuncu;
                aktifOyuncuKontrol = aktifOyuncu.GetComponent<PlayerController>();

                // Oyuncuyu yerine çivile
                if (aktifOyuncuKontrol != null) aktifOyuncuKontrol.enabled = false;
            }
            if (dumenCikisYazisi != null) dumenCikisYazisi.SetActive(true);
        }
        else
        {
            // Dümeni býraktýðýmýzda hafýzadaki oyuncuyu serbest býrak
            if (aktifOyuncuKontrol != null) aktifOyuncuKontrol.enabled = true;

            // Hafýzayý temizle
            aktifOyuncu = null;
            aktifOyuncuKontrol = null;

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