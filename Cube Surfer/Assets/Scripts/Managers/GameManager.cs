using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Karakter0.6f y�kselecek
    private int elmasSayisi;
    private int kazanilanElmasSayisi;
    private int sahneIndex;
    private int istenenEfektSayisi;
    string alttakiPlatform;
    private ShopManager shopManager;
    private CameraController cameraController;
    private VibrationManager vibrationManager= new VibrationManager();
    [Header("PLAYER")]
    public int anlikkupSayisi;
    [SerializeField]private PlayerController playerController;
    [Header("EFEKTLER")]
    public List<GameObject> OlusmaEfektleri;
    public List<GameObject> YokOlmaEfektleri;
    private List<GameObject> gonderilecekEfektler = new List<GameObject>();
    private GameObject gonderilecekEfekt;
    [Header("UI")]
    [SerializeField] private Animator LayoutGroup;
    [SerializeField] private Button[] butonlar;
    [SerializeField] private GameObject[] paneller;
    [SerializeField] private TextMeshProUGUI elmasText, levelText,kazandinElmasText,carpmaText;
    [SerializeField] private AudioSource[] sesler;
    void Awake()
    {
        SesleriKontrolEt();
        TitresimiKontrolEt();
        shopManager = GameObject.Find("ShopManager").GetComponent<ShopManager>();
        cameraController = Camera.main.GetComponent<CameraController>();
        anlikkupSayisi =playerController.cubes.Count;
        alttakiPlatform = "";
        elmasSayisi = PlayerPrefs.GetInt("elmas");
        Debug.Log("Level Bas�ndaki elmas say�s�:" + elmasSayisi);
        sahneIndex = SceneManager.GetActiveScene().buildIndex;
        elmasText.text = elmasSayisi.ToString();

        levelText.text = sahneIndex.ToString();
    }
    void TitresimiKontrolEt()
    {
        if (!PlayerPrefs.HasKey("Vibrate"))
        {
            PlayerPrefs.SetInt("Vibrate", 1);
            vibrationManager.TitresebilirligiAyarla(true);
        }
        else
        {
            if (PlayerPrefs.GetInt("Vibrate") == 1)
            {
                vibrationManager.TitresebilirligiAyarla(true);
                butonlar[4].gameObject.SetActive(true);
                butonlar[5].gameObject.SetActive(false);
            }
            else
            {
                vibrationManager.TitresebilirligiAyarla(false);
                butonlar[4].gameObject.SetActive(false);
                butonlar[5].gameObject.SetActive(true);
            }
        }
    }
    void SesleriKontrolEt()
    {
        if (!PlayerPrefs.HasKey("Sesler"))
        {
            PlayerPrefs.SetInt("Sesler", 1);
        }
        else
        {
            if(PlayerPrefs.GetInt("Sesler") == 1)
            {
                butonlar[2].gameObject.SetActive(true);
                butonlar[3].gameObject.SetActive(false);
            }
            else
            {
                butonlar[2].gameObject.SetActive(false);
                butonlar[3].gameObject.SetActive(true);
                foreach (AudioSource ses in sesler)
                {
                    ses.enabled = false;
                }
            }
        }
    }
    public void OyunuBaslat()
    {
        playerController.oyuncuHareketDurumu = true;
        paneller[0].SetActive(false);
    }
    public void KupEkle(GameObject gelenKup,bool cokluKup=false)
    {
        if (cokluKup)
        {
            istenenEfektSayisi = gelenKup.transform.childCount;                  
            OlusmaEfektleriOlustur(istenenEfektSayisi);
            playerController.CokluKupEkle(gelenKup, gonderilecekEfektler);
            anlikkupSayisi = playerController.cubes.Count;
            if (sesler[2].enabled)
                sesler[2].Play();
            Debug.Log(anlikkupSayisi);
        }
        else
        {
            playerController.KupuEkle(gelenKup, OlusmaEfektiOlustur());
            anlikkupSayisi++;
            if (sesler[2].enabled)
                sesler[2].Play();
            Debug.Log(anlikkupSayisi);
            Debug.Log("Burasi calisti");
        }
        vibrationManager.CihaziTitrestir();
    }
    public void ElmasEkle()
    {//Oyun hem �nceki levellerde kazand��� elmas say�s�n�
     //hemde g�ncel leveli oynarken kazand��� elmas say�s�n� tutuyor
        kazanilanElmasSayisi++;
        Debug.Log("Elmas say�s�:" + PlayerPrefs.GetInt("elmas"));
        elmasSayisi= PlayerPrefs.GetInt("elmas")+1;
        elmasText.text = elmasSayisi.ToString();
        PlayerPrefs.SetInt("elmas", elmasSayisi);
        if (sesler[1].enabled)
            sesler[1].Play();

    }
    private GameObject OlusmaEfektiOlustur()
    {//Olu�turulan k�p�n pozisyonu de�i�ti�i i�in metot playerController taraf�ndan �a��r�l�r
        foreach (var item in OlusmaEfektleri)
        {
            if (!item.activeInHierarchy)
            {
                item.SetActive(true);
                gonderilecekEfekt = item.gameObject;
                break;
            }
        }
        return gonderilecekEfekt;
    }
    private List<GameObject> OlusmaEfektleriOlustur(int istenenEfektSayisi)
    {
        if (gonderilecekEfektler.Count != 0)
        {
            gonderilecekEfektler.Clear();
        }
        int sayi = 0;
        while (sayi < istenenEfektSayisi)
        {
            foreach (var item in OlusmaEfektleri)
            {
                if (!item.activeInHierarchy)
                {
                    item.SetActive(true);
                    gonderilecekEfektler.Add(item);
                    sayi++;
                    break;
                }
            }
        }

        return gonderilecekEfektler;
    }
    void YokOlmaEfektiOlustur(Transform pozisyon)
    {
        //Sahnedeki YokOlmaEfektleri listesi kontrol edilir.
        //Listede bulunan ilk kapal� efekt, aktif hale getirilir.
        //Aktif edilen efekt parametredeki pozisyona gider.
        foreach (var item in YokOlmaEfektleri)
        {
            if (!item.activeInHierarchy)
            {
                item.SetActive(true);
                item.transform.position = pozisyon.position;
                item.GetComponent<ParticleSystem>().Play();
                break;
            }
        }
    }
    public void KupuSil(GameObject cikanKup,string platformadi=null,bool Lav=false)
    {
        //E�er oyuncu biti� �izgisine ge�erse bu ko�ula girer
        if (playerController.OdulCizgisiGecildi)
        {//Oyuncu platformun �st�ne ��kabiliyorsa o platfromun ad� de�i�kende tutulur.
         //En sonda tutulan de�i�kenin de�erine bak�l�r.
            if (anlikkupSayisi - 1 <= 0)
            {            
                if (platformadi == "1")
                {//Oyuncu biti� �izgisini sadece 1 k�p ile ge�erse �al���r
                    
                    //PlayerPrefs.SetInt("elmas", elmasSayisi + kazanilanElmasSayisi);
                    alttakiPlatform = platformadi;
                    Kazandin(alttakiPlatform);
                    Debug.Log("Kazan�lan elmas:" + PlayerPrefs.GetInt("elmas"));
                }
                else
                {
                    Debug.Log("Oyunu kazand�n");
                    Debug.Log("Alttaki platform " + alttakiPlatform + "X");
                    Debug.Log("Platform �ncesi kazan�lan elmas: " + kazanilanElmasSayisi);
                   // kazanilanElmasSayisi *= int.Parse(alttakiPlatform);
                    //Debug.Log("Platform sonras� kazan�lan elmas: " + kazanilanElmasSayisi);
                   // PlayerPrefs.SetInt("elmas", elmasSayisi + kazanilanElmasSayisi);
                    Debug.Log("Kazan�lan elmas:" + PlayerPrefs.GetInt("elmas"));
                    //Kazand�n paneli ortaya ��kacak
                    Kazandin(alttakiPlatform);
                }
                //paneller[1].SetActive(true);             
            }
            else
            {
                cameraController.KamerayiYukselt();
                anlikkupSayisi--;
                playerController.KupuBirak(cikanKup);
                //alttakiPlatformlar.Add(platformadi);
                alttakiPlatform = platformadi;
            }
        }
        else
        {
            if (anlikkupSayisi - 1 <= 0)
            {
                Kaybettin();
                Debug.Log("Oyunu kaybettin");
            }
            else
            {
                anlikkupSayisi--;
                if (Lav)
                {
                    playerController.KupuBirak(cikanKup,Lav);
                    YokOlmaEfektiOlustur(cikanKup.transform);
                }
                else
                {
                    playerController.KupuBirak(cikanKup);
                }              
            }
        }      
    }   
    public void CizgiGecildi()
    {
        playerController.OdulCizgisiGecildi = true;
        cameraController.isFinished = true;
    }
    public void LayoutAyarlariniAcKapat(string deger)
    {
        if (deger == "ac")
        {
            LayoutGroup.SetTrigger("Slide_in");
            butonlar[0].gameObject.SetActive(false);
            butonlar[1].gameObject.SetActive(true);
        }
        else
        {
            LayoutGroup.SetTrigger("Slide_out");
            butonlar[0].gameObject.SetActive(true);
            butonlar[1].gameObject.SetActive(false);
        }

    }    
    public void AyarlarButonlariIslevleri(string deger)
    {
        switch (deger)
        {
            case "sesikapat":
                foreach (AudioSource ses in sesler)
                {
                    ses.enabled = false;
                }
                butonlar[2].gameObject.SetActive(false);
                butonlar[3].gameObject.SetActive(true);
                PlayerPrefs.SetInt("Sesler", 0);
                break;
            case "sesiac":
                foreach (AudioSource ses in sesler)
                {
                    ses.enabled = true;
                }
                butonlar[2].gameObject.SetActive(true);
                butonlar[3].gameObject.SetActive(false);
                PlayerPrefs.SetInt("Sesler", 1);
                break;
            case "titresimiac":
                butonlar[4].gameObject.SetActive(true);
                butonlar[5].gameObject.SetActive(false);
                PlayerPrefs.SetInt("Vibrate", 1);
                vibrationManager.TitresebilirligiAyarla(true);
                //Titre�im eklendiyse kapat�lacak
                break;
            case "titresimikapat":
                butonlar[4].gameObject.SetActive(false);
                butonlar[5].gameObject.SetActive(true);
                PlayerPrefs.SetInt("Vibrate", 0);
                vibrationManager.TitresebilirligiAyarla(false);
                //Titre�im eklendiyse a��lacak
                break;
            case "tekrar":
                SceneManager.LoadScene(sahneIndex);
                break;
        }
    }   
    void Kazandin(string platformAdi)
    {
        playerController.oyuncuHareketDurumu = false;
        kazanilanElmasSayisi *= int.Parse(alttakiPlatform);
        PlayerPrefs.SetInt("elmas", elmasSayisi + kazanilanElmasSayisi);
        kazandinElmasText.text = kazanilanElmasSayisi.ToString();
        carpmaText.text = platformAdi + "X";
        paneller[1].SetActive(true);
        playerController.KazanmaAnimasyonunuTetikle();
        if (sahneIndex + 1 != 3)
        {
            PlayerPrefs.SetInt("Level", sahneIndex + 1);           
        }

    }
    void Kaybettin()
    {
        playerController.oyuncuHareketDurumu = false;
        playerController.OlmeAnimasyonunuTetikle();
        paneller[2].SetActive(true);

    }
    public void LevelYukle(string deger)
    {
        if(deger == "tekrar")
        {
            SceneManager.LoadScene(sahneIndex);
        }
        else
        {
            if(sahneIndex +1 != 3)
            {
                PlayerPrefs.SetInt("Level", sahneIndex + 1);
                SceneManager.LoadScene(sahneIndex + 1);
            }
            else
            {
                SceneManager.LoadScene(sahneIndex);
            }
            
        }

    }     
    public void OpenShop()
    {
        paneller[3].SetActive(true);
        shopManager.UpdatePurchaseButton();

    }
   
    public void KarakteriDurdur()
    {
        playerController.oyuncuHareketDurumu = false;
        Kazandin("20");
    }
}
