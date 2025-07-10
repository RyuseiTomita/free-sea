using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GhostMove : MonoBehaviour
{

	[SerializeField] float m_ghostDeathTime; // —H—ì‚ªŽ€‚Ê‚Ü‚Å‚ÌŽžŠÔ
	[SerializeField] GameObject m_player;
	[SerializeField] Transform m_ghostPos;
	[SerializeField] GameObject m_grave;

	NavMeshAgent m_agent;

	// Start is called before the first frame update
	void Start()
	{
		m_agent = GetComponent<NavMeshAgent>();
	}

	// Update is called once per frame
	void Update()
	{
		GhostAttack();
	}

	public void SetPlayer(GameObject player)
	{
		m_player = player;
	}

	private void GhostAttack()
	{
		m_ghostDeathTime -= Time.deltaTime;

		if (m_ghostDeathTime <= 0)
		{
			Instantiate(m_grave, m_ghostPos.transform.position, Quaternion.identity);
			Destroy(this.gameObject);
		}
		else
		{
			m_agent.destination = m_player.transform.position;
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			Instantiate(m_grave, m_ghostPos.transform.position, Quaternion.identity);
			Destroy(this.gameObject);
		}
	}

	public void PlayerAttackHit()
	{
		Destroy(this.gameObject);
	}
}
