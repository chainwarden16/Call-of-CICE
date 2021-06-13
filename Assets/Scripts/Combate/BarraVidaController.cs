using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarraVidaController : MonoBehaviour
{
    #region Variables
    IAEnemigo enemigo;
    Transform escalaInicial;
    Transform escalaActual;

    #endregion
    void Start()
    {
        enemigo = gameObject.GetComponentInParent<IAEnemigo>();
        escalaInicial = gameObject.transform;
        escalaActual = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemigo.vidaActual > 0 && enemigo.vidaMaxima > 0)
        {
            escalaActual.localScale = new Vector3(enemigo.vidaActual / enemigo.vidaMaxima , escalaActual.localScale.y, escalaActual.localScale.z);

        }
        else
        {
            escalaActual.localScale = new Vector3(0, escalaActual.localScale.y, escalaActual.localScale.z);
        }
    }
}
