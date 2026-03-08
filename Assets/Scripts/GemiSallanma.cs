using UnityEngine;

public class GemiSallanma : MonoBehaviour
{
    [Header("Dalga Ayarlarý")]
    public float dalgaHizi = 1f;      // Geminin ne kadar hýzlý sallanacaðý
    public float dalgaMiktari = 0.2f; // Geminin ne kadar yukarý/aþaðý çýkacaðý (Küçük tutuyoruz ki karakter üstünde titremesin)

    private Vector3 baslangicPozisyonu;

    void Start()
    {
        // Oyun baþladýðýnda geminin ilk durduðu yeri hafýzaya al
        baslangicPozisyonu = transform.position;
    }

    void Update()
    {
        // Sinüs dalgasý matematiði ile gemiyi Y ekseninde (yukarý-aþaðý) yumuþakça süzdür
        float yeniY = baslangicPozisyonu.y + Mathf.Sin(Time.time * dalgaHizi) * dalgaMiktari;
        transform.position = new Vector3(transform.position.x, yeniY, transform.position.z);
    }
}