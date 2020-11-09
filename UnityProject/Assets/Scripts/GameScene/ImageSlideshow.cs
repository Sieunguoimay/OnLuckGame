using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.DataMarts;
public class ImageSlideshow : MonoBehaviour
{
    public GameObject UiImageItemTemplate;

    public Scrollbar scrollbar;
    public float scroll_pos = 0;
    public int index = 0;
    public int currentIndex { private set; get; }

    float[] pos;
    bool firstTouch = true;
    float firstTouchScrollPos = 0;
    bool direction = false;
    bool changeImage = false;

    void Start()
    {
    }

    public void SetToDefault()
    {
        UiImageItemTemplate.SetActive(true);

        foreach (Transform child in transform)
        {
            if (child.gameObject != UiImageItemTemplate)
            {
                Destroy(child.gameObject);
            }
        }
        scrollbar.value = 0;
        scroll_pos = 0;
        index = 0;
    }

    void Update()
    {
        int childCount = transform.childCount;
        if (childCount == 0) return;

        pos = new float[childCount];
        float distance = 0;
        if (childCount == 1)
        {
            pos[0] = 0;
        }
        else
        {
            distance = 1f / (childCount - 1f);
            for (int i = 0; i < childCount; i++)
            {
                pos[i] = distance * i;
            }

        }
        if (Input.GetMouseButton(0))
        {
            scroll_pos = scrollbar.GetComponent<Scrollbar>().value;
            if (firstTouch)
            {
                firstTouchScrollPos = scroll_pos;
                firstTouch = false;
            }
        }
        else
        {

            if (!firstTouch)
            {
                firstTouch = true;
                if (scroll_pos - firstTouchScrollPos > distance * 0.2)
                {
                    changeImage = true;
                    direction = true;
                    index += (direction ? 1 : -1);
                    if (index < 0) index = 0;
                    if (index > childCount - 1) index = childCount - 1;
                }
                else if (scroll_pos - firstTouchScrollPos < -distance * 0.2)
                {
                    changeImage = true;
                    direction = false;
                    index += (direction ? 1 : -1);
                    if (index < 0) index = 0;
                    if (index > childCount - 1) index = childCount - 1;
                }
                else
                {
                    changeImage = true;
                }
            }
        }

        if (changeImage)
        {
            scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[index], 0.25f);
            if (Mathf.Abs(scrollbar.GetComponent<Scrollbar>().value - pos[index]) < 0.001f)
            {
                changeImage = false;
                scrollbar.GetComponent<Scrollbar>().value = pos[index];
            }
        }

    }
    public void SetQuestionImages(List<QuestionDataMart.Image> images)
    {

        SetToDefault();

        if (images.Count > 0)
        {
            UiImageItemTemplate.SetActive(false);

            images.ForEach((image) =>
            {
                var newItem = Instantiate(UiImageItemTemplate) as GameObject;
                newItem.GetComponent<Image>().sprite = image.sprite;
                newItem.transform.parent = transform;
                newItem.SetActive(true);
            });
        }
    }
}
