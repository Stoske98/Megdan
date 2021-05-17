using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cm;
    private void Start()
    {
        cm = GameManager.Instance.mainCamera.transform;
    }
    private void LateUpdate()
    {
        transform.LookAt(transform.position + cm.forward);
    }
}
