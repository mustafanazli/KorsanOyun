using UnityEngine;
using UnityEngine.AI;

public class IskeletAI : MonoBehaviour
{
    [Header("Saldýrý Ayarlarý")]
    public float saldiriMesafesi = 2f; // Ne kadar yaklaþýnca vuracak?
    public float saldiriHizi = 1.5f; // Kaç saniyede bir vuracak?
    public int verilecekHasar = 10; // Her vuruþta kaç can gidecek?

    private Transform oyuncu;
    private NavMeshAgent ajan;
    private float saldiriZamanlayici = 0f;

    void Start()
    {
        ajan = GetComponent<NavMeshAgent>();

        // Ýskelet doðduðu an etrafa bakýp "Player" etiketli oyuncuyu bulur
        GameObject hedeflenenOyuncu = GameObject.FindGameObjectWithTag("Player");
        if (hedeflenenOyuncu != null)
        {
            oyuncu = hedeflenenOyuncu.transform;
        }
    }

    void Update()
    {
        if (oyuncu == null) return;

        float mesafe = Vector3.Distance(transform.position, oyuncu.position);

        if (mesafe > saldiriMesafesi)
        {
            // 1. DURUM: Uzaðýz, oyuncuya doðru koþ!
            ajan.isStopped = false;
            ajan.SetDestination(oyuncu.position);
        }
        else
        {
            // 2. DURUM: Yakýnýz, dur ve yüzünü oyuncuya dönerek saldýr!
            ajan.isStopped = true;
            YuzunuOyuncuyaDon();

            saldiriZamanlayici += Time.deltaTime;
            if (saldiriZamanlayici >= saldiriHizi)
            {
                Saldir();
                saldiriZamanlayici = 0f; // Zamanlayýcýyý sýfýrla ki peþ peþe taramalý tüfek gibi vurmasýn
            }
        }
    }

    void YuzunuOyuncuyaDon()
    {
        Vector3 yon = (oyuncu.position - transform.position).normalized;
        yon.y = 0;
        Quaternion donus = Quaternion.LookRotation(yon);
        transform.rotation = Quaternion.Slerp(transform.rotation, donus, Time.deltaTime * 5f);
    }

    void Saldir()
    {
        // Oyuncunun can kodunu bul ve hasar ver
        OyuncuCanSistemi oyuncuCan = oyuncu.GetComponent<OyuncuCanSistemi>();
        if (oyuncuCan != null)
        {
            oyuncuCan.HasarAl(verilecekHasar);
        }
    }
}