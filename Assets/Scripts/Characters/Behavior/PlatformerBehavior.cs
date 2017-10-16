using UnityEngine;
using System.Collections;

public class PlatformerBehavior : MonoBehaviour
{
    protected CharacterRaycastCol m_raycastCol;
    [SerializeField] protected float m_maxJumpHeight = 4.0f;
    [SerializeField] protected float m_minJumpHeight = 1.0f;
    [SerializeField] protected float m_maxJumpTime = 0.4f;
    [SerializeField] protected float m_moveSpeed = 6.0f;
    [SerializeField] protected bool m_bCanWallJump = false;
    [SerializeField] protected Vector2 m_wallJumpClimb;
    [SerializeField] protected Vector2 m_wallJumpOff;
    [SerializeField] protected Vector2 m_wallLeap;
    [SerializeField] protected float m_wallSlideSpeedMax = 3.0f;
    [SerializeField] protected float m_wallStickTime = 0.25f;

    protected float m_accelerationTimeAirborne = 0.2f;
    protected float m_accelerationTimeGrounded = 0.1f;
    protected float m_timeToWallUnstick;
    protected float m_gravity;
    protected float m_maxJumpVelocity;
    protected float m_minJumpVelocity;
    protected Vector3 m_velocity;
    protected float m_velocitySmoothingX;
    protected Vector2 m_playerDirInput = Vector2.zero;
    protected bool m_bIsWallSliding;
    protected int m_wallDirX;

    protected bool m_bIsWalking = false;
    protected bool m_bIsFalling = false;

    //private Transform m_pickedUpTrans;
    protected float m_beingThrownForceX;
    protected bool m_bIsBeingThrown = false;

    protected Vector3 m_defaultScale = new Vector3(1.0f, 1.0f, 1.0f);

    [SerializeField] protected Animator m_anim;

    [SerializeField] protected int m_maxLife = 3;
    protected int m_curLife;
    protected bool m_bIsDamaged = false;
    [SerializeField] protected float m_damagedTimer = 1.0f;

    public CharacterRaycastCol RaycastCol
    {
        get { return m_raycastCol; }
    }

    public Vector2 PlayerDirInput
    {
        get { return m_playerDirInput; }
        set { m_playerDirInput = value; }
    }

    //public bool IsBeingThrown
    //{
    //    get { return m_bIsBeingThrown; }
    //}

    protected virtual void Awake()
    {
        m_raycastCol = GetComponent<CharacterRaycastCol>();

        if (m_anim == null)
        {
            m_anim = GetComponent<Animator>();
        }
        m_defaultScale = transform.localScale;
    }

