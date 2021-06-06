using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VidaController : MonoBehaviour
{

    [Header("Vida y munición del personaje")]
    [Tooltip("La vida y el maná actuales se usarán en otra clase, así que deben ser públicos")]
    public float vidaActual = 100f;
    float vidaMaxima = 100f;
    public float manaActual = 20f;
    float manaMaximo = 20f;


    [Header("Control de respawn")]
    [Tooltip("Si es true, deberá mostrarse una pantalla indicándolo y la posibilidad de cargar partida desde el GameManager")]
    public bool estaMuerto = false;

    void Morir()
    {
        if (vidaActual <= 0 || Input.GetKeyDown(KeyCode.M))
        {
            estaMuerto = true;
            Debug.Log("EstaMuerto es " + estaMuerto);
            GameObject.Find("Main Camera").GetComponent<CamaraController>().enabled = false; //se desactiva la cámara para quitarle el control total al jugador
            //Se llama a la pantalla de fin de partida desde el GameManager
        }
    }

    public void RecibirDaño(float daño)
    {
        vidaActual -= daño;
        //MostrarVidaActual();
    }


}
