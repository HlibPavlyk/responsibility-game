using System.Collections;
using System.Collections.Generic;
using Core.Events;
using UnityEngine;

public class BaljeetController : MonoBehaviour
{
    public Transform playerTransform;
    public float sleepInterval = 5f; // Time before monster teleports
    public float waitTimeAfterTeleport = 3f; // Time monster waits near player
    public float attackDamage = 10f; // Damage dealt to the player
    public float teleportDistance = 1f; // Distance from the player to teleport to
    public float attackDelayAfterTeleport = 1f; // Delay in seconds before the monster can attack after teleporting
    private float attackDelayTimer;


    private float sleepTimer;
    private float waitTimer;
    private bool isSleeping = true;
    private bool isNearPlayer = false;
    private Vector3 lastPlayerPosition;

    void Start()
    {
        // Initialize the sleep timer
        sleepTimer = sleepInterval;
        Debug.Log("[BaljeetController] Monster starts sleeping. Timer: " + sleepTimer + " seconds.");

        // Optionally, check if the player is already present in the scene and assign the transform
        // This is useful if the BaljeetController might be activated after the player has already spawned
        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
        if (existingPlayer != null)
        {
            playerTransform = existingPlayer.transform;
            Debug.Log("[BaljeetController] Found existing player at start. Position: " + playerTransform.position);
        }
    }


    void OnEnable()
    {
        // Subscribe to the player spawned event
        GameEvents.Player.onPlayerSpawned += UpdatePlayerTransform;
    }

    void OnDisable()
    {
        // Unsubscribe from the player spawned event
        GameEvents.Player.onPlayerSpawned -= UpdatePlayerTransform;
    }

    void UpdatePlayerTransform(Transform newPlayerTransform)
    {
        playerTransform = newPlayerTransform;
        Debug.Log("[BaljeetController] Updated player transform.");
    }

    void Update()
    {
        if (attackDelayTimer > 0)
        {
            // Reduce the attack delay timer
            attackDelayTimer -= Time.deltaTime;
        }

        if (isSleeping)
        {
            Debug.Log("[BaljeetController] Monster is sleeping.");
            // Play ticking sound here
            sleepTimer -= Time.deltaTime;
            if (sleepTimer <= 0f)
            {
                TeleportToPlayer();
            }
        }
        else if (isNearPlayer)
        {
            Debug.Log("[BaljeetController] Monster is waiting near the player.");
            waitTimer -= Time.deltaTime;
            if (waitTimer <= 0f)
            {
                GoToSleep();
            }
            else if (HasPlayerMoved())
            {
                AttackPlayer();
            }
        }
    }

    void TeleportToPlayer()
    {
        if (playerTransform == null)
        {
            Debug.LogError("[BaljeetController] Player transform is null.");
            return;
        }

        Vector2 offset = Random.insideUnitCircle.normalized; // Create a random normalized 2D direction
        offset *= teleportDistance; // Apply distance to the offset

        Vector2 teleportPosition = (Vector2)playerTransform.position + offset; // Calculate new teleport position

        transform.position = teleportPosition; // Teleport to the new position

        Debug.Log($"[BaljeetController] Teleported near player at {transform.position}");
        isSleeping = false;
        isNearPlayer = true;
        waitTimer = waitTimeAfterTeleport;
        attackDelayTimer = attackDelayAfterTeleport;

        // Start the coroutine to set lastPlayerPosition
        StartCoroutine(SetLastPlayerPositionAfterDelay(2f)); // 1 second delay or adjust as needed
    }

    IEnumerator SetLastPlayerPositionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // Wait for the specified delay

        lastPlayerPosition = playerTransform.position; // Update last known player position
        Debug.Log($"[BaljeetController] Updated last player position after delay: {lastPlayerPosition}");
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("[BaljeetController] Collided with player.");
            //AttackPlayer();
        }
    }

    bool HasPlayerMoved()
    {
        if (playerTransform.position != lastPlayerPosition)
        {
            Debug.Log("[BaljeetController] Player has moved.");
            lastPlayerPosition = playerTransform.position;
            return true;
        }
        return false;
    }

    void AttackPlayer()
    {
        // Implement player damage logic here
        // Example: playerTransform.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        Debug.Log("[BaljeetController] Attacking player.");

        if (playerTransform != null)
        {
            PlayerController playerController = playerTransform.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(1); // Decrease player's health by 1
            }
        }

        GoToSleep();
    }

    void GoToSleep()
    {
        isSleeping = true;
        isNearPlayer = false;
        sleepTimer = sleepInterval;
        Debug.Log($"[BaljeetController] Going back to sleep. Next sleep duration: {sleepTimer} seconds.");
    }
}

