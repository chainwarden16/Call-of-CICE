using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CieloController : MonoBehaviour
{
    public Material cielo;
    void Start()
    {
        RenderSettings.skybox = cielo;
    }

}
