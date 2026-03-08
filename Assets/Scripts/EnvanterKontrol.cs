using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class EnvanterKontrol : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject anaLootEkrani;
    public GameObject oyuncuEnvanterPaneli;
    public GameObject varilEnvanterPaneli;
    public GameObject hotbarPaneli;

    [Header("Hotbar Ayarlarý")]
    public Transform[] hotbarSlotlari;
    public Color aktifSlotRengi = new Color(0.5f, 0.5f, 0.5f, 1f);
    public Color pasifSlotRengi = new Color(0.15f, 0.15f, 0.15f, 0.8f);
    public int seciliSlotIndex = 0;

    [Header("Prefablar")]
    public GameObject itemPrefab;

    private RectTransform oyuncuPanelRect;

    // 1. DEÐÝÞÝKLÝK: private olan bu deðiþkeni public yaptýk ki diðer kodlar çantanýn açýk olduðunu bilsin!
    public bool envanterAcikMi = false;

    private VarilIcerigi acikVaril;

    void Start()
    {
        oyuncuPanelRect = oyuncuEnvanterPaneli.GetComponent<RectTransform>();
        if (hotbarPaneli != null) hotbarPaneli.SetActive(true);
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
        anaLootEkrani.SetActive(true);
        oyuncuEnvanterPaneli.SetActive(true);
        varilEnvanterPaneli.SetActive(true);
        oyuncuPanelRect.anchoredPosition = new Vector2(-250f, 0);
        FareyiAyirla(true);
    }

    public void VarilPaneliniDoldur(VarilIcerigi varil)
    {
        acikVaril = varil;
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
        if (acikVaril != null)
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
        anaLootEkrani.SetActive(false);
        oyuncuEnvanterPaneli.SetActive(false);
        varilEnvanterPaneli.SetActive(false);
        FareyiAyirla(false);
    }

    void FareyiAyirla(bool goster)
    {
        Cursor.lockState = goster ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = goster;
    }

    public void KendiCantamiziAc()
    {
        anaLootEkrani.SetActive(true);
        varilEnvanterPaneli.SetActive(false);
        oyuncuPanelRect.anchoredPosition = Vector2.zero;
        FareyiAyirla(true);
    }
}