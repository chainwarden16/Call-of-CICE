using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraController : MonoBehaviour
{
    // Start is called before the first frame update
    [Tooltip("Toma el transform del padre (el jugador) para limitar el giro")]
    Transform transJugador;
    float moviX;
    float moviY;
    float velGiro = 15f;
    float correctorDeltaTime = 60f;
    float minimoClamp = -90f;
    float maximoClamp = 90f;

    Vector3 controlGiro;

    void Start()
    {
        transJugador = GameObject.Find("Jugador").transform;

        controlGiro = transJugador.eulerAngles;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        moviY = Input.GetAxis("Mouse X") * Time.deltaTime * correctorDeltaTime * velGiro;
        moviX = Input.GetAxis("Mouse Y") * Time.deltaTime * correctorDeltaTime * velGiro;

        if (moviY != 0) {
            
            transJugador.rotation = Quaternion.Euler(transJugador.rotation.eulerAngles.x, transJugador.rotation.eulerAngles.y+moviY, transJugador.rotation.eulerAngles.z);
        
        }
        if (moviX != 0)
        {
            controlGiro.x -= moviX;
            controlGiro.x = Mathf.Clamp(controlGiro.x, minimoClamp, maximoClamp);
            transJugador.localRotation = Quaternion.Euler(controlGiro.x, transJugador.rotation.eulerAngles.y, transJugador.rotation.eulerAngles.z);
        }

    }
}
