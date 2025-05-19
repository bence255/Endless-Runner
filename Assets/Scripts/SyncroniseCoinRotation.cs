using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncroniseCoinRotation : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    GameManager manager;
    float currentRotation = 180;
    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.IsGameStarted() && !manager.IsGamePaused())currentRotation += rotationSpeed * Time.deltaTime;
        if (currentRotation > 360) currentRotation -= 360;
    }

    public float GetCurrentRotation()
    {
        return currentRotation;
    }
}
