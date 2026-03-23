using UnityEngine;
using Mirror;

public class SunucuBaslat : MonoBehaviour
{
    public void HostOlarakBaslat()
    {
        // Singleton (Tekil yönetici) üzerinden baþlatmak %100 güvenlidir.
        NetworkManager.singleton.StartHost();
    }
}