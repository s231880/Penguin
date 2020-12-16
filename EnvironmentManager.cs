using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PenguinRun
{
    public class EnvironmentManager : MonoBehaviour
    {
        //-----------------------------------------------------------------------
        //Elements Lists used to initialise Object Pools
        private Dictionary<string, List<GameObject>> m_ElementsDictionary = new Dictionary<string, List<GameObject>>();

        private const int NUM_OF_MOUNTAIN_SPRITES = 6;
        private const int NUM_OF_ICEBERG_SPRITES = 7;
        private const int NUM_OF_CLOUD_SPRITES = 3;
        private float m_BottomRightScreenCornerX;
        private const int NUM_OF_ELEMENT_PER_TYPE = 10;

        //-----------------------------------------------------------------------
        //Object Pools & List used to manage the activation/disactivation
        private List<string> m_ElementsKeys = new List<string>();
        private Dictionary<string, ObjectPoolManager> m_PoolsDictionary = new Dictionary<string, ObjectPoolManager>();
        private Dictionary<string, List<EnvironmentElement>> m_ActiveElementsDictionary = new Dictionary<string, List<EnvironmentElement>>();
        private Dictionary<string, List<EnvironmentElement>> m_ElementsToBeRemovedDictionary = new Dictionary<string, List<EnvironmentElement>>();
        private Dictionary<string, Dictionary<string, Vector3>> m_ElementsStartingPointsDictionary = new Dictionary<string, Dictionary<string, Vector3>>();
        private Dictionary<string, float> m_DistanceBeforeGenerateNewElement = new Dictionary<string, float>();

        //-----------------------------------------------------------------------
        //Speed variables  & constants

        public float backgroundSpeed;

        private const string MOUNTAIN = "Mountain";
        private const string ICEBERG = "Iceberg";
        private const string CLOUD = "Cloud";
        //-----------------------------------------------------------------------
        
        public void Initialise(float bottomRightScreenCornerX)
        {
            InitialiseConstants(bottomRightScreenCornerX);
            InitialiseBackgroundElements();
            SetElementsStartingPosition();
            InitialiseObjectPools();

            //Used a coroutine to manage time synchronisation with GameController
            StartCoroutine(InitialiseBackground());
        }

        void Update()
        {
            GetAndReturnElements();
        }
        //-----------------------------------------------------------------------
        //Initialisation Functions

        private void InitialiseConstants(float bottomRightScreenCornerX)
        {
            m_ElementsKeys.Add(MOUNTAIN);
            m_ElementsKeys.Add(ICEBERG);
            m_ElementsKeys.Add(CLOUD);
            m_BottomRightScreenCornerX = bottomRightScreenCornerX;
        }

        private void InitialiseBackgroundElements()
        {
            int count = 0;
            foreach (string key in m_ElementsKeys)
            {
                switch (key)
                {
                    case MOUNTAIN:
                        count = NUM_OF_MOUNTAIN_SPRITES;
                        break;
                    case ICEBERG:
                        count = NUM_OF_ICEBERG_SPRITES;
                        break;
                    case CLOUD:
                        count = NUM_OF_CLOUD_SPRITES;
                        break;
                }
                List<GameObject> prefabList = new List<GameObject>();
                for (int i = 1; i <= count; ++i)
                {
                    var element = Resources.Load<GameObject>($"Prefabs/Environment/{key}{i}_");

                    var elementScript = element.GetComponent<EnvironmentElement>();
                    if (elementScript == null)
                        element.AddComponent<EnvironmentElement>();

                    prefabList.Add(element);
                }
                m_ElementsDictionary.Add(key, prefabList);
                m_ActiveElementsDictionary.Add(key, new List<EnvironmentElement>());
                m_ElementsToBeRemovedDictionary.Add(key, new List<EnvironmentElement>());
            }
        }

        private void InitialiseObjectPools()
        {
            var environmentParent = this.transform.Find("EnviromentsElements").transform;
            var oPParentTransform = environmentParent.Find("ObjectPools").transform;
            Transform  activeObjectsTransform = environmentParent.Find("ActiveElements").transform;

            foreach (var key in m_ElementsKeys)
            {
                var pool = new GameObject(key);
                pool.transform.SetParent(oPParentTransform);
                var activeElements = new GameObject($"Active{key}s");
                activeElements.transform.SetParent(activeObjectsTransform);
                var objPool = pool.AddComponent<ObjectPoolManager>();
                objPool.CreateObjPool(m_ElementsDictionary[key], NUM_OF_ELEMENT_PER_TYPE, pool.transform, activeElements.transform);
                m_PoolsDictionary.Add(key, objPool);
            }
        }

        IEnumerator InitialiseBackground()
        {
            while (backgroundSpeed == 0)
            {
                yield return new WaitForEndOfFrame();
            }

            foreach (var key in m_ElementsKeys)
            {
                EnvironmentElement element = m_PoolsDictionary[key].GetObject().GetComponent<EnvironmentElement>();
                if (element != null)
                {
                    Vector3 startingPos = element.transform.position;
                    startingPos.x = Random.Range(-m_BottomRightScreenCornerX, m_BottomRightScreenCornerX);
                    element.Activate(backgroundSpeed, startingPos);
                    m_ActiveElementsDictionary[key].Add(element);
                    float posBeforeActivateNewElement = (m_BottomRightScreenCornerX * 2) - m_ElementsStartingPointsDictionary[key][element.name].x + GetRandomValue();
                    m_DistanceBeforeGenerateNewElement.Add(key, posBeforeActivateNewElement);
                }
            }
        }

        private float GetElementLenght(GameObject obj)
        {
            var objCollider = obj.AddComponent<BoxCollider2D>();
            float lenght = objCollider.size.x;
            DestroyImmediate(objCollider, true);
            return lenght;
        }

        private void SetElementsStartingPosition()
        {
            Vector3 elementStartingPos;
            foreach (string key in m_ElementsKeys)
            {
                var elementList = m_ElementsDictionary[key];
                Dictionary<string, Vector3> elementStartingPoint = new Dictionary<string, Vector3>(); 
                foreach (var element in elementList)
                {
                    float lenght = GetElementLenght(element);
                    elementStartingPos = element.transform.position;
                    elementStartingPos.x  = m_BottomRightScreenCornerX + (lenght / 2);

                    elementStartingPoint.Add(element.name, elementStartingPos);
                }
                m_ElementsStartingPointsDictionary.Add(key, elementStartingPoint);
            }
        }

        //-----------------------------------------------------------------------
        //Get and return elements to the object pools
        private void GetElement(string elementType)
        {
            EnvironmentElement element = m_PoolsDictionary[elementType].GetObject().GetComponent<EnvironmentElement>();
            Vector3 startingPos = m_ElementsStartingPointsDictionary[elementType][element.name];
            if (element != null)
            {
                element.Activate(backgroundSpeed, startingPos);
                m_ActiveElementsDictionary[elementType].Add(element);
                float posBeforeActivateNewElement = (m_BottomRightScreenCornerX * 2) - startingPos.x - GetRandomValue();
                m_DistanceBeforeGenerateNewElement[elementType] = posBeforeActivateNewElement;
            }
        }

        private void ReturnElement(EnvironmentElement element, string elementType)
        {
            m_PoolsDictionary[elementType].ReturnObjectToThePool(element.gameObject);
            m_ActiveElementsDictionary[elementType].Remove(element);
        }
        
        private float GetRandomValue()
        {
            float randomVal =  Random.Range(-m_BottomRightScreenCornerX, m_BottomRightScreenCornerX);
            return randomVal;
        }
        private  void GetAndReturnElements()
        {
            foreach (var key in m_ElementsKeys)
            {
                var activeElements = m_ActiveElementsDictionary[key];
                if(activeElements.Count != 0)
                {
                    foreach (var element in activeElements)
                    {
                        if (element.transform.position.x < -m_DistanceBeforeGenerateNewElement[key])
                        {
                            m_ElementsToBeRemovedDictionary[key].Add(element);
                            m_ActiveElementsDictionary[key].Remove(element);
                            GetElement(key);
                            break;
                        }
                    }
                    
                }

                var elementsToBeRemoved = m_ElementsToBeRemovedDictionary[key];
                if (elementsToBeRemoved.Count != 0)
                {
                    foreach (var element in elementsToBeRemoved)
                    {
                        if (element.transform.position.x < -m_ElementsStartingPointsDictionary[key][element.name].x)
                        {
                            m_ElementsToBeRemovedDictionary[key].Remove(element);
                            ReturnElement(element, key);
                            break;
                        }
                    }
                }
            }
        } 
        //-----------------------------------------------------------------------
    }
}

