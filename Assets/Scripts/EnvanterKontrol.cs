using UnityEngine;
using UnityEngine.UI;

public class EnvanterKontrol : MonoBehaviour
{
    [Header("Paneller")]
    public GameObject anaLootEkrani;
    public GameObject oyuncuEnvanterPaneli;
    public GameObject varilEnvanterPaneli;

    [Header("Prefablar")]
    public GameObject itemPrefab;

    private RectTransform oyuncuPanelRect;
    private bool envanterAcikMi = false;

    void Start()
    {
        oyuncuPanelRect = oyuncuEnvanterPaneli.GetComponent<RectTransform>();
    }

    public void VarilLootEkraniAc()
    {
        envanterAcikMi = true;
        anaLootEkrani.SetActive(true);
        oyuncuEnvanterPaneli.SetActive(true);
        varilEnvanterPaneli.SetActive(true);
        oyuncuPanelRect.anchoredPosition = new Vector2(-250f, 0); // Biraz daha içerde dursun
        FareyiAyirla(true);
    }

    public void VarilPaneliniDoldur(VarilIcerigi varil)
    {
        EsyaYuvasi[] slotlar = varilEnvanterPaneli.GetComponentsInChildren<EsyaYuvasi>();

        // Eski eþyalarý temizle
        foreach (var slot in slotlar)
        {
            foreach (Transform child in slot.transform) Destroy(child.gameObject);
        }

        // Yeni eþyalarý slotlara diz
        for (int i = 0; i < varil.sandikIcerigi.Count; i++)
        {
            if (i < slotlar.Length)
            {
                GameObject yeniObj = Instantiate(itemPrefab, slotlar[i].transform);
                InventoryItemUI ui = yeniObj.GetComponent<InventoryItemUI>();
                ui.SlotuGuncelle(varil.sandikIcerigi[i].data, varil.sandikIcerigi[i].miktar);

                // Sürükleme baþladýðýnda bu slota dönebilmesi için:
                yeniObj.GetComponent<SuruklenebilirEsya>().asilYuva = slotlar[i].transform;
            }
        }
    }

    public void MenuyuKapat()
    {
        envanterAcikMi = false;

        // Ekranda ne var ne yoksa hepsini kapatýyoruz!
        anaLootEkrani.SetActive(false);
        oyuncuEnvanterPaneli.SetActive(false);
        varilEnvanterPaneli.SetActive(false);

        // Fareyi tekrar oyun moduna (gizli ve kilitli) döndür
        FareyiAyirla(false);
    }

    void FareyiAyirla(bool goster)
    {
        Cursor.lockState = goster ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = goster;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!envanterAcikMi) { envanterAcikMi = true; KendiCantamiziAc(); }
            else MenuyuKapat();
        }

        // Menü açýkken ESC veya I ile kapatma
        if (envanterAcikMi && Input.GetKeyDown(KeyCode.Escape)) MenuyuKapat();

        // --- SENÝN ÝSTEDÝÐÝN KISIM ---
        // Eðer menülerden herhangi biri açýksa ve ESC'ye basýlýrsa her þeyi yok et
        if (envanterAcikMi && Input.GetKeyDown(KeyCode.X))
        {
            MenuyuKapat();
        }
    }

    public void KendiCantamiziAc()
    {
        anaLootEkrani.SetActive(true);
        varilEnvanterPaneli.SetActive(false);
        oyuncuPanelRect.anchoredPosition = Vector2.zero;
        FareyiAyirla(true);
    }
}