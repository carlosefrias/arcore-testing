﻿using System.Collections.Generic;
using GoogleARCore;
using UnityEngine;

public class FoodController : MonoBehaviour
{
    private DetectedPlane detectedPlane;
    private GameObject foodInstance;
    private float foodAge;
    private const float maxAge = 10f;

    public GameObject[] foodModels;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (detectedPlane == null)
        {
            return;
        }

        if (detectedPlane.TrackingState != TrackingState.Tracking)
        {
            return;
        }
        // Check for the plane being subsumed
        // If the plane has been subsumed switch attachment to the subsuming plane.
        while (detectedPlane.SubsumedBy != null)
        {
            detectedPlane = detectedPlane.SubsumedBy;
        }
        
        if (foodInstance == null || foodInstance.activeSelf == false)
        {
            SpawnFoodInstance();
            return;
        }
        
        foodAge += Time.deltaTime;
        if (!(foodAge >= maxAge)) return;
        DestroyObject(foodInstance);
        foodInstance = null;
    }
    
    public void SetSelectedPlane(DetectedPlane selectedPlane)
    {
        detectedPlane = selectedPlane;
    }
    
    void SpawnFoodInstance ()
    {
        var foodItem = foodModels [Random.Range (0, foodModels.Length)];
    
        // Pick a location.  This is done by selecting a vertex at random and then
        // a random point between it and the center of the plane.
        var vertices = new List<Vector3> ();
        detectedPlane.GetBoundaryPolygon (vertices);
        var pt = vertices [Random.Range (0, vertices.Count)];
        var dist = Random.Range (0.05f, 1f);
        var position = Vector3.Lerp (pt, detectedPlane.CenterPose.position, dist);
        // Move the object above the plane.
        position.y += .05f;


        var anchor = detectedPlane.CreateAnchor (new Pose (position, Quaternion.identity));

        foodInstance = Instantiate (foodItem, position, Quaternion.identity,
            anchor.transform);

        // Set the tag.
        foodInstance.tag = "food";
    
        foodInstance.transform.localScale = new Vector3 (.025f, .025f, .025f);
        foodInstance.transform.SetParent (anchor.transform);
        foodAge = 0;

        foodInstance.AddComponent<FoodMotion> ();
    }
}