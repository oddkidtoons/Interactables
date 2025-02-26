using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;

public class OrbCollector : MonoBehaviour
{
    [System.Serializable]
    public struct OrbType
    {
        public string orbName;
        public int requiredCount;
        public int subGoalCount;
        public TextMeshProUGUI uiText;
        public UnityEvent onAbilityGained;
        public UnityEvent onAbilitySubGained;
        public UnityEvent onAbilityLost;
        public string targetTag;
        public AudioClip collectSound;
        public GameObject orbPrefab;
    }

    public List<OrbType> orbTypes = new List<OrbType>();
    public float orbDespawnTimer = 5f;
    public float orbSpawnForce = 5f;
    public Transform orbSpawnPoint;

    public UnityEvent onAllOrbsMaxed;

    public Dictionary<string, int> collectedOrbs { get; private set; } = new Dictionary<string, int>();
    public Dictionary<string, OrbType> orbTypeLookup { get; private set; } = new Dictionary<string, OrbType>();

    private List<(GameObject orb, float spawnTime)> spawnedOrbs = new List<(GameObject, float)>();
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        foreach (OrbType orbType in orbTypes)
        {
            collectedOrbs[orbType.orbName] = 0;
            orbTypeLookup[orbType.targetTag] = orbType; // Create lookup by targetTag for faster matching
            UpdateUIText(orbType.orbName);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object matches any orb's target tag
        if (orbTypeLookup.TryGetValue(other.tag, out OrbType orbType))
        {
            // Update the collected count
            collectedOrbs[orbType.orbName]++;
            Destroy(other.gameObject); // Destroy the orb object

            // Play the collection sound
            PlayOrbSound(orbType.collectSound);

            // Trigger the appropriate UnityEvents
            if (collectedOrbs[orbType.orbName] >= orbType.requiredCount)
            {
                orbType.onAbilityGained.Invoke();
            }
            if (collectedOrbs[orbType.orbName] >= orbType.subGoalCount && orbType.subGoalCount > 0)
            {
                orbType.onAbilitySubGained.Invoke();
            }

            // Update the UI text
            UpdateUIText(orbType.orbName);

            // Check if all orb types are maxed
            if (AllOrbsMaxed())
            {
                onAllOrbsMaxed.Invoke();
            }
        }
    }

    public void LoseOrbs(int maxOrbsToLose)
{
    int totalOrbs = TotalCollectedOrbs();

    // Prevent crash if there are no orbs to lose
    if (totalOrbs == 0)
    {
        Debug.LogWarning("No orbs to lose!");
        return;
    }

    int orbsToLose = Random.Range(1, Mathf.Min(maxOrbsToLose, totalOrbs) + 1); // Added +1 to include the max in the range

    while (orbsToLose > 0)
    {
        int randomIndex = Random.Range(0, orbTypes.Count);
        OrbType orbType = orbTypes[randomIndex];

        if (collectedOrbs[orbType.orbName] > 0)
        {
            collectedOrbs[orbType.orbName]--;
            orbsToLose--;

            if (collectedOrbs[orbType.orbName] < orbType.requiredCount)
            {
                orbType.onAbilityLost.Invoke();
            }

            if (orbType.orbPrefab != null)
            {
                SpawnLostOrb(orbType);
            }

            UpdateUIText(orbType.orbName);
        }
    }
}


    public void UpdateUIText(string orbName)
    {
        foreach (var orbType in orbTypes)
        {
            if (orbType.orbName == orbName && orbType.uiText != null)
            {
                orbType.uiText.text = $"{orbType.orbName}: {collectedOrbs[orbName]} / {orbType.requiredCount}";
                break;
            }
        }
    }
    void Update()
    {
        for (int i = spawnedOrbs.Count - 1; i >= 0; i--)
        {
            var (orb, spawnTime) = spawnedOrbs[i];
            if (orb != null && Time.time - spawnTime > orbDespawnTimer)
            {
                Destroy(orb);
                spawnedOrbs.RemoveAt(i);
            }
        }
    }

     private void SpawnLostOrb(OrbType orbType)
    {
        Vector3 spawnPosition = orbSpawnPoint != null ? orbSpawnPoint.position : transform.position;
        Vector3 forceDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0.5f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;

        GameObject spawnedOrb = Instantiate(orbType.orbPrefab, spawnPosition, Quaternion.identity);
        spawnedOrb.GetComponent<Rigidbody>().AddForce(forceDirection * orbSpawnForce, ForceMode.Impulse);

        spawnedOrbs.Add((spawnedOrb, Time.time));
    }


    public bool AllOrbsMaxed()
    {
        foreach (var orbType in orbTypes)
        {
            if (collectedOrbs[orbType.orbName] < orbType.requiredCount)
                return false;
        }
        return true;
    }

    private int TotalCollectedOrbs()
    {
        int total = 0;
        foreach (var count in collectedOrbs.Values)
        {
            total += count;
        }
        return total;
    }

    private void PlayOrbSound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}

