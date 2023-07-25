using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UpdateCoinsDisplay : MonoBehaviour
{
    MarioMovement marioMovement;
    [SerializeField] GameObject player;
    public Text coinsCount;
    
    void Start() {
        marioMovement = player.GetComponent<MarioMovement>();
        
        //coinSound = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        coinsCount.text = "Coins: " + marioMovement.coins.ToString();
    }
}
