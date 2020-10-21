using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.DataMarts;
using Assets.Scripts;
public class ButtonSound : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //var audioSource = gameObject.AddComponent<AudioSource>();
        GetComponent<Button>().onClick.AddListener(() => {
            if (Main.Instance.soundEnabled)
            {
                if (AssetsDataMart.Instance.constantsSO.buttonClickAudioClip != null&&
                    AssetsDataMart.Instance.rAudioSource!=null)
                    AssetsDataMart.Instance.rAudioSource.PlayOneShot(AssetsDataMart.Instance.constantsSO.buttonClickAudioClip);
            }
        });
    }
}
