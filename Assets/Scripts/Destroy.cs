using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public Transform attackPos;
    public bool IsPlayerFlash;
    private void Awake()
    {
        attackPos = GameObject.Find("GunDropPoint").transform;
    }
    void Start()
    {
        Invoke("DestroyMuzzleFlash", 1f);
    }

    private void Update()
    {
        if (IsPlayerFlash)
            transform.position = attackPos.position;
    }

    void DestroyMuzzleFlash()
    {
        Destroy(gameObject);
    }
}
