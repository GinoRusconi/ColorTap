using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSound : MonoBehaviour
{
     private AudioSource audioSource;

    private void Start()
    {
        // Obtén la referencia al componente AudioSource
        audioSource = GetComponent<AudioSource>();
    }

    // Este método se llamará cada vez que se active la subemisión
    public void PlaySound()
    {
        // Asegúrate de que tengas un AudioSource adjunto al GameObject
        if (audioSource != null)
        {
            // Reproduce el sonido
            audioSource.Play();
        }
    }
}
