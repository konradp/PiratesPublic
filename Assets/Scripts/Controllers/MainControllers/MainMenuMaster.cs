using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuMaster : MonoBehaviour
{
    public GameObject[] AllCanvases;
    public Button[] activateButtons;
    void Start()
    {
        ActivateCanvas(0); //failsafe if I forget to activate main menu
        PlaylistManager.instance.ChangePlaybackStatus(false);
    }
    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
    
    /// <summary>
    /// Function to switch Main Menu Canvases, usually called from button
    /// </summary>
    /// <param name="_index">0 represents Main Menu, 1 represents options, 2 credits, 3 help, rest TBD</param>
    public void ActivateCanvas(int _index)
    {
        for (int i = 0; i < AllCanvases.Length; i++)
        {
            if (i == _index)
            {
                AllCanvases[i].SetActive(true);
                activateButtons[i].Select();
            }
            else
                AllCanvases[i].SetActive(false);
        }
    }
    public void QuitGame()
    {
        Application.Quit(0);
    }
}
