using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public int type;
    public bool isNegative;

    Recycler recycler;

    public int pushAttemptLimit = 10;
    public bool inBin = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(inBin && gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }

        if (!inBin && !gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        bool pushStatus = false;

        //If object collides with recycler, process to bin it is initiated.
        if(other.tag == "Recycler" && !inBin)
        {
            recycler = other.gameObject.GetComponent<Recycler>();

            int attemptCount = 0;
            inBin = true;
            while(!pushStatus)
            {
                pushStatus = recycler.pushToBin(gameObject, type);
                attemptCount++;
                if(attemptCount > pushAttemptLimit)
                {
                    break;
                }
            }

            if (!pushStatus) Destroy(gameObject);
        }
    }
}
