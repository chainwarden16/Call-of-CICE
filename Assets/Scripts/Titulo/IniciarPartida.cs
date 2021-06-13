using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IniciarPartida : MonoBehaviour
{
    public Material cielo;
    void Start()
    {
        RenderSettings.skybox = cielo;
    }

    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            PlayerPrefs.DeleteAll();
            GameManager.IniciarPartida();

        }else if (Input.GetButtonDown("Cancel"))
        {

            GameManager.CargarPartida();

        }
    }
}
