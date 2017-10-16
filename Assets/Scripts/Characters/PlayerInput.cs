using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoInstance<PlayerInput>
{
    private PlayerPlatformerBehavior m_player;

    protected override void Awake()
    {
        base.Awake();
        m_player = GetComponent<PlayerPlatformerBehavior>();
    }

    private void Update()
    {
        m_player.PlayerDirInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if(Input.GetButtonDown("Jump")) { m_player.OnJumpInputDown(); }
        if(Input.GetButtonUp("Jump")) {m_player.OnJumpInputUp(); }

        if(Input.GetButtonDown("Attack")) { m_player.Attack(); }
        if (Input.GetButtonDown("PickThrow")) { m_player.PickThrow(); }
    }
}
