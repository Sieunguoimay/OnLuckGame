using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
using SFB;
#endif

public class LocalProvider
{
    private static LocalProvider s_instance = null;
    public static LocalProvider Instance
    {
        get
        {
            if(s_instance == null)
            {
                s_instance = new LocalProvider();
            }
            return s_instance;
        }
    }
    private LocalProvider()
    {

    }

    public delegate void BrowseImagePathCallback(string path);
    public void BrowseImagePath(BrowseImagePathCallback callback)
    {

#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.OpenFilePanel("Select Photo", "", "png,jpg,jpeg");
        Debug.Log("Selected " + path);
        callback(path);
#else

#if UNITY_IOS ||  UNITY_ANDROID
        NativeGallery.GetImageFromGallery((path) => {
            Debug.Log("Selected " + path);
            callback(path);
        }, "Select Photo");
#endif

#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
        string path = StandaloneFileBrowser.OpenFilePanel("Select Photo", "", "png,jpg,jpeg",false)[0];
        Debug.Log("Selected " + path);
        callback(path);
#endif


#endif

    }

    public delegate void SaveImageCallback(bool status);
    public void SaveImage(Texture2D texture, string filename, SaveImageCallback callback = null)
    {

#if UNITY_EDITOR
        string path = UnityEditor.EditorUtility.SaveFilePanel("Save texture as PNG", "", filename , "png");
        if (path.Length != 0)
        {
            var pngData = texture.EncodeToPNG();
            if (pngData != null)
            {
                File.WriteAllBytes(path, pngData);
                callback(true);
                return;
            }
        }
        callback(false);
#else

#if UNITY_IOS || UNITY_ANDROID
        var pngData = texture.EncodeToPNG();
        if (pngData != null)
            NativeGallery.SaveImageToGallery(pngData,"",filename, (err) => {
                Debug.Log("Log: " + err);
                callback(true);
            });
#endif

#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
        string path = StandaloneFileBrowser.OpenFilePanel("Select Photo", "", "png,jpg,jpeg",false)[0];
        Debug.Log("Selected " + path);
#endif


#endif
    }
}
