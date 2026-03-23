using UnityEngine;
using Mirror;

public class OyuncuKurulum : NetworkBehaviour
{
    [Header("Karakterin Kendi Kamerasý")]
    public GameObject karakterKamerasi;

    void Start()
    {
        // Eðer bu doðan karakter BÝZÝM deðilse (baþka bir oyuncuysa), kamerasýný hemen kapat ki onun gözünden görmeyelim.
        if (!isLocalPlayer)
        {
            if (karakterKamerasi != null) karakterKamerasi.SetActive(false);
        }
    }

    public override void OnStartLocalPlayer()
    {
        // Karakter BÝZÝMSE lobi kamerasýný bul ve fiþini çek!
        GameObject lobiKamerasi = GameObject.Find("LobiCam");
        if (lobiKamerasi != null)
        {
            lobiKamerasi.SetActive(false);
        }

        // Bizim karakterin kamerasýný aktif et!
        if (karakterKamerasi != null) karakterKamerasi.SetActive(true);
    }
}