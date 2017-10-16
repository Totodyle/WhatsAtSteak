using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestbedScript : MonoBehaviour {

	[SerializeField] private TypewriterEffect m_typewriterTextSample;

	public TypewriterEffect textSample
	{
		get
		{
			return m_typewriterTextSample;
		}
	}

	public static TestbedScript instance;

	private string[] m_sampleStringArray = new string[]{
		"Everyday, we stray further from God",
		"Show me the money",
		"Lorem all over my ipsum",
		"Ryujin no ken wo kurae",
		"Push the goddamn payload!",
		"I invoke my right against self-discrimination",
		"Send nudes.",
		"What?",
		"Stop testing me!"
	};

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
	}

	// Use this for initialization
	void Start () {


		
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.S))
		{
			CameraEffects.instance.ScreenShake();
		}

		if(Input.GetKeyDown(KeyCode.T))
		{
			m_typewriterTextSample.ShowText(m_sampleStringArray[Random.Range(0, m_sampleStringArray.Length)]);
		}

		if(Input.GetKeyDown(KeyCode.V))
		{
			CameraEffects.instance.ToggleVignette();
		}
			
		if(Input.GetKeyDown(KeyCode.F))
		{
			CameraEffects.instance.ScreenFlash(Color.red);
		}
	}
}
