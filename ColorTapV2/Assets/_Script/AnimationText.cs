using UnityEngine;
using TMPro;

public class AnimationText : MonoBehaviour
{
    private TextMeshProUGUI rtextMeshPro;
    private float oscillationSpeed = 2.0f;
    private float elapsedTime;

    void Awake()
    {
        rtextMeshPro = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        rtextMeshPro.alpha = Mathf.Lerp(0.2f, 1.0f, 0.5f + 0.5f * Mathf.Sin(oscillationSpeed * elapsedTime));
        
    }
}