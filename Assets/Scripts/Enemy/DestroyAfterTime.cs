using UnityEngine;

/// <summary>
/// Initialize()가 호출되면 지정된 시간(초) 뒤에
/// 이 게임 오브젝트를 스스로 파괴합니다.
/// </summary>
public class DestroyAfterTime : MonoBehaviour
{
    public void Initialize(float duration)
    {
        // duration (초) 뒤에 이 게임 오브젝트를 파괴하도록 예약
        Destroy(gameObject, duration);
    }
}