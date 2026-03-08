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
                // DURUM 1: Yuva tamamen boþsa
                if (transform.childCount == 0)
                {
                    suruklenen.asilYuva = this.transform;
                }
                else
                {
                    // DURUM 2: Yuvada zaten bir eþya var
                    Transform yuvadakiEsya = transform.GetChild(0);
                    InventoryItemUI yuvadakiUI = yuvadakiEsya.GetComponent<InventoryItemUI>();

                    if (yuvadakiUI != null)
                    {
                        // A. Ýkisi de AYNI eþyaysa -> BÝRLEÞTÝR
                        if (suruklenenUI.esyaVerisi == yuvadakiUI.esyaVerisi)
                        {
                            int toplam = yuvadakiUI.miktar + suruklenenUI.miktar;
                            int maxKapasite = 20; // 3. GÖREV: HER ÞEY ÝÇÝN MAX STACK 20 OLDU!

                            if (toplam <= maxKapasite)
                            {
                                yuvadakiUI.SlotuGuncelle(yuvadakiUI.esyaVerisi, toplam);
                                Destroy(suruklenen.gameObject);
                            }
                            else
                            {
                                int eklenecek = maxKapasite - yuvadakiUI.miktar;
                                yuvadakiUI.SlotuGuncelle(yuvadakiUI.esyaVerisi, maxKapasite);
                                suruklenenUI.SlotuGuncelle(suruklenenUI.esyaVerisi, suruklenenUI.miktar - eklenecek);
                            }
                        }
                        // B. FARKLI eþyalarsa -> KUSURSUZ YER DEÐÝÞTÝR (SWAP)
                        else
                        {
                            SuruklenebilirEsya yuvadakiSuruklenen = yuvadakiEsya.GetComponent<SuruklenebilirEsya>();

                            // 4. GÖREV: Yuvadakini, sürüklenenin eski yerine gönder ve tam merkeze oturt
                            yuvadakiEsya.SetParent(suruklenen.asilYuva);
                            yuvadakiSuruklenen.asilYuva = suruklenen.asilYuva;
                            yuvadakiEsya.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

                            // Sürükleneni de yeni yuvaya al
                            suruklenen.asilYuva = this.transform;
                        }
                    }
                }
            }
        }
    }
}