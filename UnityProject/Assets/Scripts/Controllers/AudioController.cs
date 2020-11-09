using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviourSingleton<AudioController>
{
    [SerializeField] private AudioClip panelOpen;
    [SerializeField] private AudioClip buttonClicked;
    [SerializeField] private AudioClip correct;
    [SerializeField] private AudioClip wrong;

    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundOnEnterGameScene()
    {
        audioSource.PlayOneShot(panelOpen);
    }
    public void PlayButtonClickedSound()
    {
        audioSource.PlayOneShot(buttonClicked);
    }
    public void PlayPanelOpenSound()
    {
        audioSource.PlayOneShot(panelOpen);
    }
    public void PlayCorrectSound()
    {
        audioSource.PlayOneShot(correct);
    }
    public void PlayWrongSound()
    {
        audioSource.PlayOneShot(wrong);
    }
}
