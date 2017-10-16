using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPlatformerBehavior : PlatformerBehavior
{
    [SerializeField] private ParticleSystem m_deathParticle;
    private SpriteRenderer m_spriteRend;
    private const float m_wallBorderX = -6.95f;
    private bool m_bIsAttackingWall = false;

    protected override void Awake()
    {
        m_spriteRend = GetComponent<SpriteRenderer>();
        base.Awake();
    }

    protected override void TriggerDeath()
    {
        KillSaveManager.Instance.AddKill(1);
        GameflowManager.instance.OnEnemyDeactivate();
        m_spriteRend.enabled = false;
        m_raycastCol.BoxCol.enabled = false;
        m_deathParticle.Play();

        GameflowManager.instance.DelayAction(()=> 
        {
            ToggleCharacter(false);
        }, 1.0f);
    }

    public override void Reset()
    {
        base.Reset();

        m_deathParticle.Stop();
        CancelInvoke("DamageWall");
        m_bIsAttackingWall = false;
        m_curLife = m_maxLife;
        m_bIsDamaged = false;
        m_spriteRend.enabled = true;
        m_raycastCol.BoxCol.enabled = true;
    }

    protected void OnTriggerEnter2D(Collider2D p_col)
    {
        if (p_col.gameObject.tag == "PlayerAttack")
        {
            DamageSelf(p_col.transform);
        }
        if (p_col.gameObject.tag == "Player")
        {
            p_col.GetComponent<PlayerPlatformerBehavior>().DamageSelf(transform);
        }
    }

    protected override void Update()
    {
        if (!m_bIsBeingThrown)
        {
            m_playerDirInput = Vector3.left;
        }

        if (!m_bIsAttackingWall && m_raycastCol.ColInfo.IsBelowTouched && transform.position.x <= m_wallBorderX)
        {
            m_bIsAttackingWall = true;
            enabled = false;
            InvokeRepeating("DamageWall", 0.0f, 2.0f);
        }
        else if (m_bIsAttackingWall && transform.position.x > m_wallBorderX)
        {
            m_bIsAttackingWall = false;
            CancelInvoke("DamageWall");
        }

        base.Update();
; }

    public void DamageWall()
    {
        WallHealth.Instance.DamageWall(10);
    }

    public override void DamageSelf()
    {
        base.DamageSelf();
        CameraEffects.instance.ScreenShake();
    }
}
