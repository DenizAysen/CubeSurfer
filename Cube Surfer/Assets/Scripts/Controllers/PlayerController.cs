using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerController : MonoBehaviour
{
    [Header("KARAKTER ANIMASYONU")]
    [SerializeField] private Animator animator;    
    [SerializeField] private Transform Character;
    [SerializeField]private BoxCollider[] boxColliders;
    [Header("LEVEL AYARLARI")]
    public List<GameObject> cubes;
    [SerializeField] private float playerSpeed;
    [SerializeField] private float yonlendirmeHizi;
    [SerializeField] private Transform BirakilanKuplerTransform;
    public Slider _Slider;
    public GameObject OdulNoktasi;
    public bool OdulCizgisiGecildi;
    public bool oyuncuHareketDurumu;

    Vector3 anlikKarakterpos;
    Vector3 usttekikupPos;
    Rigidbody rb;
    float fark;
    private GameObject kupOlusmaEfekti;
    private GameObject kupOlusmaCanvasi;
    private Transform CokluKupChild;
    private void Awake()
    {
        OdulCizgisiGecildi = false;
        oyuncuHareketDurumu = false;
        boxColliders = Character.GetComponents<BoxCollider>();
        rb = Character.GetComponent<Rigidbody>();
    }
    private void Start()
    {
        
        fark = Vector3.Distance(transform.position, OdulNoktasi.transform.position);
        _Slider.maxValue = fark;
        //rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        if (OdulCizgisiGecildi) 
        {
            if (_Slider.value != 0)
            {
                _Slider.value -= 0.01f;
            }
        }
        else
        {
            if (!(fark < 0.8f))
            {
                fark = Vector3.Distance(transform.position, OdulNoktasi.transform.position);
                _Slider.value = fark;
            }
            else
                _Slider.value = 0.75f;
        }
        // Debug.Log(fark);
        if (oyuncuHareketDurumu)
        {
            transform.Translate(Vector3.forward * playerSpeed * Time.deltaTime);
            if (Input.GetKey(KeyCode.Mouse0))
            {
                if (Input.GetAxis("Mouse X") < 0)
                {
                    Debug.Log("Burasi çalýþýyor");
                    transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x - .1f,
                        transform.position.y, transform.position.z), yonlendirmeHizi);
                    if (transform.position.x < -1.3f)
                        transform.position = new Vector3(-1.3f, transform.position.y, transform.position.z);
                }
               else if (Input.GetAxis("Mouse X") > 0)
                {
                    transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x + .1f,
                        transform.position.y, transform.position.z), yonlendirmeHizi);
                    if (transform.position.x > 1.1f)
                        transform.position = new Vector3(1.1f, transform.position.y, transform.position.z);
                }
            }          
        }       
    }
    IEnumerator ZiplamaAnimasyonunuTetikle()
    {
        animator.SetBool("Jump", true);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("Jump", false);
    }
    IEnumerator KupuKapat(GameObject CikanKup)
    {
        yield return new WaitForSeconds(1f);
        CikanKup.SetActive(false);
    }
    IEnumerator OlmeAnimasyonu()
    {//Kod çalýþýnca karakterin ölüm animasyonu baþlar.
    //Karakterin altýndaki bütün küpler kapanýr.
    //Karakter düþerken havada kalmamasý için 2.box colliderý açýlýr.
        animator.SetTrigger("Death");
        boxColliders[0].enabled = false;
        foreach (var item in cubes)
        {
            item.SetActive(false);
        }
        
        yield return new WaitForSeconds(.3f);

        boxColliders[1].enabled = true;
    }
    IEnumerator KazanmaAnimasyonu()
    {//Kod çalýþýnca karakter  dans etmeye baþlar ve havada kalýr
     //Karakter dans ederken bir þekilde dönüp hareket etmesin diye dönebilme yeteneðini kapattým.
     //Karakterin altýndaki küpler, karakter dans etmeye baþladýktan 1 saniye sonra kapanmaya baþlar.
        animator.SetTrigger("Dance");
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
        yield return new WaitForSeconds(1f);
        foreach (var item in cubes)
        {
            item.SetActive(false);
        }
    }
    public void KupuEkle(GameObject GelenKup,GameObject GelenEfekt)
    {
        //Karakterin Pozisyonu degisir       
            StartCoroutine(ZiplamaAnimasyonunuTetikle());
            anlikKarakterpos = Character.transform.localPosition;//Tekrar tekrar Character.transform.localPosition yazmamak için deðiþkende tutuyorum
            Character.transform.localPosition = new Vector3(anlikKarakterpos.x, anlikKarakterpos.y + 0.45f, anlikKarakterpos.z);
            anlikKarakterpos = Character.transform.localPosition;
            cubes.Add(GelenKup);
            Debug.Log(GelenKup.name + " eklendi");
            //GelenKupun pozisyonu karaktere göre ayarlanýr
            GelenKup.transform.SetParent(transform); //Parametre olarak gelen obje artýk oyuncuyu takip eder.      
            GelenKup.transform.localPosition = new Vector3(anlikKarakterpos.x, anlikKarakterpos.y - 0.2f, anlikKarakterpos.z);
            GelenKup.GetComponent<BoxCollider>().isTrigger = false;
            //Efektin pozisyonu Gelenkupe göre ayarlanýr
            kupOlusmaEfekti = GelenEfekt;
            kupOlusmaCanvasi = GelenKup.transform.GetChild(1).gameObject;
            KupOlusmaEfekti();
            KupleriSirala();
            StopCoroutine(ZiplamaAnimasyonunuTetikle());               
    }
    public void CokluKupEkle(GameObject GelenKup,List<GameObject> GelenEfektler)
    {
        StartCoroutine(ZiplamaAnimasyonunuTetikle());
        for (int i = 0; i < GelenKup.transform.childCount; i++)
        {
            CokluKupChild = GelenKup.transform.GetChild(i);           
            anlikKarakterpos = Character.transform.localPosition;//Tekrar tekrar Character.transform.localPosition yazmamak için deðiþkende tutuyorum
            Character.transform.localPosition = new Vector3(anlikKarakterpos.x, anlikKarakterpos.y + 0.45f, anlikKarakterpos.z);
            anlikKarakterpos = Character.transform.localPosition;
            cubes.Add(CokluKupChild.gameObject);
            CokluKupChild.SetParent(transform);
            CokluKupChild.localPosition = new Vector3(anlikKarakterpos.x, anlikKarakterpos.y - 0.2f, anlikKarakterpos.z);
            CokluKupChild.GetComponent<BoxCollider>().enabled = true;
            kupOlusmaEfekti = GelenEfektler[i];
            kupOlusmaCanvasi = CokluKupChild.GetChild(1).gameObject;
            CokluKupChild.gameObject.tag = "Player";
            KupOlusmaEfekti();         
        }
        StopCoroutine(ZiplamaAnimasyonunuTetikle());
    }
    void KupleriSirala()
    {//Her küp kendinden 1 üstteki küpün 0.45f altýnda pozisyon alýr.
        for (int i = cubes.Count - 1; i > 0; i--)
        {
            usttekikupPos = cubes[i].transform.localPosition;
            cubes[i - 1].transform.localPosition = new Vector3(usttekikupPos.x, usttekikupPos.y - 0.45f, usttekikupPos.z);
        }
    }
    public void KupuBirak(GameObject CikanKup,bool Lav=false)
    {//Duvara çarpan küp bu metoda parametre olarak gönderilir.
     //Küp listeden ve oyuncunun child ý olmaktan çýkar.
     //Duvara çarpan küp 1 saniye sonra kapanýr
        CikanKup.transform.SetParent(BirakilanKuplerTransform);
        cubes.Remove(CikanKup);
        if (Lav == true)
        {
            CikanKup.SetActive(false);
        }
        else
        {
            StartCoroutine(KupuKapat(CikanKup));
            StartCoroutine(ZiplamaAnimasyonunuTetikle());
        }    
        
       Invoke("KupleriSirala",1f);
    } 
    public void OlmeAnimasyonunuTetikle()
    {
        StartCoroutine(OlmeAnimasyonu());
    }    
    public void KazanmaAnimasyonunuTetikle()
    {
        StartCoroutine(KazanmaAnimasyonu());
    }
    private void KupOlusmaEfekti()
    {
        kupOlusmaEfekti.transform.position = cubes[cubes.Count - 1].transform.position;
        kupOlusmaEfekti.GetComponent<ParticleSystem>().Play();
        kupOlusmaCanvasi.gameObject.SetActive(true);
    }
}
