using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CerrarJuego : MonoBehaviour
{
    public Material cielo;
    void Start()
    {
        RenderSettings.skybox = cielo;
    }

    void Update()
    {
        if (Input.GetButtonDown("CerrarJuego"))
        {
            Application.Quit();
        }
    }
}
