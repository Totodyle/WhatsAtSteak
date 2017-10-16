using UnityEngine;
using System.Collections;


[RequireComponent(typeof(BoxCollider2D))]
public class CharacterRaycastCol : MonoBehaviour
{
    protected class RaycastOrigins
    {
		public Vector2 TopLeft { get; set; } 
		public Vector2 TopRight { get; set; }
        public Vector2 BottomLeft { get; set; }
        public Vector2 BottomRight { get; set; }

        public RaycastOrigins(){}
    }

    protected const float SKINWIDTH = 0.05f;

    [SerializeField] protected LayerMask m_colMask;

    [SerializeField] protected int m_horRayCount = 4;
    [SerializeField] protected int m_vertRayCount = 4;

    protected float m_horRaySpacing;
    protected float m_vertRaySpacing;

    protected BoxCollider2D m_boxCol;
	protected RaycastOrigins m_raycastOrigins = new RaycastOrigins();

    [SerializeField] protected float m_maxSlopeAngle = 80;
    protected CollisionInfo m_colInfo = new CollisionInfo();
    protected Vector2 m_playerInput;

    protected Bounds m_skinBounds;

    public CollisionInfo ColInfo
    {
        get { return m_colInfo;  }
    }
    public BoxCollider2D BoxCol
    {
        get { return m_boxCol; }
    }

    protected virtual void Awake()
    {
        m_boxCol = GetComponent<BoxCollider2D>();
    }

    protected virtual void Start()
    {
        CalculateRaySpacing();
        m_colInfo.FaceDir = 1;
    }

    protected void UpdateRaycastOrigins()
    {
        m_skinBounds = m_boxCol.bounds;
        m_skinBounds.Expand(SKINWIDTH * -2);

        m_raycastOrigins.BottomLeft = new Vector2(m_skinBounds.min.x, m_skinBounds.min.y);
        m_raycastOrigins.BottomRight = new Vector2(m_skinBounds.max.x, m_skinBounds.min.y);
        m_raycastOrigins.TopLeft = new Vector2(m_skinBounds.min.x, m_skinBounds.max.y);
        m_raycastOrigins.TopRight = new Vector2(m_skinBounds.max.x, m_skinBounds.max.y);
    }

    protected void CalculateRaySpacing()
    {
        m_skinBounds = m_boxCol.bounds;
        m_skinBounds.Expand(SKINWIDTH * -2);

        float boundsWidth = m_skinBounds.size.x;
        float boundsHeight = m_skinBounds.size.y;

        m_horRayCount = Mathf.Clamp(m_horRayCount, 2, int.MaxValue);
        m_vertRayCount = Mathf.Clamp(m_vertRayCount, 2, int.MaxValue);

        m_horRaySpacing = boundsHeight / (m_horRayCount - 1);
        m_vertRaySpacing = boundsWidth / (m_vertRayCount - 1);
    }

    public void Move(Vector3 p_moveAmount, bool p_standingOnPlatform)
    {
        Move(p_moveAmount, Vector2.zero, p_standingOnPlatform);
    }

    public void Move(Vector2 p_moveAmount, Vector2 p_input, bool p_standingOnPlatform = false)
    {
        UpdateRaycastOrigins();

        m_colInfo.Reset();
        m_colInfo.MoveAmountOld = p_moveAmount;
        m_playerInput = p_input;

        if (p_moveAmount.y < 0)
        {
            DescendSlope(ref p_moveAmount);
        }

        if (p_moveAmount.x != 0)
        {
            m_colInfo.FaceDir = (int)Mathf.Sign(p_moveAmount.x);
        }

        UpdateHorCols(ref p_moveAmount);
        if (p_moveAmount.y != 0)
        {
            UpdateVertCols(ref p_moveAmount);
        }

        transform.Translate(p_moveAmount);

        if (p_standingOnPlatform)
        {
            m_colInfo.IsBelowTouched = true;
        }
    }

