using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour {

	public static CameraEffects instance;

	private bool bIsShaking;

	private Transform m_mainCameraTransform;

	private GameObject m_vignetteTop;
	private GameObject m_vignetteBottom;

	private bool m_bIsVignetteActive;
	private bool m_bIsVignetteMoving;

	private SpriteRenderer m_screenFlashSprite;

	void Awake()
	{
		if(instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this.gameObject);
		}

		bIsShaking = false;

		m_mainCameraTransform = Camera.main.transform;

		m_vignetteTop = transform.Find("VignetteTop").gameObject;
		m_vignetteBottom = transform.Find("VignetteBottom").gameObject;

		m_bIsVignetteActive = false;
		m_bIsVignetteMoving = false;

		m_screenFlashSprite = transform.Find("ScreenBlock").GetComponent<SpriteRenderer>();
	}

	public void ScreenShake()
	{
		if(bIsShaking)
		{
			return;
		}

        bIsShaking = true;

        InvokeRepeating("CameraShake", 0, 0.0025f);
		Invoke("StopShaking", 0.25f);
	}

	void CameraShake()
	{
		float forceAmount = 0.025f;

		float quakeAmtX = Random.value * forceAmount * 2f - forceAmount;
		float quakeAmtY = Random.value * forceAmount * 2f - forceAmount;

		Vector3 cameraPos = m_mainCameraTransform.localPosition;

		cameraPos.x += quakeAmtX;
		cameraPos.y += quakeAmtY;

		m_mainCameraTransform.localPosition = cameraPos;
	}

	void StopShaking()
	{
        bIsShaking = false;
        CancelInvoke("CameraShake");

		Vector3 cameraPos = m_mainCameraTransform.localPosition;

		cameraPos.x = 0f;
		cameraPos.y = 0f;

		m_mainCameraTransform.localPosition = cameraPos;
	}

	public void ToggleVignette()
	{
		if(m_bIsVignetteMoving) return;

		m_bIsVignetteMoving = true;

		StartCoroutine(vignetteCoroutine());
	}

	IEnumerator vignetteCoroutine()
	{
		Vector3 currentTopPosition = m_vignetteTop.transform.localPosition;
		Vector3 currentBotPosition = m_vignetteBottom.transform.localPosition;

		float currentTopY = currentTopPosition.y;
		float currentBotY = currentBotPosition.y;

		float targetTopY = (m_bIsVignetteActive) ? 7f : 5f;
		float targetBotY = (m_bIsVignetteActive) ? -7f : -5f;

		float seconds = 0f;
		float duration = 0.45f;

		while(seconds <= duration)
		{
			seconds += Time.deltaTime;

			currentTopPosition.y = Mathf.SmoothStep(currentTopY, targetTopY, seconds/duration);
			currentBotPosition.y = Mathf.SmoothStep(currentBotY, targetBotY, seconds/duration);

			m_vignetteTop.transform.localPosition = currentTopPosition;
			m_vignetteBottom.transform.localPosition = currentBotPosition;

			yield return new WaitForSeconds(Time.deltaTime);
		}

		m_bIsVignetteActive = !m_bIsVignetteActive;

		m_bIsVignetteMoving = false;

		yield return null;
	}

	public void ScreenFlash(Color p_color)
	{
		StartCoroutine(screenFlashCoroutine(p_color));
	}

	IEnumerator screenFlashCoroutine(Color p_color)
	{
		Color targetColor = p_color;
		targetColor.a = 0f;

		float seconds = 0f;
		float duration = 0.1f;

		while(seconds < duration)
		{
			seconds += Time.deltaTime;

			targetColor.a = Mathf.SmoothStep(0f, 0.45f, seconds/duration);

			m_screenFlashSprite.color = targetColor;

			yield return new WaitForSeconds(Time.deltaTime);
		}

		seconds = 0f;

		while(seconds < duration)
		{
			seconds += Time.deltaTime;

			targetColor.a = Mathf.SmoothStep(0.45f, 0f, seconds/duration);

			m_screenFlashSprite.color = targetColor;

			yield return new WaitForSeconds(Time.deltaTime);
		}
	}
}
