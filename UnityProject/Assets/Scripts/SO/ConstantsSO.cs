using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "ConstantsSO",menuName ="ScriptableObjects/ConstantsSO",order = 1)]
public class ConstantsSO : ScriptableObject
{
    public string base_url;
    public string base_api_url;
    public StringsSO stringsSO;// = new StringsSO();


    public Sprite rank1IconSprite;
    public Sprite rank2IconSprite;
    public Sprite rank3IconSprite;
    public Sprite defaultProfilePictureSprite;

    public Sprite soundIconSprite;
    public Sprite soundOffIconSprite;

    public Sprite correctAnswerSprite;
    public Sprite wrongAnswerSprite;
    public Sprite congrateCupSprite;


    public AudioClip buttonClickAudioClip;
    public AudioClip panelOpenAudioClip;
    public AudioClip correctAudioClip;
    public AudioClip wrongAudioClip;
    public AudioClip gameStartAudioClip;

}
[CustomEditor(typeof(ConstantsSO))]
[CanEditMultipleObjects]
public class ConstantsSOCE: Editor {
    private ConstantsSO constantsSO { get { return target as ConstantsSO; } }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUI.indentLevel++;
        Editor.CreateEditor(constantsSO.stringsSO).OnInspectorGUI();
        EditorGUI.indentLevel--;
    }
}