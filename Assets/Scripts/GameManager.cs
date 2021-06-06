using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Control de enemigos restantes en el combate y el jugador")]
    public static List<GameObject> enemigosRestantes;
    Text textoEnemigosRestantes;

    [Tooltip("Monitorea el estado del jugador para determinar si está vivo o no")]
    GameObject jugador;
    Text textoFinPartida;
    GameObject fondoFinPartida;

    [Tooltip("Procura que sólo haya un objeto GameManager en la escena")]
    GameManager manager;

    [Tooltip("Checkpoint que sirve también como punto de guardado")]
    GameObject checkpoint;
    bool haAparecidoCheckpoint = false;

    private void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player");
        textoEnemigosRestantes = GameObject.Find("TextoEnemigos").GetComponent<Text>();
        textoFinPartida = GameObject.Find("FinJuego").GetComponent<Text>();
        fondoFinPartida = GameObject.Find("FondoNegro");
        checkpoint = GameObject.Find("Checkpoint");
        //checkpoint.GetComponentInChildren<ParticleSystem>().Stop();
        checkpoint.GetComponent<GuardarPartida>().enabled = false;
        BuscarEnemigosTrasRespawn();
    }

    private void Update()
    {
        if (!jugador.GetComponent<JugadorController>().estaMuerto)
        {
            BuscarEnemigosTrasRespawn();
            if (enemigosRestantes.Count == 0 && !haAparecidoCheckpoint) 
            {
                HacerAparecerCheckpoint();
                haAparecidoCheckpoint = true;
            }
        }
        else
        {
            fondoFinPartida.GetComponent<MeshRenderer>().enabled = true;
            textoFinPartida.text = "Has muerto. Presiona la tecla Espacio para revivir o Esc para cerrar el juego.";
            if (Input.GetButtonDown("Jump"))
            {
                fondoFinPartida.GetComponent<MeshRenderer>().enabled = false;
                RevivirJugador();

            }
            else if (Input.GetButtonDown("CerrarJuego"))
            {
                Application.Quit();
            }
        }

    }


    public static void CargarPartida()
    {

        int escena = PlayerPrefs.GetInt("Escena", 0);

        switch (escena)
        {
            case 0: //Título
                SceneManager.LoadScene("Tutorial");
                break;

            case 1: //Tutorial
                SceneManager.LoadScene("PrimeraBatalla");
                break;
            case 2:
                SceneManager.LoadScene("SegundaBatalla");
                break;
            case 3:
                SceneManager.LoadScene("Final");
                break;
        }

    }

    public static void CargarEscena(int escena)
    {
        SceneManager.LoadScene(escena);
    }

    void RevivirJugador()
    {
        Scene escena = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escena.name);
        BuscarEnemigosTrasRespawn();
    }

    void BuscarEnemigosTrasRespawn()
    {
        enemigosRestantes = GameObject.FindGameObjectsWithTag("Enemigo").ToList();
        textoEnemigosRestantes.text = "Enemigos restantes: " + enemigosRestantes.Count;
    }

    void HacerAparecerCheckpoint()
    {
        checkpoint.GetComponent<MeshRenderer>().enabled = true;
        checkpoint.GetComponent<CapsuleCollider>().enabled = true;
        checkpoint.GetComponent<GuardarPartida>().enabled = true;
        checkpoint.GetComponentInChildren<ParticleSystem>().Play();
        checkpoint.transform.Translate(0, 100, 0);
        
    }
}
