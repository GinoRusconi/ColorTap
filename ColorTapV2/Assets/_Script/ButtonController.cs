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
    public RectTransform testtransform;
    public RectTransform canvas;
    public ParticleSystem particleSystemPress;

    [SerializeField] private Button button;
    [SerializeField] private PlayerID playerID;
    [SerializeField] private int colorID;
    public ButtonInfo info;
    public Color colorOfButton;


    private RectTransform reacTransformInformationPressing;
    private Transform _transform;
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
        _transform = GetComponent<Transform>();
        
        slotWinLoseImage = reacTransformInformationPressing.GetComponent<Image>();
        defaultColor = slotWinLoseImage.color;
        defaultLocalScale = reacTransformInformationPressing.localScale;
        defaultEulerAngle = reacTransformInformationPressing.eulerAngles;

        _AudioSource = GetComponent<AudioSource>();
        Button button = GetComponent<Button>();
        info = new ButtonInfo(button, playerID, colorID);
        info.button.onClick.AddListener(() => PlaySoundPress());
        info.button.onClick.AddListener(() => ParticlePress());
    }
    
    private void Start() 
    {
        switch (playerID)
        {
            case PlayerID.Player1:
                _QuaternionInWorld = Quaternion.Euler(0,0,0);
                break;

            case PlayerID.Player2:
                _QuaternionInWorld = Quaternion.Euler(0,0,180);
                break;
        }    
    }

    private void LateUpdate() {
        
        Vector3 localpositionInScreen = testtransform.anchoredPosition;
        Vector3 worldPosition;
        worldPosition = _transform.TransformPoint(localpositionInScreen);
        _TransformInWorld = worldPosition;
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
        particleSystemPress.Stop();
        particleSystemPress.Emit(20);
        Debug.Log("emitir");
   
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
