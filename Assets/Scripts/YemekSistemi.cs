using UnityEngine;
using UnityEngine.EventSystems; // 3. DEÐÝÞÝKLÝK: Fareyle arayüze (kutuya) týkladýðýmýzý anlamak için eklendi!
using System.Collections;

public class YemekSistemi : MonoBehaviour
{
    [Header("Görsel ve Animasyon")]
    public GameObject eldekiMuz;
    public Animator anim;

    [Header("Sistem Baðlantýlarý")]
    public PlayerHealth canSistemi;
    public EnvanterKontrol envanterSistemi;
    public ItemData muzVerisi;

    [Header("Yeme Ayarlarý")]
    public float yemeSuresi = 2f;
    public float kazanilanCan = 20f;

    private bool yemekYiyorMu = false;

    void Start()
    {
        if (eldekiMuz != null) eldekiMuz.SetActive(false);
    }

    void Update()
    {
        ItemData eldeNeVar = envanterSistemi.SeciliHotbarEsyasiniGetir();
        bool muzVarVeSecili = (eldeNeVar != null && eldeNeVar == muzVerisi);

        // Eðer envanter açýksa eldeki muzu anýnda gizle!
        if (!yemekYiyorMu && eldekiMuz != null)
        {
            eldekiMuz.SetActive(muzVarVeSecili && !envanterSistemi.envanterAcikMi);
        }

        // 4. DEÐÝÞÝKLÝK: Fareyle bir kutucuðun veya arayüzün üzerinde miyiz?
        bool arayuzeTikliyorMu = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

        // Muz seçiliyse + Sol Týka basýldýysa + Yemiyorsak + ÇANTA KAPALIYSA + EKRANDAKÝ KUTULARA TIKLAMIYORSAK yiyebiliriz!
        if (muzVarVeSecili && Input.GetMouseButtonDown(0) && !yemekYiyorMu && !envanterSistemi.envanterAcikMi && !arayuzeTikliyorMu)
        {
            StartCoroutine(MuzYemeRutini());
        }
    }

    IEnumerator MuzYemeRutini()
    {
        yemekYiyorMu = true;

        if (envanterSistemi.SeciliHotbarEsyasiniTuket(muzVerisi))
        {
            if (anim != null) anim.SetTrigger("YemekYe");
            yield return new WaitForSeconds(yemeSuresi);
            if (canSistemi != null) canSistemi.Iyilestir(kazanilanCan);
            Debug.Log("Afiyet olsun! Muz yendi!");
        }

        yemekYiyorMu = false;
    }
}