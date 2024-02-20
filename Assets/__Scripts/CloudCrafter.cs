using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudCrafter : MonoBehaviour
{
    [Header("Set in Inspector")]
    public int numClouds = 40;
    public GameObject cloudPrefab;
    public Vector3 cloudPosMin = new Vector3(-50,-5,10);
    public Vector3 cloudPosMax = new Vector3(150,100,10);
    public float cloudScaleMin = 1;
    public float cloudScaleMax = 3;
    public float cloudSpeedMult = 0.5f;
    private GameObject[] cloudInstances;

    void Awake(){
        // Make an array large enough to hold all the Cloud_ instances
        cloudInstances = new GameObject[numClouds];
        // Find the CloudAnchor parent GameObject
        GameObject anchor = GameObject.Find("CloudAnchor");
        // Iterate through and make Cloud_s
        GameObject cloud;
        for (int i=0; i<numClouds; i++){
            cloud = Instantiate<GameObject>(cloudPrefab);
            // Position the Cloud_
            Vector3 cPos = Vector3.zero;
            cPos.x = Random.Range(cloudPosMin.x, cloudPosMax.x);
            cPos.y = Random.Range(cloudPosMin.y, cloudPosMax.y);
            // Scale the Cloud_
            float scaleU = Random.value;
            float scaleVal = Mathf.Lerp(cloudScaleMin, cloudScaleMax, scaleU);
            // Smaller clouds (with smaller scaleU) should be nearer the ground
            cPos.y = Mathf.Lerp(cloudPosMin.y, cPos.y, scaleU);
            // Smaller clouds should be further away
            cPos.z = 100 - 90*scaleU;
            // Apply these transforms to the cloud
            cloud.transform.position = cPos;
            cloud.transform.localScale = Vector3.one * scaleVal;
            // Make the cloud a child of the anchor
            cloud.transform.SetParent(anchor.transform);
            // Add the cloud to cloudInstances
            cloudInstances[i] = cloud;
        }
    }

    void Update(){
        // Iterate over each cloud that was created
        foreach (GameObject cloud in cloudInstances){
            // Get the cloud scale and position
            float scaleVal = cloud.transform.localScale.x;
            Vector3 cPos = cloud.transform.position;
            // Move larger clouds faster
            cPos.x -= scaleVal * Time.deltaTime * cloudSpeedMult;
            // If a cloud has moved too far to the left...
            if (cPos.x <= cloudPosMin.x){
                // Move it to the far right
                cPos.x = cloudPosMax.x;
            }
            // Apply the new position to the cloud
            cloud.transform.position = cPos;
        }
    
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

}
