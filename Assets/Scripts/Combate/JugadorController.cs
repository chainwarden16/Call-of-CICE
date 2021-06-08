using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class JugadorController : MonoBehaviour
{
    [Header("Variables para controlar el movimiento")]
    CharacterController cc;
    float moviX;
    float moviZ;
    float velocidadMov = 10.0f;
    float correctorDelta = 60f;
    Vector3 vectorVelocidadPersonaje;

    [Header("Vida y munici�n del personaje")]
    [Tooltip("La vida y el man� actuales y m�ximos se usar�n en otra clase, as� que deben ser p�blicos")]
    public float vidaActual;
    public float vidaMaxima = 100f;
    public float manaActual;
    public float manaMaximo = 100f;
    float costeDisparo = 1f;
    [Tooltip("El da�o que causa a un enemigo cuando lo dispara")]
    float da�oDisparo = 10f;
    Text marcadorVida;
    Text marcadorMana;

    [Header("Variables para Raycast y el disparo")]
    float rango = 25f;

    [Header("Control de respawn")]
    [Tooltip("Si es true, deber� mostrarse una pantalla indic�ndolo y la posibilidad de cargar partida desde el GameManager")]
    public bool estaMuerto = false;

    [Header("Variables del ataque especial")]
    float costeAtaqueEspecial = 20f;
    [Tooltip("Indica el tiempo de enfriamiento del ataque especial")]
    float recargaAtaqueEspecial = 10f;
    [Tooltip("Indica cu�nto falta para ser capaz de usar el ataque especial de nuevo")]
    float tiempoEnfriamientoRestante = 0f;
    [Tooltip("El objeto que contiene el prefab con las part�culas, c�digo y collider del ataque especial")]
    public GameObject ataqueEspecial;
    [Tooltip("El objeto a crear y destruir en escena durante el invocado y expiraci�n del ataque especial. Instancia al prefab ya cargado y as� se evita tener que acceder a los recursos del juego cada vez que se use")]
    GameObject ataqueEspecialEnEscena;
    [Tooltip("Determina la posici�n en la que aparecer�")]
    static GameObject piesJugador;
    [Tooltip("Calcula el tiempo que le queda al efecto. No es lo mismo que el tiempo de enfriamiento (mayor que el de la duracion)")]
    float temporizadorDuracion = 4f;
    [Tooltip("Calcula el tiempo que perdura el efecto")]
    float tiempoDuracionEfecto = 4f;
    [Tooltip("El offset de la altura del jugador para la aparici�n del ataque especial")]
    const float offsetAltura = 1.12f;

    [Header("Efectos visuales sobre el jugador")]
    ParticleSystem particulas;

    [Header("Game Manager de la escena")]
    GameManager manager;

    // Start is called before the first frame update
    void Start()
    {
        cc = gameObject.GetComponent<CharacterController>();
        marcadorMana = GameObject.Find("Mana").GetComponent<Text>();
        marcadorVida = GameObject.Find("Vida").GetComponent<Text>();
        particulas = GetComponent<ParticleSystem>();
        vidaActual = PlayerPrefs.GetFloat("Vida", vidaMaxima);
        manaActual = PlayerPrefs.GetFloat("Mana", manaMaximo);
        piesJugador = GameObject.Find("PiesJugador");
        manager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        MostrarManaActual();
        MostrarVidaActual();
    }

    // Update is called once per frame
    void Update()
    {
        if (manager.haIniciadoCombate)
        {
            if (!estaMuerto)

            {

                    //Control de la vida
                    Morir();

                //Control de movimiento del personaje. Prefiero usar GetAxisRaw para detener al personaje en seco en cuanto el jugador deje de pulsar el bot�n
                moviX = Input.GetAxisRaw("Horizontal") * Time.deltaTime * correctorDelta;
                moviX = Input.GetAxisRaw("Horizontal") * Time.deltaTime * correctorDelta;
                moviZ = Input.GetAxisRaw("Vertical") * Time.deltaTime * correctorDelta;

                vectorVelocidadPersonaje = moviX * transform.right + transform.forward * moviZ;

                vectorVelocidadPersonaje = vectorVelocidadPersonaje.normalized * velocidadMov;

                cc.SimpleMove(vectorVelocidadPersonaje);

                //Control de disparo, el man� y el ataque especial
                Disparar();
                UsarAtaqueEspecial();

            }

        }
    }

    /// <summary>
    /// Dispara una bala, que consume man� y hace da�o al enemigo, suponiendo que no haya otro obst�culo en el camino
    /// </summary>
    void Disparar()
    {
        Vector3 direccion = transform.forward;

        direccion = direccion.normalized;

        Ray rayo = new Ray(transform.position, direccion); //el personaje disparar� en direcci�n recta desde donde est� mirando en ese momento

        RaycastHit hit;

        if (Input.GetButtonDown("Fire1") && manaActual - costeDisparo >= 0f)
        {
            //Se dispara el proyectil. Aunque no haya un enemigo a tiro, debe poderse disarar y gastar la munici�n

            manaActual -= costeDisparo;
            MostrarManaActual();

            if (Physics.Raycast(rayo, out hit, rango)) //S�lo golpear� lo primero que est� en su camino; si hay una pared, el proyectil no alcanzar� al enemigo
            {
                //Se mira si se apunta a un enemigo mediante Raycast
                if (hit.collider.tag == "Enemigo")
                {

                    IAEnemigo golpeado = hit.collider.gameObject.GetComponent<IAEnemigo>();

                    golpeado.RecibirDa�o(da�oDisparo);


                }


            }
            Debug.Log("Man� actual: " + manaActual);
        }
    }

    /// <summary>
    /// Permite al jugador usar su ataque especial, un �rea de efecto que hace da�o a los enemigos mientras est�n sobre ella y los ralentiza. Dura 4 segundos y es invocado alrededor del personaje
    /// Para poder utilizarlo, es necesario que tenga man� suficiente y que el tiempo de enfriamiento haya expirado
    /// </summary>
    void UsarAtaqueEspecial()
    {

            Debug.Log(cc.isGrounded);
        if (tiempoEnfriamientoRestante <= 0) //debe haber concluido el tiempo de enfriamiento. No puedo colocarlo en el if anterior porque necesito descontar el tiempo �nicamente si no ha expirado ya, independientemente de las condiciones anteriores
        {
            if (Input.GetButtonDown("Fire2") && manaActual - costeAtaqueEspecial >= 0 && cc.isGrounded) //el jugador deber� presionar el bot�n secundario del rat�n, tener man� suficiente y estar en el suelo para activar este efecto, pero adem�s...
            {
                piesJugador.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - offsetAltura, gameObject.transform.position.z);
                ataqueEspecialEnEscena = Instantiate(ataqueEspecial, piesJugador.transform.position, Quaternion.identity);
                tiempoEnfriamientoRestante = recargaAtaqueEspecial;
                manaActual -= costeAtaqueEspecial;
                MostrarManaActual();

            }
        }
        else
        {
            tiempoEnfriamientoRestante -= Time.deltaTime;

            if (ataqueEspecialEnEscena != null) //si el ataque especial todav�a perdura, descuenta
            {
                DeterminarConcluirAtaqueEspecial();
            }
        }


    }

    /// <summary>
    /// Si el jugador pierde toda la vida, morir�, perder� control del personaje y aparecer� el mensaje de fin de partida
    /// </summary>
    void Morir()
    {
        if (vidaActual <= 0)
        {
            estaMuerto = true;
            GameObject.Find("Main Camera").GetComponent<CamaraController>().enabled = false; //se desactiva la c�mara para quitarle el control total al jugador
            gameObject.tag = "Untagged";

            //Se llama a la pantalla de fin de partida desde el GameManager
        }
    }

    /// <summary>
    /// cada vez que un enemigo dispare hacia el jugador y no haya un obst�culo entre ellos, el personaje perder� vida en funci�n del da�o del oponente, actualizar� la IU y adem�s mostrar� el efecto con part�culas rojas
    /// </summary>
    /// <param name="da�o"></param>
    public void RecibirDa�o(float da�o)
    {
        float vidaRestante = vidaActual - da�o;
        if (vidaRestante < 0)
        {
            vidaActual = 0;
        }
        else
        {
            vidaActual = vidaRestante;
        }


        particulas.Play();
        MostrarVidaActual();
    }

    /// <summary>
    /// Actualiza la Interfaz de usuario mostrando la vida que posee
    /// </summary>
    public void MostrarVidaActual()
    {
        marcadorVida.text = "Vida: " + vidaActual.ToString() + "/" + vidaMaxima;
    }

    /// <summary>
    /// Edita el texto en pantalla para visualizar cu�nta munici�n (man�) tiene el personaje actualmente
    /// </summary>
    public void MostrarManaActual()
    {
        marcadorMana.text = "Man�: " + manaActual.ToString() + "/" + manaMaximo;
    }

    /// <summary>
    /// Decide si el ataque especial en escena ha expirado y deber�a ser destruido. Usa un temporizador aparte del de enfriamiento
    /// </summary>
    void DeterminarConcluirAtaqueEspecial()
    {
        if (temporizadorDuracion <= 0)
        {
            temporizadorDuracion = tiempoDuracionEfecto; //se resetea el contador para la pr�xima vez que se use
            List<GameObject> enemigosVivos = manager.enemigosRestantes.ToList();
            foreach(GameObject enemy in enemigosVivos)
            {
                enemy.GetComponent<IAEnemigo>().velocidadActual = enemy.GetComponent<IAEnemigo>().velocidadMovimiento;
            }
            Destroy(ataqueEspecialEnEscena);
        }
        else
        {
            temporizadorDuracion -= Time.deltaTime;
        }

    }



}
