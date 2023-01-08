using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    private void OnTriggerEnter(Collider other)
    {
        //Eger oyuncu küpü toplamýþsa küpün tagý Player olur
        //Toplamamýþsa küpün tagý Cubedür.
        if (gameObject.CompareTag("Player"))
        {
            if (other.CompareTag("Cube"))
            {
                gameManager.KupEkle(other.gameObject);
                other.gameObject.tag = "Player";
            }
            if (other.CompareTag("Odul"))
            {
                gameManager.CizgiGecildi();
            }
            if (other.CompareTag("Lav"))
            {
                gameManager.KupuSil(gameObject, null, true);
            }
            if (other.CompareTag("Elmas"))
            {
                gameManager.ElmasEkle();
                other.gameObject.SetActive(false);
            }
            if (other.CompareTag("MultipleCube"))
            {
                gameManager.KupEkle(other.gameObject,true);
                if(other.transform.childCount == 0)
                {
                    other.gameObject.tag = "Untagged";
                }
            }
            if (other.CompareTag("Finish"))
            {
                gameManager.KarakteriDurdur();
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.CompareTag("Wall"))
            {
                Debug.Log(gameObject.name + " duvara çarptý");
                gameManager.KupuSil(gameObject);
                gameObject.tag = "Untagged";
            }
            if (collision.gameObject.CompareTag("Platform"))
            {
                gameObject.transform.GetChild(0).GetComponent<TrailRenderer>().enabled = true;
            }
            if (collision.gameObject.CompareTag("OdulluPlatform"))
            {
                gameManager.KupuSil(gameObject,collision.gameObject.name);
                collision.gameObject.tag = "Untagged";
            }
        }       
    }
}
