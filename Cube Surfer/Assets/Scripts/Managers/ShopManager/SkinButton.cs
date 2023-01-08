using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkinButton : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private Button thisButton;
    [SerializeField] private Image cubeImage;
    [SerializeField] private GameObject lockImage;
    [SerializeField] private GameObject selector;
    private bool unlocked;
    private Color32 color;
    public void Configure(Sprite cubeSkin, bool unlocked)
    {
        cubeImage.sprite = cubeSkin;
        this.unlocked = unlocked;

        if (unlocked)
            Unlock();
        else
            Lock();
    }
    public void Unlock()
    {
        color = new Color32(255, 255, 255, 255);
        thisButton.interactable = true;
        cubeImage.gameObject.SetActive(true);
        cubeImage.color = color;
        lockImage.SetActive(false);
        unlocked = true;
    }
    public void Lock()
    {
        thisButton.interactable = false;
        cubeImage.gameObject.SetActive(false);
        lockImage.SetActive(true);
    }
    public void Select()
    {
        selector.SetActive(true);
    }
    public void DeSelect()
    {
        selector.SetActive(false);
    }
    public bool IsUnlocked()
    {
        return unlocked;
    }
    public Button GetButton()
    {
        return thisButton;
    }
}
