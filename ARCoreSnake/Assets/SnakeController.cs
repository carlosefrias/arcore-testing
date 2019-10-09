﻿using GoogleARCore;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    private DetectedPlane detectedPlane;
  
    public GameObject snakeHeadPrefab;
    public GameObject pointer;
    public Camera firstPersonCamera;
    // Speed to move.
    public float speed = 20f;
    
    private GameObject snakeInstance;
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (snakeInstance == null || snakeInstance.activeSelf == false) 
        {
            pointer.SetActive(false);
            return;
        }
        pointer.SetActive(true);

        const TrackableHitFlags rayCastFilter = TrackableHitFlags.PlaneWithinBounds;    

        if (Frame.Raycast (Screen.width/2, Screen.height/2, rayCastFilter, out var hit))
        {
            var pt = hit.Pose.position;
            //Set the Y to the Y of the snakeInstance
            pt.y = snakeInstance.transform.position.y;
            // Set the y position relative to the plane and attach the pointer to the plane
            var position = pointer.transform.position;
            var pos = position;
            pos.y = pt.y;
            position = pos; 

            // Now lerp to the position                                         
            position = Vector3.Lerp (position, pt,
                Time.smoothDeltaTime * speed);
            pointer.transform.position = position;
        }
        
        // Move towards the pointer, slow down if very close.                                                                                     
        var dist = Vector3.Distance (pointer.transform.position,
                         snakeInstance.transform.position) - 0.05f;
        if (dist < 0)
        {
            dist = 0;
        }

        var rb = snakeInstance.GetComponent<Rigidbody> ();
        rb.transform.LookAt (pointer.transform.position);
        rb.velocity = snakeInstance.transform.localScale.x *
                      snakeInstance.transform.forward * dist / .01f;
    }
    
    public void SetPlane (DetectedPlane plane)
    {
        detectedPlane = plane;
        // Spawn a new snake.
        SpawnSnake();
    }
    
    void SpawnSnake ()
    {
        if (snakeInstance != null)
        {
            DestroyImmediate (snakeInstance);
        }

        var pos = detectedPlane.CenterPose.position;

        // Not anchored, it is rigidbody that is influenced by the physics engine.
        snakeInstance = Instantiate (snakeHeadPrefab, pos,
            Quaternion.identity, transform);

        // Pass the head to the slithering component to make movement work.
        GetComponent<Slithering> ().Head = snakeInstance.transform;
    }
    
}
