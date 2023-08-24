using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class ButtonController : MonoBehaviour
{
    public ButtonsManager _ButtonManager;
    public ButtonAnimation _ButtonAnimation;
    private AudioSource _AudioSource;

    [SerializeField] private Button button;
    [SerializeField] private PlayerID playerID;
    [SerializeField] private int colorID;
    public ButtonInfo info;

    private RectTransform reacTransformInformationPressing;
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

        slotWinLoseImage = reacTransformInformationPressing.GetComponent<Image>();
        defaultColor = slotWinLoseImage.color;
        defaultLocalScale = reacTransformInformationPressing.localScale;
        defaultEulerAngle = reacTransformInformationPressing.eulerAngles;

        _AudioSource = GetComponent<AudioSource>();
        Button button = GetComponent<Button>();
        info = new ButtonInfo(button, playerID, colorID);
        info.button.onClick.AddListener(() => PlaySoundPress());

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
