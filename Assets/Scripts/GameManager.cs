using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{

    [SerializeField] private GameObject Player;
    [SerializeField] private GameObject WinText;
    [SerializeField] private GameObject[] CheckPoints;
    [SerializeField] private Material[] CheckPointMaterials;

    [Range(0,100)]
    [SerializeField] private float distanceValue;
    [SerializeField] private Transform endPos;
    [SerializeField] private Transform WinTransform;

    public Vector3 spawnPoint { get; private set; }
    private Vector3 endPoint;
    private Vector3 restartPoint;

    private List<GameObject> invisiblePlatforms = new List<GameObject>();
    private List<GameObject> activeObjects = new List<GameObject>();

    private float furthestDistance = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        furthestDistance = Player.transform.position.x - distanceValue;
        WinText.transform.position = new Vector3(Player.transform.position.x, Player.transform.position.y + (distanceValue / 4), Player.transform.position.z - 4);

        for (int i = 0; i < CheckPoints.Length; i++)
        {
            int randTag = Random.Range(0, 4);
            if (randTag > 0) 
            {
                SetRandomCheckpoint(i, "Checkpoint", 0);

            }
            else 
            {
                SetRandomCheckpoint(i, "Restart", 1);
            }
        }

        GameObject[] platforms = GameObject.FindGameObjectsWithTag("Platform");
        for (int i = 0;i < platforms.Length;i++)
        {
            int randPlat = Random.Range(0, platforms.Length);
            if (randPlat <= platforms.Length / 3) { invisiblePlatforms.Add(platforms[i]); }
        }

        spawnPoint = Player.transform.position;
        restartPoint = spawnPoint;
        
        endPoint = new Vector3(endPos.position.x, endPos.position.y + (distanceValue / 4), WinText.transform.position.z);
    }

    // Update is called once per frame

    

    void Update()
    {
        float shiftX = Player.transform.position.x - distanceValue;
        float clampedX = Mathf.Clamp(shiftX, endPoint.x, furthestDistance);
        furthestDistance = clampedX;
        WinText.transform.position = new Vector3(clampedX, WinText.transform.position.y, WinText.transform.position.z);        
    }

    public void UpdateWinSign()
    {
        
        float shiftX = Player.transform.position.x - distanceValue;
        furthestDistance = shiftX;
        WinText.transform.position = new Vector3(shiftX, WinText.transform.position.y, WinText.transform.position.z);
    }

    public void UpdateSpawnpoint(bool isRestart)
    {
        if (!isRestart)
        {    
            spawnPoint = Player.transform.position;
        }else
        {
            spawnPoint = restartPoint;
        }
    }

    public void UpdateInvisibleLevel(bool visible)
    {

        for (int i = 0; i < invisiblePlatforms.Count; i++)
        {
            if (invisiblePlatforms[i].gameObject.GetComponent<Renderer>().enabled == true && !activeObjects.Contains(invisiblePlatforms[i]))
            {
                activeObjects.Add(invisiblePlatforms[i].gameObject);
            }
        }

        if (visible)
        {
            Debug.Log("Active true: " + activeObjects.Count);
            for (int i = 0; i < activeObjects.Count; i++)
            {
                activeObjects[i].GetComponent<Renderer>().enabled = true;
            }
        }
        else
        {

            for (int i = 0; i < activeObjects.Count; i++)
            {
                activeObjects[i].GetComponent<Renderer>().enabled = false;
            }
        }

    }

    private void SetRandomCheckpoint(int index, string tag, int matIndex)
    {
        CheckPoints[index].gameObject.tag = tag;
        CheckPoints[index].gameObject.transform.GetChild(0).GetComponent<Renderer>().material = CheckPointMaterials[matIndex];
    }
}
