using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EfectoAtaqueEspecial : MonoBehaviour
{
    List<ParticleSystem> particulas;
    private const float da�oArea = 3f;

    private void Start()
    {
        particulas = GetComponentsInChildren<ParticleSystem>().ToList();

    }

    private void Update()
    {
        foreach (ParticleSystem part in particulas)
        {
            part.transform.Rotate(0, 2, 0);
        }
    }

    /// <summary>
    /// Cuando los enemigos entren en el �rea, su velocidad ser� la mitad
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemigo")
        {
            other.gameObject.GetComponent<IAEnemigo>().velocidadActual -= other.gameObject.GetComponent<IAEnemigo>().velocidadActual / 2;
        }
    }

    /// <summary>
    /// Al salir del �rea volver�n a moverse con normalidad
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemigo")
        {
            other.gameObject.GetComponent<IAEnemigo>().velocidadActual = other.gameObject.GetComponent<IAEnemigo>().velocidadMovimiento;
        }
    }

    /// <summary>
    /// Mientras est�n dentro del �rea recibir�n da�o paulatinamente por cada frame
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemigo")
        {
            if (other.gameObject.GetComponent<IAEnemigo>().vidaActual > 0)
            {
                other.gameObject.GetComponent<IAEnemigo>().RecibirDa�o(Time.deltaTime*da�oArea);
            }
        }
    }

}
