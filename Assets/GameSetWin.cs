using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameSetWin : MonoBehaviour
{
	private bool m_bossDeath;

	private float speed = 0.005f;
	private float alfa;
	private float red, green, blue;

	// Start is called before the first frame update
	void Start()
    {
		red = GetComponent<RawImage>().color.r;
		green = GetComponent<RawImage>().color.g;
		blue = GetComponent<RawImage>().color.b;

		m_bossDeath = false;
	}

    // Update is called once per frame
    void Update()
    {
		if(m_bossDeath)
		{
			GetComponent<RawImage>().color = new Color(red, green, blue, alfa);
			alfa += speed;
		}
	}

	public void PlayerWin(bool win)
	{
		m_bossDeath = win;
	}
}
