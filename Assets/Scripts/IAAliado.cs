using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IAAliado : MonoBehaviour
{
    enum TipoComportamiento
    {
        Busqueda,
        Ataque,
        FinPartida
    }

    [Header("Vida y estad�sticas del aliado")]
    [Tooltip("La vida actual se usar� en otra clase, as� que deben ser p�blicos")]
    public float vidaActual = 70f;
    [Tooltip("El da�o que causa a un enemigo cuando dispara. Ser� menor que el del jugador")]
    float da�oDisparo = 7f;
    [Tooltip("Atacan m�s lento que �l, pero tendr�n munici�n infinita")]
    float tiempoRecargaAtaque = 1.5f;
    float velocidadMovimiento = 20f;

    [Header("F�sicas del aliado y correcci�n de deltaTime")]
    CharacterController cCon;
    const float correctorDeltaTime = 60f;

    [Header("Variables para Raycast y el disparo")]
    float rango = 14f;
    [Tooltip("Define el tiempo que tiene que pasar entre ataque y ataque. Si no, la m�quina atacar�a al objetivo una vez por frame y lo matar�a al instante")]
    float contadorSiguienteAtaque = 0f;
    [Tooltip("Determina a qui�n est� atacando, por si debe cambiar el foco")]
    GameObject objetivoActual;

    [Header("Comprobaciones del estado del personaje")]
    [Tooltip("Los aliados s�lo hacen tres cosas: buscar el enemigo m�s cercano, atacarlo hasta que o bien el enemigo muera, o el propio aliado lo haga y quedarse quieto cuando no quedan enemigos (o el jugador muere).")]
    TipoComportamiento comportamiento = TipoComportamiento.Busqueda;


    [Tooltip("Si es true, deber� ser destruido")]
    public bool estaMuerto = false;

    private void Start()
    {
        cCon = gameObject.GetComponent<CharacterController>();
    }

    private void Update()
    {
        Morir();
        BuscarEnemigos();
    }

    void BuscarEnemigos()
    {
        List<GameObject> enemigos = GameObject.FindGameObjectsWithTag("Enemigo").ToList();

        GameObject jugador = GameObject.FindGameObjectsWithTag("Player").FirstOrDefault();

        if (enemigos.Count == 0 || jugador == null) //si no hay enemigos o el jugador ha muerto, los aliados se detienen
        {
            comportamiento = TipoComportamiento.FinPartida;
        }

        switch (comportamiento)
        {

            case TipoComportamiento.Busqueda:


                if (objetivoActual == null) //hay que buscar a alguien con quien luchar
                {

                    int aleatorio = Random.Range(0, enemigos.Count - 1);
                    objetivoActual = enemigos[aleatorio];
                    Debug.Log("El objetivo del aliado es: " + objetivoActual.name);

                }
                else //si ya tiene un enemigo fijado, que entre en modo persecuci�n 
                {
                    transform.LookAt(objetivoActual.transform.position);
                    cCon.SimpleMove(transform.forward * Time.deltaTime * correctorDeltaTime * velocidadMovimiento);
                    float distancia = Vector3.Distance(transform.position, objetivoActual.transform.position);
                    if (distancia < rango)
                    {

                        comportamiento = TipoComportamiento.Ataque;

                    }

                }
                break;
            case TipoComportamiento.Ataque:

                if (objetivoActual != null)
                {

                    Debug.Log("El objetivo del aliado ahora es: " + objetivoActual.name);
                    transform.LookAt(objetivoActual.transform.position); //encara al enemigo y va a por �l
                    cCon.SimpleMove(-transform.forward * Time.deltaTime * correctorDeltaTime * velocidadMovimiento); //trata de mantener la distancia con el enemigo mientras ataca
                    float disObjetivoActual = Vector3.Distance(transform.position, objetivoActual.transform.position); //calcula la distancia a la que est� el objetivo

                    if (disObjetivoActual > rango)
                    {
                        comportamiento = TipoComportamiento.Busqueda; //si se ha salido del rango de ataque, lo perseguir�
                    }
                    else
                    {
                        Disparar(); //si no, atacar�

                    }


                }
                else
                {
                    comportamiento = TipoComportamiento.Busqueda;
                }

                break;
            case TipoComportamiento.FinPartida: //no hacen nada porque el jugador ha muerto, o no quedan enemigos, y el juego se "detiene"
                break;
        }
    }



    void Morir()
    {
        if (vidaActual <= 0)
        {
            estaMuerto = true;
            //Faltan las part�culas
            Destroy(gameObject);
        }
    }

    public void RecibirDa�o(float da�o)
    {
        vidaActual -= da�o;
        Debug.Log(vidaActual);
    }

    void Disparar()
    {
        if (contadorSiguienteAtaque <= 0) //si es momento de disparar, se calcula a qu� est� apuntando el enemigo (en este caso, ser� lo que tenga enfrente dentro de su rango)
        {

            Vector3 direccion = transform.forward;

            direccion = direccion.normalized;

            Ray rayo = new Ray(transform.position, direccion); //el npc atacar� en direcci�n recta desde donde est� mirando en ese momento

            RaycastHit hit;

            if (Physics.Raycast(rayo, out hit, rango)) //S�lo golpear� lo primero que est� en su camino; si hay una pared, el proyectil no alcanzar� al objetivo
            {
                //Se mira si se apunta a un aliado mediante Raycast
                ComprobarSiYaEstaMuerto(hit);

            }

            contadorSiguienteAtaque = tiempoRecargaAtaque; //ahora deber� esperar a que pase un tiempo para volver a atacar. Los enemigos a mel� tienen un tiempo de recarga menor y hacen m�s da�o

        }
        else
        {
            contadorSiguienteAtaque -= Time.deltaTime;
        }
    }

    void ComprobarSiYaEstaMuerto(RaycastHit hit)
    {

        if (hit.collider.tag == "Enemigo") //si apunta a un enemigo, le hace da�o
        {

            IAAliado golpeado = hit.collider.gameObject.GetComponent<IAAliado>();

            if (golpeado.vidaActual - da�oDisparo <= 0) //si el objetivo va a morir con este ataque, entonces deja de ser un objetivo en el siguiente frame y deber� buscar otro, de haberlo
            {
                objetivoActual = null;
                comportamiento = TipoComportamiento.Busqueda;
            }

            golpeado.RecibirDa�o(da�oDisparo);
        }
        
    }

}
