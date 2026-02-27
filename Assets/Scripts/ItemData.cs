using UnityEngine;

[CreateAssetMenu(fileName = "Yeni Esya", menuName = "Envanter/Esya Olustur")]
public class ItemData : ScriptableObject
{
    public string esyaAdi;
    public Sprite ikon;
    public int maxYigin = 99; // Bir slotta en fazla kaç tane olabilir?
}