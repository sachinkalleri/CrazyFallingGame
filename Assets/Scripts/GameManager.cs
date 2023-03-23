using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public float gravityScale = 1.0f;
    float setGravityScale;
    float lSpanMax = StaticData.lStretched;
    float rSpanMax = StaticData.rStretched;
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

    Vector3 moveWithHands = Vector3.right;

    // Start is called before the first frame update
    void Start()
    {
        Physics.gravity = new Vector3(0, 9.8f * gravityScale, 0);
        setGravityScale = gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        //Sets and updates gravity scale.
        //if(setGravityScale != gravityScale)
        //{
        //    Physics.gravity = new Vector3(0, 9.8f * gravityScale, 0);
        //    setGravityScale = gravityScale;
        //}

        //Updates Hand Span details.
        lSpanCurr = Mathf.Abs(centerEyeHMDObject.transform.InverseTransformPoint(lHandPrefab.transform.position).x);
        rSpanCurr = Mathf.Abs(centerEyeHMDObject.transform.InverseTransformPoint(rHandPrefab.transform.position).x);

        Debug.Log("lSpanCurr:" + lSpanCurr);
        Debug.Log("rSpanCurr:" + rSpanCurr);

        ratioL2R = lSpanCurr / rSpanCurr;

        moveWithHands.x = 1 - ratioL2R;

        playerController.transform.position += moveWithHands;

        speed = (lSpanCurr + rSpanCurr) / (lSpanMax + rSpanMax);

        Physics.gravity = new Vector3(0, 9.8f * gravityScale * speed, 0);
        setGravityScale = gravityScale;
    }
}
