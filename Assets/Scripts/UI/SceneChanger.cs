using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{

    public void goToScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    public void TriggerSwitchToSettings(Animation anim)
    {
        if (anim == null) return;
        
        anim.Play("MMToSettings");
    }

    public void TriggerSwitchToMM(Animation anim)
    {
        if (anim == null) return;
        
        anim.Play("SettingsToMM");
    }

}
 