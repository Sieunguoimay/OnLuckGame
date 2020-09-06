using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.DataMarts;
using System.IO;
using System;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
public class GameDataDownloader : MonoBehaviour
{
    private WWW www;
    private bool isUnzipped = false;
    private bool isLoaded = false;
    private bool isSaved = false;
    QuestionDataMart.Season season = null;
    ZipInputStream zipInputStream = null;
    // Start is called before the first frame update
    void Start()
    {
        string url = AssetsDataMart.Instance.assetsData.base_api_url+"/downloadgamedata";
        www = new WWW(url);
        Debug.Log("GameDataDownloader::Start");
    }

    // Update is called once per frame
    void Update()
    {
        if (!www.isDone)
        {
            if (progressCallback!=null)
                progressCallback("Downloading:","KB",(float)www.size / 1024.0f,  (www.progress==0?0: (float)www.size / www.progress) / 1024.0f);
        }
        if (www.isDone && !isUnzipped)
        {
            if (!isSaved)
            {
                Debug.Log("GameDataDownloader::Load of test.zip complete");
                isSaved = true;
                SaveZipFileToLocal();
            }

            if (zipInputStream!=null)
            {
                if (progressCallback != null)
                    progressCallback("Unzipping:", "", zipInputStream.Position, 0);
                ZipEntry theEntry;
                if ((theEntry = zipInputStream.GetNextEntry()) != null)
                {
                    Debug.Log(theEntry.Name);

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    // create directory
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if (fileName != String.Empty)
                    {
                        string filename = Application.persistentDataPath+"/"+theEntry.Name;// docPath.Substring(0, docPath.Length - 8);
                        //filename += theEntry.Name;
                        Debug.Log("Unzipping: " + filename);
                        using (FileStream streamWriter = File.Create(filename))
                        {
                            int size = 2048;
                            byte[] fdata = new byte[2048];
                            while (true)
                            {
                                size = zipInputStream.Read(fdata, 0, fdata.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(fdata, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                if(theEntry==null)
                    isUnzipped = true;
            }
        }
        if (isUnzipped&&!isLoaded)
        {
            LocalProvider.Instance.LoadQuestionData(this,(ss)=> {
                season = ss;
                if (onDoneCallback != null)
                    onDoneCallback(season);
            });
            isLoaded = true;
        }
    }
    private void SaveZipFileToLocal()
    {
        byte[] data = www.bytes;

        string docPath = Application.persistentDataPath;
        docPath = docPath.Substring(0, docPath.Length - 5);
        docPath = docPath.Substring(0, docPath.LastIndexOf("/"));
        docPath += "/game_data.zip";
        Debug.Log("docPath=" + docPath);
        System.IO.File.WriteAllBytes(docPath, data);
        zipInputStream = new ZipInputStream(File.OpenRead(docPath));
    }
    private void OnDestroy()
    {
        Debug.Log("GameDataDownloader::destroyed");
    }
    public delegate void OnDoneCallback(QuestionDataMart.Season ss);
    public OnDoneCallback onDoneCallback=null;

    public delegate void ProgressCallback(string name,string unit, float downloaded,float total);
    public ProgressCallback progressCallback = null;
}
