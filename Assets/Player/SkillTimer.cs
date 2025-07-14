using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTimer : MonoBehaviour
{
	[SerializeField] float skillCoolDown;
	[SerializeField] SkillCoolDown image;

	private float seconds;

	private bool m_isCoolDown;

    void Start()
    {
		m_isCoolDown = false;
		seconds = 0f;
	}


    void Update()
    {
		
		if (!m_isCoolDown)   return;
		
		image.UpdateCoolDown(UpdateTimer());
	}

	float UpdateTimer()
	{
		seconds += Time.deltaTime;

		float timer = seconds / skillCoolDown;

		return timer;
	}

	public void CoolDown(bool coolDown)
	{
		m_isCoolDown = coolDown;

		if(!m_isCoolDown)
		{
			seconds = 0f;
		}
	}
}
