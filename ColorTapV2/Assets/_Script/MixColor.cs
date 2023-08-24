using System.Collections;
using UnityEngine;

public class MixColor : MonoBehaviour
{

    Coroutine coroutineMixColor;



    public int randomColorID;
    private int _MaxRangeColor;
    public Color[] colors;
    public float timeDelay;
    public float timeBetweenDelay;
    public float maxTime;
    private float _StartDelay;
    public Camera mainCamera;
    public AudioClip[] audioClips;
    private AudioSource _AudioSource;

    private void Awake()
    {
        _AudioSource = GetComponent<AudioSource>();
        _MaxRangeColor = colors.Length;
        _StartDelay = timeDelay;
    }
       
    public IEnumerator Mixed()
    {
        int lastColor = randomColorID = Random.Range(0, _MaxRangeColor);
        _AudioSource.clip = audioClips[0];
        timeDelay = _StartDelay;
        
        do
        {
            yield return new WaitForSeconds(timeDelay);

            do
            {
                randomColorID = Random.Range(0, _MaxRangeColor);

            } while (lastColor == randomColorID);

            lastColor = randomColorID;

            mainCamera.backgroundColor = colors[randomColorID];
            timeDelay *= timeBetweenDelay;
            
            if (timeDelay >= maxTime)
            {
                _AudioSource.clip = audioClips[1];
            }

            _AudioSource.Stop();
            _AudioSource.Play();
        }
        while (timeDelay < maxTime);
               
        Debug.Log("Finish");
    }

    public int GetRandomColor()
    {
        int randomColorID = Random.Range(0, _MaxRangeColor);

        return randomColorID;
    }

}
