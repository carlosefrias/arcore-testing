using UnityEngine;
using GoogleARCore;


public class SceneController : MonoBehaviour
{
    public Camera firstPersonCamera;
    public ScoreboardController scoreboard; 
    public SnakeController snakeController;
    
    void Start ()
    {
        QuitOnConnectionErrors ();
    }
    
    void Update() 
    {
        // The session status must be Tracking in order to access the Frame.
        if (Session.Status != SessionStatus.Tracking)
        {
            const int lostTrackingSleepTimeout = 15;
            Screen.sleepTimeout = lostTrackingSleepTimeout;
            return;
        }
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        // Add to the end of Update()
        ProcessTouches();
        
        scoreboard.SetScore(snakeController.GetLength());
    }
    
    void QuitOnConnectionErrors()
    {
        if (Session.Status ==  SessionStatus.ErrorPermissionNotGranted)
        {
            StartCoroutine(CodelabUtils.ToastAndExit(
                "Camera permission is needed to run this application.", 5));
        }
        else if (Session.Status.IsError())
        {
            // This covers a variety of errors.  See reference for details
            // https://developers.google.com/ar/reference/unity/namespace/GoogleARCore
            StartCoroutine(CodelabUtils.ToastAndExit(
                "ARCore encountered a problem connecting. Please restart the app.", 5));
        }
    }
    
    void ProcessTouches ()
    {
        Touch touch;
        if (Input.touchCount != 1 ||
            (touch = Input.GetTouch (0)).phase != TouchPhase.Began)
        {
            return;
        }

        const TrackableHitFlags rayCastFilter = TrackableHitFlags.PlaneWithinBounds |
                                                TrackableHitFlags.PlaneWithinPolygon;

        if (Frame.Raycast (touch.position.x, touch.position.y, rayCastFilter, out var hit))
        {
            SetSelectedPlane (hit.Trackable as DetectedPlane);
        }
    }
    
    void SetSelectedPlane (DetectedPlane selectedPlane)
    {
        Debug.Log ("Selected plane centered at " + selectedPlane.CenterPose.position);
        scoreboard.SetSelectedPlane(selectedPlane);
        snakeController.SetPlane(selectedPlane);
        GetComponent<FoodController>().SetSelectedPlane(selectedPlane);
    }
    
}
