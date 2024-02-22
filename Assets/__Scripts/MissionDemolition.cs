using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
using UnityEngine.UI;

public enum GameMode{
    idle,
    playing,
    levelEnd
}

public class MissionDemolition : MonoBehaviour
{
    static private MissionDemolition S; // a private Singleton

    [Header("Set in Inspector")]
    public Text uitLevel;
    public Text uitShots;
    public Text uitButton;
    public Vector3 castlePos; // Where to put castles
    public GameObject[] castles; // An array of the castles
    public GameObject GOUI;
    public GameObject GiveUpUI;
    public GameObject UIButton;
    public Text SLText;

    [Header("Set Dynamically")]
    public int level;
    public int levelMax;
    public int shotsTaken;
    public GameObject castle; // The current castle
    public GameMode mode = GameMode.idle;
    public string showing = "Show Slingshot"; // Follow cam mode
    static public int shotsLeft; // Shots the player has left
    // Start is called before the first frame update
    void Start()
    {
        S = this; // Define the Singleton

        level = 0;
        levelMax = castles.Length;
        StartLevel();
    }

    void StartLevel(){
        // Get rid of the old castle if one exists
        if (castle != null){
            Destroy(castle);
        }

        // Destroy old projectiles if they exist
        GameObject[] gos = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject pTemp in gos){
            Destroy(pTemp);
        }

        // Instantiate the new castle
        castle = Instantiate<GameObject>(castles[level]);
        castle.transform.position = castlePos;
        shotsTaken = 0;
        shotsLeft = 4;
        // GameObject Slingshot = GameObject.FindGameObjectWithTag("Slingshot");
        // string SlingText = Slingshot.GetComponent<Slingshot>().ShotsLeftText.text;
        SLText.text = "Shots Left: " + shotsLeft;

        // Reset the camera
        SwitchView("Show Both");
        ProjectileLine.S.Clear();

        // Reset the goal
        Goal.goalMet = false;

        UpdateGUI();

        mode = GameMode.playing;
    }

    void UpdateGUI(){
        // Show the data in the GUITexts
        uitLevel.text = "Level: " + (level + 1) + " of " + levelMax;
        uitShots.text = "Shots taken: " + shotsTaken;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateGUI();

        // Check for level end
        if (mode == GameMode.playing && Goal.goalMet){
            // Change mode to stop checking for level end
            mode = GameMode.levelEnd;
            // Zoom out
            SwitchView("Show Both");
            // Start the next level in 2 seconds
            Invoke("NextLevel", 2f);
        }
    }

    void NextLevel(){
        level++;
        if (level == levelMax){
            level = 0;
        }
        StartLevel();
    }

    public void SwitchView(string eView = ""){
        if (eView == ""){
            eView = uitButton.text;
        }
        showing = eView;
        switch (showing){
            case "Show Slingshot":
                FollowCam.POI = null;
                uitButton.text = "Show Castle";
                break;

            case "Show Castle":
                FollowCam.POI = S.castle;
                uitButton.text = "Show Both";
                break;

            case "Show Both":
                FollowCam.POI = GameObject.Find("ViewBoth");
                uitButton.text = "Show Slingshot";
                break;
        }
    }

    public void GameOver(){
        GiveUpUI.SetActive(false);
        GOUI.SetActive(true);
        GameObject castle = GameObject.FindGameObjectWithTag("Castle");
        castle.SetActive(false);
        GameObject Slingshot = GameObject.FindGameObjectWithTag("Slingshot"); // Add reference to the Slingshot object
        Slingshot.SetActive(true); // Call SetActive method on the Slingshot object
        UIButton.SetActive(false); // Call SetActive method on the Slingshot object

    }

    // Static method that allows code anywhere to increment shotsTaken
    public static void ShotFired(){
        S.shotsTaken++;
    }

    public void Restart(){
        GOUI.SetActive(false);
        GameObject Slingshot = GameObject.FindGameObjectWithTag("Slingshot"); // Add reference to the Slingshot object
        Slingshot.SetActive(true); // Call SetActive method on the Slingshot object
        UIButton.SetActive(true); // Call SetActive method on the Slingshot object
        Invoke("Start", 0.5f);
    }

}
