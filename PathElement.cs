using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathElement : MonoBehaviour
{
    private Vector3 m_NextPos = Vector3.zero;

    public void Initialise() { }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        this.transform.localPosition -= m_NextPos;
    }

    public void Activate(Vector3 startPos, float speed)
    {
        this.transform.position = startPos;
        m_NextPos = speed * this.transform.right;
    }
}
