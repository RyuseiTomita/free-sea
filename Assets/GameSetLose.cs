using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;
using UnityEngine.UI;

public class GameSetLose : MonoBehaviour
{
	private bool m_playerDeath;

	private float speed = 0.005f;
	private float alfa;
	private float red, green, blue;

	// Start is called before the first frame update
	void Start()
	{
		red = GetComponent<RawImage>().color.r;
		green = GetComponent<RawImage>().color.g;
		blue = GetComponent<RawImage>().color.b;

		m_playerDeath = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (m_playerDeath)
		{
			GetComponent<RawImage>().color = new Color(red, green, blue, alfa);
			alfa += speed;
		}

	}

	public void PlayerLose(bool lose)
	{
		m_playerDeath = lose;
		
	}
}
