using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 target_offset;
    private Vector3 newCameraPosition;
    public bool isFinished;
    void Start()
    {
        target_offset = transform.position - target.position;
    }
    /*0.125f*/
    // Update is called once per frame
   /* void LateUpdate()
    {
        if (!isFinished)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + target_offset, 1f);
        }
        else
        {
            transform.SetParent(target);
        }
        
    }*/
   public void KamerayiYukselt()
    {
        newCameraPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + 0.45f, transform.localPosition.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, newCameraPosition, .7f);
    }
}
