using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyObjectPool : MonoInstance<EnemyObjectPool>
{
    private const int m_poolSize = 100;

    [SerializeField] private GameObject m_enemyPrefab;
    List<EnemyPlatformerBehavior> m_enemyPool = new List<EnemyPlatformerBehavior>();

    protected override void Awake()
    {
        base.Awake();
        GeneratePool();
    }

    private void GeneratePool()
    {
        for(int i = 0; i < m_poolSize; i++)
        {
            GameObject enemyGObj = (GameObject)Instantiate(m_enemyPrefab);
            m_enemyPool.Add(enemyGObj.GetComponent<EnemyPlatformerBehavior>());
            enemyGObj.transform.parent = transform;
            enemyGObj.SetActive(false);
        }
    }

    public void SpawnFromPool()
    {
        EnemyPlatformerBehavior enemy = m_enemyPool.Where(ep => !ep.gameObject.activeSelf).FirstOrDefault();

        if(enemy != null)
        {
            enemy.ToggleCharacter(true, new Vector2(10.0f, -3.76f));
        }
    }
}
