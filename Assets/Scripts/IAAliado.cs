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

    [Header("Estad�sticas del aliado")]
    [Tooltip("El da�o que causa a un enemigo cuando dispara. Ser� menor que el del jugador")]
    float da�oDisparo = 9f;
    [Tooltip("Atacan m�s lento que �l, pero tendr�n munici�n infinita")]
    float tiempoRecargaAtaque = 1f;

    [Header("F�sicas del aliado y correcci�n de deltaTime")]
    CharacterController cCon;

    [Header("Variables para Raycast y el disparo")]
    float rango = 350f;
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

            IAEnemigo golpeado = hit.collider.gameObject.GetComponent<IAEnemigo>();

            if (golpeado.vidaActual - da�oDisparo <= 0) //si el objetivo va a morir con este ataque, entonces deja de ser un objetivo en el siguiente frame y deber� buscar otro, de haberlo
            {
                objetivoActual = null;
                comportamiento = TipoComportamiento.Busqueda;
            }

            golpeado.RecibirDa�o(da�oDisparo);
        }
        
    }

}
