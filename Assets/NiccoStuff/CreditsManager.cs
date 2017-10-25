using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
	private string[] m_creditsStrings;

	private int m_currentStringIdx;

	private Text m_creditsText;

	private void Start()
    {
		m_currentStringIdx = 0;

		m_creditsStrings = new string[] 
        {
			"What's At Steak\n\nby\nSecret Steak",
			"Art and Design\n\nCherish Socro",
			"Developers\n\nDyle Ouano\nNiccolo Manahan",
			"Music\n\n\"Caketown\"\nBy Matthew Pablo\nLicense: CC-BY-SA 3.0",
			"\n\n\"adventure\"\nBy syncopika\n\n\"Snowland Town\"\nBy Matthew Pablo\nLicense: CC-BY 3.0",
			"\n\n\n\"Castlecall\"\nBy Alexandr Zhelanov\n\n\"melancholy piano\"\nBy syncopika\nLicense: CC-BY 3.0",
			"Special Thanks\n\nHannah Donato\nHachi Donato\nDera D. Doug\nRon Schaffner\nCameron Russell\nSilverio Reynoso\nGab Dacanay\nOur",
			"Thank you for playing.",
			"You have blood on your hands."
		};

		m_creditsText = GameObject.Find("CreditsText").GetComponent<Text>();

		StartCoroutine(creditsCoroutine());
	}

	IEnumerator creditsCoroutine()
	{
		while(m_currentStringIdx < m_creditsStrings.Length)
		{
			m_creditsText.text = m_creditsStrings[m_currentStringIdx];
			m_currentStringIdx++;
			yield return new WaitForSeconds(3f);
		}
		yield return null;
	}

    private void Update()
    {
        if (Input.anyKey)
        {
            SceneManager.LoadScene("TitleScene");
        }
    }
}
