using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathJumpItem : MonoBehaviour
{
    [SerializeField] GameObject effect;
    [SerializeField] GameObject sprite;
    [SerializeField] GameObject shine;
    BallController ballController;
    public enum MathJumpItemType
    {
        Diemon,Coin
    }
    [SerializeField] MathJumpItemType itemType;
    // Start is called before the first frame update
    void Start()
    {
        effect.SetActive(false);
        sprite.SetActive(true);
        shine.SetActive(true);
        ballController = FindObjectOfType<BallController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            effect.SetActive(true);
            sprite.SetActive(false);
            shine.SetActive(false);
            if(itemType== MathJumpItemType.Coin)
            {
                BallController.Coins += 10;
            }
            else if (itemType == MathJumpItemType.Diemon)
            {
                BallController.Diemonds += 10;
            }

        }
    }
}
