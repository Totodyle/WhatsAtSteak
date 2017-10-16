using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRaycastCol : CharacterRaycastCol
{
    [SerializeField] private float m_interactableRayLength = 1.0f;
    [SerializeField] private LayerMask m_interactableMask;

    [SerializeField] private BoxCollider2D m_attackCol;

    protected override void Awake()
    {
        base.Awake();
        ToggleAttackCol(false);
    }

    public bool IsAttackColActive
    {
        get { return m_attackCol.isActiveAndEnabled; }
    }
    public void ToggleAttackCol(bool p_bIsActive)
    {
        if (m_attackCol != null)
        {
            m_attackCol.enabled = p_bIsActive;
        }
    }

    public void EnablePickUpRays(ref Transform p_pickUpTrans)
    {
        float directionX = m_colInfo.FaceDir;

        Vector2 rayOrigin = (directionX == -1) ? m_raycastOrigins.BottomLeft : m_raycastOrigins.BottomRight;
        rayOrigin += Vector2.up * m_skinBounds.size.y * 0.75f;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, m_interactableRayLength, m_interactableMask);

        Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.green);

        if (hit)
        {
            if (hit.collider.tag == "Enemy")
            {
                EnemyPlatformerBehavior hitBehavior = hit.collider.GetComponent<EnemyPlatformerBehavior>();
                if (hitBehavior != null)
                {
                    hitBehavior.enabled = false;
                }

                p_pickUpTrans = hit.collider.transform;
                p_pickUpTrans.SetParent(transform);
                p_pickUpTrans.localPosition = new Vector3(0.28f, m_skinBounds.size.y + 0.7f, 0.0f);
                p_pickUpTrans.localScale = Vector3.one;

                hit.collider.enabled = false;
            }
        }
    }
}
