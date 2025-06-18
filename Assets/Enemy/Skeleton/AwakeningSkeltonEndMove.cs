using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AwakeningSkeltonEndMove : MonoBehaviour
{
	[SerializeField] GameObject m_player;
	[SerializeField] GameObject[] m_effect;
	[SerializeField] Transform m_skeleton;
	[SerializeField] AudioClip[] m_skeletonCilps;
	[SerializeField] GameObject m_grave;

	[SerializeField] float m_skeltonDeathTime; // ä[çúÇ™éÄÇ Ç‹Ç≈ÇÃéûä‘

	NavMeshAgent m_agent;

	void Start()
	{
		m_agent = GetComponent<NavMeshAgent>();
	}

	void Update()
	{
		SkeltonAttack();
	}

	public void SetPlayer(GameObject player)
	{
		m_player = player;
	}

	public void SkeltonAttack()
	{
		m_skeltonDeathTime -= Time.deltaTime;

		if (m_skeltonDeathTime <= 0)
		{
			SoundEffect.Play2D(m_skeletonCilps[0]);
			Instantiate(m_grave, m_skeleton.transform.position, Quaternion.identity);
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
			Instantiate(m_effect[0], m_skeleton.transform.position + Vector3.up, Quaternion.identity);
			Instantiate(m_grave, m_skeleton.transform.position, Quaternion.identity);
			Destroy(this.gameObject);
		}
	}

	public void PlayerAttackHit()
	{
		SoundEffect.Play2D(m_skeletonCilps[2]);
		Instantiate(m_effect[1], m_skeleton.transform.position + Vector3.up, Quaternion.identity);
		Destroy(this.gameObject);
	}
}
