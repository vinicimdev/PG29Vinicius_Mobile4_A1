using UnityEngine;

public class ScoreZone : MonoBehaviour
{
    [SerializeField] private int _pointValue = 10;

    /// <summary>Fired when ball touches this zone. Passes point value.</summary>
    public event System.Action<int> OnCollected;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        OnCollected?.Invoke(_pointValue);

        gameObject.SetActive(false);
    }

    /// <summary>Re-enables this zone for a new round.</summary>
    public void Reset() => gameObject.SetActive(true);
}
