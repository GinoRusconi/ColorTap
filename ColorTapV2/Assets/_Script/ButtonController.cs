using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;


public class ButtonController : MonoBehaviour
{
    public ButtonsManager _ButtonManager;
    public ButtonAnimation _ButtonAnimation;
    private AudioSource _AudioSource;
    public GridLayoutGroup glgButtons;

    public Canvas canvas;
    public ParticleSystem particleSystemPress;

    [SerializeField] private Button button;
    [SerializeField] private PlayerID playerID;
    [SerializeField] private int colorID;
    public ButtonInfo info;
    public Color colorOfButton;


    private RectTransform reacTransformInformationPressing;
    private RectTransform _RectTransform;
    private Vector3 _TransformInWorld;
    private Quaternion _QuaternionInWorld;

    private Image slotWinLoseImage;
    private Color defaultColor;
    private readonly float shrinkSpeed = 5;
    private readonly float velocityOfSpin = 100;
    private Vector3 defaultLocalScale;
    private Vector3 defaultEulerAngle;

    private void Awake()
    {
        _ButtonAnimation = GetComponent<ButtonAnimation>();
        reacTransformInformationPressing = gameObject.GetComponentsInChildren<RectTransform>()[1];
        _RectTransform = GetComponent<RectTransform>();
        
        slotWinLoseImage = reacTransformInformationPressing.GetComponent<Image>();
        defaultColor = slotWinLoseImage.color;
        defaultLocalScale = reacTransformInformationPressing.localScale;
        defaultEulerAngle = reacTransformInformationPressing.eulerAngles;

        _AudioSource = GetComponent<AudioSource>();
        Button button = GetComponent<Button>();
        info = new ButtonInfo(button, playerID, colorID);
        info.button.onClick.AddListener(() => PlaySoundPress());
        info.button.onClick.AddListener(() => ParticlePress());
        glgButtons = GetComponentInParent<GridLayoutGroup>();
    }

    Vector3 GetCanvasPosition(RectTransform rectTransform)
    {
        // Convierte las coordenadas locales al espacio del Canvas
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        // El centro del elemento está en la esquina inferior izquierda (corners[0])
        return corners[0];
    }

    private void Start() 
    {
        Vector3 worldPosition;
        Vector3 localpositionButtonInGrid;
        Vector3 localPositionInWorld;   
        Vector2 localpositionInScreen;
        RectTransform positionGrid;
        switch (playerID)
        {
            case PlayerID.Player1:
                
                //localposition = _RectTransform.localPosition;

                //Vector3 cameraPosition = canvas.worldCamera.transform.position;
                //worldPosition = cameraPosition +localposition;
                //RectTransformUtility.ScreenPointToWorldPointInRectangle(_RectTransform,_RectTransform.localPosition,Camera.main,out _TransformInWorld);
                
                //localpositionInScreen = RectTransformUtility.WorldToScreenPoint(Camera.main,_RectTransform.position);
                // Obtén las coordenadas en pantalla del elemento en el Canvas
                // screenPosition = RectTransformUtility.WorldToScreenPoint(null, _RectTransform.position);
                //worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(localpositionInScreen.x,localpositionInScreen.y,Camera.main.nearClipPlane));
                // Ajusta la profundidad (coordenada z) para colocar el objeto detrás
                //Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10.0f));

                // Coloca el objeto en las coordenadas calculadas en el World Space
                //_TransformInWorld = worldPosition;
                
                //localpositionButtonInGrid = _RectTransform.localPosition;
                //GridLayoutGroup glgButtons = _RectTransform.parent.GetComponent<GridLayoutGroup>();
                //positionGrid = glgButtons.GetComponent<RectTransform>();
                //Camera camera = Camera.main;
                //localpositionButtonInGrid  = transform.localPosition;
                //GridLayoutGroup gridLayoutGroup = GetComponentInParent<GridLayoutGroup>();
                //Vector3 worldButtonPosition = gridLayoutGroup.transform.TransformPoint(localpositionButtonInGrid);
                //worldButtonPosition.z -= 1.0f;
                 // Obtiene la posición local del botón dentro del GridLayoutGroup
                //Vector3 localPosition = _RectTransform.localPosition;

                // Convierte las coordenadas locales del RectTransform a coordenadas de pantalla
                //Vector3 screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, localPosition);
                
                //screenPosition = glgButtons.transform.TransformPoint(localPosition);
                //_TransformInWorld = g
                // Luego, convierte las coordenadas de pantalla al espacio del Transform de destino
                //localPositionInWorld = new Vector3(localpositionInScreen.x, localpositionInScreen.y,0) + localpositionButtonInGrid;

                // Obtén la celda deseada (por ejemplo, la celda en la fila 0, columna 0)
        // Obtén el RectTransform del elemento
        // Obtiene el RectTransform del GridLayoutGroup
        // Obtiene el RectTransform del botón (el componente que almacena la posición)
        // Obtén el RectTransform del botón
                // Obtén el RectTransform del botón
          // Obtén el RectTransform del botón
                //    worldPosition = GetCanvasPosition(_RectTransform);
                 // Obten la posición local del botón en el canvas
        

        // Convierte la posición local al world space
        
          worldPosition = Camera.main.ScreenToWorldPoint(_RectTransform.anchoredPosition);
        _TransformInWorld = worldPosition;
                
                _QuaternionInWorld = Quaternion.Euler(0,0,0);
                Debug.Log("Posición en el espacio mundial:  " + this + _TransformInWorld);
                break;
            case PlayerID.Player2:
                worldPosition = _RectTransform.localPosition;

                // Convierte las coordenadas locales del RectTransform a coordenadas de pantalla
                //screenPosition = RectTransformUtility.WorldToScreenPoint(Camera.main, localPosition);
                //localposition = _RectTransform.TransformPoint(Vector3.zero);
                // Luego, convierte las coordenadas de pantalla al espacio del Transform de destino
                //_TransformInWorld = localposition;
                _QuaternionInWorld = Quaternion.Euler(0,0,180);
                Debug.Log("Posición en el espacio mundial:  " + this + _TransformInWorld);
                break;
        }    
    }

