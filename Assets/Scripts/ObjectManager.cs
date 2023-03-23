using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public int type;
    public bool isNegative;

    Recycler recycler;

    public bool isAttached = false;
    public bool wasAttached = false;
    public int pushAttemptLimit = 10;
    public bool inBin = false;
    float attachedTimer = 0.0f;

    //public GameManager gMan;


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

        //Executes when the creature is attached to the player.
        if(type == 2 && isAttached == true && !wasAttached)
        {
            attachedTimer += 0.1f;
            if(attachedTimer > 10.0f)
            {
                wasAttached = true;
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                isAttached = false;                
                attachedTimer = 0.0f;
            }
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
            //inBin = true;
            isAttached = false;
            wasAttached = false;
            attachedTimer = 0.0f;

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
            else inBin = true;
        }
        else
        {
            if(other.tag == "Player" && !wasAttached)
            {
                if (type == 0) { }
                if (type == 1) { }
                if (type == 2 && !wasAttached)
                {
                    gameObject.GetComponent<Rigidbody>().useGravity = false;
                    gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    gameObject.transform.position = other.gameObject.transform.position - (Vector3.up * 0.2f);
                    isAttached = true;
                }
            }
        }
    }
}
