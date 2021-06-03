using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JugadorController : MonoBehaviour
{
    [Header("Variables para controlar el movimiento")]
    CharacterController cc;
    float moviX;
    float moviZ;
    float velocidadMov = 10.0f;
    float correctorDelta = 60f;
    Vector3 vectorVelocidadPersonaje;

    [Header("Vida y munición del personaje")]
    [Tooltip("La vida y el maná actuales se usarán en otra clase, así que deben ser públicos")]
    public float vidaActual = 100f;
    float vidaMaxima = 100f;
    public float manaActual = 20f;
    float manaMaximo = 20f;
    float costeDisparo = 1f;
    float costeAtaqueEspecial = 5f;
    [Tooltip("El daño que causa a un enemigo cuando lo dispara")]
    float dañoDisparo = 10f;
    Text marcadorVida;
    Text marcadorMana;

    [Header("Variables para Raycast y el disparo")]
    float rango = 30f;

    [Header("Control de respawn")]
    [Tooltip("Si es true, deberá mostrarse una pantalla indicándolo y la posibilidad de cargar partida desde el GameManager")]
    public bool estaMuerto = false;


    // Start is called before the first frame update
    void Start()
    {
        cc = gameObject.GetComponent<CharacterController>();
        marcadorMana = GameObject.Find("Mana").GetComponent<Text>();
        marcadorVida = GameObject.Find("Vida").GetComponent<Text>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!estaMuerto)

        {

            //Control de movimiento del personaje. Prefiero usar GetAxisRaw para detener al personaje en seco en cuanto el jugador deje de pulsar el botón
            moviX = Input.GetAxisRaw("Horizontal") * Time.deltaTime * correctorDelta;
            moviZ = Input.GetAxisRaw("Vertical") * Time.deltaTime * correctorDelta;

            vectorVelocidadPersonaje = moviX * transform.right + transform.forward * moviZ;

            vectorVelocidadPersonaje = vectorVelocidadPersonaje.normalized * velocidadMov;

            cc.SimpleMove(vectorVelocidadPersonaje);

            //Control de disparo, el maná y el ataque especial
            Disparar();
            UsarAtaqueEspecial();
            //Control de la vida
            Morir();

        }


    }

    void Disparar()
    {
        Vector3 direccion = transform.forward;

        direccion = direccion.normalized;

        Ray rayo = new Ray(transform.position, direccion); //el personaje disparará en dirección recta desde donde esté mirando en ese momento

        RaycastHit hit;

        if (Input.GetButtonDown("Fire1") && manaActual - costeDisparo >= 0f)
        {
            //Se dispara el proyectil. Aunque no haya un enemigo a tiro, debe poderse disarar y gastar la munición

            manaActual -= costeDisparo;
            MostrarManaActual();

            if (Physics.Raycast(rayo, out hit, rango)) //Sólo golpeará lo primero que esté en su camino; si hay una pared, el proyectil no alcanzará al enemigo
            {
                 //Se mira si se apunta a un enemigo mediante Raycast
                if(hit.collider.tag == "Enemigo")
                {

                    IAEnemigo golpeado = hit.collider.gameObject.GetComponent<IAEnemigo>();

                    golpeado.RecibirDaño(dañoDisparo);


                }


            }
            Debug.Log("Maná actual: " + manaActual);
        }
    }

    void UsarAtaqueEspecial()
    {
        if (Input.GetButtonDown("Fire2") && manaActual - costeAtaqueEspecial >= 0)
        {
            //Se mira si se apunta a un enemigo mediante Raycast


        }
    }

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
        MostrarVidaActual();
    }

    void MostrarVidaActual()
    {
        marcadorVida.text = "Vida: "+vidaActual.ToString()+"/"+vidaMaxima;
    }

    void MostrarManaActual()
    {
        marcadorMana.text = "Maná: "+manaActual.ToString()+"/"+manaMaximo;
    }

}
