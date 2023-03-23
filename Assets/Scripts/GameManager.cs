using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

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

    Color blue = new Color(0.15f, 0.6f, 0.8f);
    Color red = new Color(0.86f, 0.16f, 0.23f);

    public GameObject lHandPrefab;
    public GameObject rHandPrefab;
    public GameObject centerEyeHMDObject;
    public GameObject playerController;
    public GameObject headset;
    public GameObject shield;

    CharacterController controller;

    Vector3 resetPosition = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = new Vector3(0, 9.8f * gravityScale, 0);
        setGravityScale = gravityScale;
        controller = playerController.GetComponent<CharacterController>();
        GetCurrentSpan();
        lSpanMin = lSpanCurr;
        rSpanMin = rSpanCurr;
    }

    // Update is called once per frame
    void Update()
    {
        gameTime += 1.0f;
        playerLife -= 0.01f;
        IsPlayerDead(); //Checks if the player is dead and does the necessary if player ran out of life.
        SetFog();
        if(shieldStatus) Shield();

        //Resets the player z-position and y-rotation.
        //if(playerController.transform.rotation.eulerAngles.y != 0)
        //{
        //    resetRotation = playerController.transform.rotation.eulerAngles;
        //    playerController.transform.rotation = Quaternion.Euler(resetRotation.x, 0, resetRotation.z);
        //}
        ////Sets and updates gravity scale.
        //if (setGravityScale != gravityScale)
        //{
        //    Physics.gravity = new Vector3(0, 9.8f * gravityScale, 0);
        //    setGravityScale = gravityScale;
        //}

        if (playerController.transform.position.z != 0)
        {
            resetPosition = playerController.transform.position;
            resetPosition.z = 0.0f;
            playerController.transform.position = resetPosition;
        }   

        //Updates Hand Span details.
        //lSpanCurr = Mathf.Abs(centerEyeHMDObject.transform.InverseTransformPoint(lHandPrefab.transform.position).x);
        //rSpanCurr = Mathf.Abs(centerEyeHMDObject.transform.InverseTransformPoint(rHandPrefab.transform.position).x);

        GetCurrentSpan();
        UpdateSpanMinMax();
        MovePlayerUsingSpan();

        //DELETELATER (For testing purposes)
        MovePlayerUsingKeyboard();         
     
        if((Mathf.Abs(lSpanCurr) + Mathf.Abs(rSpanCurr)) != 0)
        {
            //speed = (Mathf.Abs(lSpanMax) + Mathf.Abs(rSpanMax)) / (Mathf.Abs(lSpanCurr) + Mathf.Abs(rSpanCurr));
            speed = 1 / (Mathf.Abs(lSpanCurr) + Mathf.Abs(rSpanCurr));
            Physics.gravity = new Vector3(0, 9.8f * gravityScale * speed, 0);
            setGravityScale = gravityScale;
        }        
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

    //For testing purpose only.
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
            SceneManager.LoadScene("Game");
        }
    }

    void SetFog()
    {
        lifeStatus = playerLife / 10.0f;
        lifeStatusColor = Color.Lerp(red, blue, lifeStatus);
        RenderSettings.fogColor = lifeStatusColor;
        RenderSettings.fogDensity = (1.0f - lifeStatus) / 10.0f;
    }

    void Shield()
    {
        if(!shield.activeInHierarchy) shield.SetActive(true);

        Vector3 tempShieldPosition = shield.transform.position;
        tempShieldPosition.x = playerController.transform.position.x;
        shield.transform.position = tempShieldPosition;

        shieldTimer++;
        if(shieldTimer > 100.0f)
        {
            shieldTimer = 0.0f;
            shieldStatus = false;
            shield.SetActive(false);
        }
    }
}
