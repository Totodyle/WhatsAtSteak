using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorCubeGizmos : MonoBehaviour
{
    [SerializeField] private Color m_color;
    [SerializeField] BoxCollider2D m_col;

    private void OnDrawGizmos()
    {
        if(m_col != null)
        {
            Gizmos.color = m_color;
            Tools.DrawGizmosCube(transform.position, transform.localScale, transform.rotation);
        }
    }
}
