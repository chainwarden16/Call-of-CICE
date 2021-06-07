using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EfectoAtaqueEspecial : MonoBehaviour
{
    List<ParticleSystem> particulas;
    private const float dañoArea = 3f;

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
    /// Cuando los enemigos entren en el área, su velocidad será la mitad
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
    /// Al salir del área volverán a moverse con normalidad
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
    /// Mientras estén dentro del área recibirán daño paulatinamente por cada frame
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemigo")
        {
            if (other.gameObject.GetComponent<IAEnemigo>().vidaActual > 0)
            {
                other.gameObject.GetComponent<IAEnemigo>().RecibirDaño(Time.deltaTime*dañoArea);
            }
        }
    }

}
