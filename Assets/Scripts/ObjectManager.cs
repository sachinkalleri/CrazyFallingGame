using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public int type;
    public bool isNegative;
    public bool isShieldBooster;

    //Color blue = new Color (0.15f,0.6f, 0.8f);
    //Color red;
    //Color wallColor;

    Recycler recycler;
    GameObject player;
    GameManager gMan;

    public bool isAttached = false;
    public bool wasAttached = false;
    public int pushAttemptLimit = 10;
    public bool inBin = false;

    float wallColorPicker;
    float attachedTimer = 0.0f;

    public GameObject shieldBoosterIndicator;

    // Start is called before the first frame update
    void Start()
    {
        gMan = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        recycler = GameObject.FindWithTag("Recycler").GetComponent<Recycler>();
        if (type == 0) InitiateCollectibleBlue();
    }

    // Update is called once per frame
    void Update()
    {
        if(gMan == null)
        {
            gMan = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        }

        if(recycler == null)
        {
            recycler = GameObject.FindWithTag("Recycler").GetComponent<Recycler>();
        }

        ActiveStatusMonitor();

        if(type == 2) Creature();

        if(type == 3)
        {
            Wall();
        }

        //Unless the object is wall component, the z axis position is matched to that of the player.
        //if (type != 3) MoveTowardsPlayerZ();
    }

    private void OnTriggerEnter(Collider other)
    {
        //If object collides with recycler, process to bin it is initiated.        
        if(other.tag == "Recycler" && !inBin)
        {
            BinIt();
        }
        else
        {
            if (other.tag == "Player")
            {
                player = other.gameObject;

                if (type == 0)
                {
                    gMan.playerLife += 2.0f;

                    if(isShieldBooster)
                    {
                        Debug.Log("Shield Booster Collected");
                        gMan.shieldTimer = 0.0f;
                        gMan.shieldStatus = true;
                    }
                    BinIt();
                }
                if (type == 1)
                {
                    gMan.playerLife -= 1.0f;
                    BinIt();
                }
                if (type == 2 && !wasAttached && !gMan.shieldStatus)
                {
                    AttachCreature();
                }
            }

            if(other.tag == "Shield")
            {
                if (isNegative) BinIt();
            }
        }
    }

    void ActiveStatusMonitor()
    {
        if (inBin && gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }

        if (!inBin && !gameObject.activeInHierarchy)
        {
            gameObject.SetActive(true);
        }
    }

    void Creature()
    {
        //Executes when the creature is attached to the player.
        if (isAttached == true && !wasAttached)
        {
            gameObject.transform.position = player.transform.position - (Vector3.up * 0.2f);
            gMan.playerLife -= 0.01f;

            attachedTimer += 0.1f;

            if (attachedTimer > 10.0f || gMan.shieldStatus)
            {
                wasAttached = true;
                gameObject.GetComponent<Rigidbody>().useGravity = true;
                isAttached = false;
                attachedTimer = 0.0f;
            }
        }
    }

    void AttachCreature()
    {
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        gameObject.transform.position = player.transform.position - (Vector3.up * 0.2f);
        isAttached = true;
    }

    void Wall()
    {
        //wallColorPicker = gMan.playerLife / 10.0f;
        //wallColor = Color.Lerp(Color.red, blue, wallColorPicker);
        gameObject.GetComponent<Renderer>().material.SetColor("_Color", gMan.lifeStatusColor);
        gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", gMan.lifeStatusColor);
    }

    void BinIt()
    {
        if (type == 0) shieldBoosterIndicator.SetActive(false);
        bool pushStatus = false;
        int attemptCount = 0;
        isShieldBooster = false;
        isAttached = false;
        wasAttached = false;
        attachedTimer = 0.0f;

        while (!pushStatus)
        {
            pushStatus = recycler.pushToBin(gameObject, type);
            attemptCount++;
            if (attemptCount > pushAttemptLimit)
            {
                break;
            }
        }

        if (!pushStatus) Destroy(gameObject);
        else inBin = true;
    }

    public void InitiateCollectibleBlue()
    {
        if(type == 0)
        {
            int randomizer = (int)Random.Range(0, 10);
            if (randomizer < 2)
            {
                isShieldBooster = true;
                shieldBoosterIndicator.SetActive(true);
            }
            else
            {
                isShieldBooster = false;
                shieldBoosterIndicator.SetActive(false);
            }
        }
    }

    //Unless the object is wall component, the z axis position is matched to that of the player.
    void MoveTowardsPlayerZ()
    {
        Vector3 tempPosition = gameObject.transform.position;
        tempPosition.z = gMan.player.transform.position.z;
        gameObject.transform.position = tempPosition;
    }
}
