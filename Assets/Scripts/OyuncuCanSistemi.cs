using UnityEngine;
using UnityEngine.UI; // YENÝ: Arayüz (UI) elemanlarýný kullanmak için bu ÞART!

public class OyuncuCanSistemi : MonoBehaviour
{
    [Header("Can Ayarlarý")]
    public float maksimumCan = 100f; // Bölme iþlemi yapacaðýmýz için float (küsuratlý) yaptýk
    public float mevcutCan;

    [Header("Arayüz (UI) Baðlantýlarý")]
    public Image canBariGorseli; // Ekranda azalan o yeþil barýmýz

    void Start()
    {
        mevcutCan = maksimumCan; // Oyun baþlarken caný full'le
        CanBariniGuncelle(); // Barý da full göster
    }

    public void HasarAl(float hasarMiktari)
    {
        mevcutCan -= hasarMiktari;

        // Can 0'ýn altýna düþmesin diye sýnýr koyuyoruz
        if (mevcutCan < 0) mevcutCan = 0;

        Debug.Log("AH! Ýskelet vurdu! Kalan Can: " + mevcutCan);

        // Hasar yediðimiz an barý güncelle
        CanBariniGuncelle();

        if (mevcutCan <= 0)
        {
            Olum();
        }
    }

    void CanBariniGuncelle()
    {
        // Eðer can barý koda baðlandýysa, doluluk oranýný (0 ile 1 arasý) hesapla ve ekrana yansýt
        if (canBariGorseli != null)
        {
            // Örn: 50 / 100 = 0.5 (Yani barýn %50'si dolu gözükecek)
            canBariGorseli.fillAmount = mevcutCan / maksimumCan;
        }
    }

    void Olum()
    {
        Debug.Log("ÖLDÜN! KORSAN HAYATI BURAYA KADARMIÞ...");
        // Ýleride buraya ölüm ekraný falan ekleriz.
    }
}