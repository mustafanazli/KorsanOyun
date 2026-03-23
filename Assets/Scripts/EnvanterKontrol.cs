using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnvanterKontrol : MonoBehaviour
{
    [Header("Paneller (OTOMATÝK BULUNACAK)")]
    public GameObject anaLootEkrani;
    public GameObject oyuncuEnvanterPaneli;
    public GameObject varilEnvanterPaneli;
    public GameObject hotbarPaneli;

    [Header("Hotbar Ayarlarý")]
    public Transform[] hotbarSlotlari = new Transform[0]; // KIRMIZI HATAYI ÖNLER
    public Color aktifSlotRengi = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color pasifSlotRengi = new Color(0.15f, 0.15f, 0.15f, 0.8f);
    public int seciliSlotIndex = 0;

    [Header("Prefablar")]
    public GameObject itemPrefab;

    private RectTransform oyuncuPanelRect;
    public bool envanterAcikMi = false;
    private VarilIcerigi acikVaril;

    void Start()
    {
        // --- SADECE 'Canvas' ÝSÝMLÝ ANA EKRANI BUL ---
        GameObject anaCanvas = GameObject.Find("Canvas");

        if (anaCanvas != null)
        {
            Transform[] tumUIObjeleri = anaCanvas.GetComponentsInChildren<Transform>(true);

            foreach (Transform obje in tumUIObjeleri)
            {
                if (obje.name == "OyuncuEnvanterPaneli") oyuncuEnvanterPaneli = obje.gameObject;
                else if (obje.name == "VarilEnvanterPaneli") varilEnvanterPaneli = obje.gameObject;
                else if (obje.name == "HotbarPaneli") hotbarPaneli = obje.gameObject;
                else if (obje.name == "AnaLootEkrani") anaLootEkrani = obje.gameObject;
            }
        }
        else
        {
            Debug.LogError("HATA: Sahnede 'Canvas' isminde obje bulunamadý!");
        }

        if (hotbarPaneli != null)
        {
            hotbarSlotlari = new Transform[hotbarPaneli.transform.childCount];
            for (int i = 0; i < hotbarPaneli.transform.childCount; i++)
            {
                hotbarSlotlari[i] = hotbarPaneli.transform.GetChild(i);
            }
            hotbarPaneli.SetActive(true);
        }

        if (oyuncuEnvanterPaneli != null)
        {
            oyuncuPanelRect = oyuncuEnvanterPaneli.GetComponent<RectTransform>();
        }

        SlotSec(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!envanterAcikMi) { envanterAcikMi = true; KendiCantamiziAc(); }
            else MenuyuKapat();
        }

        if (envanterAcikMi && Input.GetKeyDown(KeyCode.Escape)) MenuyuKapat();
        if (envanterAcikMi && Input.GetKeyDown(KeyCode.X)) MenuyuKapat();

        if (Input.GetKeyDown(KeyCode.Alpha1)) SlotSec(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SlotSec(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SlotSec(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SlotSec(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SlotSec(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SlotSec(5);
    }

    public void SlotSec(int index)
    {
        if (hotbarSlotlari == null || hotbarSlotlari.Length == 0) return;
        if (index >= hotbarSlotlari.Length) return;

        seciliSlotIndex = index;

        for (int i = 0; i < hotbarSlotlari.Length; i++)
        {
            Image slotArkaplani = hotbarSlotlari[i].GetComponent<Image>();
            if (slotArkaplani != null)
            {
                slotArkaplani.color = (i == seciliSlotIndex) ? aktifSlotRengi : pasifSlotRengi;
            }
        }
    }

    public ItemData SeciliHotbarEsyasiniGetir()
    {
        if (hotbarSlotlari.Length > seciliSlotIndex)
        {
            Transform aktifSlot = hotbarSlotlari[seciliSlotIndex];
            if (aktifSlot.childCount > 0)
            {
                InventoryItemUI ui = aktifSlot.GetChild(0).GetComponent<InventoryItemUI>();
                if (ui != null) return ui.esyaVerisi;
            }
        }
        return null;
    }

    public bool SeciliHotbarEsyasiniTuket(ItemData beklenenEsya)
    {
        if (hotbarSlotlari.Length > seciliSlotIndex)
        {
            Transform aktifSlot = hotbarSlotlari[seciliSlotIndex];
            if (aktifSlot.childCount > 0)
            {
                InventoryItemUI ui = aktifSlot.GetChild(0).GetComponent<InventoryItemUI>();
                if (ui != null && ui.esyaVerisi == beklenenEsya)
                {
                    ui.miktar--;
                    ui.SlotuGuncelle(ui.esyaVerisi, ui.miktar);
                    if (ui.miktar <= 0) Destroy(ui.gameObject);
                    return true;
                }
            }
        }
        return false;
    }

    public void VarilLootEkraniAc()
    {
        envanterAcikMi = true;
        if (anaLootEkrani != null) anaLootEkrani.SetActive(true);
        if (oyuncuEnvanterPaneli != null) oyuncuEnvanterPaneli.SetActive(true);
        if (varilEnvanterPaneli != null) varilEnvanterPaneli.SetActive(true);
        if (oyuncuPanelRect != null) oyuncuPanelRect.anchoredPosition = new Vector2(-250f, 0);
        FareyiAyirla(true);
    }

    public void VarilPaneliniDoldur(VarilIcerigi varil)
    {
        acikVaril = varil;
        if (varilEnvanterPaneli == null) return;

        EsyaYuvasi[] slotlar = varilEnvanterPaneli.GetComponentsInChildren<EsyaYuvasi>();

        foreach (var slot in slotlar) foreach (Transform child in slot.transform) Destroy(child.gameObject);

        for (int i = 0; i < varil.sandikIcerigi.Count; i++)
        {
            if (i < slotlar.Length)
            {
                GameObject yeniObj = Instantiate(itemPrefab, slotlar[i].transform);
                InventoryItemUI ui = yeniObj.GetComponent<InventoryItemUI>();
                ui.SlotuGuncelle(varil.sandikIcerigi[i].data, varil.sandikIcerigi[i].miktar);
                yeniObj.GetComponent<SuruklenebilirEsya>().asilYuva = slotlar[i].transform;
            }
        }
    }

    public void MenuyuKapat()
    {
        if (acikVaril != null && varilEnvanterPaneli != null)
        {
            acikVaril.sandikIcerigi.Clear();
            EsyaYuvasi[] varilSlotlari = varilEnvanterPaneli.GetComponentsInChildren<EsyaYuvasi>();

            foreach (var slot in varilSlotlari)
            {
                if (slot.transform.childCount > 0)
                {
                    InventoryItemUI ui = slot.transform.GetChild(0).GetComponent<InventoryItemUI>();
                    if (ui != null)
                    {
                        VarilIcerigi.EnvanterYuvasi kayit = new VarilIcerigi.EnvanterYuvasi();
                        kayit.data = ui.esyaVerisi;
                        kayit.miktar = ui.miktar;
                        acikVaril.sandikIcerigi.Add(kayit);
                    }
                }
            }
            acikVaril = null;
        }

        envanterAcikMi = false;
        if (anaLootEkrani != null) anaLootEkrani.SetActive(false);
        if (oyuncuEnvanterPaneli != null) oyuncuEnvanterPaneli.SetActive(false);
        if (varilEnvanterPaneli != null) varilEnvanterPaneli.SetActive(false);
        FareyiAyirla(false);
    }

    void FareyiAyirla(bool goster)
    {
        Cursor.lockState = goster ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = goster;
    }

    public void KendiCantamiziAc()
    {
        if (anaLootEkrani != null) anaLootEkrani.SetActive(true);
        if (varilEnvanterPaneli != null) varilEnvanterPaneli.SetActive(false);
        if (oyuncuPanelRect != null) oyuncuPanelRect.anchoredPosition = Vector2.zero;
        FareyiAyirla(true);
    }
}