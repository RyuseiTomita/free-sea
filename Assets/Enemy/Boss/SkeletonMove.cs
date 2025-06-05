using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SkeletonMove : MonoBehaviour
{
	[SerializeField] GameObject m_boss;
	[SerializeField] GameObject m_player;
	[SerializeField] GameObject m_effect;
	[SerializeField] Transform m_skeleton;
	[SerializeField] AudioClip[] m_skeletonCilps;
	 
	private float m_skeltonDeathTime; // ä[çúÇ™éÄÇ Ç‹Ç≈ÇÃéûä‘

	NavMeshAgent m_agent;
	
	void Start()
    {
		m_agent = GetComponent<NavMeshAgent>();
		m_skeltonDeathTime = 10f;

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
			SoundEffect.Play2D(m_skeletonCilps[1]);
			Instantiate(m_effect, m_skeleton.transform.position + Vector3.up, Quaternion.identity);
			Destroy(this.gameObject);
		}
	}
}
