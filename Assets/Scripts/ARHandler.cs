using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

/// <summary>
/// this program has for objective to detect images and simulate the audio environment described in the picture.
/// in collaboration with the SMART initiative and photograph artist Malika squalli and ESAV.
/// this app is to be used in conjunction with Malika's work.
///
/// This app is an effort to promote Art for the visually impaired and allow them to enjoy a visual work with auditive response.
///
/// possible applications are Virtual tour of musea and art exhibitions. 
/// </summary>

[RequireComponent(typeof(ARRaycastManager))]
public class ARHandler : MonoBehaviour
{

    private ARTrackedImageManager arTrackedManager;
    private ARRaycastManager arRaycastManager;
    private ARReferencePointManager aRReferencePointManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    private List<ARReferencePoint> referencePoints = new List<ARReferencePoint>();



    [SerializeField]
    private GameObject placedPrefab;
    private GameObject placingPrefab;

    private Vector3 refPoint;

    public GameObject PlacedPrefab{
        get{
            return placedPrefab;
        }
        set{
            placedPrefab = value;
        }
    }

    public Text debug;

    [SerializeField]
    private GameObject[] exhibitions;

    [SerializeField]
    private Vector3 scaleFactor = new Vector3(.1f,.1f,.1f);

    private Dictionary<string,GameObject> arObjects = new Dictionary<string, GameObject>();

    private void Awake() {
        arTrackedManager = FindObjectOfType<ARTrackedImageManager>();
        aRReferencePointManager = GetComponent<ARReferencePointManager>();
        arRaycastManager = GetComponent<ARRaycastManager>();

        foreach (GameObject arObject in exhibitions)
        {
            GameObject newARObject = Instantiate(arObject,Vector3.zero,Quaternion.identity);
            newARObject.name = arObject.name;
            arObjects.Add(arObject.name,newARObject);
            newARObject.SetActive(false);
        }


    }

    private void OnEnable() {
        arTrackedManager.trackedImagesChanged += OnImageChanged;
    }

    private void OnDisable() {
        arTrackedManager.trackedImagesChanged -= OnImageChanged;
    }

    private void OnImageChanged(ARTrackedImagesChangedEventArgs args){
        foreach (var trackedImage in args.added)
        {
            UpdateARImage(trackedImage);
        }

        foreach (var trackedImage in args.updated)
        {
            UpdateARImage(trackedImage);
        }

        foreach (var trackedImage in args.removed)
        {
            arObjects[trackedImage.name].SetActive(false);
        }
    }

    private void UpdateARImage(ARTrackedImage trackedImage){
        AssignGameObject(trackedImage.referenceImage.name,trackedImage.transform.position);
    }

    private void AssignGameObject(string name, Vector3 position)
    {
        if(exhibitions != null){

            GameObject goARObject = arObjects[name];    

            goARObject.SetActive(true); 
            goARObject.transform.position = position;
            goARObject.transform.localScale = scaleFactor;

            foreach (GameObject go in arObjects.Values)
            {
                if(go.name != name){
                    go.SetActive(false);
                }
            } 
        }
    }

    bool TryGetTouchPosition(out Vector2 touchPosition){
        if(Input.touchCount > 0){
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }

    bool TryGetPosition(){
        return false;
    }

    void Placement(){
        if(!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if(arRaycastManager.Raycast(touchPosition,hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon)){
            var hitPose = hits[0].pose;
            if(placingPrefab == null)
                placingPrefab = Instantiate(placedPrefab, hitPose.position,hitPose.rotation);
        }
    }

    void ReferencePointPlacement(){
        if(!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if(arRaycastManager.Raycast(touchPosition,hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon)){
            var hitPose = hits[0].pose;
            ARReferencePoint referencePoint = aRReferencePointManager.AddReferencePoint(hitPose);

            if(referencePoint == null)
                Debug.Log("nothing");
            else
                referencePoints.Add(referencePoint);

        }
    }

    void ReferencePointTrackPlacement(Vector3 track){
        

        if(arRaycastManager.Raycast(track,hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon)){
            var hitPose = hits[0].pose;
            ARReferencePoint referencePoint = aRReferencePointManager.AddReferencePoint(hitPose);

            if(referencePoint == null)
                Debug.Log("nothing");
            else
                referencePoints.Add(referencePoint);

        }
    }

    void ReferencePointTrackPlacement2(Vector3 track){
        

        if(arRaycastManager.Raycast(track,hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon)){
            //var hitPose = hits[0].pose;
            ARReferencePoint referencePoint = aRReferencePointManager.AddReferencePoint(new Pose(track,Quaternion.identity));

            if(referencePoint == null)
                Debug.Log("nothing");
            else
                referencePoints.Add(referencePoint);

        }
    }

    void ReferenceVectorPlacement(Vector3 track){
        ARReferencePoint referencePoint = aRReferencePointManager.AddReferencePoint(new Pose(track,Quaternion.identity));
    }
   
    // Update is called once per frame
    void Update()
    {
        //ReferenceVectorPlacement(refPoint);
    }
}
