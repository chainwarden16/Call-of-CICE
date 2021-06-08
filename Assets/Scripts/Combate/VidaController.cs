using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VidaController : MonoBehaviour
{

    [Header("Vida y munici�n del personaje")]
    [Tooltip("La vida y el man� actuales se usar�n en otra clase, as� que deben ser p�blicos")]
    public float vidaActual = 100f;
    float vidaMaxima = 100f;
    public float manaActual = 20f;
    float manaMaximo = 20f;


    [Header("Control de respawn")]
    [Tooltip("Si es true, deber� mostrarse una pantalla indic�ndolo y la posibilidad de cargar partida desde el GameManager")]
    public bool estaMuerto = false;

    void Morir()
    {
        if (vidaActual <= 0 || Input.GetKeyDown(KeyCode.M))
        {
            estaMuerto = true;
            Debug.Log("EstaMuerto es " + estaMuerto);
            GameObject.Find("Main Camera").GetComponent<CamaraController>().enabled = false; //se desactiva la c�mara para quitarle el control total al jugador
            //Se llama a la pantalla de fin de partida desde el GameManager
        }
    }

    public void RecibirDa�o(float da�o)
    {
        vidaActual -= da�o;
        //MostrarVidaActual();
    }


}
