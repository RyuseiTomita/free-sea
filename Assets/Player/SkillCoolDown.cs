using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillCoolDown : MonoBehaviour
{
	Image skillCoolDown;

    // Start is called before the first frame update
    void Start()
    {
       skillCoolDown = GetComponent<Image>();
    }

	public void UpdateCoolDown(float coolDown)
	{
		skillCoolDown.fillAmount = coolDown;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
