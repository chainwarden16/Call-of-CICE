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
        Ataque
    }

    public enum TipoEnemigo
    {
        Arquero,
        Espadachin
    }

    [Header("Estadísticas del enemigo")]
    public float vidaActual;
    public float velocidadMovimiento = 9f;
    public float daño;
    bool estaMuerto = false;
    const float multiplicadorDañoColor = 2f;

    [Header("Físicas del enemigo y corrector de deltaTime")]
    CharacterController charCon;
    const float correctorDeltaTime = 60f;

    [Header("Variables de comportamiento del enemigo")]
    public ComportamientoEnemigo comportamiento = ComportamientoEnemigo.Patrulla;
    [Tooltip("Esto determina su rango de persecución y de ataque, así como sus estadísticas (daño & vida)")]
    public TipoEnemigo tipo;
    public float rangoAtaque;
    public float rangoPersecucion;
    [Tooltip("Define el tiempo que tiene que pasar entre ataque y ataque. Si no, la máquina atacaría al objetivo una vez por frame y lo mataría al instante")]
    public float tiempoRecargaAtaque;
    float tiempoCambioDireccion = 1f;
    [Tooltip("Determina a quién está atacando, por si debe cambiar el foco")]
    GameObject objetivoActual;


    // Start is called before the first frame update
    void Start()
    {
        charCon = gameObject.GetComponent<CharacterController>();
    }

    // Update is called once per frame
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
    /// Se llama desde JugadorController al dsparar y acertar a un enemigo. Resta vida al enemigo y lo vuelve más azulado para representar la carga de daño actual
    /// </summary>
    /// <param name="daño">El daño entrante. Los aliados del jugador no golpean con la misma fuerza que éste y la coloración azulada cambia según el daño recibido</param>
    public void RecibirDaño(float daño)
    {
        vidaActual -= daño;


        SkinnedMeshRenderer[] componentes = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer com in componentes)
        {
            Color32 col = com.material.color;
            if (col.r - daño * multiplicadorDañoColor > 30)

            {

                col = new Color32((byte)(col.r - daño * multiplicadorDañoColor), 255, 255, 255);
                com.GetComponent<SkinnedMeshRenderer>().material.color = col;

            }
            else
            {
                col = new Color32(0, 255, 255, 255);
                com.GetComponent<SkinnedMeshRenderer>().material.color = col;
            }
        }
        Debug.Log("La vida del enemigo es: " + vidaActual);
    }

    /// <summary>
    /// Destruye a un enemigo cuando su vida es 0, tras mostrar un efecto de partículas
    /// </summary>
    void Morir()
    {
        if (vidaActual <= 0)
        {
            estaMuerto = true;
            //faltan las partículas
            Destroy(gameObject, 3);
        }
    }

    void BuscarJugadorOAliado()
    {
        List<GameObject> oponentes = GameObject.FindGameObjectsWithTag("Aliado").ToList();
        oponentes.Add(GameObject.FindGameObjectsWithTag("Player").First());

        switch (comportamiento)
        {

            case ComportamientoEnemigo.Patrulla:

                //El enemigo se mueve en una dirección

                charCon.SimpleMove(transform.forward * Time.deltaTime * correctorDeltaTime * velocidadMovimiento);

                if (objetivoActual == null) //hay que buscar a alguien con quien luchar, siempre que esté en el rango de detección
                {
                    foreach (GameObject gam in oponentes)
                    {
                        float distancia = Vector3.Distance(gam.transform.position, transform.position);
                        if (distancia < rangoPersecucion)
                        {
                            objetivoActual = gam;
                            comportamiento = ComportamientoEnemigo.Persecucion;
                            Debug.Log("El objetivo actual es: " + objetivoActual);
                        }
                    }
                }
                else //si ya tiene un enemigo fijado, que entre en modo persecución 
                {

                    comportamiento = ComportamientoEnemigo.Persecucion;

                }
                break;
            case ComportamientoEnemigo.Persecucion:

                float distanciaObjetivo = Vector3.Distance(transform.position, objetivoActual.transform.position);

                transform.LookAt(objetivoActual.transform.position);
                charCon.SimpleMove(transform.forward * Time.deltaTime * correctorDeltaTime * velocidadMovimiento);

                if (distanciaObjetivo > rangoPersecucion)
                {
                    comportamiento = ComportamientoEnemigo.Patrulla;
                    objetivoActual = null;
                }

                break;
            case ComportamientoEnemigo.Ataque:
                break;
        }
    }

    void CambiarDireccion()
    {
        if (comportamiento == ComportamientoEnemigo.Patrulla) //se comprueba si el enemigo está patrullando para decidir en qué dirección se girará
        {

            tiempoCambioDireccion -= Time.deltaTime;

            if (tiempoCambioDireccion < 0) //si el tiempo ha expirado, se mirará en qué dirección patrullará esta vez

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

}


