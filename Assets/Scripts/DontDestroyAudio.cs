using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyAudio : MonoBehaviour
{
    private static DontDestroyAudio _instance;
    private void Awake()
    {
        if (!_instance)
            _instance = this;
        
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(transform.gameObject);
    }
}
