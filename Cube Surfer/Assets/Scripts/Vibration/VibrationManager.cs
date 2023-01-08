using UnityEngine;

public class VibrationManager 
{
    bool titresebilirMi;
    //bool isVibrateble;
    public void TitresebilirligiAyarla(bool titresmeDurumu)
    {
        titresebilirMi = titresmeDurumu;
    }
    public bool GetVibrationState()
    {
        return titresebilirMi;
    }
    public void CihaziTitrestir()
    {
        if (titresebilirMi)
        {
            Debug.Log("Titre�ebilir");
            Vibrator.Vibrate();
        }
        //Handheld.Vibrate();
    }
}
