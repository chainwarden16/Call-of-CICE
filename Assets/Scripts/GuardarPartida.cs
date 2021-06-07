using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuardarPartida : MonoBehaviour
{

    /// <summary>
    /// En cuanto el jugador entre en el checkpoint, se guardar� su vida, man� y escena actual completada, para juego llamar a la siguiente que corresponda
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            JugadorController jc = other.gameObject.GetComponent<JugadorController>();
            PlayerPrefs.SetFloat("Vida", jc.vidaActual);
            PlayerPrefs.SetFloat("Mana", jc.manaActual);
            PlayerPrefs.SetInt("Escena", SceneManager.GetActiveScene().buildIndex); //dependiendo de lo que valga este n�mero, se cargar� la siguiente escena que corresponda, incluso si concluye la sesi�n
            GameManager.CargarPartida();
            //Destroy(gameObject);
        }
    }
}
