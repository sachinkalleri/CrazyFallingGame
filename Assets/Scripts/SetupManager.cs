using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetupManager : MonoBehaviour
{
    public OVRHand lHand;
    public OVRHand rHand;

    public GameObject lHandPrefab;
    public GameObject rHandPrefab;
    public GameObject centerEyeHMDObject;
    public GameObject setupCube1;
    public GameObject setupCube2;

    public Material redMaterial;
    public Material blueMaterial;
    public Material greenMaterial;

    Vector3 tempHandPositionX;

    bool lStretchAcquired = false;
    bool rStretchAcquired = false;
    bool lClosedAcquired = false;
    bool rClosedAcquired = false;


    //public List<ActiveStateSelector> poses;
    // Start is called before the first frame update
    void Start()
    {
        //foreach (var item in poses)
        //{
        //    item.WhenSelected += () => setLimit(item.gameObject.name);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //Tracks setup cube 1 to left hand.
        tempHandPositionX = setupCube1.transform.position;
        tempHandPositionX.x = lHandPrefab.transform.position.x;
        setupCube1.transform.position = tempHandPositionX;

        //Tracks setup cube 2 to right hand.
        tempHandPositionX = setupCube2.transform.position;
        tempHandPositionX.x = rHandPrefab.transform.position.x;
        setupCube2.transform.position = tempHandPositionX;

       

        //if(lHand.IsTracked && rH)
        //{
        //    if(!(lStretchAcquired && rStretchAcquired))
        //    {
        //        if (!lStretchAcquired)
        //        {
        //            setupCube1.GetComponent<MeshRenderer>().material = blueMaterial;
        //        }

        //        else
        //        {
        //            setupCube1.GetComponent<MeshRenderer>().material = greenMaterial;
        //        }

        //        if (!rStretchAcquired)
        //        {
        //            setupCube2.GetComponent<MeshRenderer>().material = blueMaterial;
        //        }

        //        else
        //        {
        //            setupCube2.GetComponent<MeshRenderer>().material = greenMaterial;
        //        }
        //    }

        //    else
        //    {
        //        if (!lClosedAcquired)
        //        {
        //            setupCube1.GetComponent<MeshRenderer>().material = blueMaterial;
        //        }

        //        else
        //        {
        //            setupCube1.GetComponent<MeshRenderer>().material = greenMaterial;
        //        }

        //        if (!rClosedAcquired)
        //        {
        //            setupCube2.GetComponent<MeshRenderer>().material = blueMaterial;
        //        }

        //        else
        //        {
        //            setupCube2.GetComponent<MeshRenderer>().material = greenMaterial;
        //        }
        //    }
            
        //}

        if(lHand.IsTracked)
        {
            if(!lStretchAcquired || lStretchAcquired && rStretchAcquired && !lClosedAcquired)
            {
                setupCube1.GetComponent<MeshRenderer>().material = blueMaterial;
            }

            else
            {
                setupCube1.GetComponent<MeshRenderer>().material = greenMaterial;
            }
        }

        else
        {
            setupCube1.GetComponent<MeshRenderer>().material = redMaterial;
        }

        if (rHand.IsTracked)
        {
            if (!rStretchAcquired || lStretchAcquired && rStretchAcquired && !rClosedAcquired)
            {
                setupCube2.GetComponent<MeshRenderer>().material = blueMaterial;
            }

            else
            {
                setupCube2.GetComponent<MeshRenderer>().material = greenMaterial;
            }
        }

        else
        {
            setupCube2.GetComponent<MeshRenderer>().material = redMaterial;
        }

        if (lStretchAcquired && rStretchAcquired && lClosedAcquired && rClosedAcquired)
        {
            SceneManager.LoadScene("Game");
        }
    }

    //private void setLimit(string poseName)
    //{
    //    if(poseName == "ThumbsUpRight")
    //    {
    //        if(!rStretchAcquired)
    //        {

    //        }
    //    }
    //}

    public void SetLStretch()
    {
        if(!lStretchAcquired)
        {
            StaticData.lStretched = Mathf.Abs(centerEyeHMDObject.transform.InverseTransformPoint(lHandPrefab.transform.position).x);
            lStretchAcquired = true;
        }
    }

    public void SetRStretch()
    {
        if (!rStretchAcquired)
        {
            StaticData.rStretched = Mathf.Abs(centerEyeHMDObject.transform.InverseTransformPoint(rHandPrefab.transform.position).x);
            rStretchAcquired = true;
        }
    }

    public void SetLClosed()
    {
        if (!lClosedAcquired)
        {
            StaticData.lClosed = Mathf.Abs(centerEyeHMDObject.transform.InverseTransformPoint(lHandPrefab.transform.position).x);
            lClosedAcquired = true;
        }
    }

    public void SetRClosed()
    {
        if (!rClosedAcquired)
        {
            StaticData.rClosed = Mathf.Abs(centerEyeHMDObject.transform.InverseTransformPoint(rHandPrefab.transform.position).x);
            rClosedAcquired = true;
        }
    }
}
