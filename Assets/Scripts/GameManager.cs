using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region Variables
    [Header("Control de enemigos restantes en el combate y el jugador")]
    public List<GameObject> enemigosRestantes;
    static Text textoEnemigosRestantes;


    [Tooltip("Monitorea el estado del jugador para determinar si está vivo o no")]
    GameObject jugador;
    Text textoFinPartida;
    [Tooltip("Primitiva que actúa como fondo translúcido para la pantalla de game over")]
    GameObject fondoFinPartida;
    public bool haIniciadoCombate = false;

    [Header("Control de respawn y el propio Manager")]
    [Tooltip("Checkpoint que sirve también como punto de guardado")]
    GameObject checkpoint;
    bool haAparecidoCheckpoint = false;

    [Tooltip("Procura que sólo haya un objeto GameManager en la escena")]
    GameManager manager;

    #endregion

    private void Start()
    {
        jugador = GameObject.FindGameObjectWithTag("Player");
        textoEnemigosRestantes = GameObject.Find("TextoEnemigos").GetComponent<Text>();
        textoFinPartida = GameObject.Find("FinJuego").GetComponent<Text>();
        fondoFinPartida = GameObject.Find("FondoNegro");
        checkpoint = GameObject.Find("Checkpoint");
        checkpoint.GetComponent<GuardarPartida>().enabled = false;
        BuscarEnemigos();
    }

    private void Update()
    {
        if (haIniciadoCombate)
        {

            if (!jugador.GetComponent<JugadorController>().estaMuerto)
            {
                
                if (enemigosRestantes.Count == 0 && !haAparecidoCheckpoint)
                {
                    HacerAparecerCheckpoint();
                    haAparecidoCheckpoint = true;
                }
            }
            else
            {
                fondoFinPartida.GetComponent<MeshRenderer>().enabled = true;
                textoFinPartida.text = "Has muerto. Presiona la tecla Espacio para revivir o R para cerrar el juego.";
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
        else
        {
            IniciarEmboscada();
        }

    }


    #region Cargado de escenas

    /// <summary>
    /// Permite cargar la partida a partir de PlayerPrefs si se cierra la sesión o si matan al personaje durante la misma. Si no hay Playerprefs, entonces se toma la escena 0 (el título) por defecto
    /// </summary>
    public static void CargarPartida()
    {

        int escena = PlayerPrefs.GetInt("Escena",0);
        Debug.Log(PlayerPrefs.GetInt("Escena"));
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

    /// <summary>
    /// Carga la escena seleccionada dentro de la build
    /// </summary>
    /// <param name="escena"></param>
    public static void IniciarPartida()
    {
        SceneManager.LoadScene("Tutorial");
    }

    #endregion

    #region Funciones de control de estado del combate

    /// <summary>
    /// Permite al jugador recargar la última escena en la que ha muerto, incluso si cierra el juego
    /// </summary>
    void RevivirJugador()
    {
        Scene escena = SceneManager.GetActiveScene();
        SceneManager.LoadScene(escena.name);
        BuscarEnemigos();
    }

    public void BuscarEnemigos()
    {
        enemigosRestantes = GameObject.FindGameObjectsWithTag("Enemigo").ToList();
        textoEnemigosRestantes.text = "Enemigos restantes: " + enemigosRestantes.Count;
    }

    public void EliminarTextoCombate()
    {
        textoEnemigosRestantes.text = "";
    }

    /// <summary>
    /// Una vez todos los enemigos han muerto, el GameManager hace aparecer el checkpoint del nivel correspondiente
    /// </summary>
    void HacerAparecerCheckpoint()
    {
        checkpoint.GetComponent<MeshRenderer>().enabled = true;
        checkpoint.GetComponent<CapsuleCollider>().enabled = true;
        checkpoint.GetComponent<GuardarPartida>().enabled = true;
        checkpoint.GetComponentInChildren<ParticleSystem>().Play();
        checkpoint.transform.Translate(0, 100, 0);

    }

    void IniciarEmboscada()
    {
        
        if (Input.GetButtonDown("Cancel") && !haIniciadoCombate)
        {
            textoFinPartida.text = "";
            fondoFinPartida.GetComponent<MeshRenderer>().enabled = false;
            List<GameObject> enemigos = GameObject.FindGameObjectsWithTag("Enemigo").ToList();
            foreach (GameObject enemy in enemigos)
            {
                enemy.GetComponent<IAEnemigo>().enabled = true;
            }
            List<GameObject> aliados = GameObject.FindGameObjectsWithTag("Aliado").ToList();
            foreach (GameObject ali in aliados)
            {
                ali.GetComponent<IAAliado>().enabled = true;
            }

            haIniciadoCombate = true;

        }
    }

    #endregion
}
