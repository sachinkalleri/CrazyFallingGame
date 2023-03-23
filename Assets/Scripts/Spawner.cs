using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Recycler recycler;
    public GameObject[] spawnPrefab;

    GameObject tempObj;
    bool spawnFlag = true;
    int spawnType;
    float spawnTimer = 0.0f;
    float timerLimit = 10.0f;
    float spawnOffsetX;
    float spawnOffsetZ;
    Vector3 spawnPosition;

    Vector3 rotationAngles;
    Quaternion spawnRotation;

    public float xOffsetLimit = 9.0f;
    public float zOffsetLimit = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnFlag)
        {            
            //Random offset is added to X and Z values of the spawn position.
            spawnOffsetX = Random.Range(-xOffsetLimit, xOffsetLimit);
            spawnOffsetZ = Random.Range(-zOffsetLimit, zOffsetLimit);
            spawnPosition = gameObject.transform.position + (Vector3.right * spawnOffsetX) + (Vector3.forward * spawnOffsetZ);
            spawnType = (int) Random.Range(0, spawnPrefab.Length);

            if (spawnType == 2)
            {
                spawnRotation.eulerAngles = new Vector3(180, 0, 0);
            }
            else
            {
                spawnRotation = gameObject.transform.rotation;
            }

            tempObj = recycler.getFromBin(spawnType);            

            //If getFromBin fetched relevant object from corresponding bin.
            if (tempObj != null)
            {

                tempObj.transform.rotation = spawnRotation;
                tempObj.transform.position = spawnPosition;
                tempObj.GetComponent<ObjectManager>().inBin = false;
                tempObj.GetComponent<Rigidbody>().velocity = Vector3.zero;
                tempObj.SetActive(true);
                spawnTimer = 0.0f;
                timerLimit = Random.Range(5.0f, 10.0f);
                spawnFlag = false;

                Debug.Log("Object spawned from recycle. Object type:" + spawnType);
            }
            else
            {
                Instantiate(spawnPrefab[spawnType], spawnPosition, spawnRotation);               

                spawnTimer = 0.0f;
                timerLimit = Random.Range(5.0f, 10.0f);
                spawnFlag = false;
            }
            
        }

        else
        {            
            spawnTimer += 0.1f;
            if(spawnTimer >= timerLimit)
            {
                spawnFlag = true;
            }
        }
    }
}
