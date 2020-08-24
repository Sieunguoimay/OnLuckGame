using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.DataMarts;
public class ImageSlideshow : MonoBehaviour
{
    public GameObject scrollbar;
    public float scroll_pos = 0;
    float[] pos;
    bool firstTouch = true;
    float firstTouchScrollPos = 0;
    bool direction = false;
    bool changeImage = false;
    bool dontChangeIndex = false;
    // Start is called before the first frame update
    int index = 0;
    void Start()
    {
        Debug.Log("ImageSlideshow:Start");
    }

    // Update is called once per frame
    //public int ChildCountActive(Transform t)
    //{
    //    int k = 0;
    //    foreach (Transform c in t)
    //    {
    //        if (c.gameObject.activeSelf)
    //            k++;
    //    }
    //    return k;
    //}
    //private int childCount = 0;
    //public void Init()
    //{
    //    childCount = ChildCountActive(transform);
    //}
    public int currentIndex { private set; get; }
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
                    dontChangeIndex = false;
                    direction = true;
                }
                else if (scroll_pos - firstTouchScrollPos < -distance * 0.2)
                {
                    changeImage = true;
                    dontChangeIndex = false;
                    direction = false;
                }
                else
                {
                    changeImage = true;
                    dontChangeIndex = true;
                }
            }
        }

        if (changeImage)
        {
            if (!dontChangeIndex)
            {
                index += (direction ? 1 : -1);
                if (index < 0) index = 0;
                if (index > childCount - 1) index = childCount - 1;
            }

            scrollbar.GetComponent<Scrollbar>().value = Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, pos[index], 0.25f);
            if (Mathf.Abs(scrollbar.GetComponent<Scrollbar>().value - pos[index]) < 0.001f)
            {
                changeImage = false;
                scrollbar.GetComponent<Scrollbar>().value = pos[index];
            }
        }


        //for (int i = 0; i < childCount; i++)
        //{
        //    if (scroll_pos < pos[i] + (distance / 2) && scroll_pos > pos[i] - (distance / 2))
        //    {
        //        transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1f, 1f), 0.25f);
        //        for(int a = 0; a< childCount; a++)
        //        {
        //            if (a != i)
        //            {
        //                transform.GetChild(a).localScale = Vector2.Lerp(transform.GetChild(a).localScale, new Vector2(0.8f, 0.8f), 0.25f);
        //            }
        //        }
        //    }
        //}
    }
}
