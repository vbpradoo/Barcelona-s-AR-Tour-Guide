using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Redirections : MonoBehaviour {

    //ESFERAS
    public GameObject SagradaFamilia;
    public GameObject esfer2;


    //Sagrada Familia
    float longitudObj1 = 2.174348831176758f;
    float latitudObj1 = 41.40366012210204f;
    //Arc de triomf
    float longitudObj2 = 2.180614471435547f;
    float latitudObj2 = 41.39107282369714f;


    float distance1 = 0f;
    float distance2 = 0f;


    IEnumerator Start()
    {
        SagradaFamilia = GameObject.Find("Sphere");
       // esfer2 = GameObject.Find("esfera2");
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        // Start service before querying location
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

          //  Programa(Input.location.lastData.longitude, Input.location.lastData.latitude);


        }


        // Stop service if there is no need to query location updates continuously
        //Input.location.Stop();

    }


    // Update is called once per frame
    void Update()
    {

        float longitud = Input.location.lastData.longitude;
        float latitud = Input.location.lastData.latitude;
        float distancia = CalcDistance(longitud, latitud, longitudObj1, latitudObj1);
        float radianes = Mathf.Atan2((latitudObj1 - latitud), (longitudObj1 - longitud));

        float newX = Mathf.Cos(radianes) * 1500f;
        float newZ = Mathf.Sin(radianes) * 1500f;
        if (distancia < 0.5f)
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        }
        if (distancia > 3f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        if (distancia < 3f && distancia > 0.5f)
        {
            float s = (distancia - 0.5f) * 0.4f;
            transform.localScale = new Vector3(s, s, s);
        }
        transform.localPosition = new Vector3(newX, 0f, newZ);
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
    /*
    public void Programa(float lon, float lat)
    {

        distance1 = CalcDistance(longitudObj1, latitudObj1, lon, lat);
        distance2 = CalcDistance(longitudObj2, latitudObj2, lon, lat);
        Debug.Log("ASI ES:" + Mathf.RoundToInt(distance1));
        //		Debug.Log(lon);
        //		Debug.Log(lat);

        //Aqui decidimos la distancia a la que nos deberia de mostrar el objeto
        if (distance1 < 5)
        {

            SagradaFamilia.GetComponent<Renderer>().enabled = true;

        }
        else
        {
            Debug.Log("Prog");

            SagradaFamilia.GetComponent<Renderer>().enabled = false;

        }
        if (distance2 < 5)
        {
            esfer2.GetComponent<Renderer>().enabled = true;

        }
        else
        {
            Debug.Log("Prog");

            esfer2.GetComponent<Renderer>().enabled = false;

        }


    }
    */
}
