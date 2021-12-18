using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlatformActivator : MonoBehaviour
{
    [SerializeField] float waitBeforeDestroy = 5;
    [SerializeField] GameObject effect;
    [SerializeField] GameObject platform;
    [SerializeField] GameObject glow;
    BallController ballController;
    [SerializeField] TextMeshPro timerText;

    // Start is called before the first frame update
    private void Awake()
    {
         glow.SetActive(true);
        effect.SetActive(false);       
        platform.SetActive(false);
    }
    private void Start()
    {
        ballController = FindObjectOfType<BallController>();
        timerText.gameObject.SetActive(false);


    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if(ballController.ballStatus == BallController.Ball.OnTheWay)
            {
                StartCoroutine(ActAction());
            }
            transform.GetComponent<BoxCollider>().enabled = false;        
        }
        
    }
    IEnumerator ActAction()
    {       
        effect.SetActive(true);
        glow.SetActive(false);
        yield return new WaitForSeconds(1); 
        platform.SetActive(true);
        yield return new WaitUntil(() => ballController.ballStatus == BallController.Ball.Stoped);
        StartCoroutine(ShowTimer());
        ballController.ShowNextDestination();
        Destroy(gameObject, waitBeforeDestroy);
        //yield return new WaitForSeconds(2);       
        effect.SetActive(false);
        //yield return new WaitForSeconds(3);
        //platform.SetActive(false);       
    }
    IEnumerator ShowTimer()
    {
        timerText.gameObject.SetActive(true);
        timerText.text = waitBeforeDestroy.ToString();
        yield return new WaitForSeconds(0.5f);      
        int t = (int)waitBeforeDestroy;
        while (t > 0)
        {
            t--;
            timerText.text = t.ToString();
            yield return new WaitForSeconds(1);
        }       

    }
}
