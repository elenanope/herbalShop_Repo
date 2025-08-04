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
    public TMP_Text moneyText;
    public Sprite[] herbs = new Sprite[10];
    public Camera[] cameras;
    
    [SerializeField] float fadeDuration = 1.5f;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);

    }

    // SOLO PUEDE HABER UNA CAJA DE MUGS EN EL JUEGO, ASI QUE CUANDO LLEGUE A LA NOCHE, SI NO LA HAS GASTADO TE LA ROBARAN O LE PASARÁN COSAS, NADA MÁS LA COMPRES SE AGOTARÁ EN LA TIENDA 


    // Start is called before the first frame update
    void Start()
    {
        
    }

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
        Debug.Log("Mensaje espernado");
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
    
}
