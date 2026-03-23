using UnityEngine;
using UnityEngine.AI;

public class IskeletAI : MonoBehaviour
{
    [Header("Can Ayarlarý")]
    public float can = 50f;
    public int dusecekAltin = 15; // Kesince ne kadar para verecek?

    [Header("Saldýrý Ayarlarý")]
    public float saldiriMesafesi = 2f;
    public float saldiriHizi = 1.5f;
    public int verilecekHasar = 10;

    private Transform oyuncu;
    private NavMeshAgent ajan;
    private float saldiriZamanlayici = 0f;
    private bool oluMu = false;

    void Start()
    {
        ajan = GetComponent<NavMeshAgent>();

        GameObject hedeflenenOyuncu = GameObject.FindGameObjectWithTag("Player");
        if (hedeflenenOyuncu != null)
        {
            oyuncu = hedeflenenOyuncu.transform;
        }
    }

    void Update()
    {
        if (oyuncu == null || oluMu) return;

        float mesafe = Vector3.Distance(transform.position, oyuncu.position);

        if (mesafe > saldiriMesafesi)
        {
            ajan.isStopped = false;
            ajan.SetDestination(oyuncu.position);
        }
        else
        {
            ajan.isStopped = true;
            YuzunuOyuncuyaDon();

            saldiriZamanlayici += Time.deltaTime;
            if (saldiriZamanlayici >= saldiriHizi)
            {
                Saldir();
                saldiriZamanlayici = 0f;
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
        OyuncuCanSistemi oyuncuCan = oyuncu.GetComponent<OyuncuCanSistemi>();
        if (oyuncuCan != null)
        {
            oyuncuCan.HasarAl(verilecekHasar);
        }
    }

    public void HasarAl(float hasar)
    {
        if (oluMu) return;

        can -= hasar;
        if (can <= 0)
        {
            Olum();
        }
    }

    void Olum()
    {
        oluMu = true;

        // 1. Ekip kasasýný bul ve altýný yatýr
        EkipKasasi kasa = Object.FindAnyObjectByType<EkipKasasi>();
        if (kasa != null)
        {
            kasa.KasayaAltinEkle(dusecekAltin);
        }

        // 2. NPC'yi bul ve görev için sayacý artýrmasýný söyle (YENÝ EKLENDÝ)
        NPCGorevSistemi npc = Object.FindAnyObjectByType<NPCGorevSistemi>();
        if (npc != null)
        {
            npc.IskeletKestik();
        }

        // 3. Spawner'a öldüðünü haber ver
        IskeletSpawner spawner = GetComponentInParent<IskeletSpawner>();
        if (spawner != null)
        {
            spawner.IskeletOldu();
        }

        // 4. Ýskeleti yok et
        Destroy(gameObject);
    }
}