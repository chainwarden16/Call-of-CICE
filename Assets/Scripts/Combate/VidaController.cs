using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VidaController : MonoBehaviour
{
    /// <summary>
    /// Esta clase es para las rocas del tutorial con las que practicar
    /// </summary>
    [Header("Vida y munici�n del personaje")]
    [Tooltip("La vida del dummy")]
    public float vidaActual = 100f;
    public float manaActual = 20f;

    void Morir()
    {
        if (vidaActual <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void RecibirDa�o(float da�o)
    {
        vidaActual -= da�o;
        //MostrarVidaActual();
    }


}
