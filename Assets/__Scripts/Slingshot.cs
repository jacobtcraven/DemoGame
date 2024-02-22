using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slingshot : MonoBehaviour
{
    static private Slingshot S;
    //fields set in the Unity Inspector pane
    [Header("Set in Inspector")]
    public GameObject prefabProjectile;
    public float velocityMult = 10f;

    [Header("Set Dynamically")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    private Rigidbody projectileRigidbody;
    public Text ShotsLeftText;
    public GameObject GiveUpUI;
    static public Vector3 LAUNCH_POS{
        get{
            if (S == null) return Vector3.zero;
            return S.launchPos;
        }
    }

    void Awake(){
        S = this;
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }
    void OnMouseEnter(){
        // print("Slingshot:OnMouseEnter()");
        launchPoint.SetActive(true);
    }

    void OnMouseExit(){
        // print("Slingshot:OnMouseExit()");
        launchPoint.SetActive(false);
    }

    void OnMouseDown(){
        // The player has pressed the mouse button while over Slingshot
        aimingMode = true;
        if(MissionDemolition.shotsLeft > 0){
            // Instantiate a Projectile
            projectile = Instantiate(prefabProjectile) as GameObject;
            // Start it at the launchPoint
            projectile.transform.position = launchPos;
            // Set it to isKinematic for now
            projectile.GetComponent<Rigidbody>().isKinematic = true;
            projectileRigidbody = projectile.GetComponent<Rigidbody>();
            projectileRigidbody.isKinematic = true;
        }
    }

    void Update(){
        // If Slingshot is not in aimingMode, don't run this code
        if (!aimingMode) return;
        if(MissionDemolition.shotsLeft > 0){
        // Get the current mouse position in 2D screen coordinates
            Vector3 mousePos2D = Input.mousePosition;
            // Convert the mouse position to 3D world coordinates
            mousePos2D.z = -Camera.main.transform.position.z;
            Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);
            // Find the delta from the launchPos to the mousePos3D
            Vector3 mouseDelta = mousePos3D - launchPos;
            // Limit mouseDelta to the radius of the Slingshot SphereCollider
            float maxMagnitude = this.GetComponent<SphereCollider>().radius;
            if (mouseDelta.magnitude > maxMagnitude){
                mouseDelta.Normalize();
                mouseDelta *= maxMagnitude;
            }
            // Move the projectile to this new position
            Vector3 projPos = launchPos + mouseDelta;
            if (projectile != null){
                projectile.transform.position = projPos;
            }
            if (Input.GetMouseButtonUp(0)){
                // The mouse has been released
                aimingMode = false;
                projectileRigidbody.isKinematic = false;
                projectileRigidbody.velocity = -mouseDelta * velocityMult;
                FollowCam.POI = projectile;
                MissionDemolition.ShotFired();
                ProjectileLine.S.poi = projectile;
                projectile = null;
                MissionDemolition.shotsLeft--;
                ShotsLeftText.text = "Shots Left: " + MissionDemolition.shotsLeft;
                if (MissionDemolition.shotsLeft == 0){
                    // VerifyOver(5);
                    // MissionDemolition.GameOver();
                    GiveUpUI.SetActive(true);
                }
            }

        }
    }

    // IEnumerator VerifyOver(float waitTime)
    // {
    //     yield return new WaitForSecondsRealtime(waitTime);
    //    if (MissionDemolition.shotsLeft <= 0){
            
    //         MissionDemolition.GameOver();
    //    }
    // }
}
