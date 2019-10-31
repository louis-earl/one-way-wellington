using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{

    public Image blackScreen;
    public CanvasScaler canvasScaler;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;

        blackScreen.gameObject.SetActive(true);
        blackScreen.color = Color.clear;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NewEmptyGame()
    {
        StartCoroutine(TransitionOut("MainNew"));
    }

    public void NewStarterGame()
    {
        StartCoroutine(TransitionOut("MainLoad"));
    }

    private IEnumerator TransitionOut(string sceneName)
    {
        // Zoom camera 
        float t = 0;
        while (t < 1)
        {
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a + Time.deltaTime);
            Camera.main.orthographicSize += Time.deltaTime * 60;
            canvasScaler.scaleFactor += Time.deltaTime * 6 * canvasScaler.scaleFactor;
            t += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        SceneManager.LoadScene(sceneName);
    }


}