    protected void UpdateHorCols(ref Vector2 p_moveAmount)
    {
        float directionX = m_colInfo.FaceDir;
        float rayLength = Mathf.Abs(p_moveAmount.x) + SKINWIDTH;

        if (Mathf.Abs(p_moveAmount.x) < SKINWIDTH)
        {
            rayLength = 2 * SKINWIDTH;
        }

        for (int i = 0; i < m_horRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? m_raycastOrigins.BottomLeft : m_raycastOrigins.BottomRight;
            rayOrigin += Vector2.up * (m_horRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, m_colMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {
                if (hit.distance == 0){ continue; }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= m_maxSlopeAngle)
                {
                    if (m_colInfo.IsDescendingSlope)
                    {
                        m_colInfo.IsDescendingSlope = false;
                        p_moveAmount = m_colInfo.MoveAmountOld;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != m_colInfo.SlopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - SKINWIDTH;
                        p_moveAmount.x -= distanceToSlopeStart * directionX;
                    }
                    ClimbSlope(ref p_moveAmount, slopeAngle, hit.normal);
                    p_moveAmount.x += distanceToSlopeStart * directionX;
                }

                if (!m_colInfo.IsClimbingSlope || slopeAngle > m_maxSlopeAngle)
                {
                    p_moveAmount.x = (hit.distance - SKINWIDTH) * directionX;
                    rayLength = hit.distance;

                    if (m_colInfo.IsClimbingSlope)
                    {
                        p_moveAmount.y = Mathf.Tan(m_colInfo.SlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(p_moveAmount.x);
                    }

                    m_colInfo.IsLeftTouched = directionX == -1;
                    m_colInfo.IsRightTouched = directionX == 1;
                }
            }
        }
    }

    protected void UpdateVertCols(ref Vector2 p_moveAmount)
    {
        float directionY = Mathf.Sign(p_moveAmount.y);
        float rayLength = Mathf.Abs(p_moveAmount.y) + SKINWIDTH;

        for (int i = 0; i < m_vertRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? m_raycastOrigins.BottomLeft : m_raycastOrigins.TopLeft;
            rayOrigin += Vector2.right * (m_vertRaySpacing * i + p_moveAmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, m_colMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)
            {
                if (hit.collider.tag == "PassableFloor")
                {
                    if (directionY == 1 || hit.distance == 0)
                    {
                        continue;
                    }
                    if (m_colInfo.IsPassableFloor)
                    {
                        continue;
                    }
                    if (m_playerInput.y == -1)
                    {
                        m_colInfo.IsPassableFloor = true;
                        Invoke("ResetPassableFloor", 0.5f);
                        continue;
                    }
                }

                p_moveAmount.y = (hit.distance - SKINWIDTH) * directionY;
                rayLength = hit.distance;

                if (m_colInfo.IsClimbingSlope)
                {
                    p_moveAmount.x = p_moveAmount.y / Mathf.Tan(m_colInfo.SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(p_moveAmount.x);
                }

                m_colInfo.IsBelowTouched = directionY == -1;
				m_colInfo.IsAboveTouched = directionY == 1;
            }
        }

        if(m_colInfo.IsClimbingSlope)
        {
            float directionX = Mathf.Sign(p_moveAmount.x);
            rayLength = Mathf.Abs(p_moveAmount.x) + SKINWIDTH;
            Vector2 rayOrigin = ((directionX == -1) ? m_raycastOrigins.BottomLeft : m_raycastOrigins.BottomRight) + Vector2.up * p_moveAmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, m_colMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != m_colInfo.SlopeAngle)
                {
                    p_moveAmount.x = (hit.distance - SKINWIDTH) * directionX;
                    m_colInfo.SlopeAngle = slopeAngle;
                    m_colInfo.SlopeNormal = hit.normal;
                }
            }
        }
    }

    protected void ClimbSlope(ref Vector2 p_moveAmount, float p_slopeAngle, Vector2 p_slopeNormal)
    {
        float moveDistance = Mathf.Abs(p_moveAmount.x);
        float climbmoveAmountY = Mathf.Sin(p_slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (p_moveAmount.y <= climbmoveAmountY)
        {
            p_moveAmount.y = climbmoveAmountY;
            p_moveAmount.x = Mathf.Cos(p_slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(p_moveAmount.x);
            m_colInfo.IsBelowTouched = true;
            m_colInfo.IsClimbingSlope = true;
            m_colInfo.SlopeAngle = p_slopeAngle;
            m_colInfo.SlopeNormal = p_slopeNormal;
        }
    }

    protected void DescendSlope(ref Vector2 moveAmount)
    {

        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(m_raycastOrigins.BottomLeft, Vector2.down, Mathf.Abs(moveAmount.y) + SKINWIDTH, m_colMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(m_raycastOrigins.BottomRight, Vector2.down, Mathf.Abs(moveAmount.y) + SKINWIDTH, m_colMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmount);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveAmount);
        }

        if (!m_colInfo.IsSlidingDownMaxSlope)
        {
            float directionX = Mathf.Sign(moveAmount.x);
            Vector2 rayOrigin = (directionX == -1) ? m_raycastOrigins.BottomRight : m_raycastOrigins.BottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, m_colMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= m_maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - SKINWIDTH <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x))
                        {
                            float moveDistance = Mathf.Abs(moveAmount.x);
                            float descendmoveAmountY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmount.x);
                            moveAmount.y -= descendmoveAmountY;

                            m_colInfo.SlopeAngle = slopeAngle;
                            m_colInfo.IsDescendingSlope = true;
                            m_colInfo.IsBelowTouched = true;
                            m_colInfo.SlopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    protected void SlideDownMaxSlope(RaycastHit2D p_hit, ref Vector2 p_moveAmount)
    {
        if (p_hit)
        {
            float slopeAngle = Vector2.Angle(p_hit.normal, Vector2.up);
            if (slopeAngle > m_maxSlopeAngle)
            {
                p_moveAmount.x = Mathf.Sign(p_hit.normal.x) * (Mathf.Abs(p_moveAmount.y) - p_hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                m_colInfo.SlopeAngle = slopeAngle;
                m_colInfo.IsSlidingDownMaxSlope = true;
                m_colInfo.SlopeNormal = p_hit.normal;
            }
        }

    }

    protected void ResetPassableFloor()
    {
        m_colInfo.IsPassableFloor = false;
    }

    public class CollisionInfo
    {
		public bool IsAboveTouched { get; set; }
        public bool IsBelowTouched { get; set; }
        public bool IsLeftTouched { get; set; }
        public bool IsRightTouched { get; set; }

        public bool IsClimbingSlope { get; set; }
        public bool IsDescendingSlope { get; set; }
        public bool IsSlidingDownMaxSlope { get; set; }

        public float SlopeAngle { get; set; }
        public float SlopeAngleOld { get; set; }
        public Vector2 SlopeNormal { get; set; }
        public Vector2 MoveAmountOld { get; set; }
        public int FaceDir { get; set; }
        public bool IsPassableFloor { get; set; }

		public CollisionInfo()
		{
			Reset();
		}

        public void Reset()
        {
            IsAboveTouched = IsBelowTouched = false;
            IsLeftTouched = IsRightTouched = false;
            IsClimbingSlope = false;
            IsDescendingSlope = false;
            IsSlidingDownMaxSlope = false;
            SlopeNormal = Vector2.zero;

            SlopeAngleOld = SlopeAngle;
            SlopeAngle = 0;
        }
    }
}
