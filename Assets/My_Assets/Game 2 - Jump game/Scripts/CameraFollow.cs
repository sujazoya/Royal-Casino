using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {

    public Transform playerTrans;

    // Update is called once per frame
    void FixedUpdate()
    {
        float playerHeight = playerTrans.position.y;       

        //Update camera position if the player has climbed and if the player is too low: Set gameover.
        float currentCameraHeight = transform.position.y;
        float newHeight = Mathf.Lerp(currentCameraHeight, playerHeight, Time.deltaTime * 10);

        //if (playerTrans.position.y > currentCameraHeight)
        //{
            transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
        //}

        //else
        //{
        //    //Player is lower..maybe below the cameras view?
        //    if (playerHeight < (currentCameraHeight - 10))
        //    {
        //        //GameOver();
        //    }
        //}

    }

}
