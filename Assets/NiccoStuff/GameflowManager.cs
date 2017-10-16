using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameflowManager : MonoBehaviour
{
	public enum GameState
	{
		CUTSCENE,
		ACTIVE,
		GAME_OVER
	};

	//What wave is the player currently on? Start count from 0.
	private int m_currentWaveCount;

	//How many waves does the game have?
	private int m_totalWaves;

	//Switch this out with actual string/dialogue data
	private string[] m_sampleStringArray;

	//Self explanatory
	private GameState m_currentGameState;

	//Each game state has their own update loop
	private delegate void updateFunc();
	private Dictionary<GameState, updateFunc> m_updateDictionary = new Dictionary<GameState, updateFunc>();

	//How long the wave lasts in seconds.
	private float[] m_waveDurations;
	private float m_currentWaveDuration;

	//Spawn an enemy every x seconds.
	private float[] m_spawnIntervals;
	private float m_currentSpawnInterval;

	//How long do we wait before wave starts?
	private float m_waveDowntime;

	//How many enemies are still alive?
	private int m_activeEnemyCount;

	//True if wave timer is done and there are still active enemies
	private bool bIsOvertime;

	public static GameflowManager instance;

    [SerializeField] private GameObject m_openDoor;
    [SerializeField] private GameObject m_bb;
    [SerializeField] private Transform m_bbShowPosRef;
    [SerializeField] private Transform m_bbHidePosRef;
    private bool m_bIsBbActive = false;
    private float m_bbShowHideTime = 0.8f;


    [SerializeField] private GameObject m_wurst;
    [SerializeField] private Transform m_wurstShowPosRef;
    [SerializeField] private Transform m_wurstHidePosRef;
    private bool m_bIsWurstActive = false;
    private float m_wurstShowHideTime = 0.8f;

    public GameState CurrentGameState
    {
        get { return m_currentGameState; }
    }

    private void Awake()
	{
		if(instance != null)
		{
			Destroy(this.gameObject);
		}
		else
		{
			instance = this;
		}
					
	}

	private void Start ()
    {

		//We start the game at a cutscene
		m_currentGameState = GameState.CUTSCENE;

		m_currentWaveCount = 0;

		//4 for now. Subject to change.
		m_totalWaves = 4;

		//These are placeholders
		m_sampleStringArray = new string[]{
			"I order you to kill these invaders.",
			"Good job. Now go kill some more.",
			"Do not feel mercy for these \"people\"",
			"The last of them are approaching. Slit their throats."
		};

		//Variable initializations
		m_updateDictionary = new Dictionary<GameState, updateFunc>();

		m_updateDictionary.Add(GameState.CUTSCENE, CutsceneUpdate);
		m_updateDictionary.Add(GameState.ACTIVE, ActiveUpdate);
		m_updateDictionary.Add(GameState.GAME_OVER, GameOverUpdate);

		m_waveDurations = new float[]{ 15f, 20f, 20f, 25f };
		m_currentWaveDuration = m_waveDurations[0];

		m_spawnIntervals = new float[]{4f, 3f, 2f, 1f};
		m_currentSpawnInterval = m_spawnIntervals[0];

		m_waveDowntime = 3f;

		m_activeEnemyCount = 0;

		bIsOvertime = false;


        //Get the show rolling!
        HUDManager.Instance.ToggleHUD(false);
        Invoke("TriggerDialogue", 0.5f);
    }

	void TriggerDialogue()
	{
        CameraEffects.instance.ToggleVignette();

        ToggeleBb(true);
        DelayAction(() => { DialogueCanvas.Instance.OpenDialogueBox(m_currentWaveCount, true, () => { StartWave(); }); }, 0.7f);
	}

	public void StartWave()
	{
        ToggeleBb(false);
        DelayAction(() => { HUDManager.Instance.ToggleHUD(true); }, 0.7f);
        CameraEffects.instance.ToggleVignette();
        m_currentGameState = GameState.ACTIVE;

        Debug.Log("Start wave: " + m_currentWaveCount);
        PlayerInput.Instance.enabled = true;

		//Set duration of wave and enemy spawn intervals
		m_currentWaveDuration = m_waveDurations[m_currentWaveCount];
		m_currentSpawnInterval = m_spawnIntervals[m_currentWaveCount];

		bIsOvertime = false;

        if (EnemyObjectPool.Instance == null)
        {
            return;
        }

        Invoke("EndWave", m_currentWaveDuration);
        Invoke("TriggerWindowSpeach", m_currentWaveDuration/2);
		InvokeRepeating("SpawnEnemy", 0f, m_currentSpawnInterval);
	}

	void SpawnEnemy()
	{
        EnemyObjectPool.Instance.SpawnFromPool();

        m_activeEnemyCount++;
	}

	void EndWave()
	{
		//Stop spawning enemies
		CancelInvoke("SpawnEnemy");

		//We can't end the wave if there are still enemies.
		if(m_activeEnemyCount > 0)
		{
			Debug.Log("Wave timer is up, entering overtime.");

			bIsOvertime = true;
			return;
		}

		Debug.Log("End of wave " + m_currentWaveCount);

		m_currentWaveCount++;

		m_currentGameState = GameState.CUTSCENE;

		//Check if the game ends
		if(m_currentWaveCount >= m_totalWaves)
		{
			Debug.Log("Game over!");
			m_currentGameState = GameState.GAME_OVER;
			//Fetch results from whatever manages score data. True for now. Change later.
			TriggerEnding(true);
		}
		else
		{
            DelayAction(() =>
            {
                HUDManager.Instance.ToggleHUD(false);
                Invoke("TriggerDialogue", 0.7f);
            }, 1.5f);
        }
	}

	void CutsceneUpdate()
	{
		/*
		 * Ideally, we add to the condition if all the respective dialogue has been displayed.
		 */

		//placeholder key. this behavior is subject to change.
		//if(Input.GetKeyDown(KeyCode.Space))
		//{
		//	m_currentGameState = GameState.ACTIVE;

		//	//Placeholder call.
		//	//TestbedScript.instance.textSample.ClearText();

		//	Invoke("StartWave", m_waveDowntime);
		//}
	}

    void TriggerWindowSpeach()
    {
        ToggleWurst(true);
        DelayAction(()=> 
        {
            DialogueCanvas.Instance.OpenDialogueBox(m_currentWaveCount + 4, false, () => 
            {
                ToggleWurst(false);
            });
        }, m_wurstShowHideTime);
    }


    void TriggerEnding(bool p_bIsBad)
	{
		Debug.Log("Ending trigger!");
        SceneManager.LoadScene("Credits");
	}

    public void OnEnemyDeactivate()
    {
        m_activeEnemyCount--;
    }

	void ActiveUpdate()
	{
		//Constantly check if enemies are gone to end wave
		if(bIsOvertime && m_activeEnemyCount <= 0)
		{
			bIsOvertime = false;
			EndWave();
		}
	}

	void GameOverUpdate()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		m_updateDictionary[m_currentGameState]();
	}

    public void DelayAction(Action p_action, float m_delay)
    {
        StartCoroutine(DelayActionCour(p_action, m_delay));
    }

    IEnumerator DelayActionCour(Action p_action, float m_delay)
    {
        yield return new WaitForSeconds(m_delay);
        p_action();
    }

    private void ToggleWurst(bool p_bIsActive)
    {
        m_bIsWurstActive = p_bIsActive;
        if (m_bIsWurstActive)
        {
            iTween.MoveTo(m_wurst, iTween.Hash("x", m_wurstShowPosRef.position.x, "time", m_wurstShowHideTime, "easeType", iTween.EaseType.easeOutQuad));
        }
        else
        {
            iTween.MoveTo(m_wurst, iTween.Hash("x", m_wurstHidePosRef.position.x, "time", m_wurstShowHideTime, "easeType", iTween.EaseType.easeOutQuad));
        }
    }
    private void ToggeleBb(bool p_bIsActive)
    {
        m_bIsBbActive = p_bIsActive;
        if (m_bIsBbActive)
        {
            m_openDoor.SetActive(true);
            iTween.MoveTo(m_bb, iTween.Hash("x", m_bbShowPosRef.position.x, "y", m_bbShowPosRef.position.y, "time", m_bbShowHideTime, "easeType", iTween.EaseType.easeOutQuad));
            iTween.ValueTo(m_bb, iTween.Hash("from", 0.0f, "to", 1.0f, "time", m_bbShowHideTime, "easeType", iTween.EaseType.easeOutQuad, "onupdatetarget", this.gameObject, "onupdate", "SetBBAlpha"));
        }
        else
        {
            Debug.Log("sdfa1");
            iTween.MoveTo(m_bb, iTween.Hash("x", m_bbHidePosRef.position.x, "y", m_bbHidePosRef.position.y, "time", m_bbShowHideTime, "easeType", iTween.EaseType.easeOutQuad));
            iTween.ValueTo(m_bb, iTween.Hash("from", 1.0f, "to", 0.0f, "time", m_bbShowHideTime, "easeType", iTween.EaseType.easeOutQuad, "onupdatetarget", this.gameObject, "onupdate", "SetBBAlpha"));
            DelayAction(() => { m_openDoor.SetActive(false); }, m_bbShowHideTime);
        }
    }

    public void SetBBAlpha(float p_a)
    {
        Color tempColor = m_bb.GetComponent<SpriteRenderer>().color;
        tempColor.a = p_a;
        m_bb.GetComponent<SpriteRenderer>().color = tempColor;
    }
}
