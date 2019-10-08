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

    public GameObject wormholeScreen;
    public GameObject backgroundRainbow;
    private bool isWormholeMode; //used to avoid conflicting planet arrival 

    // Start is called before the first frame update
    void Start()
    {
        blackScreen.enabled = false;

        wormholeScreen.SetActive(true);
        wormholeScreen.GetComponent<PanelAlpha>().alpha = 0;
        wormholeScreen.SetActive(false);

        backgroundRainbow.SetActive(false);

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

        InputController.Instance.cameraBoundMin = -100000;
        InputController.Instance.cameraBoundMax = 100000;
        float t = 0;
        while (t < 0.5)
        {
            InputController.Instance.desiredCameraZoom += 500 * Time.unscaledDeltaTime;
            // InputController.Instance.UpdateBackgroundScale();
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a + 2 * Time.unscaledDeltaTime);
            t += Time.unscaledDeltaTime;
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
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
        

        blackScreen.enabled = true;
        blackScreen.color = new Color(0, 0, 0, 1);

        InputController.Instance.cameraSizeMin = 0.1f;
        InputController.Instance.cameraSizeMax = 300;
        InputController.Instance.cameraPosZ = 1010;
        Camera.main.transform.position = new Vector3(0, 0, 1010);
        InputController.Instance.desiredCameraPos = new Vector3(0, 0, 1010);
        Camera.main.orthographicSize = 0.1f;
        InputController.Instance.desiredCameraZoom = 0.1f;

        // Switch background GO 
        InputController.Instance.backgroundGO_Ship.SetActive(false);
        InputController.Instance.backgroundGO_Map.SetActive(true);

        float t = 0;
        while (t < 0.66)
        {
            InputController.Instance.desiredCameraZoom += 350 * Time.unscaledDeltaTime;
            // InputController.Instance.UpdateBackgroundScale();
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a - 2 * Time.unscaledDeltaTime);
            t += Time.unscaledDeltaTime;
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }
        InputController.Instance.cameraSizeMin = 20;
        InputController.Instance.cameraZoomEnabled = true;
        InputController.Instance.cameraBoundMin = -600;
        InputController.Instance.cameraBoundMax = 600;
        yield return null;
    }



    private IEnumerator TransitionOutMap()
    {
        blackScreen.enabled = true;
        blackScreen.color = new Color(0, 0, 0, 0);

        InputController.Instance.cameraBoundMin = -100000;
        InputController.Instance.cameraBoundMax = 100000;

        InputController.Instance.cameraZoomEnabled = false;
        InputController.Instance.cameraSizeMin = 0.1f;
        float zoomPerFrame = Camera.main.orthographicSize / 0.66f;
        
        float t = 0;
        while (t < 0.66)
        {
            InputController.Instance.desiredCameraZoom -= zoomPerFrame * Time.unscaledDeltaTime;
            if (InputController.Instance.desiredCameraZoom < 1) InputController.Instance.desiredCameraZoom = 1; // Things get weird when orthographic size < 0
            // InputController.Instance.UpdateBackgroundScale();

            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a + 2 * Time.unscaledDeltaTime);
            t += Time.unscaledDeltaTime;

            yield return new WaitForSeconds(Time.unscaledDeltaTime);
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

        // Switch background GO 
        InputController.Instance.backgroundGO_Ship.SetActive(true);
        InputController.Instance.backgroundGO_Map.SetActive(false);

        InputController.Instance.desiredCameraPos = new Vector3(50, 50, -10);

        InputController.Instance.cameraSizeMin = 3;
        InputController.Instance.cameraSizeMax = 1000;

        InputController.Instance.cameraBoundMin = -100000;
        InputController.Instance.cameraBoundMax = 100000;

        Camera.main.orthographicSize = 850;
        InputController.Instance.desiredCameraZoom = 850;

        // TODO: Resume travel speed or time here?
        UserInterfaceController.Instance.ShowMainUI();
        


        float t = 0;
        while (t < 0.5)
        {
            InputController.Instance.desiredCameraZoom -= 1540 * Time.unscaledDeltaTime;
            // InputController.Instance.UpdateBackgroundScale();
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a - 2 * Time.unscaledDeltaTime);
            t += Time.unscaledDeltaTime;
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }
        InputController.Instance.desiredCameraZoom = 40;
        yield return new WaitForSeconds(1);
        InputController.Instance.cameraBoundMin = -20;
        InputController.Instance.cameraBoundMax = 120;
        InputController.Instance.cameraSizeMax = 50;

        InputController.Instance.cameraZoomEnabled = true;


        yield return null;
    }



    // Called by journey controller on planet arrival 
    public IEnumerator TransitionArrival(Planet planet)
    {
        if (!isWormholeMode)
        {

            // Quick fade to black 
            blackScreen.enabled = true;
            blackScreen.color = new Color(0, 0, 0, 0);
            while (blackScreen.color.a < 1)
            {
                blackScreen.color = new Color(0, 0, 0, blackScreen.color.a + (2 * Time.unscaledDeltaTime));
                yield return new WaitForSeconds(Time.unscaledDeltaTime);
            }
            blackScreen.color = new Color(0, 0, 0, 1);

            JourneyController.Instance.shipSpeedCurrent = 0;
            UserInterfaceController.Instance.ShowLandUI();


            // return black screen
            while (blackScreen.color.a > 0)
            {
                blackScreen.color = new Color(0, 0, 0, blackScreen.color.a - (2 * Time.unscaledDeltaTime));
                yield return new WaitForSeconds(Time.unscaledDeltaTime);
            }
            blackScreen.color = new Color(0, 0, 0, 0);
            blackScreen.enabled = false;

            // Planet
            planetLandGO = planet.gameObject;
            if (isMapMode)
            {
                yield break;
            }
            planet.allowClick = false;

            planetLandGO.transform.localScale = new Vector3(8, 8, 1);
            planetLandGO.transform.parent = gameObject.transform;
            planetLandGO.transform.localPosition = new Vector3(180, 50, 0);

            while (planetLandGO.transform.localPosition.x > 115f)
            {
                // Slow down as planet approaches target position 
                float speed = (40 - Mathf.Clamp(120 - planetLandGO.transform.localPosition.x, 0, 40));
                planetLandGO.transform.position = new Vector3(planetLandGO.transform.position.x - (speed * Time.unscaledDeltaTime), planetLandGO.transform.position.y, planetLandGO.transform.position.z);

                yield return new WaitForSeconds(Time.unscaledDeltaTime);
            }
        }
        yield return null;
    }

    // Called by journey controller on planet land 
    public IEnumerator TransitionLanding()
    {
        InputController.Instance.cameraSizeMax = 10000;
        InputController.Instance.cameraBoundMin = -100000;
        InputController.Instance.cameraBoundMax = 100000;
        InputController.Instance.cameraZoomEnabled = false;

        // Zoom camera and scale up planet 

        blackScreen.enabled = true;
        blackScreen.color = new Color(0, 0, 0, 0);
        float time = 0f;
        while (time < 0.5f)
        {
            InputController.Instance.desiredCameraZoom += 450 * Time.unscaledDeltaTime;

            planetLandGO.transform.localScale = new Vector3(planetLandGO.transform.localScale.x + (60 * Time.unscaledDeltaTime), planetLandGO.transform.localScale.y + (60 * Time.unscaledDeltaTime), 1);
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a + (2 * Time.unscaledDeltaTime));

            time += Time.unscaledDeltaTime;
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }        
        
        blackScreen.color = new Color(0, 0, 0, 1);

        JourneyController.Instance.OnLandComplete();

        blackScreen.color = new Color(0, 0, 0, 1);

        // return black screen
        while (blackScreen.color.a > 0)
        {
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a - (2 * Time.unscaledDeltaTime));
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }
        blackScreen.color = new Color(0, 0, 0, 0);
        blackScreen.enabled = false;

        yield return null;
    }

    // Called when first launching journey to board passengers at first planet 
    public IEnumerator TransitionLandingWithoutZoom()
    {
        blackScreen.enabled = true;
        blackScreen.color = new Color(0, 0, 0, 0);
        InputController.Instance.cameraZoomEnabled = false;

        while (blackScreen.color.a < 1)
        {
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a + 1 * Time.unscaledDeltaTime);
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }
        mapGO.SetActive(false);

        JourneyController.Instance.OnLandComplete();

        yield return null;
    }

    // Called when first launching journey to board passengers at first planet 
    public IEnumerator TransitionWormhole(Planet planet)
    {
        // Note: Replaces TransitionLanding() and TransitionArrival() 
        isWormholeMode = true;
        JourneyController.Instance.nextPlanetVisit = planet;

        InputController.Instance.cameraZoomEnabled = false;

        // Activate wormhole screen
        wormholeScreen.SetActive(true);
        wormholeScreen.GetComponent<PanelAlpha>().alpha = 0;
        while (wormholeScreen.GetComponent<PanelAlpha>().alpha < 1)
        {
            wormholeScreen.GetComponent<PanelAlpha>().alpha += Time.unscaledDeltaTime * 2f;
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }
 
        // Set scene to rainbow-wormhole travel 
        backgroundRainbow.SetActive(true);
        backgroundRainbow.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        StartCoroutine(TransitionInMain());
        isMapMode = false;

        // Position camera 
        InputController.Instance.desiredCameraPos = new Vector3(50, 50, -10);
        Camera.main.transform.position = new Vector3(50, 50, -10);

        InputController.Instance.desiredCameraZoom = 50;
        Camera.main.orthographicSize = 50;
        InputController.Instance.cameraZoomEnabled = false;


        // Move ship (with disengaged position) 
        JourneyController.Instance.shipSpeedCurrent = 30;
        JourneyController.Instance.shipCoordinates = Vector2.positiveInfinity;

        // Wait for wormhole screen 
        yield return new WaitForSeconds(1.5f);


        // Deactivate wormhole screen
        while (wormholeScreen.GetComponent<Image>().color.a > 0)
        {
            wormholeScreen.GetComponent<PanelAlpha>().alpha -= Time.unscaledDeltaTime * 2f;
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }
        wormholeScreen.SetActive(false);
        InputController.Instance.cameraZoomEnabled = false;


        // Wait for rainow-wormhole travel 
        yield return new WaitForSeconds(3);

        blackScreen.enabled = true;
        blackScreen.color = new Color(0, 0, 0, 0);

        // Finish rainow-wormhole travel 
        while (backgroundRainbow.GetComponent<SpriteRenderer>().color.a > 0)
        {
            backgroundRainbow.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, backgroundRainbow.GetComponent<SpriteRenderer>().color.a - (2*Time.unscaledDeltaTime));
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a + (2*Time.unscaledDeltaTime));
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }

        // Position ship at planet
        JourneyController.Instance.shipSpeedCurrent = 0;
        JourneyController.Instance.shipCoordinates = planet.GetPlanetCoordinates();


        // Planet graphic
        planetLandGO = planet.gameObject;
        planetLandGO.transform.localScale = new Vector3(8, 8, 1);
        planetLandGO.transform.parent = gameObject.transform;
        planetLandGO.transform.localPosition = new Vector3(115, 50, 0);
        planet.allowClick = false;

        // Board passengers 
        JourneyController.Instance.OnLandComplete();

        // return black screen
        while (blackScreen.color.a > 0)
        {
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a - (4 * Time.unscaledDeltaTime));
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }
        blackScreen.color = new Color(0, 0, 0, 0);
        blackScreen.enabled = false;

        // Remove map journey line
        Destroy(JourneyController.Instance.distanceLineTeleportInstance);

        InputController.Instance.cameraZoomEnabled = true;
        isWormholeMode = false;

        yield return null;
    }

    public IEnumerator QuickFadeToBlack()
    {
        StopCoroutine(QuickFadeFromBlack());

        // Quick fade to black 
        blackScreen.enabled = true;
        blackScreen.color = new Color(0, 0, 0, 0);
        while (blackScreen.color.a < 1)
        {
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a + (2 * Time.unscaledDeltaTime));
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }
        blackScreen.color = new Color(0, 0, 0, 1);
        yield return null;

    }

    public IEnumerator QuickFadeFromBlack()
    {
        StopCoroutine(QuickFadeToBlack());

        blackScreen.color = new Color(0, 0, 0, 1);

        // return black screen
        while (blackScreen.color.a > 0)
        {
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a - (2 * Time.unscaledDeltaTime));
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }
        blackScreen.color = new Color(0, 0, 0, 0);
        blackScreen.enabled = false;
        yield return null;

    }

    public IEnumerator TransitionDeparture()
    {

        InputController.Instance.cameraBoundMin = -100000;
        InputController.Instance.cameraBoundMax = 100000;
        InputController.Instance.cameraZoomEnabled = false;

        // Reset camera parameters
        InputController.Instance.cameraPosZ = -10;
        Camera.main.transform.position = new Vector3(50, 50, -10);
        InputController.Instance.desiredCameraPos = new Vector3(50, 50, -10);
        InputController.Instance.cameraSizeMin = 3;
        InputController.Instance.cameraSizeMax = 50;

        // Switch background GO 
        InputController.Instance.backgroundGO_Ship.SetActive(true);
        InputController.Instance.backgroundGO_Map.SetActive(false);

        isMapMode = false;

        blackScreen.color = new Color(0, 0, 0, 1);
        while (blackScreen.color.a > 0)
        {
            blackScreen.color = new Color(0, 0, 0, blackScreen.color.a - (4 * Time.unscaledDeltaTime));
            yield return new WaitForSeconds(Time.unscaledDeltaTime);
        }
        blackScreen.enabled = false;
        InputController.Instance.cameraBoundMin = -20;
        InputController.Instance.cameraBoundMax = 120;
        InputController.Instance.cameraZoomEnabled = true;

        yield return null;
    }

}
