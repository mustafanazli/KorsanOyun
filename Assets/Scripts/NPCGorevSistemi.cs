using UnityEngine;
using UnityEngine.UI;
using Mirror; // MULTIPLAYER SİHRİ BURADA

public class NPCGorevSistemi : NetworkBehaviour // Artık MonoBehaviour değil!
{
    [Header("Arayüz (UI)")]
    public GameObject gorevPaneli;

    [Header("Görev Butonları")]
    public Button gorev1Buton;
    public Button gorev2Buton;
    public Button gorev3Buton;
    public Button gorev4Buton;
    public Button gorev5Buton;

    public enum AktifGorev { Yok, Ada1_Sandik, Ada2_Iskelet, Ada3_Loot, Ada4_Altin, Efsanevi }

    [Header("Görev Takibi (AĞ ÜZERİNDEN ORTAK)")]
    // [SyncVar] sayesinde bu değişken sunucuda değiştiğinde TÜM oyuncuların ekranında anında değişir.
    [SyncVar(hook = nameof(OnGorevDurumuDegisti))]
    public AktifGorev suAnkiGorev = AktifGorev.Yok;

    // Görev seviyesi arttığında herkesin buton kilitleri aynı anda açılır
    [SyncVar(hook = nameof(OnSeviyeDegisti))]
    public int acikOlanSeviye = 1;

    [Header("Görev Sayaçları")]
    [SyncVar] public int kesilenIskelet = 0;
    public int gerekenIskelet = 5;
    public int gerekenAltin = 5000;

    [SyncVar] public bool sandikGetirildi = false;

    void Start()
    {
        if (gorevPaneli != null) gorevPaneli.SetActive(false);
        ButonlariGuncelle(acikOlanSeviye);
    }

    // --- UI KONTROLLERİ (Sadece o an NPC ile konuşan oyuncuda çalışır) ---
    public void NPCIleKonus()
    {
        if (gorevPaneli != null)
        {
            bool panelAcikMi = gorevPaneli.activeSelf;
            gorevPaneli.SetActive(!panelAcikMi);

            if (!panelAcikMi)
            {
                // Paneli açtığında sunucuya "Görev bitti mi kontrol et" diyoruz
                CmdGorevTeslimKontrolu();

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

    // Oyuncu UI butonuna tıkladığında bu çalışır, asıl işi Sunucuya (Host) devreder
    public void ButondanGorevSec(int gorevNo)
    {
        if (suAnkiGorev != AktifGorev.Yok)
        {
            Debug.Log("Kaptan, zaten ekibin aktif bir görevi var!");
            return;
        }

        // Sunucuya "Şu görevi başlat" emri veriyoruz (Herkesin görevi değişir)
        CmdGorevBaslat(gorevNo);

        NPCIleKonus(); // Menüyü kapat
    }

    // --- SUNUCU KOMUTLARI (Tüm ekibi etkileyecek hile korumalı işlemler) ---

    // requiresAuthority = false: Tayfadaki herhangi bir oyuncu bu NPC'ye emir verebilir demek.
    [Command(requiresAuthority = false)]
    public void CmdGorevBaslat(int gorevNo)
    {
        if (suAnkiGorev != AktifGorev.Yok) return;

        if (gorevNo == 1 && acikOlanSeviye >= 1) suAnkiGorev = AktifGorev.Ada1_Sandik;
        else if (gorevNo == 2 && acikOlanSeviye >= 2) suAnkiGorev = AktifGorev.Ada2_Iskelet;
        else if (gorevNo == 3 && acikOlanSeviye >= 3) suAnkiGorev = AktifGorev.Ada3_Loot;
        else if (gorevNo == 4 && acikOlanSeviye >= 4) suAnkiGorev = AktifGorev.Ada4_Altin;
        else if (gorevNo == 5 && acikOlanSeviye >= 5) suAnkiGorev = AktifGorev.Efsanevi;
    }

    [Command(requiresAuthority = false)]
    public void CmdGorevTeslimKontrolu()
    {
        if (suAnkiGorev == AktifGorev.Ada1_Sandik && sandikGetirildi)
        {
            EkipKasasinaParaEkle(500);
            acikOlanSeviye = 2;
            suAnkiGorev = AktifGorev.Yok;
            sandikGetirildi = false;
        }
        else if (suAnkiGorev == AktifGorev.Ada2_Iskelet && kesilenIskelet >= gerekenIskelet)
        {
            EkipKasasinaParaEkle(1000);
            acikOlanSeviye = 3;
            suAnkiGorev = AktifGorev.Yok;
            kesilenIskelet = 0;
        }
        else if (suAnkiGorev == AktifGorev.Ada3_Loot)
        {
            EkipKasasinaParaEkle(1500);
            acikOlanSeviye = 4;
            suAnkiGorev = AktifGorev.Yok;
        }
        else if (suAnkiGorev == AktifGorev.Ada4_Altin)
        {
            EkipKasasi kasa = Object.FindAnyObjectByType<EkipKasasi>();
            if (kasa != null && kasa.ortakKasa >= gerekenAltin)
            {
                acikOlanSeviye = 5;
                suAnkiGorev = AktifGorev.Yok;
            }
        }
    }

    [Command(requiresAuthority = false)]
    public void SandigiSirtla()
    {
        if (suAnkiGorev == AktifGorev.Ada1_Sandik)
        {
            sandikGetirildi = true;
        }
    }

    [Server]
    public void IskeletKestik()
    {
        if (suAnkiGorev == AktifGorev.Ada2_Iskelet)
        {
            kesilenIskelet++;
        }
    }

    [Server]
    private void EkipKasasinaParaEkle(int miktar)
    {
        EkipKasasi kasa = Object.FindAnyObjectByType<EkipKasasi>();
        if (kasa != null) kasa.KasayaAltinEkle(miktar);
    }

    // --- HOOKS (Değerler değiştiğinde herkesin bilgisayarında otomatik çalışan kısım) ---

    private void OnGorevDurumuDegisti(AktifGorev eskiGorev, AktifGorev yeniGorev)
    {
        if (yeniGorev != AktifGorev.Yok)
        {
            Debug.Log("EKİP BİLDİRİMİ: Tayfa yeni bir görev aldı -> " + yeniGorev.ToString());
            // İleride buraya ekranın ortasında herkesin göreceği "YENİ GÖREV BAŞLADI" yazısı ekleyeceğiz!
        }
        else if (eskiGorev != AktifGorev.Yok && yeniGorev == AktifGorev.Yok)
        {
            Debug.Log("EKİP BİLDİRİMİ: Görev tamamlandı! Ödüller kasaya eklendi.");
            // BAHSETTİĞİN UPGRADE'LERİ İLERİDE BURAYA YAZACAĞIZ (Can artırma, kılıç güçlendirme vb.)
        }
    }

    private void OnSeviyeDegisti(int eskiSeviye, int yeniSeviye)
    {
        ButonlariGuncelle(yeniSeviye);
        Debug.Log("EKİP BİLDİRİMİ: Yeni görev seviyesinin kilidi açıldı! Güncel Seviye: " + yeniSeviye);
    }

    private void ButonlariGuncelle(int seviye)
    {
        if (gorev1Buton != null) gorev1Buton.interactable = (seviye >= 1);
        if (gorev2Buton != null) gorev2Buton.interactable = (seviye >= 2);
        if (gorev3Buton != null) gorev3Buton.interactable = (seviye >= 3);
        if (gorev4Buton != null) gorev4Buton.interactable = (seviye >= 4);
        if (gorev5Buton != null) gorev5Buton.interactable = (seviye >= 5);
    }
}