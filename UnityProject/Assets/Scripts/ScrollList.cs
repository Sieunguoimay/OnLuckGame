using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollList : MonoBehaviour
{
    [SerializeField]
    public GameObject ItemTemplate;

    private List<GameObject> m_cachedItemList = new List<GameObject>();
    void Awake()
    {
        ItemTemplate.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public delegate void ShowListCallback<T>(T item, int index);
    public void CreateList<T>(int number,ShowListCallback<T> callback)
    {
        foreach(GameObject item in m_cachedItemList)
        {
            Destroy(item);
        }
        m_cachedItemList.Clear();

        for (int i = 0; i < number; i++)
        {
            GameObject newItem = Instantiate(ItemTemplate) as GameObject;
            newItem.SetActive(true);
            newItem.transform.SetParent(ItemTemplate.transform.parent, false);

            T item = newItem.GetComponent<T>();
            callback(item,i);

            m_cachedItemList.Add(newItem);
        }
    }
    public void CreateList(int number, ShowListCallback<GameObject> callback)
    {
        foreach (GameObject item in m_cachedItemList)
        {
            Destroy(item);
        }
        m_cachedItemList.Clear();

        for (int i = 0; i < number; i++)
        {
            GameObject newItem = Instantiate(ItemTemplate) as GameObject;
            newItem.SetActive(true);
            newItem.transform.SetParent(ItemTemplate.transform.parent, false);

            callback(newItem, i);

            m_cachedItemList.Add(newItem);
        }
    }
    public void Clear()
    {
        foreach (GameObject item in m_cachedItemList)
        {
            Destroy(item);
        }
        m_cachedItemList.Clear();
    }
}
