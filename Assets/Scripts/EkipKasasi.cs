using UnityEngine;
using Mirror;
using TMPro;

public class EkipKasasi : NetworkBehaviour
{
    [Header("Arayüz Bağlantıları (OTOMATİK BULUNACAK)")]
    public TextMeshProUGUI sagUstAltinYazisi;

    [SyncVar(hook = nameof(OnAltinDegisti))]
    public int ortakKasa = 0;

    void Start()
    {
        // --- UI RADARI: ALTIN YAZISINI OTOMATİK BUL ---
        Canvas anaCanvas = FindAnyObjectByType<Canvas>();
        if (anaCanvas != null)
        {
            Transform[] tumUIObjeleri = anaCanvas.GetComponentsInChildren<Transform>(true);
            foreach (Transform obje in tumUIObjeleri)
            {
                // DİKKAT: Hiyerarşideki Altın yazısı objenin adı harfi harfine bu olmalı!
                if (obje.name == "AltinYazisiUI") sagUstAltinYazisi = obje.GetComponent<TextMeshProUGUI>();
            }
        }

        // Oyun başladığında sıfır da olsa ekrana yazdırsın
        if (sagUstAltinYazisi != null) sagUstAltinYazisi.text = ortakKasa.ToString() + " Altın ";
    }

    [Server]
    public void KasayaAltinEkle(int miktar)
    {
        ortakKasa += miktar;
        Debug.Log("Sunucu: Kasaya altın eklendi. Yeni Bakiye: " + ortakKasa);
    }

    private void OnAltinDegisti(int eskiMiktar, int yeniMiktar)
    {
        if (sagUstAltinYazisi != null)
        {
            sagUstAltinYazisi.text = yeniMiktar.ToString() + " Altın ";
        }
    }
}