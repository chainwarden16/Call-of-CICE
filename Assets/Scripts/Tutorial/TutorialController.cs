using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialController : MonoBehaviour
{
    [Header("Variables y objetos del mapa del tutorial")]
    public Material cielo;
    [Tooltip("Texto que aparece por pantalla para comunicar los controles al jugador")]
    Text textoDialogo;
    [Tooltip("Contador que controla qu� di�logo deber�a mostrarse")]
    int contadortexto = 0;
    [Tooltip("Roca-diana para eltutorial")]
    GameObject rocaEnemiga;
    float temporizadorVueltaAlTitulo = 3f;
    public GameObject prefabVida;
    public GameObject prefabMana;
    GameObject botellaVida;
    GameObject botellaMana;
    JugadorController jugador;
    GameObject mago;

    [Header("Control de movimiento durante el tutorial")]
    float movX;
    float movY;

    [Header("Giro de c�mara")]
    float mouseX;
    float mouseY;

    [Header("Game Manager de la escena")]
    GameManager manager;

    void Start()
    {
        RenderSettings.skybox = cielo;
        textoDialogo = GameObject.Find("Instrucciones").GetComponent<Text>();
        rocaEnemiga = GameObject.Find("Roca");
        manager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        manager.haIniciadoCombate = true;
        jugador = GameObject.FindGameObjectWithTag("Player").GetComponent<JugadorController>();
        mago = GameObject.FindGameObjectWithTag("Aliado");

    }
    void Update()
    {



        switch (contadortexto)
        {
            case 0:
                textoDialogo.text = "De todos los j�venes del pueblo, t� eres el que menos esperaba ver aqu�. Supongo que habr�s o�do los rumores sobre los planes de invasi�n de la regi�n de Umbrafor...\n\n (Pulsa Enter para continuar)";
                if (Input.GetButtonDown("Submit"))
                {
                    contadortexto++;
                }
                break;
            case 1:
                textoDialogo.text = "Esas sabandijas no saben vivir fuera de la guerra. De las cinco naciones, eran los �nicos en desacuerdo con el tratado de paz... pero no pensaba que te importar�a lo suficiente para querer unirte a la misi�n. " +
                    "\n\n�Acaso has descubierto por fin tu vocaci�n?";
                if (Input.GetButtonDown("Submit"))
                {
                    contadortexto++;
                }
                break;
            case 2:
                movX = Input.GetAxisRaw("Horizontal");
                movY = Input.GetAxisRaw("Vertical");

                textoDialogo.text = "En cualquier caso, debo ver si eres capaz de luchar. Empecemos con lo b�sico... �Eres �gil? \n\n(Pulsa las teclas direccionales o WASD para moverte y la rueda del rat�n para correr)";
                if (movX != 0 || movY != 0)
                {

                    contadortexto++;
                }
                break;
            case 3:
                mouseY = Input.GetAxisRaw("Mouse X");
                mouseX = Input.GetAxisRaw("Mouse Y");
                textoDialogo.text = "Hm... M�s te vale estar espabilado y vigilar tus alrededores con frecuencia. \n\n(Mueve la c�mara con el rat�n)";
                if (mouseX != 0 || mouseY != 0)
                {

                    contadortexto++;
                }
                break;
            case 4:
                textoDialogo.text = "Bien. Ahora... �ves esa roca? Usa magia para romperla. \n\n(Presiona el bot�n principal del rat�n para disparar)";
                if (rocaEnemiga == null)
                {
                    textoDialogo.text = "Sabes apuntar. Al menos es algo. \n\n�Conoces alg�n hechizo? �Qu� sabes hacer? \n\n(Presiona el bot�n secundario del rat�n para usar un conjuro)";
                    contadortexto++;
                }
                else if (jugador.manaActual <= 0)
                {
                    textoDialogo.text = "... \n\nMuchacho, creo que deber�as sopesar tu decisi�n de unirte a la misi�n. Aunque quiz�s no todo est� perdido. Restaurar� tu man� y probaremos otra cosa. " +
                        "\n\n�Conoces alg�n hechizo? �Qu� sabes hacer? \n\n(Presiona el bot�n secundario del rat�n para usar un conjuro)";
                    jugador.manaActual = jugador.manaMaximo;
                    jugador.MostrarManaActual();
                    contadortexto++;
                }

                break;
            case 5:
                
                if (Input.GetButtonDown("Fire2") && jugador.manaActual >= 20)
                {
                    contadortexto++;
                    
                }
                else if (jugador.manaActual < 20)
                {
                    textoDialogo.text = "... \n\nNo est�s listo para entrar en combate, hijo. Da la vuelta y vuelve a casa.";
                    if (temporizadorVueltaAlTitulo <= 0)
                    {
                        SceneManager.LoadScene("Titulo");
                    }
                    else
                    {
                        temporizadorVueltaAlTitulo -= Time.deltaTime;
                    }
                }
                break;
            case 6:
                //Instanciar botellas de man� y vida
                textoDialogo.text = "Un hechizo en �rea que da�a y ralentiza a los oponentes... No est� mal para un novato.\n\n Ten en cuenta que tu man� disminuir� con cada disparo y cuando invoques la ventisca. " +
                    "Esos hombres no tendr�n piedad por ti: ser�s joven, pero pocos sobreviven unas heridas graves.\n\n";
                    
                if (Input.GetButtonDown("Submit"))
                {
                    contadortexto++;
                    botellaVida = Instantiate(prefabVida, mago.transform.position + new Vector3(-1f, 0, 1f), Quaternion.identity);
                    botellaMana = Instantiate(prefabMana, mago.transform.position + new Vector3(1f, 0, 1f), Quaternion.identity);
                }
                break;
            case 7:
                textoDialogo.text = "Si ves que est�s en un aprieto, siempre puedes beber una poci�n de man� o vida, respectivamente. Son como �stas. El equipo ha dejado algunas por la zona por si las necesitaseis.";
                if (botellaMana == null && botellaVida == null)
                {
                    contadortexto++;
                    GameObject.Find("Checkpoint").transform.Translate(0, 100, 0);
                }
                break;
            case 8:
                textoDialogo.text = "Eso es todo por hoy. Dir�gete al c�rculo de teletransporte morado bajando esta colina y descansa. Ma�ana ser� un d�a duro.";
                break;

        }
    }
}
