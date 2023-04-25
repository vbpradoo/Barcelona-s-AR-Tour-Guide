using System.Collections;   
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour {


    public Sprite Imagen;

    public float longi;
    public float lati;

    float distance;
    //Distancia minima de rendering de imagen  (unidad=km)
    const float mindist= 1;
    /*
    float longi=2.174348831176758f;
    float lati=41.40366012210204f;
    */
    float mylati;
    float mylongi;

    public RectTransform Northlayer;
    public RectTransform MissionLayer;

    Vector3 myplace;
    Vector3 missionplace;

    //Factor de correcion
    const float fc = 10;

    private Vector3 NorthDirection;
    private Vector3 MissionDir;
    private Quaternion MissionDirection;

    //Objetos de información
    public GameObject Panel;

    static bool flag=true;


    void Awake()
    {
        Debug.Log("AWAKADO");
        Panel = GameObject.Find("Image");
        Panel.AddComponent<SpriteRenderer>();
    }


    IEnumerator Start()
    {

        Debug.Log("ENTRA");

        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        // Start service before querying location
        Input.compass.enabled = true;

        Input.location.Start(1f, 0.1f);

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            Debug.Log("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Debug.Log("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
            Debug.Log("ENTRA");

            mylati = Input.location.lastData.latitude;
            mylongi = Input.location.lastData.longitude;

            // NorthDirection.z = Input.compass.trueHeading;


            //  Programa(Input.location.lastData.longitude, Input.location.lastData.latitude);
        }
        //  Debug.Log("ENTRA");

        Debug.Log("ENTRA: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);


    }



    // Update is called once per frame
    void Update() {

        ChangeNorthDirection();
        ChangeMissionDirection();
        ShowMonument();

    }

    public void ChangeNorthDirection() {

        NorthDirection.z = Input.compass.trueHeading;
        Northlayer.localEulerAngles = NorthDirection;
    }

    public void ChangeMissionDirection()
    {

        myplace = new Vector3(mylati, mylongi, 1);
        missionplace = new Vector3(lati, longi, 1);
        distance = CalcDistance(mylongi, mylati, longi, lati);


        Debug.Log("MYPLACE:" + myplace.x.ToString() + ";" + myplace.y.ToString());

        Vector3 dir = myplace - missionplace;
        Debug.Log("ASI ES:" + dir.x.ToString() + ";" + dir.y.ToString());

        Vector3 point;

        //Creo un punto arbitrario para estudiar el angulo entre mi posicion y la del monumento
        point.x = myplace.x;
        point.y = missionplace.y;
        point.z = 0;

        Vector3 pseudoNorth = myplace - point;


        float dire = dir.magnitude / pseudoNorth.magnitude;

        Debug.Log("DireDire:  " + dire.ToString());

        float angle = (Mathf.Acos(dire) * 180) / Mathf.PI;

        Debug.Log("DireNorth:  " + NorthDirection.z);
        Debug.Log("DireAngle:  " + angle);

        var fangle = Mathf.Acos(dire);


        Vector3 Miss;

        //Seteamos el sentido con repecto al Norte y aplicamos el factor de correcion
        angle = angle - fc;
        if (myplace.y > missionplace.y)
            Miss = new Vector3(0, 0, NorthDirection.z + angle);
        else
            Miss = new Vector3(0, 0, NorthDirection.z - angle);

        //MissionDirection = Quaternion.LookRotation(Miss);

        Debug.Log("DireMiss::" + Miss.z);

        MissionLayer.localEulerAngles = Miss;

    }

    public float CalcDistance(float lon1, float lat1, float lon2, float lat2)
    {
        Debug.Log("CALCUS:" + lon1.ToString() + " , " + lat1.ToString() + " , " + lat2.ToString() + " , " + lon2.ToString());
        float R = 6371; // radius of earth in km
        var dLat = (lat2 - lat1) * (Mathf.PI / 180);
        var dLon = (lon2 - lon1) * (Mathf.PI / 180);
        lat1 = lat1 * (Mathf.PI / 180);
        lat2 = lat2 * (Mathf.PI / 180);
        var a = Mathf.Sin(dLat / 2) * Mathf.Sin(dLat / 2) +
        Mathf.Sin(dLon / 2) * Mathf.Sin(dLon / 2) * Mathf.Cos(lat1) * Mathf.Cos(lat2);
        var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
        var d = R * c;
        Debug.Log("CALCU1:" + d.ToString() + "\n" + d);

        return d; //distance in kms
    }

    public void ShowMonument() {

        //Si estamos a "mindist" km se muestra la información una vez detecte el target
        if (distance < mindist )
        {
            Debug.Log("Chega Imaxe");
            Panel.SetActive(true);
            // Panel.GetComponent<Renderer>().enabled = true;
            // renderer.sprite = Resources.Load<Sprite>("Sprites/MySprite");
            Panel.GetComponent<Image>().sprite = Imagen;

            flag = false;
        }
        /*if (distance > mindist && !flag)
        {
            Debug.Log("Sae Imaxe");

            Panel.GetComponent<Renderer>().enabled = false;

            flag = true;
        }*/
        else
        {
            Debug.Log("Sae Imaxe");
            Panel.SetActive(false);
           // Panel.GetComponent<Renderer>().enabled = false;
        }
    }
}
