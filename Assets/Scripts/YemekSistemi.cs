using UnityEngine;

public class YemekSistemi : MonoBehaviour
{
    [Header("Görsel ve Animasyon")]
    public GameObject eldekiMuz; // Karakterin elindeki 3D muz
    public Animator anim;

    [Header("Sistemler")]
    public PlayerHealth canSistemi; // Can barý kodumuz
    // public EnvanterKontrol envanter; // (Bunu birazdan envanterden silmek için kullanacaðýz)

    private bool muzElindeMi = false;

    void Update()
    {
        // 3 Tuþuna basýldýðýnda
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            MuzuEleAl();
        }

        // Muz elindeyken Farenin Sol Týk'ýna basarsa
        if (muzElindeMi && Input.GetMouseButtonDown(0))
        {
            YemegiYe();
        }
    }

    void MuzuEleAl()
    {
        // TODO: Ýleride buraya "Envanterde muz var mý?" kontrolü ekleyeceðiz.
        // Þimdilik 3'e basýnca direkt eline alýp býraksýn.

        muzElindeMi = !muzElindeMi; // Durumu tersine çevir (Elindeyse býrak, deðilse al)
        eldekiMuz.SetActive(muzElindeMi); // Ekranda göster/gizle
    }

    void YemegiYe()
    {
        // 1. Animasyonu tetikle
        if (anim != null) anim.SetTrigger("YemekYe");

        // 2. Caný doldur (Örn: 20 can versin)
        if (canSistemi != null) canSistemi.Iyilestir(20f);

        // 3. Muzu elinden kaybet
        muzElindeMi = false;
        eldekiMuz.SetActive(false);

        // TODO: Envanterden 1 adet muzu silme kodunu buraya ekleyeceðiz!
    }
}