    protected virtual void Start()
    {
        m_gravity = -(2 * m_maxJumpHeight) / Mathf.Pow(m_maxJumpTime, 2);
        m_maxJumpVelocity = Mathf.Abs(m_gravity) * m_maxJumpTime;
        m_minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(m_gravity) * m_minJumpHeight);
        Reset();
    }

    public void ToggleCharacter(bool p_bIsActive, Vector2 p_pos)
    {
        transform.position = p_pos;
        ToggleCharacter(p_bIsActive);
    }
    public void ToggleCharacter(bool p_bIsActive)
    {
        Reset();
        gameObject.SetActive(p_bIsActive);
        this.enabled = p_bIsActive;
    }
    protected virtual void TriggerDeath()
    {
    }
    public virtual void Reset()
    {
        m_curLife = m_maxLife;

        m_bIsBeingThrown = false;
    }

    protected virtual void Update()
    {
        if(m_bIsBeingThrown)
        {
            ThrownBehavior();
        }

        CalculateVelocity();
        if(m_bCanWallJump)
        {
            HandleWallSliding();
        }

        m_raycastCol.Move(m_velocity * Time.deltaTime, m_playerDirInput);

        if (m_raycastCol.ColInfo.IsAboveTouched || m_raycastCol.ColInfo.IsBelowTouched)
        {
            if (m_raycastCol.ColInfo.IsSlidingDownMaxSlope)
            {
                m_velocity.y += m_raycastCol.ColInfo.SlopeNormal.y * -m_gravity * Time.deltaTime;
            }
            else
            {
                m_velocity.y = 0;
            }
        }

        UpdatePlayerSpriteDir();

        m_bIsWalking = m_raycastCol.ColInfo.IsBelowTouched && (Mathf.Round(m_velocity.x) != 0);
        m_bIsFalling = m_velocity.y <= -0.6f && !m_raycastCol.ColInfo.IsBelowTouched;


        if(m_anim != null)
        {
            m_anim.SetBool("bIsFalling", m_bIsFalling);
            m_anim.SetBool("bIsBelow", m_raycastCol.ColInfo.IsBelowTouched);
            m_anim.SetBool("bIsWalking", m_bIsWalking);
        }
    }

    protected void UpdatePlayerSpriteDir()
    {
        if (m_raycastCol.ColInfo.FaceDir < 0)
        {
            Vector3 tempPos = m_defaultScale;
            tempPos.x = -m_defaultScale.x;
            transform.localScale = tempPos;
        }
        else if (m_raycastCol.ColInfo.FaceDir > 0) { transform.localScale = m_defaultScale; }

    }

    public void OnJumpInputDown()
    {
        if (m_anim != null)
        {
            m_anim.SetTrigger("JumpTrig");
        }

        if (m_bIsWallSliding)
        {
            if (m_wallDirX == m_playerDirInput.x)
            {
                m_velocity.x = -m_wallDirX * m_wallJumpClimb.x;
                m_velocity.y = m_wallJumpClimb.y;
            }
            else if (m_playerDirInput.x == 0)
            {
                m_velocity.x = -m_wallDirX * m_wallJumpOff.x;
                m_velocity.y = m_wallJumpOff.y;
            }
            else
            {
                m_velocity.x = -m_wallDirX * m_wallLeap.x;
                m_velocity.y = m_wallLeap.y;
            }
        }
        if (m_raycastCol.ColInfo.IsBelowTouched)
        {
            if (m_raycastCol.ColInfo.IsSlidingDownMaxSlope)
            {
                if (PlayerDirInput.x != -Mathf.Sign(m_raycastCol.ColInfo.SlopeNormal.x))
                { 
                    m_velocity.y = m_maxJumpVelocity * m_raycastCol.ColInfo.SlopeNormal.y;
                    m_velocity.x = m_maxJumpVelocity * m_raycastCol.ColInfo.SlopeNormal.x;
                }
            }
            else
            {
                m_velocity.y = m_maxJumpVelocity;
            }
        }
    }

    public void OnJumpInputUp()
    {
        if (m_velocity.y > m_minJumpVelocity)
        {
            m_velocity.y = m_minJumpVelocity;
        }
    }



    public void AddJumpForce(float p_forceY)
    {
        m_velocity.y = p_forceY;
    }

    protected void HandleWallSliding()
    {
        m_wallDirX = (m_raycastCol.ColInfo.IsLeftTouched) ? -1 : 1;
        m_bIsWallSliding = false;
        if ((m_raycastCol.ColInfo.IsLeftTouched || m_raycastCol.ColInfo.IsRightTouched) && !m_raycastCol.ColInfo.IsBelowTouched && m_velocity.y < 0)
        {
            m_bIsWallSliding = true;

            if (m_velocity.y < -m_wallSlideSpeedMax)
            {
                m_velocity.y = -m_wallSlideSpeedMax;
            }

            if (m_timeToWallUnstick > 0)
            {
                m_velocitySmoothingX = 0;
                m_velocity.x = 0;

                if (m_playerDirInput.x != m_wallDirX && m_playerDirInput.x != 0)
                {
                    m_timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    m_timeToWallUnstick = m_wallStickTime;
                }
            }
            else
            {
                m_timeToWallUnstick = m_wallStickTime;
            }

        }

    }

    protected void CalculateVelocity()
    {
        float targetVelocityX = m_playerDirInput.x * m_moveSpeed;
        m_velocity.x = Mathf.SmoothDamp(m_velocity.x, targetVelocityX, ref m_velocitySmoothingX, (m_raycastCol.ColInfo.IsBelowTouched) ? m_accelerationTimeGrounded : m_accelerationTimeAirborne);
        m_velocity.y += m_gravity * Time.deltaTime;
    }

    private void ThrownBehavior()
    {
        m_velocity.x = m_beingThrownForceX;

        if (m_raycastCol.ColInfo.IsBelowTouched)
        {
            m_bIsBeingThrown = false;
        }
    }

    public void GetThrown(float p_forceX, float p_forceY)
    {
        m_beingThrownForceX = p_forceX;
        m_raycastCol.ColInfo.IsBelowTouched = false;
        m_bIsBeingThrown = true;
        AddJumpForce(p_forceY);
    }

    public virtual void DamageSelf()
    {
        if (m_bIsDamaged) { return; }

        m_curLife--;

        if (m_curLife <= 0)
        {
            TriggerDeath();
            return;
        }

        m_bIsDamaged = true;
        GameflowManager.instance.DelayAction(() => { m_bIsDamaged = false; }, m_damagedTimer);
    }

    public virtual void DamageSelf(Transform p_objectBumpTo)
    {
        if (m_bIsDamaged) { return; }

        DamageSelf();
        StopMoveX();
        GetThrown(Mathf.Sign(transform.position.x - p_objectBumpTo.transform.position.x) * 7.0f, 5.0f);
    }

    public void StopMoveX()
    {
        m_velocity.x = 0.0f;
    }
}