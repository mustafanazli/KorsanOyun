using UnityEngine;
using UnityEngine.UI; // UI ile iþlem yapacaðýmýz için bunu eklemeliyiz

public class PlayerHealth : MonoBehaviour
{
    [Header("Can Ayarlarý")]
    public float maxCan = 100f;
    public float mevcutCan;

    [Header("Arayüz (UI)")]
    public Image canBariUI; // Kýrmýzý olan CanBariOn objesini buraya koyacaðýz

    void Start()
    {
        // Oyun baþladýðýnda caný fulle
        mevcutCan = maxCan;
        CanBariniGuncelle();
    }

    public void HasarAl(float hasarMiktari)
    {
        mevcutCan -= hasarMiktari;
        if (mevcutCan < 0) mevcutCan = 0; // Can eksiye düþmesin

        CanBariniGuncelle();

        if (mevcutCan == 0)
        {
            Olu();
        }
    }

    public void Iyilestir(float miktar)
    {
        mevcutCan += miktar;
        if (mevcutCan > maxCan) mevcutCan = maxCan; // Can max sýnýrý geçmesin

        CanBariniGuncelle();
    }

    void CanBariniGuncelle()
    {
        if (canBariUI != null)
        {
            // Fill Amount 0 ile 1 arasýnda bir sayý ister (Örn: 50 / 100 = 0.5)
            canBariUI.fillAmount = mevcutCan / maxCan;
        }
    }

    void Olu()
    {
        Debug.Log("Karakter Öldü! YARRR!");
        // Daha sonra buraya ölüm animasyonu veya yeniden doðma ekleyeceðiz
    }

    // TEST ÝÇÝN: K tuþuna basýnca hasar alsýn diye geçici bir Update ekliyoruz
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            HasarAl(10f);
        }
    }
}