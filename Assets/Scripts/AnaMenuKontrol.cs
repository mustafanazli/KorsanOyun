using UnityEngine;
using UnityEngine.SceneManagement; // Sahneler arasý geçiþ için bu kütüphane þart!

public class AnaMenuKontrol : MonoBehaviour
{
    // Oyuna Baþla butonuna týklandýðýnda çalýþacak kod
    public void OyunaBasla()
    {
        // "SampleScene" yazan yere, senin asýl oyun sahnenin adýný tam olarak yazmalýsýn.
        // Eðer oyun sahnenin adý farklýysa burayý mutlaka deðiþtir!
        SceneManager.LoadScene("SampleScene");
    }

    // Çýkýþ butonuna týklandýðýnda çalýþacak kod
    public void OyundanCik()
    {
        Debug.Log("Oyundan çýkýlýyor..."); // Unity editöründe çalýþtýðýný görmek için
        Application.Quit(); // Bu kod oyunu build aldýktan sonra (EXE olunca) çalýþýr
    }
}