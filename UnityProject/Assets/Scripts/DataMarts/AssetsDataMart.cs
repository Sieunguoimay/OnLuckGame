using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts.DataMarts
{
    public class AssetsDataMart : MonoBehaviourSingleton<AssetsDataMart>
    {
        /*This class is a Singleton*/
        //private static AssetsDataMart s_instance = null;
        //public static AssetsDataMart Instance
        //{
        //    get
        //    {
        //        if (s_instance == null)
        //            s_instance = new AssetsDataMart();
        //        return s_instance;
        //    }
        //}
        //private AssetsDataMart() { }
        /*End of Singleton Declaration*/

        public ConstantsSO constantsSO;

        //public Sprite rank1IconSprite;
        //public Sprite rank2IconSprite;
        //public Sprite rank3IconSprite;
        //public Sprite defaultProfilePictureSprite;

        //public Sprite soundIconSprite;
        //public Sprite soundOffIconSprite;

        //public Sprite correctAnswerSprite;
        //public Sprite wrongAnswerSprite;
        //public Sprite congrateCupSprite;


        //public AudioClip buttonClickAudioClip;
        //public AudioClip panelOpenAudioClip;
        //public AudioClip correctAudioClip;
        //public AudioClip wrongAudioClip;
        //public AudioClip gameStartAudioClip;

        [NonSerialized] public AudioSource rAudioSource = null;

        //public void LoadAssets()

        //{
            //buttonClickAudioClip = Resources.Load<AudioClip>("Sounds/button");
            //panelOpenAudioClip = Resources.Load<AudioClip>("Sounds/panel_open");
            //correctAudioClip = Resources.Load<AudioClip>("Sounds/correct");
            //wrongAudioClip = Resources.Load<AudioClip>("Sounds/wrong");
            //gameStartAudioClip = Resources.Load<AudioClip>("Sounds/start");

            //rank1IconSprite = Resources.Load<Sprite>("Sprites/cup");
            //rank2IconSprite = Resources.Load<Sprite>("Sprites/cup");
            //rank3IconSprite = Resources.Load<Sprite>("Sprites/cup");
            //defaultProfilePictureSprite = Resources.Load<Sprite>("Sprites/default_profile_picture");

            //soundIconSprite = Resources.Load<Sprite>("Sprites/sound_icon");
            //soundOffIconSprite = Resources.Load<Sprite>("Sprites/sound_off_icon");

            //correctAnswerSprite = Resources.Load<Sprite>("Sprites/correct_answer");
            //wrongAnswerSprite = Resources.Load<Sprite>("Sprites/wrong_answer");
            //congrateCupSprite = Resources.Load<Sprite>("Sprites/cup");



            //assetsData = LocalProvider.Instance.LoadAssetsData();
            //Debug.Log("LoadAssets: " + assetsData.strings.login);
            //Debug.Log("LoadAssets: " + assetsData.strings.new_game_data_available);
            //Debug.Log("LoadAssets: " + assetsData.strings.download);
        //}
        //[Serializable]
        //public class AssetsData
        //{
        //    public string base_url;
        //    public string base_api_url;
        //    public Strings strings;
        //}
        //[Serializable]
        //public class Strings
        //{
        //    public string login;
        //    public string logout;
        //    public string login_with_facebook;
        //    public string sigup;
        //    public string upload_photo;
        //    public string rename;
        //    public string not_yet_loggedin;

        //    public string intro;
        //    public string guideline;
        //    public string scoreboard;
        //    public string new_game_data_available;
        //    public string no_internet;
        //    public string download;
        //    public string try_again;
        //    public string startup_text_question_pack;
        //    public string startup_text_question_no;
        //    public string startup_text_complete;
        //    public string resume;
        //    public string start;
        //    public string open;
        //    public string user_name;
        //    public string qoute_popup_title;
        //    public string enter_new_password;
        //    public string congrate;
        //    public string correct;
        //    public string incorrect;
        //    public string next_question;
        //    public string the_answer_is;
        //    public string timeout;
        //    public string press_one_more_time;

        //}
    }
}
