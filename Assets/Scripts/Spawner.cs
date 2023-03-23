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
    float spawnOffsetX;
    float spawnOffsetZ;
    Vector3 spawnPosition;

    public float spawnRangeLimit = 4.0f;

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
            spawnOffsetX = Random.Range(-spawnRangeLimit, spawnRangeLimit);
            spawnOffsetZ = Random.Range(-spawnRangeLimit, spawnRangeLimit);
            spawnPosition = gameObject.transform.position + (Vector3.right * spawnOffsetX) + (Vector3.forward * spawnOffsetZ);
            //spawnType = (int) Random.Range(0, 6);
            spawnType = 0;

            tempObj = recycler.getFromBin(spawnType);

            //If getFromBin fetched relevant object from corresponding bin.
            if (tempObj != null)
            {
                
                tempObj.transform.rotation = Quaternion.identity;
                tempObj.transform.position = spawnPosition;
                tempObj.GetComponent<ObjectManager>().inBin = false;
                tempObj.SetActive(true);
                spawnTimer = 0.0f;
                spawnFlag = false;

                Debug.Log("Object spawned from recycle. Object type:" + spawnType);
            }
            else
            {
                Instantiate(spawnPrefab[spawnType], spawnPosition, gameObject.transform.rotation);
                spawnTimer = 0.0f;
                spawnFlag = false;
            }
            
        }

        else
        {            
            spawnTimer += 0.1f;
            if(spawnTimer >= 10.0f)
            {
                spawnFlag = true;
            }
        }
    }
}
