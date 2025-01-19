using UnityEngine;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

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

    private Dictionary<string, int> collectedOrbs = new Dictionary<string, int>();
    private List<GameObject> spawnedOrbs = new List<GameObject>(); 

    public float orbDespawnTimer = 5f; 
    public float orbSpawnForce = 5f; 
    public Transform orbSpawnPoint; 

    public UnityEvent onAllOrbsMaxed; 

    void Start()
    {
        foreach (OrbType orbType in orbTypes)
        {
            collectedOrbs.Add(orbType.orbName, 0);
            UpdateUIText(orbType.orbName);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        foreach (OrbType orbType in orbTypes)
        {
            if (other.gameObject.CompareTag(orbType.targetTag))
            {
                collectedOrbs[orbType.orbName]++;
                Destroy(other.gameObject); 

                // Play collect sound
                if (orbType.collectSound != null)
                {
                    AudioSource.PlayClipAtPoint(orbType.collectSound, transform.position); 
                }

                if (collectedOrbs[orbType.orbName] >= orbType.requiredCount)
                {
                    orbType.onAbilityGained.Invoke();
                }

                if (collectedOrbs[orbType.orbName] >= orbType.subGoalCount && orbType.subGoalCount > 0) 
                {
                    orbType.onAbilitySubGained.Invoke();
                }

                UpdateUIText(orbType.orbName); 

                // Check if ALL orbs are at max
                if (orbTypes.All(orb => collectedOrbs[orb.orbName] >= orb.requiredCount))
                {
                    onAllOrbsMaxed.Invoke();
                }

                break; // Exit the loop after finding the matching OrbType
            }
        }
    }

    private Dictionary<GameObject, float> orbSpawnTimes = new Dictionary<GameObject, float>();

public void LoseOrbs(int maxOrbsToLose)
{
    int orbsToLose = Random.Range(1, Mathf.Min(maxOrbsToLose, collectedOrbs.Values.Sum()));

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
                Vector3 spawnPosition = orbSpawnPoint != null ? orbSpawnPoint.position : transform.position;
                Vector3 forceDirection = Random.insideUnitSphere;
                forceDirection.y = Mathf.Abs(forceDirection.y);

                GameObject spawnedOrb = Instantiate(orbType.orbPrefab, spawnPosition, Quaternion.identity);
                orbSpawnTimes[spawnedOrb] = Time.time;
                spawnedOrb.GetComponent<Rigidbody>().AddForce(forceDirection * orbSpawnForce, ForceMode.Impulse);
                spawnedOrbs.Add(spawnedOrb);

                Debug.Log($"Orb {orbType.orbName} spawned successfully at {spawnPosition}!");
            }

            UpdateUIText(orbType.orbName);
        }
    }
}

void Update()
{
    foreach (var orb in spawnedOrbs.ToList())
    {
        if (orb != null && Time.time - orbSpawnTimes[orb] > orbDespawnTimer)
        {
            Destroy(orb);
            orbSpawnTimes.Remove(orb);
            spawnedOrbs.Remove(orb);
        }
    }
}


    void UpdateUIText(string orbName) 
    {
        foreach (OrbType orbType in orbTypes)
        {
            if (orbType.orbName == orbName)
            {
                if (orbType.uiText != null)
                {
                    orbType.uiText.text = $"{orbType.orbName}: {collectedOrbs[orbType.orbName]} / {orbType.requiredCount}"; 
                }
                break;
            }
        }
    }
}