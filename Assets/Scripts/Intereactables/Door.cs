using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private const float m_switchTime = 10.0f;

    private bool m_bIsOpen = false;

    private void OnTriggerEnter2D(Collider2D p_col)
    {
        if (p_col.gameObject.tag == "Enemy")
        {
            p_col.GetComponent<EnemyPlatformerBehavior>().ToggleCharacter(false);
            p_col.GetComponent<EnemyPlatformerBehavior>().Reset();
            KillSaveManager.Instance.AddSave(1);
            GameflowManager.instance.OnEnemyDeactivate();
        }
    }

    private void ToggleDoor()
    {
        m_bIsOpen = !m_bIsOpen;
    }

    public void StartOpenCloseBehavior()
    {
        InvokeRepeating("ToggleDoor", 0.0f, m_switchTime);
    }
    public void StopOpenCloseBehavior()
    {
        CancelInvoke("ToggleDoor");
    }
}
