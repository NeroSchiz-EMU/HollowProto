using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [Tooltip("Represents possible levels of water. Should go from highest to lowest level")]
    [SerializeField] List<float> waterLevels;
    [Tooltip("The water level to start at")]
    [SerializeField] int startingLevelIndex = 0;
    [Tooltip("Speed at which water level decreases")]
    [SerializeField] float waterSpeed = 0.2f;


    public Health player;

    // tracks index of current water level
    int waterLevelIndex;

    public void SetY(float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set the starting water level
        waterLevelIndex = startingLevelIndex;
        SetY(waterLevels[startingLevelIndex]);

        // Find and subscribe to all lever events
        WaterLever[] waterLevers = GameObject.FindObjectsByType<WaterLever>(FindObjectsSortMode.None);
        foreach (WaterLever waterLever in waterLevers)
        {
            waterLever.leverHitOnEvent.AddListener(LowerLevel);
            waterLever.leverHitOffEvent.AddListener(RaiseLevel);
        }

        GameObject.Find("Player").GetComponent<Health>().PlayerDeath.AddListener(ResetLevel);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            LowerLevel();
        }
    }

    // Function which lowers the water level
    private void LowerLevel()
    {
        if (waterLevelIndex < waterLevels.Count)
        {
            StopAllCoroutines();
            StartCoroutine(LevelChangeRoutine(waterLevelIndex + 1));
        }
        else
        {
            Debug.LogError("Water level index out of bounds. This most likely means there are more levers than water levels.");
        }
    }

    private void RaiseLevel()
    {
        if (waterLevelIndex >= 0)
        { 
            StopAllCoroutines();
            StartCoroutine(LevelChangeRoutine(waterLevelIndex - 1));
        }
        else
        {
            Debug.LogError("Water level index out of bounds. Water level is being raised when it can't go any higher");
        }
    }

    //upon player death. reset water height
    private void ResetLevel()
    {
        StopAllCoroutines();
        waterLevelIndex = startingLevelIndex;
        SetY(waterLevels[startingLevelIndex]);

         WaterLever[] waterLevers = GameObject.FindObjectsByType<WaterLever>(FindObjectsSortMode.None);
        foreach (WaterLever waterLever in waterLevers)
        {
            waterLever.Reset();
        }
    }

    IEnumerator LevelChangeRoutine(int newLevelIndex)
    {
        waterLevelIndex = newLevelIndex;
        
        float newY = waterLevels[newLevelIndex];
        float startingWaterLevel = transform.position.y;
        float changeTime = Mathf.Abs(startingWaterLevel - newY) / waterSpeed;

        float startTime = Time.time;
        float progress = (Time.time - startTime) / changeTime;
        while (progress < 1)
        {
            progress = (Time.time - startTime) / changeTime;

            SetY(Mathf.Lerp(startingWaterLevel, newY, progress));

            yield return null;
        }

        SetY(newY);

        
    }
}
