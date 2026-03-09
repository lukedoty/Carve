using UnityEngine;
using UnityEngine.EventSystems;

public class SceneChanger : MonoBehaviour
{

    public void GoToScene(string sceneName)
    {
        GameManager.Scene.LoadSceneAndSwap(sceneName);
    }

    public void QuitApplication()
    {
        GameManager.Scene.QuitApplication();
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
 