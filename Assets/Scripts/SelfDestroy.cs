using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    [SerializeField] private float destroyDelayTime;

    void Start()
    {
        Destroy(gameObject, destroyDelayTime);
    }

}
