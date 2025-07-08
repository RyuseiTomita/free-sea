using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurseSkeletonHead : MonoBehaviour
{
	[SerializeField] Transform m_player;

    void Start()
    {
        
    }

	public void SetPlayer(Transform player)
	{
		m_player = player;
	}

    void FixedUpdate()
    {
		SetLook();
	}

	void SetLook()
	{
		Vector3 forward = m_player.position - transform.position;
		forward.Scale(new Vector3(1, 0, 1));
		transform.rotation = Quaternion.LookRotation(forward.normalized);
	}
}
