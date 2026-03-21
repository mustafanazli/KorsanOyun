using UnityEngine;

public class HazineNoktasi : MonoBehaviour
{
    [Header("Hazine Ayarlarý")]
    public GameObject sandikPrefab; // Çýkacak sandýk
    public int gerekenKazmaSayisi = 3; // Kaç kere sol týk yaparsak çýksýn?

    private bool cikarildiMi = false;

    public void Kaz()
    {
        if (cikarildiMi) return;

        gerekenKazmaSayisi--;
        Debug.Log("Toprak kazýlýyor... Kalan kazma: " + gerekenKazmaSayisi);

        // Ýleride buraya toprak sýçrama efekti (Particle System) ekleyebiliriz!

        if (gerekenKazmaSayisi <= 0)
        {
            HazineyiCikar();
        }
    }

    void HazineyiCikar()
    {
        cikarildiMi = true;
        Debug.Log("BÝNGOOOO! SANDIK BULUNDU!");

        // Sandýðý topraðýn hemen üstünde oluþtur (Y ekseninde biraz yukarýda)
        Vector3 cikisNoktasi = transform.position + new Vector3(0, 0.5f, 0);
        Instantiate(sandikPrefab, cikisNoktasi, transform.rotation);

        // Ýþimiz bittiðine göre bu görünmez kazý alanýný yok et
        Destroy(gameObject);
    }
}