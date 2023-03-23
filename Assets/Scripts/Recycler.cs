using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recycler : MonoBehaviour
{
    public const int objectTypeCount = 7;
    public const int binLength = 10;
    public int whileLimit = 3000;
    int[] lastBinObjectPointer = new int[objectTypeCount] { -1, -1, -1, -1, -1, -1, -1 }; // -1 indicates the corresponding bin is empty.

    //Array of bins with different object types in different rows.
    /* 0 - Collectible Blue (positive)
     * 1 - Collectible Red (negative)
     * 2 - Leech (Monster/Creature)
     * 3 - Portal Blue (positive)
     * 4 - Portal Red (negative)
     * 5 - Blaster (Tool/Weapon)
     * 6 - Shield (Tool/Weapon)
     */
    GameObject[,] bin = new GameObject[objectTypeCount, binLength];

    //Flags for each bin to indicate their status.
    bool[] getOngoing = new bool[7] { false, false, false, false, false, false, false };
    bool[] pushOngoing = new bool[7] { false, false, false, false, false, false, false };

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool pushToBin(GameObject obj, int type)
    {
        int binStatusCheckTimer = 0;

        while (pushOngoing[type] || getOngoing[type])
        {
            binStatusCheckTimer++;
            if (binStatusCheckTimer > whileLimit)
            {
                Debug.Log("While breaker initiated. pushToBin failed for object type:" +type);
                return false;
            }
        }

        pushOngoing[type] = true;

        lastBinObjectPointer[type]++;
        bin[type, lastBinObjectPointer[type]] = obj;

        pushOngoing[type] = false;

        Debug.Log("Push Succesful. Object type pushed:" + type);
        return true;
    }

    public GameObject getFromBin(int type)
    {
        int binStatusCheckTimer = 0;
        GameObject getObj;

        //While loops waits for the bin to be free or exits when the timer (counter) runs out of limit.
        while (pushOngoing[type] || getOngoing[type])
        {
            binStatusCheckTimer++;
            if (binStatusCheckTimer > whileLimit)
            {
                Debug.Log("While breaker initiated. getFromBin failed for object type:" + type);
                return null;
            }
        }

        if (lastBinObjectPointer[type] != -1)
        {
            getOngoing[type] = true;

            getObj = bin[type, lastBinObjectPointer[type]];
            bin[type, lastBinObjectPointer[type]] = null;
            lastBinObjectPointer[type]--;

            getOngoing[type] = false;

            Debug.Log("Successfully fetched from bin. Object type:" + type);
            return getObj;            
        }
        else
        {
            Debug.Log("Corresponding bin is found empty. getFromBin failed for object type:" + type);
            return null;
        }
    }
}
