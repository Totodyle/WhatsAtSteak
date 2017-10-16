using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashScreen : MonoBehaviour {

	private Image m_splashScreen;

	// Use this for initialization
	void Start () {

		m_splashScreen = GameObject.Find("SplashImage").GetComponent<Image>();

		StartCoroutine(FadeOutCoroutine());
	}

	IEnumerator FadeOutCoroutine()
	{
		yield return new WaitForSeconds(1.5f);

		float seconds = 0f;
		float duration = 1f;

		Color targetColor = m_splashScreen.color;
		targetColor.a = 1f;

		while(seconds < duration)
		{
			seconds += Time.deltaTime;

			targetColor.a = Mathf.Lerp(1f, 0f, seconds/duration);

			m_splashScreen.color = targetColor;

			yield return new WaitForSeconds(Time.deltaTime);
		}

		SceneManager.LoadScene("TitleScene");

	}
}
