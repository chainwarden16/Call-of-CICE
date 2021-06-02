using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAEnemigo : MonoBehaviour
{

    public enum ComportamientoEnemigo
    {

    }

    float vidaActual = 50f;
    float velocidadMovimiento = 2f;
    bool estaMuerto = false;
    const float multiplicadorDañoColor = 3f;

    Rigidbody rigidB;

    [Header("Variables de comportamiento del enemigo")]
    ComportamientoEnemigo comp;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RecibirDaño(float daño)
    {
        vidaActual -= daño;


        SkinnedMeshRenderer[] componentes = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer com in componentes)
        {
            Color32 col = com.material.color;
            if (col.g - daño * multiplicadorDañoColor > 30)

            {

                col = new Color32(255, (byte)(col.g - daño * multiplicadorDañoColor), (byte)(col.b - daño * multiplicadorDañoColor), 255);
                com.GetComponent<SkinnedMeshRenderer>().material.color = col;

            }else
            {
                col = new Color32(255, 0, 0, 255);
                com.GetComponent<SkinnedMeshRenderer>().material.color = col;
            }
        }
        Debug.Log("La vida del enemigo es: " + vidaActual);
    }

    void Morir()
    {
        if (vidaActual <= 0)
        {
            estaMuerto = true;
        }
    }

}
