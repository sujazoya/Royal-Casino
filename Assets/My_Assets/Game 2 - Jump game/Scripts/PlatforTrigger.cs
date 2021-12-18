using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatforTrigger : MonoBehaviour
{
    BallController ballController;
    // Start is called before the first frame update
    void Start()
    {
        ballController = FindObjectOfType<BallController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            ballController.timeToCreate = true;
            //ballController.CreteObject();
        }
    }
    
}