    public void ResetDefault()
    {
        slotWinLoseImage.sprite = null;
        slotWinLoseImage.color = defaultColor;
        reacTransformInformationPressing.localScale = defaultLocalScale;
        reacTransformInformationPressing.eulerAngles = defaultEulerAngle;

        ChangeTransparency(0);
    }
    public void ShowResultToPressedButton(bool result, Sprite spriteResult)
    {
        if (result)
        {
            slotWinLoseImage.sprite = spriteResult;
            slotWinLoseImage.color = Color.green;
        }
        else
        {
            slotWinLoseImage.sprite = spriteResult;
            slotWinLoseImage.color = Color.red;
        }
        

    }

    private void ParticlePress()
    {

        particleSystemPress.gameObject.transform.SetPositionAndRotation(_TransformInWorld, _QuaternionInWorld);
        
        var main = particleSystemPress.main;
        main.startColor = colorOfButton;

        particleSystemPress.Emit(20);
   
    }

    public void PlaySoundPress()
    {
        _AudioSource.Stop();
        _AudioSource.Play();
    }

    public void ChangeTransparency(float trasnparence)
    {
        ColorBlock colorBlock = info.button.colors;
        Color disabledColor = colorBlock.disabledColor;
        disabledColor.a = trasnparence / 255f; // Dividir por 255 para obtener un valor entre 0 y 1
        colorBlock.disabledColor = disabledColor;
        info.button.colors = colorBlock;
    }

    public IEnumerator ClearSlotWinLose()
    {
        while (IsScalePositive(reacTransformInformationPressing))
        {
            reacTransformInformationPressing.localScale -= Vector3.one * (shrinkSpeed * Time.deltaTime);
            reacTransformInformationPressing.eulerAngles += new Vector3(0f, 0f, velocityOfSpin * Time.deltaTime * 100f);
            yield return null;
        }
    }

    private bool IsScalePositive(RectTransform rectTransform)
    {
        Vector3 localScale = rectTransform.localScale;
        return (localScale.x >= 0f && localScale.y >= 0f && localScale.z >= 0f);
    }
}
   
public enum PlayerID
{
    Player1,
    Player2
}

public struct ButtonInfo
{
    public Button button;
    public PlayerID playerID;
    public int colorID;


    public ButtonInfo(Button button, PlayerID playerID, int colorID)
    {
        this.colorID = colorID;
        this.button = button;
        this.playerID = playerID;
    }
}
