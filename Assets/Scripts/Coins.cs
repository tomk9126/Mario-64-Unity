using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Coins : MonoBehaviour
{
    public bool isRed;
    //public AudioSource coinSound;
    bool collected = false;

    MarioMovement marioMovement;
    [SerializeField] GameObject player;
    
    void Start() {
        marioMovement = player.GetComponent<MarioMovement>();
        //coinSound = GetComponent<AudioSource>();
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Player") //used for crash prevention
        {
            if (!collected) //this prevents coins from being collected more than once before they're destroyed.
            {   
                collected = true;
                Debug.Log("Coin");
                marioMovement.coins = marioMovement.coins + 1;
                if (isRed)  { marioMovement.coins = marioMovement.coins + 1; } //extra coin counted if red
                //coinSound.Play();
                Destroy(this.gameObject);
                
            }
            
            
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newRotation = new Vector3(0, 1, 0);
        transform.eulerAngles += newRotation;
    }
}