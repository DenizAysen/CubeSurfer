using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeManager : MonoBehaviour
{
    [SerializeField] private MeshRenderer[] Cubes;
    [SerializeField] private Material[] CubeColors;
    [SerializeField] private Material[] CubeTrailMaterials;
    private Material[] _cubesMats;
    /*
    void Awake()
    {
        if (PlayerPrefs.HasKey("Color"))
        {
            KuplerinRenginiDegistir(PlayerPrefs.GetInt("Color"));
        }
        else
        {
            PlayerPrefs.SetInt("Color", 0);
            KuplerinRenginiDegistir(PlayerPrefs.GetInt("Color"));
        }
    }*/
    public void KuplerinRenginiDegistir(int ColorIndex)
    {
        for (int i = 0; i < Cubes.Length; i++)
        {
            _cubesMats = Cubes[i].materials;
            _cubesMats[0] = CubeColors[ColorIndex];
            Cubes[i].materials = _cubesMats;
            Cubes[i].transform.GetChild(0).GetComponent<TrailRenderer>().material= CubeTrailMaterials[ColorIndex];
        }
    }
}
