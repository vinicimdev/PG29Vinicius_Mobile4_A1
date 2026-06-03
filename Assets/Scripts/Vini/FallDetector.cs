using UnityEngine;

public class FallDetector : MonoBehaviour
{
    [SerializeField] private float _timePenalty = 5f;
    [SerializeField] private BallController _ball;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _ball.ResetBall();

        // Apply time penalty via GameManager (optional, only if game is running)
        // GameManager doesn't expose time directly, so we use a simple event approach
        OnBallFell?.Invoke(_timePenalty);
    }

    /// <summary>Fires when the ball falls, passing the time penalty amount.</summary>
    public static event System.Action<float> OnBallFell;
}
