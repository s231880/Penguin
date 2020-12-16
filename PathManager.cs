using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PenguinRun
{
    public class PathManager : MonoBehaviour
    {
        private const string PATH = "Path";
        private const int NUM_OF_PATH = 3;
        private const int NUM_OF_ELEMENTS = 5;
        private Vector3 m_ElementsStartingPoint = Vector3.zero;
        private List<GameObject> m_PathList = new List<GameObject>();
        private List<PathElement> m_ActiveElements = new List<PathElement>();
        private float m_ElementLenght;
        public float pathSpeed = 0;
        private ObjectPoolManager m_Pool;

        private void Update()
        {

        }

        public void Initialise(float bottomRightScreenCornerX)
        {
            InitialiseElements();
            GenerateObjPool();
            FindStartingPoint(bottomRightScreenCornerX);

            StartCoroutine(InitialiseScene());
        }

        private void GenerateObjPool()
        {
            var environmentParent = this.transform.Find("EnviromentsElements").transform;
            var oPParentTransform = environmentParent.Find("ObjectPools").transform;
            Transform activeObjectsTransform = environmentParent.Find("ActiveElements").transform;

            var pool = new GameObject(PATH);
            pool.transform.SetParent(oPParentTransform);
            var activeElements = new GameObject($"Active{PATH}s");
            activeElements.transform.SetParent(activeObjectsTransform);
            m_Pool = pool.AddComponent<ObjectPoolManager>();
            m_Pool.CreateObjPool(m_PathList, NUM_OF_ELEMENTS, pool.transform, activeElements.transform);
        }

        private void InitialiseElements()
        {
            for (int i = 0; i < NUM_OF_PATH; ++i)
            {
                var element = Resources.Load<GameObject>($"Prefabs/Environment/{PATH}{i}");

                var elementScript = element.GetComponent<PathElement>();
                if (elementScript == null)
                    element.AddComponent<PathElement>();

                m_PathList.Add(element);
            }
        }

        private void FindStartingPoint(float bottomRightScreenCornerX)
        {
            var element = m_PathList[0];

            var objCollider = element.AddComponent<BoxCollider2D>();
            m_ElementLenght = objCollider.size.x;
            DestroyImmediate(objCollider, true);

            m_ElementsStartingPoint = element.transform.position;
            m_ElementsStartingPoint.x = bottomRightScreenCornerX + (m_ElementLenght / 2);
        }

        IEnumerator InitialiseScene()
        {
            while (pathSpeed == 0)
            {
                yield return new WaitForEndOfFrame();
            }

            var element = m_Pool.GetObject().GetComponent<PathElement>();
            element.Activate(m_ElementsStartingPoint, pathSpeed);
            m_ActiveElements.Add(element);


            Vector3 secondStartingPoint = m_ElementsStartingPoint;
            secondStartingPoint.x -= (m_ElementLenght / 2);
            var secondElement = m_Pool.GetObject().GetComponent<PathElement>();
            secondElement.Activate(secondStartingPoint, pathSpeed);
            m_ActiveElements.Add(secondElement);

            Vector3 thirdStartingPoint = secondStartingPoint;
            thirdStartingPoint.x -= m_ElementLenght;
            var thirdElement = m_Pool.GetObject().GetComponent<PathElement>();
            thirdElement.Activate(thirdStartingPoint, pathSpeed);
            m_ActiveElements.Add(thirdElement);
        }

    }
}
