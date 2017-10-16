using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPlatformerBehavior : PlatformerBehavior
{
    [SerializeField] ParticleSystem m_damageParticle;
    private PlayerRaycastCol m_playerRaycastCol;
    private Transform m_pickedUpTrans;
    private SpriteRenderer m_pickedUpSpriteRend;

    protected override void Awake()
    {
        base.Awake();
        m_playerRaycastCol = (PlayerRaycastCol)m_raycastCol;
    }

    protected override void TriggerDeath()
    {
        SceneManager.LoadScene("GameOverScene");
    }

    public override void Reset()
    {
        base.Reset();
        m_damageParticle.Stop();
        HUDManager.Instance.HealthText.text = m_curLife.ToString();
    }

    public void Attack()
    {
        if (!m_playerRaycastCol.IsAttackColActive)
        {
            StartCoroutine(AttackCour());
        }

        if (m_anim != null)
        {
            m_anim.SetTrigger("AttackTrig");
        }
    }
    IEnumerator AttackCour()
    {
        m_playerRaycastCol.ToggleAttackCol(true);
        yield return new WaitForSeconds(0.1f);
        m_playerRaycastCol.ToggleAttackCol(false);
    }

    public void PickThrow()
    {
        if (m_pickedUpTrans != null)
        {
            EnemyPlatformerBehavior hitBehavior = m_pickedUpTrans.GetComponent<EnemyPlatformerBehavior>();
            if (hitBehavior != null)
            {
                hitBehavior.enabled = true;
            }

            m_pickedUpTrans.SetParent(null);

            hitBehavior.GetThrown(m_velocity.x + (5.0f * m_raycastCol.ColInfo.FaceDir), 5.0f);

            m_pickedUpSpriteRend.sortingLayerName = "NPC";
            m_pickedUpSpriteRend.sortingOrder = 0;

            BoxCollider2D col = m_pickedUpTrans.GetComponent<BoxCollider2D>();
            GameflowManager.instance.DelayAction(() => { col.enabled = true; }, 0.3f);

            m_pickedUpSpriteRend = null;
            m_pickedUpTrans = null;
        }
        else
        {
            m_playerRaycastCol.EnablePickUpRays(ref m_pickedUpTrans);

            if(m_pickedUpTrans != null)
            {
                m_pickedUpSpriteRend = m_pickedUpTrans.GetComponent<SpriteRenderer>();
                m_pickedUpSpriteRend.sortingLayerName = "Player";
                m_pickedUpSpriteRend.sortingOrder = 10;
            }
        }


        m_anim.SetBool("bIsPickingUp", m_pickedUpTrans != null);

    }

    public override void DamageSelf()
    {
        base.DamageSelf();
        m_damageParticle.Play();
        CameraEffects.instance.ScreenFlash(Color.red);
        HUDManager.Instance.HealthText.text = m_curLife.ToString();
    }
}
