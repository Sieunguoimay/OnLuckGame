using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollList : MonoBehaviour
{
    [SerializeField] public GameObject ItemTemplate;
    [NonSerialized] public Transform parent;
    private List<GameObject> m_cachedItemList = new List<GameObject>();
    void Awake()
    {
        ItemTemplate.SetActive(false);
        parent = ItemTemplate.transform.parent;
    }
    // Start is called before the first frame update
    void Start()
    {

    }
    public T CreateItem<T>()
    {
        var newItem = Instantiate(ItemTemplate) as GameObject;

        newItem.SetActive(true);

        newItem.transform.SetParent(parent, false);

        T item = newItem.GetComponent<T>();

        m_cachedItemList.Add(newItem);

        return item;
    }
    public void CreateList<T>(int number,Action<T,int> callback)
    {
        foreach(GameObject item in m_cachedItemList)
        {
            Destroy(item);
        }

        m_cachedItemList.Clear();

        for (int i = 0; i < number; i++)
        {
            callback(CreateItem<T>(), i);
        }
    }

    public void CreateList(int number, Action<GameObject, int> callback)
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
            newItem.transform.SetParent(parent, false);

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
