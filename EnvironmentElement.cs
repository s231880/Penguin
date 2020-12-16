using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PenguinRun
{
    public class EnvironmentElement : MonoBehaviour
    {
        private Vector3 m_NextPos = new Vector3();

        void Update()
        {
            Move();
        }

        public void Activate(float speed, Vector3 startPos)
        {
            this.transform.position = startPos;
            m_NextPos = speed * this.transform.right;
        }

        private void Move()
        {
            this.transform.localPosition -= m_NextPos;
        }
    }
}
