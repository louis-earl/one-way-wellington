using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TransitionController : MonoBehaviour
{
    public static TransitionController Instance;

    public GameObject mapGO;
    public Image blackScreen;

    private GameObject planetLandGO;

    public bool isMapMode;

    // Start is called before the first frame update
    void Start()
    {
        blackScreen.enabled = false;
        if (Instance == null) Instance = this;
    }

    // Called from UI button
    public void StartTransitionToMap()
    {
        StartCoroutine(TransitionOutMain());
        
    }

    public void StartTransitionToMain()
    {
        StartCoroutine(TransitionOutMap());
        
    }

    private IEnumerator TransitionOutMain()
    {
        blackScreen.enabled = true;
        blackScreen.color = new Color(0, 0, 0, 0);
        InputController.Instance.cameraZoomEnabled = false;
        InputController.Instance.cameraSizeMax = 1000;

        float t = 0;
        while (t < 0.5)
        {
            InputController.Instance.desiredCameraZoom += 500 * Time.deltaTime;
            InputController.Instance.UpdateBackgroundScale();
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a + 2 * Time.deltaTime);
            t += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }


        StartCoroutine(TransitionInMap());

        yield return null;
    }

    public IEnumerator TransitionInMap()
    {
        isMapMode = true;
        JourneyController.Instance.OnMapMode();

        mapGO.SetActive(true);

        // TODO: Pause travel speed or time 
        UserInterfaceController.Instance.ShowMapUI();
        InputController.Instance.SetMode_None();
        InputController.Instance.cameraBoundMin = -600;
        InputController.Instance.cameraBoundMax = 600;

        blackScreen.enabled = true;
        blackScreen.color = new Color(0, 0, 0, 1);

        InputController.Instance.cameraSizeMin = 0.1f;
        InputController.Instance.cameraSizeMax = 300;
        InputController.Instance.cameraPosZ = 1010;
        Camera.main.transform.position = new Vector3(0, 0, 1010);
        Camera.main.orthographicSize = 0.1f;
        InputController.Instance.desiredCameraZoom = 0.1f;

        float t = 0;
        while (t < 0.66)
        {
            InputController.Instance.desiredCameraZoom += 350 * Time.deltaTime;
            InputController.Instance.UpdateBackgroundScale();
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a - 2 * Time.deltaTime);
            t += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        InputController.Instance.cameraSizeMin = 20;
        InputController.Instance.cameraZoomEnabled = true;
        yield return null;
    }



    private IEnumerator TransitionOutMap()
    {
        blackScreen.enabled = true;
        blackScreen.color = new Color(0, 0, 0, 0);

        InputController.Instance.cameraZoomEnabled = false;
        InputController.Instance.cameraSizeMin = 0.1f;
        float zoomPerFrame = Camera.main.orthographicSize / 0.66f;
        
        float t = 0;
        while (t < 0.66)
        {
            InputController.Instance.desiredCameraZoom -= zoomPerFrame * Time.deltaTime;
            if (InputController.Instance.desiredCameraZoom < 1) InputController.Instance.desiredCameraZoom = 1; // Things get weird when orthographic size < 0
            InputController.Instance.UpdateBackgroundScale();

            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a + 2 * Time.deltaTime);
            t += Time.deltaTime;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        Destroy(JourneyController.Instance.distanceRingInstance);

        JourneyController.Instance.panel_FuelCost.SetActive(false);

        StartCoroutine(TransitionInMain());
        isMapMode = false;
        yield return null;
    }


    private IEnumerator TransitionInMain()
    {
        blackScreen.enabled = true;
        blackScreen.color = new Color(0, 0, 0, 1);
        mapGO.SetActive(false);


        InputController.Instance.cameraPosZ = -10;
        Camera.main.transform.position = new Vector3(50, 50, -10);

        InputController.Instance.cameraSizeMin = 3;
        InputController.Instance.cameraSizeMax = 1000;
        Camera.main.orthographicSize = 850;
        InputController.Instance.desiredCameraZoom = 850;

        // TODO: Resume travel speed or time here?
        UserInterfaceController.Instance.ShowMainUI();
        InputController.Instance.cameraBoundMin = 0;
        InputController.Instance.cameraBoundMax = 100;


        float t = 0;
        while (t < 0.5)
        {
            InputController.Instance.desiredCameraZoom -= 1550 * Time.deltaTime;
            InputController.Instance.UpdateBackgroundScale();
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a - 2 * Time.deltaTime);
            t += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        InputController.Instance.cameraSizeMax = 50;
        InputController.Instance.cameraZoomEnabled = true;


        yield return null;
    }

    

    // Called by journey controller on planet arrival 
    public IEnumerator TransitionArrival(Planet planet)
    {
        planetLandGO = planet.gameObject;
        if (isMapMode)
        {
            yield break;
        }
        
        planetLandGO.transform.localScale = new Vector3(10 * BackgroundController.Instance.parallax.transform.localScale.x, 10 * BackgroundController.Instance.parallax.transform.localScale.x, 1);
        planetLandGO.transform.parent = Camera.main.transform.GetChild(0);
        planetLandGO.transform.localPosition = new Vector3(150, 0, 0);

        planetLandGO.GetComponent<PlanetSpin>().InitSprites(planet.GetSurface(), planet.GetClouds());

        
        while (planetLandGO.transform.localPosition.x > 80f) 
        {
            // Slow down as planet approaches target position 
            float speed = (40 - Mathf.Clamp(120 - planetLandGO.transform.localPosition.x, 0, 40)) * BackgroundController.Instance.parallax.transform.localScale.x;
            planetLandGO.transform.position = new Vector3(planetLandGO.transform.position.x - (speed * Time.deltaTime), planetLandGO.transform.position.y, planetLandGO.transform.position.z);
            

            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return null;
    }

    // Called by journey controller on planet land 
    public IEnumerator TransitionLanding()
    {
        InputController.Instance.cameraSizeMax = 1000;

        blackScreen.enabled = true;
        blackScreen.color = new Color(0, 0, 0, 0);
        float time = 0f;
        while (time < 1)
        {
            InputController.Instance.desiredCameraZoom += 500 * Time.deltaTime;
            InputController.Instance.UpdateBackgroundScale();
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a + 1 * Time.deltaTime);

            planetLandGO.transform.localScale = new Vector3(planetLandGO.transform.localScale.x + (10 * Time.deltaTime), planetLandGO.transform.localScale.y + (10 * Time.deltaTime), 1);
            time += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        // Return planet to map
        mapGO.SetActive(true);

        planetLandGO.transform.parent = mapGO.transform;
        planetLandGO.transform.localScale = new Vector3(planetLandGO.GetComponent<Planet>().planetScale, planetLandGO.GetComponent<Planet>().planetScale, 1);
        planetLandGO.transform.localPosition = new Vector3(planetLandGO.GetComponent<Planet>().GetPlanetCoordinates().x, planetLandGO.GetComponent<Planet>().GetPlanetCoordinates().y, 0);

        mapGO.SetActive(false);

        JourneyController.Instance.OnLandComplete();

        yield return null;
    }

    // Called when first launching journey to board passengers at first planet 
    public IEnumerator TransitionLandingWithoutZoom()
    {
        blackScreen.enabled = true;
        blackScreen.color = new Color(0, 0, 0, 0);
        while (blackScreen.color.a < 1)
        {
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a + 1 * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        mapGO.SetActive(false);

        JourneyController.Instance.OnLandComplete();

        yield return null;
    }

    public IEnumerator TransitionDeparture()
    {
        // Reset camera parameters
        InputController.Instance.cameraPosZ = -10;
        Camera.main.transform.position = new Vector3(50, 50, -10);
        InputController.Instance.cameraSizeMin = 3;
        InputController.Instance.cameraSizeMax = 50;
        InputController.Instance.cameraBoundMin = 0;
        InputController.Instance.cameraBoundMax = 100;

        isMapMode = false;

        blackScreen.color = new Color(0, 0, 0, 1);
        float time = 0f;
        while (time < 1)
        {
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a - 1 * Time.deltaTime);
            time += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        blackScreen.enabled = false;

        yield return null;
    }

}
