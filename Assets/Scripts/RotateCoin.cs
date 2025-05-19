using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCoin : MonoBehaviour
{
    SyncroniseCoinRotation controller;
    // Start is called before the first frame update
    void Start()
    {
        controller = GameObject.Find("GameManager").GetComponent<SyncroniseCoinRotation>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(90, controller.GetCurrentRotation(), 0);
    }
}
