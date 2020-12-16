using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PenguinRun
{
    public enum HazardType
    {
        SingleFire,
        LowBird,
        HighBird,
        DoubleCloseFires,
        DoubleDistantFires,
        DoubleHighBird,
        DoubleLowBird,
        TripleDistantFires,
        TripleLowBird
    }


    public class HazardsManager : MonoBehaviour
    {
        private Dictionary<string, GameObject> m_HazardPrefabDictionary = new Dictionary<string, GameObject>();

        private List<string> m_HazardListKeys = new List<string>();
        //-------------------------------------------------------------------------------
        //Item containers => to get and return elements during the gameplay
        private Dictionary<string, ObjectPoolManager> m_ObjPoolsDictionary = new Dictionary<string, ObjectPoolManager>();
        private Dictionary<string, List<HazardElement>> m_ActiveHazardsDictionary = new Dictionary<string, List <HazardElement>>();
        private Dictionary<string, float> m_HazardsEndX = new Dictionary<string, float>();
        private Dictionary<string, Vector3> m_HazardsStartPos = new Dictionary<string, Vector3>();

        
        //-------------------------------------------------------------------------------
        //Constant variables
        private const int EASY_HAZARDS = 2;
        private const int MEDIUM_HAZARDS = 6;
        private const int HARD_HAZARDS = 8;
        private const int HIGH_BIRD_MULTIPLIER_CONST = 5;
        private const int NUM_OF_HAZARDS_PER_TYPE = 7;
        private const string FIRE = "Fire";
        private const string BIRD = "Bird";
        //-------------------------------------------------------------------------------

        private int m_HazardsCount;
        public float hazardsSpeed = 0;

        private Vector3 m_PenguinPosition = new Vector3();
        private float m_PenguinWidth = 0;
        private Dictionary<string, float> m_HazardsWidthDictionary = new Dictionary<string, float>();

        //-------------------------------------------------------------------------------

        private HazardType m_CurrentHazard;
        public HazardType currentHazard
        {
            get { return m_CurrentHazard; }
            set
            {
                m_CurrentHazard = value;
                GetHazard();
            }
        }


        public void Initialise(Vector3 penguinPosition, float penguinWidth, float bottomRightScreenCornerX)
        {
            m_PenguinPosition = penguinPosition;
            m_PenguinWidth = penguinWidth * 2;

            //Find all hazards and initialise Object Pools
            InitialiseVariables();
            InitialiseHazards();
            InitialiseObjectPools();
            InitialiseHazardsKeyPositions(bottomRightScreenCornerX);

            StartCoroutine(InitialiseFirstHazard());
        }

        void Update()
        {
            GetAndReturnHazards();
        }

        private void InitialiseHazards()
        {
            foreach (var key in m_HazardListKeys)
            {
                var prefab = Resources.Load<GameObject>($"Prefabs/Hazards/{key}");
                float prefabWidth = prefab.GetComponent<BoxCollider2D>().size.x;
                m_HazardsWidthDictionary.Add(key, prefabWidth);

                var elementScript = prefab.GetComponent<HazardElement>();
                if (elementScript == null)
                    prefab.AddComponent<HazardElement>();

                m_HazardPrefabDictionary.Add(key, prefab);
            }
        }

        private void InitialiseObjectPools()
        {
            //Finding the transforms where store the elements
            var environmentParent = this.transform.Find("EnviromentsElements").transform;
            var oPParentTransform = environmentParent.Find("ObjectPools").transform;
            Transform activeObjectsTransform = environmentParent.Find("ActiveElements").transform;         

            foreach (var key in m_HazardListKeys)
            {
                var pool = new GameObject(key);
                pool.transform.SetParent(oPParentTransform);
                var activeHazards = new GameObject($"Active{key}");
                activeHazards.transform.SetParent(activeObjectsTransform);
                var objPool = pool.AddComponent<ObjectPoolManager>();
                objPool.CreateObjPool(m_HazardPrefabDictionary[key], NUM_OF_HAZARDS_PER_TYPE, pool.transform, activeHazards.transform);
                m_ObjPoolsDictionary.Add(key, objPool);
            }
        }

        //Function to set the elements starting pos. I've set variables through this approach 
        //because I took into account that differnt devices have diffenrent screen width
        private void InitialiseHazardsKeyPositions(float bottomRightScreenCornerX)
        {
            foreach (var key in m_HazardListKeys)
            {
                Vector3 hazardStartingPosition = m_HazardPrefabDictionary[key].transform.position;
                hazardStartingPosition.x = bottomRightScreenCornerX + (m_HazardsWidthDictionary[key] / 2);
                m_HazardsStartPos.Add(key, hazardStartingPosition);
                m_HazardsEndX.Add(key, -hazardStartingPosition.x);
            }
        }

        private void InitialiseVariables()
        {
            //Initialising container to get and return elements
            m_HazardListKeys.Add(BIRD);
            m_HazardListKeys.Add(FIRE);

            foreach (var key in m_HazardListKeys)
            {
                List<HazardElement> list = new List<HazardElement>();
                m_ActiveHazardsDictionary.Add(key, list);
            }
        }

        private void GetHazard()
        {
            switch (m_CurrentHazard)
            {
                case HazardType.SingleFire:
                    ActivateSingleHazard(FIRE);
                    break;
                case HazardType.LowBird:
                    ActivateSingleHazard(BIRD);
                    break;
                case HazardType.HighBird:
                    ActivateSingleHazard(BIRD, true);
                    break;
                case HazardType.DoubleCloseFires:
                    ActivateMultipleHazard(FIRE, 2);
                    break;
                case HazardType.DoubleDistantFires:
                    ActivateMultipleHazard(FIRE, 2, false);
                    break;
                case HazardType.DoubleHighBird:
                    ActivateMultipleHazard(BIRD, 2, false, true);
                    break;
                case HazardType.DoubleLowBird:
                    ActivateMultipleHazard(BIRD, 2, false, false);
                    break;
                case HazardType.TripleDistantFires:
                    ActivateMultipleHazard(FIRE, 3, false);
                    break;
                case HazardType.TripleLowBird:
                    ActivateMultipleHazard(BIRD, 3, false, true);
                    break;
                default:
                    break;
            }
        }

        private void ActivateSingleHazard(string type, bool highBird = false)
        {
            HazardElement hazard = m_ObjPoolsDictionary[type].GetObject().GetComponent<HazardElement>();
            if (hazard != null)
            {
                Vector3 startingPos = m_HazardsStartPos[type];

                if (highBird)
                {
                    startingPos.y *= HIGH_BIRD_MULTIPLIER_CONST;
                    Debug.LogError("HIGHBIRD");
                }

                hazard.Activate(startingPos, hazardsSpeed);
                m_ActiveHazardsDictionary[type].Add(hazard);
            }
        }

        private void ActivateMultipleHazard(string type, int count, bool close = true ,bool highBird = false)
        {
            HazardElement hazard;
            Vector3 startingPos;
            
            for (int i = 0; i < count; ++i)
            {
                startingPos = m_HazardsStartPos[type];
                hazard = m_ObjPoolsDictionary[type].GetObject().GetComponent<HazardElement>();
                if (hazard != null)
                {
                    if (close)
                        startingPos.x += m_HazardsWidthDictionary[type] * i;
                    else
                        startingPos.x += m_PenguinWidth * i;

                    hazard.Activate(startingPos, hazardsSpeed);
                    m_ActiveHazardsDictionary[type].Add(hazard);
                }
            }
        }

        private void SetNewHazard()
        {
            currentHazard = (HazardType)Random.Range(0, m_HazardsCount);
        }

        public void SetHazardCount(GameDifficulty currentDifficulty)
        {
            switch (currentDifficulty)
            {
                case GameDifficulty.Easy:
                    m_HazardsCount = EASY_HAZARDS;
                    break;
                case GameDifficulty.Medium:
                    m_HazardsCount = MEDIUM_HAZARDS;
                    break;
                case GameDifficulty.Hard:
                    m_HazardsCount = HARD_HAZARDS;
                    break;
            }
        }

        IEnumerator InitialiseFirstHazard()
        {
            while (hazardsSpeed == 0)
            {
                yield return new WaitForEndOfFrame();
            }
            SetNewHazard();
        }

        private void GetAndReturnHazards()
        {
            foreach (var key in m_HazardListKeys)
            {
                List<HazardElement> list = m_ActiveHazardsDictionary[key];
                if (list.Count != 0)
                {
                    foreach (var hazard in list)//Check every element in the list
                    {
                        if (hazard.transform.position.x < m_PenguinPosition.x)//If the element has passed the penguin
                        {
                            if( GameController.Instance.gameDifficulty == GameDifficulty.Easy)
                            {
                                //If hazard is no more visible
                                if (hazard.transform.position.x < m_HazardsEndX[key])
                                {
                                    ReturnHazard(hazard, key);
                                    SetNewHazard();
                                    break;
                                }
                            }
                            else
                            {
                                
                            }
                            
                        }
                    }
                }
            }
        }

        private void ReturnHazard(HazardElement hazard, string hazardType)
        {
            m_ObjPoolsDictionary[hazardType].ReturnObjectToThePool(hazard.gameObject);
            m_ActiveHazardsDictionary[hazardType].Remove(hazard);
        }
    }
}
