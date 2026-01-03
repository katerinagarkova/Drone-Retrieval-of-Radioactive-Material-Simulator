using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PreMissionController : MonoBehaviour
{
    [Header("Dependencies")]
    public MissionManager missionManager;
    public GameObject preMissionPanel;
    public GameObject postMissionPanel;
    public GameObject inMissionPanel;
    public CameraFollow cameraScript;

    [Header("UI")]
    public TMP_Text droneStatusText;
    public TMP_Text sourceStatusText;
    public Slider batterySlider;
    public TMP_Text batteryValueText;

    [Header("Map Selection")]
    public Camera mainCamera;
    public LayerMask terrainLayer;
    public GameObject droneMarkerPrefab;
    public GameObject sourceMarkerPrefab;

    [Header("Spawning")]
    public GameObject realDronePrefab;    
    public GameObject realSourcePrefab;   

    private bool selectingDrone = false;
    private bool selectingSource = false;

    private GameObject droneMarker;
    private GameObject sourceMarker;

    public Vector3 DroneStartPosition { get; private set; }
    public Vector3 SourceStartPosition { get; private set; }

    void Start()
    {
        // Initialize UI State
        preMissionPanel.SetActive(true);
        postMissionPanel.SetActive(false);
        inMissionPanel.SetActive(false);

        batteryValueText.text = $"Battery: {(int)batterySlider.value}%";
        droneStatusText.text = "Drone Start: Not Set";
        sourceStatusText.text = "Radiation Source: Not Set";
    }

    void Update()
    {
        if ((selectingDrone || selectingSource) && Input.GetMouseButtonDown(0))
        {
            HandleMapClick();
        }
    }

    public void BeginDroneSelection()
    {
        selectingDrone = true;
        selectingSource = false;
        droneStatusText.text = "Click Map to Place Drone...";
    }

    public void BeginSourceSelection()
    {
        selectingSource = true;
        selectingDrone = false;
        sourceStatusText.text = "Click Map to Place Source...";
    }

    public void UpdateBatteryText()
    {
        batteryValueText.text = $"Battery: {(int)batterySlider.value}%";
    }

    void HandleMapClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit, 10000f, terrainLayer))
            return;

        if (selectingDrone)
        {
            DroneStartPosition = hit.point + Vector3.up * 1.0f;
            selectingDrone = false;

            PlaceMarker(ref droneMarker, droneMarkerPrefab, hit.point);
            droneStatusText.text = $"Drone Set: {hit.point}";
        }
        else if (selectingSource)
        {
            SourceStartPosition = hit.point;
            selectingSource = false;

            PlaceMarker(ref sourceMarker, sourceMarkerPrefab, hit.point);
            sourceStatusText.text = $"Source Set: {hit.point}";
        }
    }

    void PlaceMarker(ref GameObject marker, GameObject prefab, Vector3 pos)
    {
        if (marker != null) Destroy(marker);
        marker = Instantiate(prefab, pos, Quaternion.identity);
    }

    public void StartMission()
    {
        if (DroneStartPosition == Vector3.zero || SourceStartPosition == Vector3.zero)
        {
            Debug.LogError("Positions not set! Cannot start mission.");
            return;
        }

        if (realDronePrefab == null || realSourcePrefab == null)
        {
            Debug.LogError("Real Prefabs are missing in the Inspector!");
            return;
        }

        GameObject activeDrone = Instantiate(realDronePrefab, DroneStartPosition, Quaternion.identity);
        GameObject activeSource = Instantiate(realSourcePrefab, SourceStartPosition, Quaternion.identity);
        if (cameraScript != null)
        {
            cameraScript.SetTarget(activeDrone.transform);
        }

        missionManager.InitializeMission(
            activeDrone.GetComponent<DroneController>(), 
            activeSource.transform, 
            batterySlider.value,
            DroneStartPosition,
            SourceStartPosition
        );

        if (droneMarker) Destroy(droneMarker);
        if (sourceMarker) Destroy(sourceMarker);

        preMissionPanel.SetActive(false);
        postMissionPanel.SetActive(false);
        inMissionPanel.SetActive(true);
    }
}