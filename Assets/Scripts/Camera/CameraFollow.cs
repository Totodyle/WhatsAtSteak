using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private PlatformerBehavior m_targetPlatformerBehavior;
    [SerializeField] private float m_verticalOffset;
    [SerializeField] private float m_lookAheadDstX;
    [SerializeField] private float m_lookSmoothTimeX;
    [SerializeField] private float m_verticalSmoothTime;
    [SerializeField] private Vector2 m_focusAreaSize;

    private FocusArea m_focusArea;

    private Collider2D m_playerCol;

    private float m_currentLookAheadX;
    private float m_targetLookAheadX;
    private float m_lookAheadDirX;
    private float m_smoothLookVelocityX;
    private float m_smoothVelocityY;

    private bool m_lookAheadStopped;

    void Awake()
    {
        m_playerCol = m_targetPlatformerBehavior.GetComponent<Collider2D>();
    }

    void Start()
    {
        m_focusArea = new FocusArea(m_playerCol.bounds, m_focusAreaSize);
    }

    void LateUpdate()
    {
        m_focusArea.Update(m_playerCol.bounds);

        Vector2 focusPosition = m_focusArea.centre + Vector2.up * m_verticalOffset;

        if (m_focusArea.velocity.x != 0)
        {
            m_lookAheadDirX = Mathf.Sign(m_focusArea.velocity.x);
            if (Mathf.Sign(m_targetPlatformerBehavior.PlayerDirInput.x) == Mathf.Sign(m_focusArea.velocity.x) && m_targetPlatformerBehavior.PlayerDirInput.x != 0)
            {
                m_lookAheadStopped = false;
                m_targetLookAheadX = m_lookAheadDirX * m_lookAheadDstX;
            }
            else
            {
                if (!m_lookAheadStopped)
                {
                    m_lookAheadStopped = true;
                    m_targetLookAheadX = m_currentLookAheadX + (m_lookAheadDirX * m_lookAheadDstX - m_currentLookAheadX) / 4f;
                }
            }
        }


        m_currentLookAheadX = Mathf.SmoothDamp(m_currentLookAheadX, m_targetLookAheadX, ref m_smoothLookVelocityX, m_lookSmoothTimeX);

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref m_smoothVelocityY, m_verticalSmoothTime);
        focusPosition += Vector2.right * m_currentLookAheadX;
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(m_focusArea.centre, m_focusAreaSize);
    }

    struct FocusArea
    {
        public Vector2 centre;
        public Vector2 velocity;
        float left, right;
        float top, bottom;
        
        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }
            top += shiftY;
            bottom += shiftY;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }
    }

}