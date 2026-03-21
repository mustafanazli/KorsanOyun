using UnityEngine;

public class NPCGorevSistemi : MonoBehaviour
{
    public enum GorevDurumu { GorevYok, GorevAlindi, SandikSirtlandi }

    [Header("Görev Ayarları")]
    public GorevDurumu durum = GorevDurumu.GorevYok;
    public int oyuncuParasi = 0;

    public void NPCIleKonus()
    {
        if (durum == GorevDurumu.GorevYok)
        {
            Debug.Log("NPC: Selam çaylak! Bana şu adadaki gömülü sandığı bul. Git kaz ve getir!");
            durum = GorevDurumu.GorevAlindi;
        }
        else if (durum == GorevDurumu.GorevAlindi)
        {
            Debug.Log("NPC: Hala burada mısın? Çabuk git ve o sandığı kazıp bana getir!");
        }
        else if (durum == GorevDurumu.SandikSirtlandi)
        {
            Debug.Log("NPC: Harika bir iş çıkardın! Al bakalım, işte 500 Korsan Parası!");

            oyuncuParasi += 500;
            Debug.Log("Mevcut Paran: " + oyuncuParasi + " Altın 💰");

            durum = GorevDurumu.GorevYok; // Görevi sıfırla ki tekrar görev alabilelim
        }
    }

    // YENİ: Sandığı yerden alınca bu çalışacak
    public void SandigiSirtla()
    {
        if (durum == GorevDurumu.GorevAlindi)
        {
            durum = GorevDurumu.SandikSirtlandi;
            Debug.Log("Sandığı sırtladın! Hemen NPC'ye koş ve sat.");
        }
    }
}