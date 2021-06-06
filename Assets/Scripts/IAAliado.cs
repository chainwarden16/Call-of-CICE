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

    [Header("Estadísticas del aliado")]
    [Tooltip("El daño que causa a un enemigo cuando dispara. Será menor que el del jugador")]
    float dañoDisparo = 9f;
    [Tooltip("Atacan más lento que él, pero tendrán munición infinita")]
    float tiempoRecargaAtaque = 1f;

    [Header("Físicas del aliado y corrección de deltaTime")]
    CharacterController cCon;

    [Header("Variables para Raycast y el disparo")]
    float rango = 350f;
    [Tooltip("Define el tiempo que tiene que pasar entre ataque y ataque. Si no, la máquina atacaría al objetivo una vez por frame y lo mataría al instante")]
    float contadorSiguienteAtaque = 0f;
    [Tooltip("Determina a quién está atacando, por si debe cambiar el foco")]
    GameObject objetivoActual;

    [Header("Comprobaciones del estado del personaje")]
    [Tooltip("Los aliados sólo hacen tres cosas: buscar el enemigo más cercano, atacarlo hasta que o bien el enemigo muera, o el propio aliado lo haga y quedarse quieto cuando no quedan enemigos (o el jugador muere).")]
    TipoComportamiento comportamiento = TipoComportamiento.Busqueda;


    [Tooltip("Si es true, deberá ser destruido")]
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
                else //si ya tiene un enemigo fijado, que entre en modo persecución 
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
                    transform.LookAt(objetivoActual.transform.position); //encara al enemigo y va a por él
                    
                    float disObjetivoActual = Vector3.Distance(transform.position, objetivoActual.transform.position); //calcula la distancia a la que está el objetivo

                    if (disObjetivoActual > rango)
                    {
                        comportamiento = TipoComportamiento.Busqueda; //si se ha salido del rango de ataque, lo perseguirá
                    }
                    else
                    {
                        Disparar(); //si no, atacará

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
        if (contadorSiguienteAtaque <= 0) //si es momento de disparar, se calcula a qué está apuntando el enemigo (en este caso, será lo que tenga enfrente dentro de su rango)
        {

            Vector3 direccion = transform.forward;

            direccion = direccion.normalized;

            Ray rayo = new Ray(transform.position, direccion); //el npc atacará en dirección recta desde donde esté mirando en ese momento

            RaycastHit hit;

            if (Physics.Raycast(rayo, out hit, rango)) //Sólo golpeará lo primero que esté en su camino; si hay una pared, el proyectil no alcanzará al objetivo
            {
                //Se mira si se apunta a un aliado mediante Raycast
                ComprobarSiYaEstaMuerto(hit);

            }

            contadorSiguienteAtaque = tiempoRecargaAtaque; //ahora deberá esperar a que pase un tiempo para volver a atacar. Los enemigos a melé tienen un tiempo de recarga menor y hacen más daño

        }
        else
        {
            contadorSiguienteAtaque -= Time.deltaTime;
        }
    }

    void ComprobarSiYaEstaMuerto(RaycastHit hit)
    {

        if (hit.collider.tag == "Enemigo") //si apunta a un enemigo, le hace daño
        {

            IAEnemigo golpeado = hit.collider.gameObject.GetComponent<IAEnemigo>();

            if (golpeado.vidaActual - dañoDisparo <= 0) //si el objetivo va a morir con este ataque, entonces deja de ser un objetivo en el siguiente frame y deberá buscar otro, de haberlo
            {
                objetivoActual = null;
                comportamiento = TipoComportamiento.Busqueda;
            }

            golpeado.RecibirDaño(dañoDisparo);
        }
        
    }

}
