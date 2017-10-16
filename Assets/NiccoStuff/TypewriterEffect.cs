using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Text))]
public class TypewriterEffect : MonoBehaviour
{
	private Text m_text;
    private IEnumerator m_typewriterCor = null;

    private void Awake ()
    {
		m_text = GetComponent<Text>();
	}

	public void ShowText(string p_text)
	{
		m_text.text = "";

        if(m_typewriterCor != null){ StopCoroutine(m_typewriterCor); }
        m_typewriterCor = TypewriterCor(p_text);
        StartCoroutine(m_typewriterCor);
	}

	public void ClearText()
	{
        if(m_text != null)
        {
            m_text.text = "";
        }
	}

	IEnumerator TypewriterCor(string p_text)
	{
		int stringLength = p_text.Length;

		string targetString = p_text;
		string currentDisplayString = "";

		for(int i = 0; i < stringLength; i++)
		{
			currentDisplayString += targetString[i];
			m_text.text = currentDisplayString;
			yield return new WaitForSeconds(0.025f);
		}

		yield return null;
	}
}
