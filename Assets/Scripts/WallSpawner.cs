using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSpawner : MonoBehaviour
{
    public GameObject wallTile;
    public Recycler recycler;

    GameObject tempObj;

    GameObject lastObject = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(lastObject != null)
        {
            if(Mathf.Abs(gameObject.transform.position.y) - Mathf.Abs(lastObject.transform.position.y) > 5.0f)
            {
                tempObj = recycler.getFromBin(3);

                //If getFromBin fetched relevant object from corresponding bin.
                if (tempObj != null)
                {

                    tempObj.transform.rotation = gameObject.transform.rotation;
                    tempObj.transform.position = gameObject.transform.position;
                    tempObj.GetComponent<ObjectManager>().inBin = false;
                    tempObj.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 9.8f, 0.0f);
                    tempObj.SetActive(true);
                    
                    lastObject = tempObj;

                    Debug.Log("Object spawned from recycle. Object type: WALL TILES");
                }
                else
                {
                    lastObject = Instantiate(wallTile, gameObject.transform.position, gameObject.transform.rotation);
                    lastObject.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 9.8f, 0.0f);
                }
            }
        }
        else
        {
            lastObject = Instantiate(wallTile, gameObject.transform.position, gameObject.transform.rotation);
            lastObject.GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 9.8f, 0.0f);
        }
    }
}
