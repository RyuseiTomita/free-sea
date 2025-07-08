using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossSkillFade : MonoBehaviour
{
	private float speed = 0.01f;
	private float alfa;
	private float red, green, blue;

	// Start is called before the first frame update
	void Start()
	{
		red = GetComponent<RawImage>().color.r;
		green = GetComponent<RawImage>().color.g;
		blue = GetComponent<RawImage>().color.b;
	}

	// Update is called once per frame
	void Update()
	{
		GetComponent<RawImage>().color = new Color(red, green, blue, alfa);
		alfa += speed;
	}
}
