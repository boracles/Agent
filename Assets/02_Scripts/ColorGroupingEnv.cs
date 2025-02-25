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
        // 1. 빨강/초록/파랑 에이전트 스폰
        SpawnAgents(redCount, ColorType.Red);
        SpawnAgents(greenCount, ColorType.Green);
        SpawnAgents(blueCount, ColorType.Blue);
        
        // 2. 생성된 에이전트 목록(agents)을 모두 순회하여 allAgents 할당
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
            
            // **색상 지정**: MeshRenderer 또는 Renderer 컴포넌트를 찾아서 색상 변경
            // (1) 직접 Color 값을 할당
            var rend = newAgent.GetComponent<MeshRenderer>();
            if (rend != null)
            {
                switch (ctype)
                {
                    case ColorType.Red:
                        rend.material.color = Color.red;
                        break;
                    case ColorType.Green:
                        rend.material.color = Color.green;
                        break;
                    case ColorType.Blue:
                        rend.material.color = Color.blue;
                        break;
                }
            }
            
            agents.Add(newAgent);
        }
    }
}
