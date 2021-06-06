using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class IAEnemigo : MonoBehaviour
{

    public enum ComportamientoEnemigo
    {
        Patrulla,
        Persecucion,
        Ataque,
        FinPartida
    }

    public enum TipoEnemigo
    {
        Arquero,
        Espadachin
    }

    [Header("Estad�sticas del enemigo")]
    public float vidaActual;
    float velocidadMovimiento = 20f;
    float da�o;
    bool estaMuerto = false;
    const float multiplicadorDa�oColor = 2f;

    [Header("F�sicas del enemigo y corrector de deltaTime")]
    CharacterController charCon;
    const float correctorDeltaTime = 60f;

    [Header("Variables de comportamiento del enemigo")]
    public ComportamientoEnemigo comportamiento = ComportamientoEnemigo.Patrulla;
    [Tooltip("Esto determina su rango de persecuci�n y de ataque, as� como sus estad�sticas (da�o & vida)")]
    public TipoEnemigo tipo;
    float rangoAtaque;
    float rangoPersecucion = 25;
    [Tooltip("Define el tiempo que tiene que pasar entre ataque y ataque. Si no, la m�quina atacar�a al objetivo una vez por frame y lo matar�a al instante")]
    float tiempoRecargaAtaque;
    float contadorSiguienteAtaque = 0f;
    [Tooltip("Determina a qui�n est� atacando, por si debe cambiar el foco")]
    GameObject objetivoActual;
    float tiempoCambioDireccion = 1f;

    [Header("Efectos visuales sobre el enemigo")]
    ParticleSystem particulas;
    ParticleSystem.EmissionModule emision;


    void Start()
    {
        charCon = gameObject.GetComponent<CharacterController>();
        switch (tipo)
        {
            case TipoEnemigo.Arquero:

                vidaActual = 60f;
                da�o = 5f;
                rangoAtaque = 20f;
                tiempoRecargaAtaque = 1f;

                break;
            case TipoEnemigo.Espadachin:

                vidaActual = 120f;
                da�o = 10f;
                rangoAtaque = 5f;
                tiempoRecargaAtaque = 0.5f;

                break;
        }

        particulas = gameObject.GetComponent<ParticleSystem>();
        emision = particulas.emission;
    }

    void Update()
    {
        if (!estaMuerto)
        {
            Morir();
            BuscarJugadorOAliado();
            CambiarDireccion();

        }
    }

    /// <summary>
    /// Se llama desde JugadorController al dsparar y acertar a un enemigo. Resta vida al enemigo y lo vuelve m�s azulado para representar la carga de da�o actual
    /// </summary>
    /// <param name="da�o">El da�o entrante. Los aliados del jugador no golpean con la misma fuerza que �ste y la coloraci�n azulada cambia seg�n el da�o recibido</param>
    public void RecibirDa�o(float da�o)
    {
        vidaActual -= da�o;


        SkinnedMeshRenderer[] componentes = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer com in componentes)
        {
            Color32 col = com.material.color;
            if (col.r - da�o * multiplicadorDa�oColor > 30)

            {

                col = new Color32((byte)(col.r - da�o * multiplicadorDa�oColor), 255, 255, 255);
                com.GetComponent<SkinnedMeshRenderer>().material.color = col;

            }
            else
            {
                col = new Color32(0, 255, 255, 255);
                com.GetComponent<SkinnedMeshRenderer>().material.color = col;
            }
        }
        particulas.Play();
    }

    /// <summary>
    /// Destruye a un enemigo cuando su vida es 0, tras mostrar un efecto de part�culas
    /// </summary>
    void Morir()
    {
        if (vidaActual <= 0)
        {
            estaMuerto = true;
            //faltan las part�culas
            particulas.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            emision.rateOverTime = 40f;
            var configSisPart = particulas.main;
            configSisPart.startLifetime = 2f;
            configSisPart.duration = 2f;
            particulas.Play();
            gameObject.tag = "Untagged"; //deja de ser marcado como enemigo para que los aliados dejen de atacarlo y busquen a otro objetivo
            Destroy(gameObject, 2);
        }
    }

    void BuscarJugadorOAliado()
    {

        GameObject jugador = GameObject.FindGameObjectsWithTag("Player").FirstOrDefault();


        if (jugador == null)
        {
            comportamiento = ComportamientoEnemigo.FinPartida;
        }


        switch (comportamiento)
        {

            case ComportamientoEnemigo.Patrulla:

                //El enemigo se mueve en una direcci�n

                charCon.SimpleMove(transform.forward * Time.deltaTime * correctorDeltaTime * velocidadMovimiento);

                if (objetivoActual == null) //hay que buscar a alguien con quien luchar, siempre que est� en el rango de detecci�n
                {

                    float distancia = Vector3.Distance(jugador.transform.position, transform.position);
                    if (distancia < rangoPersecucion)
                    {
                        objetivoActual = jugador;
                        comportamiento = ComportamientoEnemigo.Persecucion;

                    }

                }
                else //si ya tiene un enemigo fijado, que entre en modo persecuci�n 
                {

                    comportamiento = ComportamientoEnemigo.Persecucion;

                }
                break;
            case ComportamientoEnemigo.Persecucion:

                if (objetivoActual != null)
                {

                    float distanciaObjetivo = Vector3.Distance(transform.position, objetivoActual.transform.position);

                    transform.LookAt(objetivoActual.transform.position);
                    charCon.SimpleMove(transform.forward * Time.deltaTime * correctorDeltaTime * velocidadMovimiento);

                    if (distanciaObjetivo > rangoPersecucion)
                    {
                        comportamiento = ComportamientoEnemigo.Patrulla;
                        objetivoActual = null;
                    }
                    else if (distanciaObjetivo <= rangoAtaque)
                    {
                        comportamiento = ComportamientoEnemigo.Ataque;
                    }

                }
                else
                {
                    comportamiento = ComportamientoEnemigo.Patrulla;
                }


                break;
            case ComportamientoEnemigo.Ataque:

                if (objetivoActual != null)
                {

                    transform.LookAt(objetivoActual.transform.position); //encara al enemigo y va a por �l
                    charCon.SimpleMove(transform.forward * Time.deltaTime * correctorDeltaTime * velocidadMovimiento);
                    float disObjetivoActual = Vector3.Distance(objetivoActual.transform.position, transform.position); //calcula la distancia a la que est� el objetivo

                    if (disObjetivoActual > rangoAtaque)
                    {
                        comportamiento = ComportamientoEnemigo.Persecucion; //si se ha salido del rango de ataque, lo perseguir�
                    }
                    else
                    {
                        Disparar(); //si no, atacar�

                    }


                }
                else
                {
                    comportamiento = ComportamientoEnemigo.Patrulla;
                }

                break;
            case ComportamientoEnemigo.FinPartida: //no hacen nada porque el jugador ha muerto y el juego se "detiene"
                break;
        }
    }

    void CambiarDireccion()
    {
        if (comportamiento == ComportamientoEnemigo.Patrulla) //se comprueba si el enemigo est� patrullando para decidir en qu� direcci�n se girar�
        {

            tiempoCambioDireccion -= Time.deltaTime;

            if (tiempoCambioDireccion < 0) //si el tiempo ha expirado, se mirar� en qu� direcci�n patrullar� esta vez

            {

                float rand = Random.Range(0, 4);
                switch (rand)
                {
                    case 0:
                        gameObject.transform.rotation = Quaternion.Euler(0, 90, 0);
                        break;
                    case 1:
                        gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);

                        break;
                    case 2:
                        gameObject.transform.rotation = Quaternion.Euler(0, 270, 0);
                        break;
                    default:
                        break; //en este caso, no se gira nada

                }

                tiempoCambioDireccion = 1f;

            }

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

            if (Physics.Raycast(rayo, out hit, rangoAtaque)) //S�lo golpear� lo primero que est� en su camino; si hay una pared, el proyectil no alcanzar� al objetivo
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

        if (hit.collider.tag == "Player") //en este caso, apunta al jugador
        {
            JugadorController golpeado = hit.collider.gameObject.GetComponent<JugadorController>();

            if (golpeado.vidaActual - da�o <= 0) //si el objetivo va a morir con este ataque, entonces deja de ser un objetivo en el siguiente frame y el GameManager deber� llamar a la pantalla de Game Over
            {
                objetivoActual = null;
                comportamiento = ComportamientoEnemigo.FinPartida;
            }

            golpeado.RecibirDa�o(da�o);

        }

    }

}


