using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject howToUsePanel;
    public string simulationSceneName = "SampleScene";

    public void StartSimulation()
    {
        SceneManager.LoadScene(simulationSceneName);
    }

    public void ShowHowToUse()
    {
        howToUsePanel.SetActive(true);
    }

    public void HideHowToUse()
    {
        howToUsePanel.SetActive(false);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
}
