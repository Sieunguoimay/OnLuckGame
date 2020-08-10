using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
namespace Assets.Scripts.DataMarts
{
    public class AssetsDataMart
    {
        /*This class is a Singleton*/
        private static AssetsDataMart s_instance = null;
        public static AssetsDataMart Instance
        {
            get
            {
                if (s_instance == null)
                    s_instance = new AssetsDataMart();
                return s_instance;
            }
        }
        private AssetsDataMart() { }
        /*End of Singleton Declaration*/


        public Sprite rank1IconSprite;
        public Sprite rank2IconSprite;
        public Sprite rank3IconSprite;
        public Sprite defaultProfilePictureSprite;

        public Sprite soundIconSprite;
        public Sprite soundOffIconSprite;

        public Sprite correctAnswerSprite;
        public Sprite wrongAnswerSprite;
        public Sprite congrateCupSprite;


        public Texture2D hoChiMinhSprite;
        public Texture2D noImageSprite;
        public Texture2D turkeySprite;

        public string[] guidelines;
        public string[] guidelineTitles;

        public void LoadAssets()
        {
            rank1IconSprite = Resources.Load<Sprite>("Sprites/cup");
            rank2IconSprite = Resources.Load<Sprite>("Sprites/cup");
            rank3IconSprite = Resources.Load<Sprite>("Sprites/cup");
            defaultProfilePictureSprite = Resources.Load<Sprite>("Sprites/default_profile_picture");

            soundIconSprite = Resources.Load<Sprite>("Sprites/sound_icon");
            soundOffIconSprite = Resources.Load<Sprite>("Sprites/sound_off_icon");

            correctAnswerSprite = Resources.Load<Sprite>("Sprites/correct_answer");
            wrongAnswerSprite = Resources.Load<Sprite>("Sprites/wrong_answer");
            congrateCupSprite = Resources.Load<Sprite>("Sprites/cup");

            hoChiMinhSprite = Resources.Load<Texture2D>("Sprites/ho_chi_minh");
            noImageSprite = Resources.Load<Texture2D>("Sprites/no_image");
            turkeySprite = Resources.Load<Texture2D>("Sprites/turkey");

            guidelineTitles = new string[] { "HUONG DAN","GIOI THIEU","HUONG DAN KIEM XU" };
            guidelines = new string[] { 
                "If you have only two options while making a decision, you are perhaps not imaginative enough.\n"
                +"In reality, there are multiple options while taking any decision in life.It is similar to selecting a mate.You have hundreds of options to choose from and not only among any two.\n"
                +"The difficulty is not to make a decision when such dilemma arises.\n"
                +"The difficulty is to stick to your decision after it is taken.\n"
                +"Just like grass is greener from the other side of the fence, most people keeping thinking about the alternative decisions(not taken by them) even after the option is exercised.\n"
                +"You have to stop thinking about the options which has not been exercised by you simply because you can’t exercise them now. You have to trust yourself and your ability to take decision by ensuring that whatever options has been exercised by you is correct one.",
                "The user tokens listed here are provided for convenience to test your apps. They expire like any other user access token and should not be hard coded into your apps. App tokens do not expire and should be kept secret as they are related to your app secret. For more information on how access tokens work and should be used, see the documentation. If you want to debug an access token issue, try using the access token debugger.",
                "You have to understand that just taking a decision does not make you successful or unsuccessful. It is your will to go with your decision that gives you the success.\n"
                +"For example, it is incorrect to simply select your life partner and then believe that everything would be well. You have to constantly work towards strengthening the relationship. If you take the relationship for granted, it is doomed sooner or later.Most relationships fail not because the couples are not incompatible with each other, but because they don’t work to build the relationship." };
        }
    }
}
