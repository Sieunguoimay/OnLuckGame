using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
{
    enum Filters
    {
        NULL = -1,
        HIGH_SCORE = 0,
        ONLUCK = 1,
        TOP_PLAYER = 2
    };

    [SerializeField] private ScrollList ScoreboardScrollList;
    [SerializeField] private Button mode1Button;
    [SerializeField] private Button mode2Button;
    [SerializeField] private Button mode3Button;
    [SerializeField] private GameObject paginateButtons;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    [SerializeField] private Spinner spinner;

    private Button currentButton = null;
    private bool shouldRefresh = false;

    HttpClient.Paginate<HttpClient.UserScoreResponseData> paginate = null;

    Filters filter = Filters.NULL;

    public void Show()
    {
        gameObject.SetActive(true);

        if (currentButton == null)
        {
            ShowHighScore();
        }
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ShowHighScore()
    {
        if (currentButton != mode1Button)
        {
            if (currentButton != null)
            {
                currentButton.interactable = true;
            }

            currentButton = mode1Button;

            shouldRefresh = true;

            filter = Filters.HIGH_SCORE;
            LoadScoreboard(null, (int)filter);

            mode1Button.interactable = false;
        }
    }

    public void ShowOnluck()
    {
        if (currentButton != mode2Button)
        {
            if (currentButton != null)
            {
                currentButton.interactable = true;
            }

            currentButton = mode2Button;

            shouldRefresh = true;

            filter = Filters.ONLUCK;
            LoadScoreboard(null, (int)filter);

            mode2Button.interactable = false;
        }
    }

    public void ShowTopPlayers()
    {
        if (currentButton != mode3Button)
        {
            if (currentButton != null)
            {
                currentButton.interactable = true;
            }

            currentButton = mode3Button;

            shouldRefresh = true;

            filter = Filters.TOP_PLAYER;
            LoadScoreboard(null, (int)filter);

            mode3Button.interactable = false;
        }

    }

    private bool LoadScoreboard(string paginateUrl,int mode)
    {
        if (shouldRefresh)
        {
            shouldRefresh = false;

            spinner.gameObject.SetActive(true);//.Show(ScoreboardScrollList.transform);

            HttpClient.Instance.LoadScoreboard(paginateUrl, mode, (paginate) =>
             {
                 this.paginate = paginate;

                 if (paginate != null)
                 {
                     paginateButtons.transform.parent = transform;
                     Clear();

                     for (int i = 0; i < paginate.data.Count; i++)
                     {
                         var item = paginate.data[i];

                         var url = HttpClient.Instance.BaseUrl;

                         AddScoreboardItem(i, item.name, url + item.profile_picture, item.score);
                     }
                     paginateButtons.transform.parent = ScoreboardScrollList.parent;
                     paginateButtons.SetActive(paginate.to >= paginate.per_page);
                     nextButton.interactable = paginate.next_page_url != null && !paginate.next_page_url.Equals("");
                     prevButton.interactable = paginate.prev_page_url != null && !paginate.prev_page_url.Equals("");
                 }
                 else
                 {
                     currentButton.interactable = true;
                 }

                //spinner.Hide();
                spinner.gameObject.SetActive(false);//.Show(ScoreboardScrollList.transform);
            });
            return true;
        }
        return false;
    }

    public void Clear()
    {
        ScoreboardScrollList.Clear();
    }
    private void AddScoreboardItem(int index, string name, string profilePic, int score)
    {
        var item = ScoreboardScrollList.CreateItem<ScoreboardItem>();

        item.SetRank(index);
        item.SetUserName(name);
        item.SetScore(score);
        item.SetProfilePicture(profilePic);
    }


    public void HandleButton1Clicked()
    {
        ShowHighScore();
    }
    public void HandleButton2Clicked()
    {
        ShowOnluck();
    }
    public void HandleButton3Clicked()
    {
        ShowTopPlayers();
    }

    public void HandleNextButtonClicked()
    {
        if (!paginate.next_page_url.Equals(""))
        {
            LoadScoreboard(paginate.next_page_url, (int)filter);
        }
    }
    public void HandlePrevButtonClicked()
    {
        if (!paginate.prev_page_url.Equals(""))
        {
            LoadScoreboard(paginate.prev_page_url, (int)filter);
        }
    }
}
