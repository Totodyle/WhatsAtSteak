using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WallHealth : MonoInstance<WallHealth>
{
    private int m_maxLife = 500;
    private int m_curLife;

    protected override void Awake()
    {
        m_curLife = m_maxLife;
    }

    public void DamageWall(int p_damage)
    {
        m_curLife -= p_damage;
        CameraEffects.instance.ScreenShake();

        if(m_curLife <= 0)
        {
            SceneManager.LoadScene("GameOverScene");
        }
    }
}
