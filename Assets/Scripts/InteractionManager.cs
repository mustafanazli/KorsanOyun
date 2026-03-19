using UnityEngine;
using TMPro; // Yazýyý kodla deðiþtirmek için bu kütüphane þart!

public class InteractionManager : MonoBehaviour
{
    public float interactionDistance = 3f;
    public EnvanterKontrol envanterSistemi;

    [Header("Arayüz")]
    public GameObject etkilesimYazisiObjesi; // Inspector'daki F_Bas_Yazisi'ni buraya koyacaðýz
    private TextMeshProUGUI etkilesimMetni;

    void Start()
    {
        // Oyun baþlarken obje içindeki yazý bileþenini bulup hafýzaya alýyoruz
        if (etkilesimYazisiObjesi != null)
        {
            etkilesimMetni = etkilesimYazisiObjesi.GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        bool yaziAcikMi = false;
        string gosterilecekMetin = "";

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Varil"))
            {
                yaziAcikMi = true;
                gosterilecekMetin = "Ýçine Bak [F]"; // Varile bakýnca bu yazacak

                if (Input.GetKeyDown(KeyCode.F) && !envanterSistemi.envanterAcikMi)
                {
                    VarilIcerigi varil = hit.collider.GetComponent<VarilIcerigi>();
                    if (varil != null)
                    {
                        envanterSistemi.VarilPaneliniDoldur(varil);
                        envanterSistemi.VarilLootEkraniAc();
                    }
                }
            }
            else if (hit.collider.CompareTag("Dumen"))
            {
                GemiKontrol gemi = hit.collider.GetComponentInParent<GemiKontrol>();

                if (gemi != null && !gemi.dumenBende)
                {
                    yaziAcikMi = true;
                    gosterilecekMetin = "Gemiyi Sür [F]"; // Dümene bakýnca bu yazacak

                    if (Input.GetKeyDown(KeyCode.F) && !envanterSistemi.envanterAcikMi)
                    {
                        gemi.DumeniTutVeyaBirak();
                    }
                }
            }
            else if (hit.collider.CompareTag("Top"))
            {
                yaziAcikMi = true;
                gosterilecekMetin = "Topu Kullan [F]";

                if (Input.GetKeyDown(KeyCode.F) && !envanterSistemi.envanterAcikMi)
                {
                    TopSistemi top = hit.collider.GetComponentInParent<TopSistemi>();
                    PlayerController oyuncu = GetComponentInParent<PlayerController>();

                    if (top != null && oyuncu != null)
                    {
                        top.TopuKullanmayaBasla(oyuncu);

                        // ÝÞTE ÇILDIRMANI ENGELLEYECEK O SÝHÝRLÝ KELÝME:
                        // Bu komut, F'ye bastýðýmýz an Update döngüsünü anýnda iptal eder.
                        // Böylece kod aþaðýya inip yazýyý tekrar açamaz!
                        return;
                    }
                }
            }
            else if (hit.collider.CompareTag("Su"))
            {
                
            }

        }

        // --- YAZIYI EKRANDA GÜNCELLEME KISMI ---
        if (etkilesimYazisiObjesi != null)
        {
            etkilesimYazisiObjesi.SetActive(yaziAcikMi);

            // Eðer yazý açýksa ve içinde bir text bileþeni varsa, yazýyý anýnda deðiþtir
            if (yaziAcikMi && etkilesimMetni != null)
            {
                etkilesimMetni.text = gosterilecekMetin;
            }
        }   

    }

    // Bu yeni fonksiyon: Script kapatýldýðý (disable) an çalýþýr
    private void OnDisable()
    {
        // Script uyutulduðunda ekrandaki "Topu Kullan [F]" yazýsýný zorla kapatýyoruz!
        if (etkilesimYazisiObjesi != null)
        {
            etkilesimYazisiObjesi.SetActive(false);
        }
    }
}