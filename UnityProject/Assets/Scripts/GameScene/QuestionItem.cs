using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestionItem : MonoBehaviour
{
    private Button UiButton;
    public Text UiIndexText;

    public GameObject UiCorrectlyAnsweredIcon;
    public GameObject UiWronglyAnsweredIcon;
    public GameObject UiUnlockedIcon;
    public GameObject UiLockedIcon;

    public int Index { get; private set; }
    public States State { get; private set; }

    public enum States
    {
        QI_LOCKED,
        QI_ACTIVE,
        QI_WRONGLY_ANSWERED,
        QI_CORRECTLY_ANSWERED,
    }
    // Start is called before the first frame update
    void Awake()
    {
        State = States.QI_LOCKED;
        UiButton = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetIndex(int index)
    {
        UiIndexText.text = (index+1).ToString();
        Index = index;
    }
    public void SetState(States state)
    {
        State = state;
        switch (State)
        {
            case States.QI_LOCKED:
                UiButton.interactable = false;
                ShowIcon(0);
                break;
            case States.QI_ACTIVE:
                UiButton.interactable = true;
                ShowIcon(1);
                break;
            case States.QI_WRONGLY_ANSWERED:
                ShowIcon(2);
                break;
            case States.QI_CORRECTLY_ANSWERED:
                ShowIcon(3);
                break;
        }
    }
    public static States CharToState(char state)
    {
        if (state == 'l')
            return States.QI_LOCKED;
        else if (state == 'u')
            return States.QI_ACTIVE;
        else if (state == 'c')
            return States.QI_CORRECTLY_ANSWERED;
        else if (state == 'w')
            return States.QI_WRONGLY_ANSWERED;
        else
            return States.QI_LOCKED;
    }
    private GameObject GetIcon(int icon)
    {
        switch (icon)
        {
            case 0:
                return UiLockedIcon;
            case 1:
                return UiUnlockedIcon;
            case 2:
                return UiWronglyAnsweredIcon;
            case 3:
                return UiCorrectlyAnsweredIcon;
        }
        return null;
    }
    private void ShowIcon(int icon)
    {
        UiLockedIcon.SetActive(false);
        UiUnlockedIcon.SetActive(false);
        UiWronglyAnsweredIcon.SetActive(false);
        UiCorrectlyAnsweredIcon.SetActive(false);

        GetIcon(icon).SetActive(true);
    }
}
