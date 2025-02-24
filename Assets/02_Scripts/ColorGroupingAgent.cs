using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public enum ColorType
{
    Red,
    Green,
    Blue
}

[RequireComponent(typeof(Rigidbody))]
public class ColorGroupingAgent : Agent
{
    [Header("Agent Settings")]
    public ColorType colorType;
    public float moveSpeed = 2f;

    // 환경 매니저가 모든 에이전트 리스트를 건네준다.
    public List<ColorGroupingAgent> allAgents;
    public float areaSize = 5.0f;
    
    private Rigidbody rb;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // 에피소드가 시작될 때 에이전트 위치/속도 초기화
        float randomX = Random.Range(-areaSize, areaSize);
        float randomZ = Random.Range(-areaSize, areaSize);
        transform.localPosition = new Vector3(randomX, 0f, randomZ);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 1. 에이전트의 현재 위치 (x, z) - 정규화(-1~1)
        var localPosition = transform.localPosition;
        sensor.AddObservation(localPosition.x/areaSize);
        sensor.AddObservation(localPosition.z/areaSize);

        // 2. 에이전트의 색 (One-hot)
        switch(colorType)
        {
            case ColorType.Red:
                sensor.AddObservation(new float[] {1, 0, 0});
                break;
            case ColorType.Green:
                sensor.AddObservation(new float[] {0,1,0});
                break;
            case ColorType.Blue:
                sensor.AddObservation(new float[] {0,0,1});
                break;
        }
        
        // 3. 주변 다른 에이전트들의 상대위치/색
        int maxObs = 3; // 관찰할 다른 에이전트 수
        var sortedAgents = new List<ColorGroupingAgent>(allAgents);
        sortedAgents.Remove(this);
        sortedAgents.Sort((a, b) => Vector3.Distance(a.transform.localPosition, transform.localPosition).CompareTo(Vector3.Distance(b.transform.localPosition, transform.localPosition)));

        for (int i = 0; i < maxObs; i++)
        {
            if (i < sortedAgents.Count)
            {
                var other = sortedAgents[i];
                Vector3 diff = other.transform.localPosition - transform.localPosition;
                // 상대 위치
                sensor.AddObservation(diff.x/areaSize);
                sensor.AddObservation(diff.z/areaSize);
                
                // 색상
                switch (other.colorType)
                {
                    case ColorType.Red: sensor.AddObservation(new float[] {1, 0, 0});
                        break;
                    case ColorType.Green:   sensor.AddObservation(new float[] {0, 1, 0});
                        break;
                    case ColorType.Blue: sensor.AddObservation(new float[] {0,0,1});
                        break;
                }
            }
            else
            {
                // 남은 관찰 대상이 없으면 0으로 채움
                sensor.AddObservation(0f);
                sensor.AddObservation(0f);
                sensor.AddObservation(new float[] {0,0,0});
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // 연속형 2개(moveX, moveZ)
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        
        // 이동
        Vector3 force = new Vector3(moveX, 0, moveZ) * moveSpeed;
        rb.AddForce(force, ForceMode.VelocityChange);
        
        // 맵 밖으로 벗어나면 패널티
        if(Mathf.Abs(transform.localPosition.x)>areaSize || Mathf.Abs(transform.localPosition.z)>areaSize)
        {
            AddReward(-0.01f);
        }
        
        // 보상 설계 : 같은 색상 가까이, 다른 색상은 멀리
        foreach (var other in allAgents)
        {
            if (other == this) continue;
            float dist = Vector3.Distance(transform.localPosition, other.transform.localPosition);
            if (dist < 1.5f)
            {
                if (other.colorType == this.colorType)
                    AddReward(0.01f); // 같은 색이면 +
                else
                    AddReward(-0.01f); // 다른 색이면 -
            }
        }

        // 스텝당 약간의 시간 페널티(지연 방지)
        AddReward(-0.0005f);
    }

    // 사람 테스트용 (Heuristic)
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var cont = actionsOut.ContinuousActions;
        cont[0] = Input.GetAxis("Horizontal");
        cont[1] = Input.GetAxis("Vertical");
    }

}
