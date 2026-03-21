using UnityEngine;

public class IskeletSpawner : MonoBehaviour
{
    [Header("Spawn Ayarlarý")]
    public GameObject iskeletPrefab; // Doðacak iskelet þablonu
    public int baslangicIskeletSayisi = 3; // Oyun baþlayýnca bu bölgede kaç iskelet olsun?
    public float spawnYaricapi = 5f; // Ne kadar geniþ bir alana saçýlsýnlar?

    public int mevcutIskeletSayisi = 0;

    void Start()
    {
        // Oyun baþlar baþlamaz belirlediðimiz sayý kadar iskeleti anýnda üret
        for (int i = 0; i < baslangicIskeletSayisi; i++)
        {
            IskeletUret();
        }
    }

    void IskeletUret()
    {
        if (iskeletPrefab == null) return;

        // Spawner'ýn etrafýnda rastgele bir zemin noktasý seç (X ve Z ekseninde)
        Vector2 rastgeleDaire = Random.insideUnitCircle * spawnYaricapi;
        Vector3 spawnNoktasi = transform.position + new Vector3(rastgeleDaire.x, 0, rastgeleDaire.y);

        // Ýskeleti o rastgele noktada yarat
        GameObject yeniIskelet = Instantiate(iskeletPrefab, spawnNoktasi, Quaternion.identity);

        // Ortalýk karýþmasýn diye üretilen iskeleti bu spawner'ýn içine (altýna) atýyoruz
        yeniIskelet.transform.SetParent(this.transform);

        mevcutIskeletSayisi++;
    }

    // Ýleride iskeletleri kýlýçla kestiðimizde bu fonksiyonu çaðýrýp sayýyý azaltacaðýz
    public void IskeletOldu()
    {
        mevcutIskeletSayisi--;
    }

    // Unity editöründe spawn alanýný yeþil bir daire olarak görmek için sihirli kod
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnYaricapi);
    }
}