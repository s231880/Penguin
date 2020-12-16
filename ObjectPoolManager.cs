using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    private List<GameObject> m_Objects = new List<GameObject>();
    private Transform m_ObjectPoolParent;
    private Transform m_ActiveElementsTransform;

    public void CreateObjPool(List<GameObject> cachedObjList, float amountPerType, Transform parent, Transform activeElementsParten)
    {
        m_ObjectPoolParent = parent;
        m_ActiveElementsTransform = activeElementsParten;
        foreach (var cachedObj in cachedObjList)
        {
           for (int i = 0; i < amountPerType; i++)
            {
                var obj = Object.Instantiate(cachedObj);
                obj.name = cachedObj.name /*+ i*/;
                obj.transform.localScale = new Vector3(1, 1, 1);
                obj.transform.SetParent(m_ObjectPoolParent);
                m_Objects.Add(obj);
           }
        }
        ShuffleDisablePoolElements();
    }

    public void CreateObjPool(GameObject cachedObj, float amount, Transform parent, Transform activeElementsParten)
    {
        m_ObjectPoolParent = parent;
        m_ActiveElementsTransform = activeElementsParten;
        for (int i = 0; i < amount; i++)
        {
            var obj = Object.Instantiate(cachedObj);
            obj.name = cachedObj.name + i;
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.SetParent(m_ObjectPoolParent);
            m_Objects.Add(obj);
        }
        ShuffleDisablePoolElements();
    }
    public  void ReturnObjectToThePool(GameObject obj, bool initialise = false)
    {
        obj.transform.SetParent(m_ObjectPoolParent);
        obj.SetActive(initialise);
        m_Objects.Add(obj);
    }

    public GameObject GetObject()
    {
        if (m_Objects.Count > 0)
        {
            GameObject cachedObj = m_Objects[0];
            m_Objects.RemoveAt(0);
            cachedObj.transform.SetParent(m_ActiveElementsTransform);
            cachedObj.SetActive(true);
            return cachedObj;
        }
        return null;
    }

    //Fisher_Yates_Shuffle
    private void ShuffleDisablePoolElements()
    {
        GameObject temporaryGameObject;
        int count = m_Objects.Count;

        for (int i = 0; i < count; ++i)
        {
            int randomValue = Random.Range(i, count - 1);
            temporaryGameObject = m_Objects[randomValue];
            m_Objects[randomValue] = m_Objects[i];
            m_Objects[i] = temporaryGameObject;
            m_Objects[i].SetActive(false);
        }
    }
}
