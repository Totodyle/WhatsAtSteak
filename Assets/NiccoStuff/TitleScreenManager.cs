using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour {

	private bool m_bIsTransitioning;

	private Image m_screenFadeImage;

	// Use this for initialization
	void Start () {

		m_bIsTransitioning = false;

		m_screenFadeImage = GameObject.Find("Fade").GetComponent<Image>();
		
	}

	public void PlayClick()
	{
		if(m_bIsTransitioning) return;
		//GameplayScene
		ScreenFade("GameplayScene");
	}

	public void CreditsClick()
	{
		if(m_bIsTransitioning) return;

		ScreenFade("Credits");
	}

    public void Menu()
    {
        if (m_bIsTransitioning) return;
        //GameplayScene
        ScreenFade("TitleScene");
    }

    void ScreenFade(string p_sceneName)
	{
		StartCoroutine(ScreenFadeCoroutine(p_sceneName));
	}

	IEnumerator ScreenFadeCoroutine(string p_sceneName)
	{
		float seconds = 0f;
		float duration = 1f;

		Color targetColor = m_screenFadeImage.color;
		targetColor.a = 0f;

		m_bIsTransitioning = true;

		while(seconds < duration)
		{
			seconds += Time.deltaTime;

			targetColor.a = Mathf.Lerp(0f, 1f, seconds/duration);

			m_screenFadeImage.color = targetColor;

			yield return new WaitForSeconds(Time.deltaTime);
		}

		SceneManager.LoadScene(p_sceneName);

		yield return null;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
