using UnityEngine;
using UnityEngine.EventSystems;

public class EsyaYuvasi : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            SuruklenebilirEsya suruklenen = eventData.pointerDrag.GetComponent<SuruklenebilirEsya>();
            InventoryItemUI suruklenenUI = eventData.pointerDrag.GetComponent<InventoryItemUI>();

            if (suruklenen != null && suruklenenUI != null)
            {
                // DURUM 1: Yuva tamamen boþsa direkt yerleþ
                if (transform.childCount == 0)
                {
                    suruklenen.asilYuva = this.transform;
                }
                else
                {
                    // DURUM 2: Yuvada zaten bir eþya var!
                    Transform yuvadakiEsya = transform.GetChild(0);
                    InventoryItemUI yuvadakiUI = yuvadakiEsya.GetComponent<InventoryItemUI>();

                    if (yuvadakiUI != null)
                    {
                        // A. Ýkisi de AYNI eþyaysa (Muz + Muz) -> BÝRLEÞTÝR
                        if (suruklenenUI.esyaVerisi == yuvadakiUI.esyaVerisi)
                        {
                            int toplam = yuvadakiUI.miktar + suruklenenUI.miktar;
                            int maxKapasite = yuvadakiUI.esyaVerisi.maxYigin;

                            if (toplam <= maxKapasite)
                            {
                                // Hepsi sýðdý: Yuvadaki eþyanýn sayýsýný artýr, sürükleneni yok et
                                yuvadakiUI.SlotuGuncelle(yuvadakiUI.esyaVerisi, toplam);
                                Destroy(suruklenen.gameObject);
                            }
                            else
                            {
                                // Sýðmayan kaldýysa: Yuvayý fulle, kalaný sürüklenende býrak
                                int eklenecek = maxKapasite - yuvadakiUI.miktar;
                                yuvadakiUI.SlotuGuncelle(yuvadakiUI.esyaVerisi, maxKapasite);
                                suruklenenUI.SlotuGuncelle(suruklenenUI.esyaVerisi, suruklenenUI.miktar - eklenecek);
                                // Kalan miktar kendi eski yuvasýna geri döner
                            }
                        }
                        // B. FARKLI eþyalarsa (Muz + Gülle) -> YER DEÐÝÞTÝR (SWAP)
                        else
                        {
                            SuruklenebilirEsya yuvadakiSuruklenen = yuvadakiEsya.GetComponent<SuruklenebilirEsya>();

                            // Yuvadakini, sürüklenenin eski yerine gönder
                            yuvadakiSuruklenen.asilYuva = suruklenen.asilYuva;
                            yuvadakiEsya.SetParent(suruklenen.asilYuva);

                            // Sürükleneni de yeni yuvaya al
                            suruklenen.asilYuva = this.transform;
                        }
                    }
                }
            }
        }
    }
}