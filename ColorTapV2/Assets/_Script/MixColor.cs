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
    private int countColors = 0;
    private int lastColor = 1;

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
            ChangeColorMainCamera(randomColorID);
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

    public void ChangeColorMainCamera(int idColor)
    {
        mainCamera.backgroundColor = colors[idColor];
    }


    public int GetRandomColor()
    {
        bool isSameColor;
        int randomColorID;
        do
        {
            randomColorID = Random.Range(0, _MaxRangeColor);
            if(lastColor == randomColorID && countColors > 2){
                isSameColor = true;
            }else if(lastColor == randomColorID && countColors < 2){
                isSameColor = false;
                countColors++;
            }else{
                isSameColor = false;
                countColors = 0;
            }
            
        } while (isSameColor);

        lastColor = randomColorID;
        return randomColorID;
    }

}
