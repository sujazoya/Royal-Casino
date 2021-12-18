using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using suja;

public class BallController : MonoBehaviour
{
    


    public enum GameMode
    {
        Menu,Playing,Paused,Gameover
    }
    public enum Ball
    {
        OnTheWay,Stoped,Destroied,Falling
    }
    public Ball ballStatus;
    [SerializeField] Slider slider;
    [SerializeField] List<Animator> platformAnimator;
    string pushAnim = "push";
    float maxTime = 4;
    [SerializeField] float pushTime;
    Rigidbody ballRigid;
    [SerializeField] GameObject ball;
    public ForceMode forceMode=ForceMode.Force;
    float maxHitForce = 2500;
    float hitForce;
    [SerializeField] Text powerText;
    [SerializeField] Text distanceText;
    [SerializeField] Text distanceText1;
    [SerializeField] Text totallDistanceText;
    public GameObject[] items;
    float platformDistance;
    [SerializeField] GameObject currentPlatform;
    float storedYPose;
    [SerializeField] Transform Base;
    public bool timeToCreate=true;
    [HideInInspector] public bool clicking;
    [SerializeField] GameObject menu;
    [SerializeField] GameObject hud;
    [SerializeField] GameObject gameoverPanel;
    [SerializeField] Text GO_totallDistanceText;
    [SerializeField] Text GO_bestDistanceText;
    [SerializeField] Text GO_statusText;   
    public GameMode gameMode;
    public AudioSource music;
    public bool playMusic;
    float yPose;
    int index;
    public static BallController instance;
    [SerializeField] GameObject transition;
    [SerializeField] Text coinText;
    [SerializeField] Text diemondText;
    private void Awake()
    {
        instance = this;
    }
    public static float BestDistance
    {
        get { return PlayerPrefs.GetFloat("BestDistance", 0.0f); }
        set { PlayerPrefs.SetFloat("BestDistance", value); }
    }
    public static int Diemonds
    {
        get { return PlayerPrefs.GetInt("Diemonds", 0); }
        set { PlayerPrefs.SetInt("Diemonds", value); }
    }
    public static int Coins
    {
        get { return PlayerPrefs.GetInt("Coins", 0); }
        set { PlayerPrefs.SetInt("Coins", value); }
    }

    // Start is called before the first frame update
    void Start()
    {   
        OnStart();
        //PlayerPrefs.DeleteAll();
    }
    void OnStart()
    {
        gameMode = GameMode.Menu;
        slider.minValue = 0;
        slider.maxValue = maxTime;
        ballRigid = ball.transform.GetComponent<Rigidbody>();       
        gameoverPanel.SetActive(false);
        hud.SetActive(false);
        menu.SetActive(true);
    }
    IEnumerator PlayTransition()
    {
        transition.SetActive(true);
        yield return new WaitForSeconds(2f);
        transition.SetActive(false);

    }
    public void Play()
    {
        StartCoroutine(StartPlaying());
    }
    IEnumerator StartPlaying()
    {
        StartCoroutine(PlayTransition());
        yield return new WaitForSeconds(1f);
        gameoverPanel.SetActive(false);
        hud.SetActive(true);
        menu.SetActive(false);
        yield return new WaitForSeconds(2f);
        gameMode = GameMode.Playing;
        ShowNextDestination();
        CreteObject();
    }
    public void CreteObject()
    {
        if (!timeToCreate)
            return;       
        GameObject platform = Instantiate(items[index]);
        platform.transform.position = new Vector3(0, yPose, 0);
        currentPlatform = platform;        
        storedYPose = Vector3.Distance(Base.position, currentPlatform.transform.position);       
        timeToCreate = false;
    }
    public void ShowNextDestination()
    {
        platformDistance = Random.Range(10, 100);
        yPose = currentPlatform.transform.position.y + platformDistance;       
        //if (storedYPose > 0) { yPose -= storedYPose; }
        index = Random.Range(0, items.Length);
        distanceText.text ="Next Destination" + platformDistance.ToString();
        StartCoroutine(ShowDestination());
        timeToCreate = true;
    }
    
    IEnumerator ShowDestination()
    {
        distanceText.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.15f);
        distanceText.gameObject.SetActive(true);
        //distanceText.text = gap.ToString();
        yield return new WaitForSeconds(4.2f);
        distanceText.gameObject.SetActive(false);

    }
    GameObject[] animatorObjects;
    // Update is called once per frame
    void Update()
    {
        if (CrossPlatformInputManager.GetButton("Push"))
        {            
            if (pushTime < maxTime)
            {
                pushTime += Time.deltaTime;
            }
            clicking = true;
        }
        else
             if (pushTime > 0)
        {
            pushTime -= Time.deltaTime;
        }
        slider.value = pushTime;
        hitForce = pushTime * 650;
        powerText.text = hitForce.ToString("F1");
       
        if (CrossPlatformInputManager.GetButtonDown("Push"))
        {
            platformAnimator.Clear();           
            animatorObjects=GameObject.FindGameObjectsWithTag("Platform");           
            for (int i = 0; i < animatorObjects.Length; i++)
            {
                platformAnimator.Add(animatorObjects[i].transform.GetComponent<Animator>());               
                platformAnimator[i].SetTrigger(pushAnim);
            }
            //platformAnimator.SetTrigger(pushAnim);           
            
        }
        if (CrossPlatformInputManager.GetButtonUp("Push"))
        {           
            platformAnimator.Clear();
            animatorObjects = GameObject.FindGameObjectsWithTag("Platform");
            for (int i = 0; i < animatorObjects.Length; i++)
            {
                platformAnimator.Add(animatorObjects[i].transform.GetComponent<Animator>());
                platformAnimator[i].SetTrigger("throw");
            }           
            //platformAnimator.SetTrigger("throw");
            StartCoroutine(PushTheBall());
            if (timeToCreate)
            {
                CreteObject();
            }
        }
        distanceText1.text= platformDistance.ToString();
        totallDistanceText.text = ball.transform.GetComponent<Ball_Object>().totalDistance.ToString();
        if(gameMode==GameMode.Playing)
        {
            if(playMusic && !music.isPlaying)
            {
                music.Play();
            }
        }
        else
        {
            if (music.isPlaying)
            {
                music.Stop();
            }
        }
    }
    public void OnGameover()
    {
        gameMode = GameMode.Gameover;
        gameoverPanel.SetActive(true);
        GO_totallDistanceText.text = ball.transform.GetComponent<Ball_Object>().totalDistance.ToString("F1");
        if(BestDistance < ball.transform.GetComponent<Ball_Object>().totalDistance)
        {
            BestDistance = ball.transform.GetComponent<Ball_Object>().totalDistance;
            GO_statusText.text = "NEW SCORE";
            GO_statusText.color = Color.green;
            suja.SoundManager.PlaySfx("HighScore");
        }
        else if (BestDistance > ball.transform.GetComponent<Ball_Object>().totalDistance)
        {
            GO_statusText.color = Color.red;
            GO_statusText.text = "GAME OVER";
            suja.SoundManager.PlaySfx("fail");
        }
        GO_bestDistanceText.text = BestDistance.ToString("F1");
        ball.SetActive(false);
    }
    IEnumerator PushTheBall()
    {
        yield return new WaitForSeconds(0.15f);
        ballRigid.AddForce(Vector3.up * hitForce, forceMode);
        clicking = false;
    } 
    public void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }
}
