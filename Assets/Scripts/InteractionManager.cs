using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public float interactionDistance = 3f;
    public EnvanterKontrol envanterSistemi;

    // Not: Ekranda yazý çýkma iþini Billboard.cs yaptýðý için 
    // buradaki mesafe kontrolünü sadeleþtirdik.

    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag("Varil"))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    // 1. Bakýlan varilin içindeki eþya listesini al
                    VarilIcerigi varil = hit.collider.GetComponent<VarilIcerigi>();

                    if (varil != null)
                    {
                        // 2. Önce envanter panelini o eþyalarla doldur
                        envanterSistemi.VarilPaneliniDoldur(varil);
                        // 3. Sonra paneli aç
                        envanterSistemi.VarilLootEkraniAc();
                    }
                }
            }
        }
    }
}