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
    [Tooltip("La vida y el man� actuales se usar�n en otra clase, as� que deben ser p�blicos")]
    public float vidaActual = 100f;
    float vidaMaxima = 100f;
    public float manaActual = 20f;
    float manaMaximo = 20f;
    float costeDisparo = 1f;
    float costeAtaqueEspecial = 5f;
    [Tooltip("El da�o que causa a un enemigo cuando lo dispara")]
    float da�oDisparo = 10f;
    Text marcadorVida;
    Text marcadorMana;

    [Header("Variables para Raycast y el disparo")]
    float rango = 25f;

    [Header("Control de respawn")]
    [Tooltip("Si es true, deber� mostrarse una pantalla indic�ndolo y la posibilidad de cargar partida desde el GameManager")]
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
        IniciarEmboscada();
        if (!estaMuerto)

        {
            //Control de la vida
            Morir();

            //Control de movimiento del personaje. Prefiero usar GetAxisRaw para detener al personaje en seco en cuanto el jugador deje de pulsar el bot�n
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
                if(hit.collider.tag == "Enemigo")
                {

                    IAEnemigo golpeado = hit.collider.gameObject.GetComponent<IAEnemigo>();

                    golpeado.RecibirDa�o(da�oDisparo);


                }


            }
            Debug.Log("Man� actual: " + manaActual);
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
            GameObject.Find("Main Camera").GetComponent<CamaraController>().enabled = false; //se desactiva la c�mara para quitarle el control total al jugador
            gameObject.tag = "Untagged";
            //Se llama a la pantalla de fin de partida desde el GameManager
        }
    }

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
        
        MostrarVidaActual();
    }

    void MostrarVidaActual()
    {
        marcadorVida.text = "Vida: "+vidaActual.ToString()+"/"+vidaMaxima;
    }

    void MostrarManaActual()
    {
        marcadorMana.text = "Man�: "+manaActual.ToString()+"/"+manaMaximo;
    }

    void IniciarEmboscada()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            List<GameObject> enemigos = GameObject.FindGameObjectsWithTag("Enemigo").ToList();
            foreach(GameObject enemy in enemigos)
            {
                enemy.GetComponent<IAEnemigo>().enabled = true;
            }
            List<GameObject> aliados = GameObject.FindGameObjectsWithTag("Aliado").ToList();
            foreach(GameObject ali in aliados)
            {
                ali.GetComponent<IAAliado>().enabled = true;
            }
            
        }
    }

}
