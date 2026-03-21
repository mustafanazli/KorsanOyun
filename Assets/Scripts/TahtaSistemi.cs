using UnityEngine;

public class TahtaSistemi : MonoBehaviour
{
    [Header("Tahta Ayarlarý")]
    public GameObject tahtaModeli; // Kameranýn altýndaki tahtamýz
    public float etkilesimMesafesi = 3f;

    [Header("Envanter Baðlantýsý")]
    public ItemData tahtaEsyasiVerisi; // Inspector'dan Tahta'nýn ItemData'sýný buraya sürükleyeceðiz

    [Header("Referanslar")]
    public Camera oyuncuKamerasi;
    public GemiSuSistemi gemiSuSistemi;
    public EnvanterKontrol envanterKontrol; // Envanter sistemimize eriþim

    void Update()
    {
        // 1. Hotbar'da seçili olan eþya bizim "Tahta" mý? Onu bulalým.
        bool tahtaSeciliMi = false;
        if (envanterKontrol != null && tahtaEsyasiVerisi != null)
        {
            ItemData seciliEsya = envanterKontrol.SeciliHotbarEsyasiniGetir();
            if (seciliEsya == tahtaEsyasiVerisi)
            {
                tahtaSeciliMi = true;
            }
        }

        // 2. Eðer tahta seçiliyse 3D Modeli ekranda göster, baþka bir þey seçiliyse gizle
        if (tahtaModeli != null)
        {
            tahtaModeli.SetActive(tahtaSeciliMi);
        }

        // 3. Tahta seçiliyken (elimizdeyken) sol týka basarsak tamir et!
        if (tahtaSeciliMi && Input.GetMouseButtonDown(0))
        {
            TamirEt();
        }
    }

    void TamirEt()
    {
        Ray ray = oyuncuKamerasi.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, etkilesimMesafesi))
        {
            if (hit.collider.CompareTag("Delik"))
            {
                // ÖNCE KONTROL: Envanterde (seçili slotta) gerçekten "Tahta" var mý ve tüketebildik mi?
                if (envanterKontrol != null && tahtaEsyasiVerisi != null)
                {
                    bool tahtaHarcandi = envanterKontrol.SeciliHotbarEsyasiniTuket(tahtaEsyasiVerisi);

                    if (tahtaHarcandi)
                    {
                        // EÐER TAHTA HARCANDIYSA TAMÝR ÝÞLEMÝNÝ YAP
                        Debug.Log("ÇAK ÇAK ÇAK! Delik tamir edildi! 1 Tahta harcandý.");

                        Destroy(hit.collider.gameObject);

                        if (gemiSuSistemi != null && gemiSuSistemi.aktifDelikSayisi > 0)
                        {
                            gemiSuSistemi.aktifDelikSayisi--;
                        }
                    }
                    else
                    {
                        Debug.Log("Elinizde yeterli tahta yok veya yanlýþ eþya seçili!");
                    }
                }
            }
        }
    }
}