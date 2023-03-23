using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayer : MonoBehaviour
{
    public GameObject player;
    Vector3 tempPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tempPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.position = tempPosition;
    }
}
