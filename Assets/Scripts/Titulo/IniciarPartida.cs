using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IniciarPartida : MonoBehaviour
{
    public Material cielo;
    // Start is called before the first frame update
    void Start()
    {
        RenderSettings.skybox = cielo;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {

            GameManager.IniciarPartida();

        }else if (Input.GetButtonDown("Cancel"))
        {

            GameManager.CargarPartida();

        }
    }
}
