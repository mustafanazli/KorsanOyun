using UnityEngine;
using System.Collections.Generic;

public class VarilIcerigi : MonoBehaviour
{
    public List<ItemData> olasiEsyalar; // Inspector'dan Muz, Tahta, Gülle'yi buraya ekle

    [System.Serializable]
    public class EnvanterYuvasi
    {
        public ItemData data;
        public int miktar;
    }

    public List<EnvanterYuvasi> sandikIcerigi = new List<EnvanterYuvasi>();

    void Start()
    {
        // Oyun baþýnda varili rastgele doldur
        int kacFarkliEsya = Random.Range(1, 4); // 1 ile 3 arasý farklý türde eþya

        for (int i = 0; i < kacFarkliEsya; i++)
        {
            EnvanterYuvasi yeniEsya = new EnvanterYuvasi();
            yeniEsya.data = olasiEsyalar[Random.Range(0, olasiEsyalar.Count)];
            yeniEsya.miktar = Random.Range(3, 15);
            sandikIcerigi.Add(yeniEsya);
        }
    }
}