using UnityEngine;
using TMPro;
using Mirror;

public class InteractionManager : NetworkBehaviour
{
    public float interactionDistance = 3f;
    public EnvanterKontrol envanterSistemi;

    [Header("Arayüz (OTOMATÝK BULUNACAK)")]
    public GameObject etkilesimYazisiObjesi;
    private TextMeshProUGUI etkilesimMetni;

    private Camera oyuncuKamerasi;

    [Header("Taþýma Sistemi")]
    public bool sandikTasiyorMu = false; // YENÝ: Karakterin sýrtýnda sandýk var mý?

    void Start()
    {
        if (envanterSistemi == null) envanterSistemi = GetComponent<EnvanterKontrol>();

        oyuncuKamerasi = GetComponentInChildren<Camera>();

        GameObject anaCanvas = GameObject.Find("Canvas");
        if (anaCanvas != null)
        {
            Transform[] tumUIObjeleri = anaCanvas.GetComponentsInChildren<Transform>(true);
            foreach (Transform obje in tumUIObjeleri)
            {
                if (obje.name == "F_Bas_Yazisi") etkilesimYazisiObjesi = obje.gameObject;
            }
        }

        if (etkilesimYazisiObjesi != null)
        {
            etkilesimMetni = etkilesimYazisiObjesi.GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        if (!isLocalPlayer) return;

        Ray ray;
        if (oyuncuKamerasi != null) ray = new Ray(oyuncuKamerasi.transform.position, oyuncuKamerasi.transform.forward);
        else ray = new Ray(transform.position, transform.forward);

        RaycastHit hit;
        bool yaziAcikMi = false;
        string gosterilecekMetin = "";

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Varil"))
            {
                yaziAcikMi = true;
                gosterilecekMetin = "Ýçine Bak [F]";

                if (Input.GetKeyDown(KeyCode.F) && envanterSistemi != null && !envanterSistemi.envanterAcikMi)
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
                    gosterilecekMetin = "Gemiyi Sür [F]";

                    if (Input.GetKeyDown(KeyCode.F) && envanterSistemi != null && !envanterSistemi.envanterAcikMi)
                    {
                        PlayerController asilAdam = GetComponentInParent<PlayerController>();

                        if (asilAdam != null)
                        {
                            gemi.DumeniTutVeyaBirak(asilAdam.gameObject);
                        }
                    }
                }
            }
            else if (hit.collider.CompareTag("Top"))
            {
                yaziAcikMi = true;
                gosterilecekMetin = "Topu Kullan [F]";

                if (Input.GetKeyDown(KeyCode.F) && envanterSistemi != null && !envanterSistemi.envanterAcikMi)
                {
                    TopSistemi top = hit.collider.GetComponentInParent<TopSistemi>();
                    PlayerController oyuncu = GetComponentInParent<PlayerController>();

                    if (top != null && oyuncu != null)
                    {
                        top.TopuKullanmayaBasla(oyuncu);
                        return;
                    }
                }
            }
            // --- SANDIK ALMA SÝSTEMÝ ---
            else if (hit.collider.CompareTag("Sandik"))
            {
                // Eðer zaten sýrtýmýzda sandýk yoksa yerden alabiliriz
                if (!sandikTasiyorMu)
                {
                    yaziAcikMi = true;
                    gosterilecekMetin = "Sandýðý Sýrtla [F]";

                    if (Input.GetKeyDown(KeyCode.F) && envanterSistemi != null && !envanterSistemi.envanterAcikMi)
                    {
                        sandikTasiyorMu = true; // Sandýðý aldýk!
                        Destroy(hit.collider.gameObject); // Sandýðý yerden sil
                    }
                }
            }
            // --- NPC ETKÝLEÞÝMÝ (SATIÞ VE GÖREV) ---
            else if (hit.collider.CompareTag("NPC"))
            {
                yaziAcikMi = true;

                // EÐER SIRTINDA SANDIK VARSA: SATIÞ MODU
                if (sandikTasiyorMu)
                {
                    gosterilecekMetin = "Sandýðý Sat (100 Altýn) [F]";

                    if (Input.GetKeyDown(KeyCode.F) && envanterSistemi != null && !envanterSistemi.envanterAcikMi)
                    {
                        sandikTasiyorMu = false; // Sandýðý verdik, sýrtýmýz boþaldý
                        CmdSandikSat(); // Sunucuya "Bana 100 altýn yaz!" emrini yolla
                    }
                }
                // EÐER SIRTINDA SANDIK YOKSA: NORMAL GÖREV/KONUÞMA MODU
                else
                {
                    gosterilecekMetin = "Konuþ [F]";

                    if (Input.GetKeyDown(KeyCode.F) && envanterSistemi != null && !envanterSistemi.envanterAcikMi)
                    {
                        NPCGorevSistemi npc = hit.collider.GetComponent<NPCGorevSistemi>();
                        if (npc != null)
                        {
                            npc.NPCIleKonus(); // O havalý görev paneli açýlýr
                        }
                    }
                }
            }
        }

        if (etkilesimYazisiObjesi != null)
        {
            etkilesimYazisiObjesi.SetActive(yaziAcikMi);

            if (yaziAcikMi && etkilesimMetni != null)
            {
                etkilesimMetni.text = gosterilecekMetin;
            }
        }
    }

    private void OnDisable()
    {
        if (etkilesimYazisiObjesi != null)
        {
            etkilesimYazisiObjesi.SetActive(false);
        }
    }

    // --- MULTÝPLAYER SÝHRÝ: ÝSTEMCÝDEN SUNUCUYA PARA ÝSTEÐÝ ---
    [Command]
    public void CmdSandikSat()
    {
        EkipKasasi kasa = FindAnyObjectByType<EkipKasasi>();
        if (kasa != null)
        {
            kasa.KasayaAltinEkle(100); // 100 Altýn direkt kasaya geçer ve herkeste güncellenir!
        }
    }
}