using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class ColorGroupingEnv : MonoBehaviour
{
    public ColorGroupingAgent agentPrefab;
    public int redCount = 10;
    public int greenCount = 10;
    public int blueCount = 10;
    public float areaSize = 40.0f;

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

    private void Update()
    {
        CheckGlobalColorRatio();
    }

    private void CheckGlobalColorRatio()
    {
        float thresholdGlobal = 0.40f;
        Dictionary<ColorType, int> colorCount = new Dictionary<ColorType, int>();
        colorCount[ColorType.Red] = 0;
        colorCount[ColorType.Red] = 0;
        colorCount[ColorType.Blue] = 0;

        foreach (var ag in agents)
        {
            colorCount[ag.colorType]++;
        }

        int total = agents.Count;
        float redRatio   = (float)colorCount[ColorType.Red]   / total;
        float greenRatio = (float)colorCount[ColorType.Green] / total;
        float blueRatio  = (float)colorCount[ColorType.Blue]  / total;
        
        // 임계 대중 체크
        if (redRatio > thresholdGlobal)
        {
            ConvertRandomAgentsTo(ColorType.Red, 0.10f);
        }

        if (greenRatio > thresholdGlobal)
        {
            ConvertRandomAgentsTo(ColorType.Green, 0.10f);
        }

        if (blueRatio > thresholdGlobal)
        {
            ConvertRandomAgentsTo(ColorType.Blue, 0.10f);
        }
    }
    
    // 일부 확률로 색 전환
    private void ConvertRandomAgentsTo(ColorType targetColor, float convertProb)
    {
        foreach (var ag in agents)
        {
            if (ag.colorType != targetColor)
            {
                float rand = UnityEngine.Random.value;
                if (rand < convertProb)
                {
                    ag.ChangeColor(targetColor);
                }
            }
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
