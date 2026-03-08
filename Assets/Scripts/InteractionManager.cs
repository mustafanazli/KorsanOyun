using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public float interactionDistance = 3f;
    public EnvanterKontrol envanterSistemi;
    public GameObject dumenEtkilesimYazisi;

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        bool dumeneBakiyor = false;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Varil"))
            {
                // 2. DEÐÝÞÝKLÝK: F'ye bastýðýmýzda, sadece çanta KAPALIYSA varili aç!
                if (Input.GetKeyDown(KeyCode.F) && !envanterSistemi.envanterAcikMi)
                {
                    VarilIcerigi varil = hit.collider.GetComponent<VarilIcerigi>();
                    if (varil != null)
                    {
                        envanterSistemi.VarilPaneliniDoldur(varil);
                        envanterSistemi.VarilLootEkraniAc();
                    }
                }
            }
            else if (hit.collider.CompareTag("Dumen"))
            {
                GemiKontrol gemi = hit.collider.GetComponentInParent<GemiKontrol>();

                if (gemi != null && !gemi.dumenBende)
                {
                    dumeneBakiyor = true;

                    // Ayný þekilde, çanta açýkken yanlýþlýkla dümeni de tutmayalým
                    if (Input.GetKeyDown(KeyCode.F) && !envanterSistemi.envanterAcikMi)
                    {
                        gemi.DumeniTutVeyaBirak();
                    }
                }
            }
        }

        if (dumenEtkilesimYazisi != null)
        {
            dumenEtkilesimYazisi.SetActive(dumeneBakiyor);
        }
    }
}