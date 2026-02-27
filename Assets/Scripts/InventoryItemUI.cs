using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryItemUI : MonoBehaviour
{
    public Image ikonResmi;
    public TextMeshProUGUI miktarYazisi;
    public ItemData esyaVerisi;

    // YENÝ: Eþyanýn miktarýný matematiksel olarak aklýnda tutacak deðiþken
    public int miktar;

    public void SlotuGuncelle(ItemData data, int yeniMiktar)
    {
        esyaVerisi = data;
        miktar = yeniMiktar; // Miktarý hafýzaya al

        ikonResmi.sprite = data.ikon;
        miktarYazisi.text = miktar.ToString();

        // Miktar 1 ise yazýyý gizle
        miktarYazisi.gameObject.SetActive(miktar > 1);
    }
}