using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MissionManager : MonoBehaviour
{
    [Header("Dependencies")]
    public PreMissionController preMissionController;

    [Header("Panels")]
    public GameObject inMissionPanel;
    public GameObject postMissionPanel;

    [Header("In-Mission UI")]
    public TMP_Text batteryText;
    public TMP_Text radiationText;
    public TMP_Text distanceText;

    [Header("Post-Mission UI")]
    public TMP_Text missionResultText;

    // Simulation State
    private DroneController activeDrone;
    private Transform activeSource;
    
    private float initialBattery;
    private float currentBattery;
    private Vector3 startPos;
    private Vector3 sourcePos;
    
    private bool isRunning = false;
    private bool missionEnded = false;

    private float successDistance = 15.0f;
    private float batteryDrainRate = 2.0f;

    private List<Pose> positionHistory = new List<Pose>();
    private float historyTimer = 0f;

    public void InitializeMission(DroneController drone, Transform source, float battery, Vector3 dStart, Vector3 sStart)
    {
        activeDrone = drone;
        activeSource = source;
        initialBattery = battery;
        currentBattery = battery;
        startPos = dStart;
        sourcePos = sStart;

        positionHistory.Clear();
        missionEnded = false;
        isRunning = true;
        Time.timeScale = 1.0f;

        inMissionPanel.SetActive(true);
        postMissionPanel.SetActive(false);

        activeDrone.MoveTo(sourcePos);
    }

    void Update()
    {
        if (!isRunning || missionEnded || activeDrone == null) return;

        UpdateStatsAndCheckGameState();
        RecordHistory();
    }

    void UpdateStatsAndCheckGameState()
    {
        float dist = Vector3.Distance(activeDrone.transform.position, activeSource.position);
        distanceText.text = $"Dist: {dist:F1} m";
        
        float rads = (dist < 1.0f) ? 999f : (1000f / (dist * dist));
        radiationText.text = $"Radiation: {rads:F2} mSv";

        currentBattery -= Time.deltaTime * batteryDrainRate; 
        batteryText.text = $"Battery: {Mathf.Max(0, currentBattery):F1}%";

        if (dist <= successDistance)
        {
            EndMission(true);
        }
        else if (currentBattery <= 0)
        {
            EndMission(false);
        }
    }

    void EndMission(bool successful)
    {
        missionEnded = true;
        isRunning = false;
        Time.timeScale = 0f;

        inMissionPanel.SetActive(false);
        postMissionPanel.SetActive(true);

        if (successful)
        {
            missionResultText.text = "Mission Successful";
            missionResultText.color = Color.green;
        }
        else
        {
            missionResultText.text = "Mission Unsuccessful";
            missionResultText.color = Color.red;
        }
    }

    void RecordHistory()
    {
        historyTimer += Time.deltaTime;
        if (historyTimer >= 0.5f)
        {
            historyTimer = 0;
            positionHistory.Add(new Pose(activeDrone.transform.position, activeDrone.transform.rotation));
            if (positionHistory.Count > 60) positionHistory.RemoveAt(0);
        }
    }

    public void OnPause() { Time.timeScale = 0f; }
    public void OnPlay() { Time.timeScale = 1f; }
    public void OnFastForward() { Time.timeScale = 2.0f; }

    public void OnRewind()
    {
        if (activeDrone == null || positionHistory.Count == 0 || missionEnded) return;

        int stepBackAmount = 10; 
        int targetIndex = Mathf.Max(0, positionHistory.Count - stepBackAmount);
        Pose pastPose = positionHistory[targetIndex];
        positionHistory.RemoveRange(targetIndex, positionHistory.Count - targetIndex);
        activeDrone.Teleport(pastPose.position, pastPose.rotation);
        
        currentBattery += (stepBackAmount * 0.5f * batteryDrainRate); 
    }

    public void OnRestartMission()
    {
        if (activeDrone == null) return;

        Time.timeScale = 1.0f;
        currentBattery = initialBattery;
        missionEnded = false;
        isRunning = true;
        positionHistory.Clear();

        postMissionPanel.SetActive(false);
        inMissionPanel.SetActive(true);

        activeDrone.Teleport(startPos, Quaternion.identity);
        
        activeDrone.MoveTo(sourcePos);
    }

    public CameraFollow cameraScript;

public void OnNewMission()
{
    if (activeDrone != null) Destroy(activeDrone.gameObject);
    if (activeSource != null) Destroy(activeSource.gameObject);

    isRunning = false;
    missionEnded = false;
    Time.timeScale = 1.0f;

    if (cameraScript != null)
    {
        cameraScript.ResetCamera();
    }

    postMissionPanel.SetActive(false);
    inMissionPanel.SetActive(false);
    preMissionController.preMissionPanel.SetActive(true);
}
}