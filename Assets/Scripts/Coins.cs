using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Coins : MonoBehaviour
{
    
    void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Player") //used for crash prevention
        {
            Debug.Log("Coin");
            Destroy(this.gameObject);
            
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newRotation = new Vector3(0, 1, 0);
        transform.eulerAngles += newRotation;
    }
}
