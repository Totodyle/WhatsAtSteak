using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoInstance<GameManager>
{
    [SerializeField] private Camera m_mainCam;

    public Camera MainCam
    {
        get { return m_mainCam; }
    }
}