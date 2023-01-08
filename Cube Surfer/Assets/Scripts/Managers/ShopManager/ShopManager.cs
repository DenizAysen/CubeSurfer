using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ShopManager : MonoBehaviour
{
    [SerializeField] private CubeManager cubeManager;
    [Header("Elements")]
    [SerializeField] private Button purchaseButton;
    [SerializeField] private SkinButton[] skinButtons;
    [SerializeField] private TextMeshProUGUI[] elmasTextleri;
    [SerializeField] private GameObject ShopPanel;
    [Header("Skins")]
    [SerializeField] private Sprite[] skins;
    [Header("Prices")]
    [SerializeField] private int skinPrice;
    [SerializeField] private TextMeshProUGUI priceText;
    int elmasSayisi;
    void Start()
    {//Default olarak sarý küp rengi açýlýr
        elmasSayisi = PlayerPrefs.GetInt("elmas");
        UpdatePurchaseButton();
        ConfigureButtons();
        priceText.text = skinPrice.ToString();
        elmasTextleri[1].text = PlayerPrefs.GetInt("elmas").ToString();
        if (PlayerPrefs.HasKey("Color"))
        {
            cubeManager.KuplerinRenginiDegistir(PlayerPrefs.GetInt("Color"));
        }
        else
        {
            UnlockSkin(0);
            SelectSkin(0);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UnlockSkin(Random.Range(0, skinButtons.Length));
        }
        /*if (Input.GetKeyDown(KeyCode.D))
            PlayerPrefs.DeleteAll();*/
    }
    private void ConfigureButtons()
    {
        
        for(int i = 0; i < skinButtons.Length; i++)
        {
            bool unlocked = PlayerPrefs.GetInt("skinButton" + i) == 1;
            skinButtons[i].Configure(skins[i], unlocked);
            int skinIndex = i;
            skinButtons[i].GetButton().onClick.AddListener(() => SelectSkin(skinIndex));
        }
    }
    private void SelectSkin(int skinIndex)
    {
        //Debug.Log("Skin " + skinIndex + " has been selected");
        for (int i = 0; i < skinButtons.Length; i++)
        {
            if (skinIndex == i)
            {
                skinButtons[i].Select();
                cubeManager.KuplerinRenginiDegistir(i);
                PlayerPrefs.SetInt("Color", i);
            }
            else
                skinButtons[i].DeSelect();
        }
    }
    public void UnlockSkin(int skinIndex)
    {
        PlayerPrefs.SetInt("skinButton" + skinIndex,1);
        skinButtons[skinIndex].Unlock();
    }
    private void UnlockSkin(SkinButton skinButton)
    {
        int skinIndex = skinButton.transform.GetSiblingIndex();
        UnlockSkin(skinIndex);
    }
    public void PurchaseSkin()
    {
        List<SkinButton> skinButtonsList = new List<SkinButton>();
        for (int i = 0; i < skinButtons.Length; i++)
        {//Eðer oyuncu skini açmamýþsa listeye ekler
            if (!skinButtons[i].IsUnlocked())
            {
                skinButtonsList.Add(skinButtons[i]);
            }
        }
        if(skinButtonsList.Count <= 0)
        {
            return;
        }
        //Listeden rastgele bir skin açýlýr ve seçilir
        SkinButton randomSkinButton = skinButtonsList[Random.Range(0, skinButtonsList.Count)];
        UnlockSkin(randomSkinButton);
        SelectSkin(randomSkinButton.transform.GetSiblingIndex());
        UseCoins();
        UpdatePurchaseButton();     
    }
    public void UpdatePurchaseButton()
    {
        if (PlayerPrefs.GetInt("elmas") < skinPrice)
        {
            purchaseButton.interactable = false;
        }
        else
            purchaseButton.interactable = true;
    }
    private void UseCoins()
    {
        if (PlayerPrefs.GetInt("elmas") - skinPrice > 0)
        {         
            PlayerPrefs.SetInt("elmas", elmasSayisi-skinPrice);
            elmasSayisi -= skinPrice;
            Debug.Log("Para Harcandýktan sonraki elmas sayýsý:" + PlayerPrefs.GetInt("elmas"));
        }

        else
        {
            elmasSayisi = 0;
            PlayerPrefs.SetInt("elmas", elmasSayisi);
        }
            
        foreach (TextMeshProUGUI elmasText in elmasTextleri)
        {
            elmasText.text = elmasSayisi.ToString();
        }
    }
    public void CloseShop()
    {
        ShopPanel.SetActive(false);
    }
}
