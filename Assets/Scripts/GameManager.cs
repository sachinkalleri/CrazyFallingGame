using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
    public float gravityScale = 1.0f;
    public float characterSpeed = -1.0f;

    public float playerLife = 10.0f;
    public float gameTime = 0.0f;
    public bool shieldStatus = false;    
    public float shieldTimer = 0.0f;
    public float lifeStatus;
    public Color lifeStatusColor = new Color(0.15f, 0.6f, 0.8f); //Initially set as blue.

    public float maxFogDensity;
    
    float moveComponent = -1.0f;
    float setGravityScale;
    //float lSpanMax = StaticData.lStretched;
    //float rSpanMax = StaticData.rStretched;
    float lSpanMax = 0.0f;
    float rSpanMax = 0.0f;
    float lSpanMin;
    float rSpanMin;
    //float lSpanMin = StaticData.lClosed;
    //float rSpanMin = StaticData.rClosed;
    float lSpanCurr;
    float rSpanCurr;
    float ratioL2R;
    float speed;
    float rotationResetTimer = 0.0f;
    float scoreCardTimer = 0.0f;
    float isRHandExtended = 0.0f; //Used for Controller and Hand pose mode to control speed of the fall.
    float isLHandExtended = 0.0f; //Used for Controller and Hand pose mode to control speed of the fall.
    float speedConstant = 0.3f;
    float playerLifeTimeFactor;

    int gameTimeDisplaySeconds;
    int gameTimeDisplayMinutes;
    int lifeDisplay;

    bool isLHandFist = false;
    bool isRHandFist = false;
    bool isLHandThumbsUp = false;
    bool isRHandThumbsUp = false;
    

    Color blue = new Color(0.15f, 0.6f, 0.8f);
    Color red = new Color(0.86f, 0.16f, 0.23f);

    public GameObject lHandPrefab;
    public GameObject rHandPrefab;
    public GameObject centerEyeHMDObject;
    public GameObject playerController;
    public GameObject player;
    public GameObject headset;
    public GameObject shield;
    public GameObject spawners;
    public GameObject InfoSlab;
    public GameObject controlSelectionTextObject;
    public GameObject scoreTextObject;

    public Text scoreText;
    public Text lifeText;

    public bool isPlayActive = false;
    public bool isStarting = true;
    public int inputModeSelected = -1;

    public OVRMeshRenderer rHandMeshRenderer;
    public OVRMeshRenderer lHandMeshRenderer;
    CharacterController controller;

    Vector3 resetPosition = Vector3.zero;
    Vector3 resetRotation = Vector3.zero;

    public AudioSource audioSource;
    public AudioClip[] collisionClips;

    private InputDevice rightController;
    private InputDevice leftController;


    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = new Vector3(0, 9.8f * gravityScale, 0);
        setGravityScale = gravityScale;
        controller = playerController.GetComponent<CharacterController>();
        GetCurrentSpan();
        lSpanMin = lSpanCurr;
        rSpanMin = rSpanCurr;
        SetControllers();
    }



    // Update is called once per frame
    void Update()
    {
        if (!rHandMeshRenderer.shouldRender && !lHandMeshRenderer.shouldRender && (leftController == null || rightController == null)) SetControllers();
        if (isPlayActive) UpdateStats();
        DisplayStats();
        IsPlayerDead(); //Checks if the player is dead and does the necessary if player ran out of life.
        SetFog();
        if(shieldStatus) Shield();
        ConstraintOnPlayerTransform();

        //SetGravityLegacy();
        //ObtainCurrentHandSpanLegacyMethod();        

        GetCurrentSpan();
        UpdateSpanMinMax();
        ManageMovementAndPlayModes();

        //DELETELATER (For testing purposes)
        //MovePlayerUsingKeyboard();

        if (inputModeSelected == 0) SetGravityWithSpan();
        if (inputModeSelected == 1) SetGravityWithHandPoseAndControllers();
    }
    /*Update Ends here*/



    //To set the controllers up
    void SetControllers()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);

        if (devices.Count > 0)
        {
            rightController = devices[0];
        }

        InputDeviceCharacteristics leftControllerCharacteristics = InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller;
        InputDevices.GetDevicesWithCharacteristics(leftControllerCharacteristics, devices);

        if (devices.Count > 0)
        {
            leftController = devices[0];
        }
    }

    //Sets gravity as multiple of 9.8, based on the set gravity scale. Used to invert gravity.
    void SetGravityLegacy()
    {
        //Sets and updates gravity scale.
        if (setGravityScale != gravityScale)
        {
            Physics.gravity = new Vector3(0, 9.8f * gravityScale, 0);
            setGravityScale = gravityScale;
        }
    }

    //Controls gravity with span to control the speed.
    void SetGravityWithSpan()
    {
        if ((Mathf.Abs(lSpanCurr) + Mathf.Abs(rSpanCurr)) != 0)
        {
            //speed = (Mathf.Abs(lSpanMax) + Mathf.Abs(rSpanMax)) / (Mathf.Abs(lSpanCurr) + Mathf.Abs(rSpanCurr));
            speed = 1 / (Mathf.Abs(lSpanCurr) + Mathf.Abs(rSpanCurr));
            Physics.gravity = new Vector3(0, 9.8f * gravityScale * speed, 0);
            setGravityScale = gravityScale;
        }
    }

    //Adjusting speed by considering 
    void SetGravityWithHandPoseAndControllers()
    {
        speed = 1 / (isLHandExtended + isRHandExtended + speedConstant);
        Physics.gravity = new Vector3(0, 9.8f * gravityScale * speed, 0);
        setGravityScale = gravityScale;
    }

    //Tries to get hand span, not used anymore as better working solution has been found.
    void ObtainCurrentHandSpanLegacyMethod()
    {
        //Updates Hand Span details.
        lSpanCurr = Mathf.Abs(centerEyeHMDObject.transform.InverseTransformPoint(lHandPrefab.transform.position).x);
        rSpanCurr = Mathf.Abs(centerEyeHMDObject.transform.InverseTransformPoint(rHandPrefab.transform.position).x);
    }

    //Gets the current span value of left hand and right by calculating the distance between the handprefab and centre eye camera.
    void GetCurrentSpan()
    {
        Vector2 hmdIn2DWorld = new Vector2(centerEyeHMDObject.transform.position.x, centerEyeHMDObject.transform.position.z);
        Vector2 lHandIn2DWorld = new Vector2(lHandPrefab.transform.position.x, lHandPrefab.transform.position.z);
        Vector2 rHandIn2DWorld = new Vector2(rHandPrefab.transform.position.x, rHandPrefab.transform.position.z);

        lSpanCurr = Mathf.Abs(Vector2.Distance(hmdIn2DWorld, lHandIn2DWorld));
        rSpanCurr = Mathf.Abs(Vector2.Distance(hmdIn2DWorld, rHandIn2DWorld));
    }

    //Dynamically sets the min and max span values based on current span values.
    void UpdateSpanMinMax()
    {
        lSpanMax = lSpanCurr > lSpanMax ? lSpanCurr : lSpanMax;
        rSpanMax = rSpanCurr > rSpanMax ? rSpanCurr : rSpanMax;

        lSpanMin = lSpanCurr < lSpanMin ? lSpanCurr : lSpanMin;
        rSpanMin = rSpanCurr < rSpanMin ? rSpanCurr : rSpanMin;
    }

    //Based on the current span values player is moved accordingly.
    void MovePlayerUsingSpan()
    {
        if (rSpanCurr != 0)
        {
            ratioL2R = Mathf.Abs(lSpanCurr) / Mathf.Abs(rSpanCurr);

            if (ratioL2R > 1)
            {
                moveComponent = (1 / ratioL2R) * -1;
            }

            else
            {
                moveComponent = ratioL2R;
            }

            controller.Move(2.0f * headset.transform.right * moveComponent * Time.deltaTime * characterSpeed);
        }
    }

    //Moves the player with hand pose input.
    void MovePlayerUsingHandPose()
    {
        if (isRHandFist)
        {            
            controller.Move(-2.0f * headset.transform.right * Time.deltaTime * characterSpeed);
        }
        if (isLHandFist)
        {
            controller.Move(2.0f * headset.transform.right * Time.deltaTime * characterSpeed);
        }
    }

    //Moves player using controllers.
    void MovePlayerUsingControllers()
    {
        leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool leftPrimaryButtonValue);
        rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool rightPrimaryButtonValue);

        if (rightPrimaryButtonValue)
        {
            controller.Move(-2.0f * headset.transform.right * Time.deltaTime * characterSpeed);
            isRHandExtended = 0.3f;
        }
        else isRHandExtended = 0.0f;

        if (leftPrimaryButtonValue)
        {
            controller.Move(2.0f * headset.transform.right * Time.deltaTime * characterSpeed);
            isLHandExtended = 0.3f;
        }
        else isLHandExtended = 0.0f;
    }

    //For testing purpose only. Moves player using keyboard input.
    void MovePlayerUsingKeyboard()
    {
        if (Input.GetKey(KeyCode.D))
        {
            //rSpanCurr = 10;
            //lSpanCurr = 1;

            controller.Move(-2.0f * headset.transform.right * Time.deltaTime * characterSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            //lSpanCurr = 10;
            //rSpanCurr = 1;

            controller.Move(headset.transform.right * Time.deltaTime * characterSpeed);
        }
    }

    //Checks if the player is dead; if dead, does the necessary.
    void IsPlayerDead()
    {
        if (playerLife <= 0.0f)
        {
            //SceneManager.LoadScene("Game");
            int finalScoreDisplay = (int)Mathf.Round(gameTime);
            scoreTextObject.GetComponent<Text>().text = "Score: " + finalScoreDisplay.ToString();
            isStarting = false;
            isPlayActive = false;
        }

        if (playerLife > 10.0f) playerLife = 10.0f;
    }

    //Sets the fog color and density based on remaining life.
    void SetFog()
    {
        if (isPlayActive)
        {
            lifeStatus = playerLife / 10.0f;
            lifeStatusColor = Color.Lerp(red, blue, lifeStatus);
            RenderSettings.fogColor = lifeStatusColor;
            RenderSettings.fogDensity = (1.0f - lifeStatus) / 10.0f;
        }
        else RenderSettings.fogDensity = 0.0f;
    }

    //Activates and manages shield.
    void Shield()
    {
        if(!shield.activeInHierarchy) shield.SetActive(true);

        Vector3 tempShieldPosition = shield.transform.position;
        tempShieldPosition.x = player.transform.position.x;
        shield.transform.position = tempShieldPosition;

        shieldTimer++;
        if(shieldTimer > 100.0f)
        {
            shieldTimer = 0.0f;
            shieldStatus = false;
            shield.SetActive(false);
        }
    }

    //Called when Fist/Rock pose is detected on left hand.
    public void LHandFistOn()
    {
        if (!isLHandFist) isLHandFist = true;
        isLHandExtended = 0.3f;
    }

    //Called when Fist/Rock pose is deactivated on left hand.
    public void LHandFistOff()
    {
        if (isLHandFist) isLHandFist = false;
        isLHandExtended = 0.0f;
    }

    //Called when Fist/Rock pose is detected on right hand.
    public void RHandFistOn()
    {
        if (!isRHandFist) isRHandFist = true;
        isRHandExtended = 0.3f;
    }

    //Called when Fist/Rock pose is deactivated on right hand.
    public void RHandFistOff()
    {
        if (isRHandFist) isRHandFist = false;
        isRHandExtended = 0.0f;
    }

    //Called when thumbs up pose is detected on right hand.
    public void RHandThumbsUpOn()
    {
        if (!isRHandThumbsUp) isRHandThumbsUp = true;
    }

    //Called when thumbs up pose is deactivated on right hand.
    public void RHandThumbsUpOff()
    {
        if (isRHandThumbsUp) isRHandThumbsUp = false;
    }

    //Called when thumbs up pose is detected on left hand.
    public void LHandThumbsUpOn()
    {
        if (!isLHandThumbsUp) isLHandThumbsUp = true;
    }

    //Called when thumbs up pose is deactivated on left hand.
    public void LHandThumbsUpOff()
    {
        if (isLHandThumbsUp) isLHandThumbsUp = false;
    }

    //Calculates time of play for score, and player life is decremented as a function of time.
    void UpdateStats()
    {
        gameTime += 1.0f * Time.deltaTime;
        //playerLife -= 0.5f * Time.deltaTime;
        //playerLife -= Time.deltaTime * (Mathf.Pow(1.1f, (gameTime * 0.00001f)) + 0.5f);
        playerLife -= (0.5f + (gameTime * playerLifeTimeFactor)) * Time.deltaTime;
    }

    //Displays the life & time near either hands of the player.
    void DisplayStats()
    {
        lifeDisplay = (int)Mathf.Round(playerLife);
        lifeText.text = "Life: " + lifeDisplay.ToString();

        gameTimeDisplaySeconds = (int)Mathf.Round(gameTime);
        gameTimeDisplayMinutes = gameTimeDisplaySeconds / 60;
        gameTimeDisplaySeconds = gameTimeDisplaySeconds % 60;
        if (gameTimeDisplayMinutes > 0) scoreText.text = gameTimeDisplayMinutes.ToString() + "Minutes   " + gameTimeDisplaySeconds.ToString() + "Seconds";
        else scoreText.text = "   " + gameTimeDisplaySeconds.ToString() + "Seconds";
    }

    //Makes sure the player is locked in the play zone.
    void ConstraintOnPlayerTransform()
    {
        //Resets the player y-rotation for the first few seconds.
        if (rotationResetTimer < 3.0f)
        {
            rotationResetTimer += 1.0f * Time.deltaTime;

            if (playerController.transform.rotation.eulerAngles.y != 0)
            {
                resetRotation = playerController.transform.rotation.eulerAngles;
                playerController.transform.rotation = Quaternion.Euler(resetRotation.x, 0, resetRotation.z);
            }
        }

        /*Keeps the player's z value as 0 to make sure the player doesn't move along the Z axis as the player 
          is not provided any controls to move along that axis.*/
        if (player.transform.position.z != 0)
        {
            resetPosition = player.transform.position;
            resetPosition.z = 0.0f;
            player.transform.position = resetPosition;
        }
    }     

    //Called when the Hand Span mode is chosen by the player.
    void PlayWithHandSpan()
    {
        inputModeSelected = 0;
        playerLifeTimeFactor = 0.0005f;
        isPlayActive = true;
        spawners.SetActive(true);
    }

    //Called when the Hand Pose or Controller mode is chosen by the player.
    void PlayWithHandPoseOrControllers()
    {
        inputModeSelected = 1;
        playerLifeTimeFactor = 0.002f;
        isPlayActive = true;
        spawners.SetActive(true);
    }

    //Manages the mode switching and calls the locomotion method depending on the mode selected.
    void ManageMovementAndPlayModes()
    {
        if(isPlayActive)
        {
            if (controlSelectionTextObject.activeInHierarchy) controlSelectionTextObject.SetActive(false);
            if (scoreTextObject.activeInHierarchy) scoreTextObject.SetActive(false);
            if (InfoSlab.activeInHierarchy) InfoSlab.SetActive(false);

            switch (inputModeSelected)
            {
                case 0: MovePlayerUsingSpan();
                    break;

                case 1: MovePlayerUsingHandPose();
                        MovePlayerUsingControllers();
                    break;

                default: isPlayActive = false;
                         spawners.SetActive(false);
                         inputModeSelected = -1;
                    break;
            }
        }
        else
        {
            if (spawners.activeInHierarchy) spawners.SetActive(false);
            if (!InfoSlab.activeInHierarchy) InfoSlab.SetActive(true);

            if(isStarting)
            {

                if (scoreTextObject.activeInHierarchy) scoreTextObject.SetActive(false);
                if (!controlSelectionTextObject.activeInHierarchy) controlSelectionTextObject.SetActive(true);

                //Setting the mode using hand pose.
                if (isRHandThumbsUp || isLHandThumbsUp) PlayWithHandSpan();
                if (isRHandFist || isLHandFist) PlayWithHandPoseOrControllers();

                //Setting the mode using controllers.
                rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool rightPrimaryButtonValue);
                rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool rightSecondaryButtonValue);
                leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool leftPrimaryButtonValue);
                leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool leftSecondaryButtonValue);

                if (rightPrimaryButtonValue || leftPrimaryButtonValue) PlayWithHandSpan();
                if (rightSecondaryButtonValue || leftSecondaryButtonValue) PlayWithHandPoseOrControllers();
            }
            else
            {
                if (controlSelectionTextObject.activeInHierarchy) controlSelectionTextObject.SetActive(false);
                if (!scoreTextObject.activeInHierarchy) scoreTextObject.SetActive(true);

                scoreCardTimer += 1.0f * Time.deltaTime;

                if(scoreCardTimer > 5.0f)
                {
                    //isStarting = true;
                    SceneManager.LoadScene("Game");
                }
            }
        }
    }

}
