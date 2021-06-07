using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecogerObjeto : MonoBehaviour
{

    private void FixedUpdate()
    {
        gameObject.GetComponent<Rigidbody>().AddTorque(0, 10, 0, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider collid)
    {
        if (collid.gameObject.tag == "Player")
        {
            JugadorController jc = collid.gameObject.GetComponent<JugadorController>();
            if (gameObject.tag == "PocionVida" && jc.vidaActual < jc.vidaMaxima)
            {
                if (jc.vidaActual > jc.vidaMaxima / 2)
                {
                    jc.vidaActual = jc.vidaMaxima;
                    jc.MostrarVidaActual();
                }
                else
                {

                    jc.vidaActual += jc.vidaMaxima / 2;
                    jc.MostrarVidaActual();

                }
            }
            else if (gameObject.tag == "PocionMana" && jc.manaActual < jc.manaMaximo)
            {
                jc.manaActual = jc.manaMaximo;
                jc.MostrarManaActual();
            }

            Destroy(gameObject);

        }

    }
}
