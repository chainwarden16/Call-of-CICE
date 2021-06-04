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

    [Header("Estad�sticas del enemigo")]
    public float vidaActual;
    public float velocidadMovimiento = 9f;
    public float da�o;
    bool estaMuerto = false;
    const float multiplicadorDa�oColor = 2f;

    [Header("F�sicas del enemigo y corrector de deltaTime")]
    CharacterController charCon;
    const float correctorDeltaTime = 60f;

    [Header("Variables de comportamiento del enemigo")]
    public ComportamientoEnemigo comportamiento = ComportamientoEnemigo.Patrulla;
    [Tooltip("Esto determina su rango de persecuci�n y de ataque, as� como sus estad�sticas (da�o & vida)")]
    public TipoEnemigo tipo;
    public float rangoAtaque;
    public float rangoPersecucion;
    [Tooltip("Define el tiempo que tiene que pasar entre ataque y ataque. Si no, la m�quina atacar�a al objetivo una vez por frame y lo matar�a al instante")]
    public float tiempoRecargaAtaque;
    float tiempoCambioDireccion = 1f;
    [Tooltip("Determina a qui�n est� atacando, por si debe cambiar el foco")]
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
        Debug.Log("La vida del enemigo es: " + vidaActual);
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

                //El enemigo se mueve en una direcci�n

                charCon.SimpleMove(transform.forward * Time.deltaTime * correctorDeltaTime * velocidadMovimiento);

                if (objetivoActual == null) //hay que buscar a alguien con quien luchar, siempre que est� en el rango de detecci�n
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
                else //si ya tiene un enemigo fijado, que entre en modo persecuci�n 
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

}


