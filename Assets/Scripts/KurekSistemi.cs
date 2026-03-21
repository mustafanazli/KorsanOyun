using UnityEngine;

public class KurekSistemi : MonoBehaviour
{
    [Header("Kürek Ayarlarý")]
    public float kazmaMesafesi = 3f;
    public Camera oyuncuKamerasi;

    [Header("Eþya (Equip) Ayarlarý")]
    public GameObject eldeTutulanKurekGorseli; // Kameranýn altýna koyduðumuz kürek
    public KeyCode kurekTusu = KeyCode.J; // Tuþ J olarak ayarlandý

    private bool kurekElimizdeMi = false;

    void Start()
    {
        // Oyun baþlarken kürek elimizde olmasýn (gizli kalsýn)
        if (eldeTutulanKurekGorseli != null)
        {
            eldeTutulanKurekGorseli.SetActive(false);
        }
    }

    void Update()
    {
        // 1. KÜREÐÝ ELÝNE AL / BIRAK (J Tuþu ile)
        if (Input.GetKeyDown(kurekTusu))
        {
            kurekElimizdeMi = !kurekElimizdeMi; // Durumu tam tersine çevir

            if (eldeTutulanKurekGorseli != null)
            {
                eldeTutulanKurekGorseli.SetActive(kurekElimizdeMi); // Görseli aç veya kapat
            }

            // Konsolda görelim
            if (kurekElimizdeMi) Debug.Log("Kürek Eline Alýndý!");
            else Debug.Log("Kürek Sýrtýna Asýldý!");
        }

        // 2. SADECE KÜREK ELÝMÝZDEYSE VE SOL TIKLANIRSA KAZ!
        if (kurekElimizdeMi && Input.GetMouseButtonDown(0))
        {
            KazmayiDene();
        }
    }

    void KazmayiDene()
    {
        Ray ray = new Ray(oyuncuKamerasi.transform.position, oyuncuKamerasi.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, kazmaMesafesi))
        {
            HazineNoktasi hazine = hit.collider.GetComponent<HazineNoktasi>();
            if (hazine != null)
            {
                hazine.Kaz();
            }
        }
    }
}