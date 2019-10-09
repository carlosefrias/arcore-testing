﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodConsumer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("food")) return;
        collision.gameObject.SetActive(false);
        var s = GetComponentInParent<Slithering>();

        if (s != null)
        {
            s.AddBodyPart();
        }
    }
}
