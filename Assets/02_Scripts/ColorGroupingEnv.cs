using System.Collections.Generic;
using UnityEngine;

public class ColorGroupingEnv : MonoBehaviour
{
    public ColorGroupingAgent agentPrefab;
    public int redCount = 3;
    public int greenCount = 3;
    public int blueCount = 3;
    public float areaSize = 5.0f;

    private List<ColorGroupingAgent> agents = new List<ColorGroupingAgent>();

    private void Start()
    {
        // 각각 N개씩 스폰
        SpawnAgents(redCount, ColorType.Red);
        SpawnAgents(greenCount, ColorType.Green);
        SpawnAgents(blueCount, ColorType.Blue);
        
        // 각 에이전트에 "allAgents" 리스트를 할당
        foreach (var ag in agents)
        {
            ag.allAgents = agents;
        }
    }

    private void SpawnAgents(int count, ColorType ctype)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(UnityEngine.Random.Range(-areaSize, areaSize), 0f, UnityEngine.Random.Range(-areaSize, areaSize));
            var newAgent = Instantiate(agentPrefab, pos, Quaternion.identity, transform);
            newAgent.colorType = ctype;
            newAgent.areaSize = areaSize;
            agents.Add(newAgent);
        }
    }
}
