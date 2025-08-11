using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("GameManager is null!");
            }
            return instance;
        }
    }

    public string playerName;
    public int money;
    public TMP_Text infoUpdatesText;
    [SerializeField] TMP_Text moneyText;
    public Sprite[] herbs = new Sprite[10];
    public Camera[] cameras;
    [SerializeField] Dictionary<string, int> payments = new Dictionary<string, int>(); // pagar el carrito, la tasa de circulación, otras tasas, alquiler, comida, agua para negocio, tazas
                                                                                       // TAMBIEN BENEFICIOS! como tu salario, propinas, dinero encontrado por la calle
    //hacer que todos estos impuestos puedan estar visibles en un menu que salga al final del dia de manera ordenada
    //que de igual si hay 3 que 8, si hay 8 que una barra como slider esté disponible para que recorras todos bajando el slider
    // tmb añadir que te haga operación del dinero total? y gastos en color distinto a ganancias?? quien sabe,...
    
    [SerializeField] float fadeDuration = 1.5f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);

    }

    // SOLO PUEDE HABER UNA CAJA DE MUGS EN EL JUEGO, ASI QUE CUANDO LLEGUE A LA NOCHE, SI NO LA HAS GASTADO TE LA ROBARAN O LE PASARÁN COSAS, NADA MÁS LA COMPRES SE AGOTARÁ EN LA TIENDA 


    // Update is called once per frame
    void Update()
    {
        if(moneyText.text != "$" + money.ToString())
        {
            moneyText.text = "$" + money.ToString();
        }
    }
    public IEnumerator FadeWait()
    {
        infoUpdatesText.gameObject.SetActive(true);
        //money++;
        Color infoColor = infoUpdatesText.color;
        infoColor.a = 1f;
        infoUpdatesText.color = infoColor;
        yield return new WaitForSeconds(2); //WaitForSecondsRealtime?

        while (infoColor.a > 0f)
        {
            infoColor.a -= fadeDuration * Time.deltaTime; // es "menos" para FadeIn pq el panel tiene el alpha a tope 
            infoColor.a = Mathf.Clamp01(infoColor.a);
            infoUpdatesText.color = infoColor;

            yield return null;
        }
        infoUpdatesText.gameObject.SetActive(false);
    }
    
    void Taxes()
    {
        /*
        panel taxes:
        [everyday]
            ahorros
            ganancias de hoy (?)
            - venta ambulante       SINO, TE PERSIGUE LA POLI DURANTE EL DÍA Y SI TE ATRAPAN TE COGERÁN 18$ O TODO LO QUE TENGAS EN E MOMENTO
            - recolecta hierbas     SINO, EN CUANTO COJAS HIERBAS DE LA MONTAÑA ESE DÍA, UN GUARDIA FORESTAL TE PERSEGUIRÁ Y LE TENDRÁS QUE HACER UNA INFUSIÓN EXTRA GRATIS
            - alquiler carro        SINO TE PERSIGUE UN SEÑOR (EL QUE TE LO ALQULA) Y SE QUEDA MIRANDO COMO TE PAGAN PARA INTERCEPTAR EL DINERO HASTA QUE CONSIGA 10$ POR MOLESTIAS
            - compra alimentos      SINO 5 VECES, TE MUERES O HACES JORNADAS MÁSS CORTAS
        [ocasionally: sehr selten]
            - carro roto        SINO, IRÁ LIGERAMENTE MÁS LENTO A LO LARGO DE LOS DÍAS, SI LLEGA EL DÍA 5 TENDRÁS QUE CALENTARLO TU A MANO REGULANDO LA LLAMA? O CON CAMPING GAS
                si no lo pagas 3 días seguidos se rompe del todo 
        y tienes que pagar más, sino puedes pagar:
        no te darán tips los clientes y te pagarán menos (dia 3 y 4 de deuda)
        a partir del dia 5:
        pasarás un día más sin salir de casa pero con impuestos que pagar y se llevaran tu carro y lo arreeglaran, si pasa eso 3 veces, se acabó! 

        MIRAR EN GENERAL QUE HACER SI ENDEUDAS DINERO, SI NO TIENES EL CARRO ALQUILADO, SI NO LO ARREGLAS, ETC.
        SI NO LO ARREGLAS EN 3/5 DÍAS TIENES QUE COCINAR CON UN CAMPING GAS, MÁS LENTO Y NO PUEDES CAMBIAR EL CARRO DE ZONA
        [everyweek]
            -alquiler casa      SINO, LA CASERA PONDRÁ UN CANDADO EN TU PUERTA, CUANDO LO VEAS APARECERÁ UN SEÑOR QUE TE DIRÁ QUE PUEDES DORMIR EN SU TIENDA DE CAMPAÑA EN MITAD DEL BOSQUE: LOS DÍAS QUE DUERMAS AHÍ (1 SEMANA MINIMO?) TE LEVANTARÁS A LAS 13

        (--water bottle in shop)
        (--mugs in shop)
        

        texto dinero restante
         */
    }
}
