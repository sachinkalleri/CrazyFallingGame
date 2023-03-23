using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float gravityScale = 1.0f;
    public float characterSpeed = -1.0f;

    float playerLife = 10.0f;

    float moveComponent = -1.0f;
    float setGravityScale;
    //float lSpanMax = StaticData.lStretched;
    //float rSpanMax = StaticData.rStretched;
    float lSpanMax = 0.0f;
    float rSpanMax = 0.0f;
    float lSpanMin = StaticData.lClosed;
    float rSpanMin = StaticData.rClosed;
    float lSpanCurr;
    float rSpanCurr;
    float ratioL2R;
    float speed;

    public GameObject lHandPrefab;
    public GameObject rHandPrefab;
    public GameObject centerEyeHMDObject;
    public GameObject playerController;
    public GameObject headset;

    CharacterController controller;

    Vector3 moveWithHands = Vector3.right;
    Vector3 resetRotation = Vector3.zero;

    Vector3 resetPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = new Vector3(0, 9.8f * gravityScale, 0);
        setGravityScale = gravityScale;
        controller = playerController.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
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

        //if(playerController.transform.position.z != 0)
        //{
        //    resetPosition = playerController.transform.position;
        //    resetPosition.z = 0.0f;
        //    playerController.transform.position = resetPosition;
        //}

        //Updates Hand Span details.
        //lSpanCurr = Mathf.Abs(centerEyeHMDObject.transform.InverseTransformPoint(lHandPrefab.transform.position).x);
        //rSpanCurr = Mathf.Abs(centerEyeHMDObject.transform.InverseTransformPoint(rHandPrefab.transform.position).x);

        Vector2 hmdIn2DWorld = new Vector2(centerEyeHMDObject.transform.position.x, centerEyeHMDObject.transform.position.z);
        Vector2 lHandIn2DWorld = new Vector2(lHandPrefab.transform.position.x, lHandPrefab.transform.position.z);
        Vector2 rHandIn2DWorld = new Vector2(rHandPrefab.transform.position.x, rHandPrefab.transform.position.z);

        lSpanCurr = Mathf.Abs(Vector2.Distance(hmdIn2DWorld, lHandIn2DWorld));
        rSpanCurr = Mathf.Abs(Vector2.Distance(hmdIn2DWorld, rHandIn2DWorld));

        lSpanMax = lSpanCurr > lSpanMax ? lSpanCurr : lSpanMax;
        rSpanMax = rSpanCurr > rSpanMax ? rSpanCurr : rSpanMax;

        //DELETELATER

        //if (Input.GetKey(KeyCode.D))
        //{
        //    rSpanCurr = 10;
        //    lSpanCurr = 1;
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    lSpanCurr = 10;
        //    rSpanCurr = 1;
        //}

        //Debug.Log("lSpanCurr:" + lSpanCurr);
        //Debug.Log("rSpanCurr:" + rSpanCurr);

        //if (rSpanCurr == 0) rSpanCurr = 0.000000001f;
        if (rSpanCurr != 0)
        {
            ratioL2R = Mathf.Abs(lSpanCurr) / Mathf.Abs(rSpanCurr);

            if(ratioL2R > 1)
            {
                moveComponent = (1 / ratioL2R) * -1;
            }

            else
            {
                moveComponent = ratioL2R;
            }
            //moveWithHands.x = 1 - ratioL2R;
            
            controller.Move( headset.transform.right * moveComponent * Time.deltaTime * characterSpeed);
        }
        
        //if(playerController.transform.position.z < -4.0f)
        //{
        //    resetVector = playerController.transform.position;
        //    resetVector.z = -3.9f;
        //    playerController.transform.position = resetVector;
        //}

        //else
        //{
        //    if(playerController.transform.position.z > 4.0f)
        //    {
        //        resetVector = playerController.transform.position;
        //        resetVector.z = 3.9f;
        //        playerController.transform.position = resetVector;
        //    }

        //    else
        //    {
        //        playerController.transform.position += moveWithHands;
        //    }
        //}


        //playerController.transform.position += moveWithHands;
        if((Mathf.Abs(lSpanCurr) + Mathf.Abs(rSpanCurr)) != 0)
        {
            //speed = (Mathf.Abs(lSpanMax) + Mathf.Abs(rSpanMax)) / (Mathf.Abs(lSpanCurr) + Mathf.Abs(rSpanCurr));
            speed = 1 / (Mathf.Abs(lSpanCurr) + Mathf.Abs(rSpanCurr));
            Physics.gravity = new Vector3(0, 9.8f * gravityScale * speed, 0);
            setGravityScale = gravityScale;
        }        
    }
}
