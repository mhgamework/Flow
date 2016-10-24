using UnityEngine;
using System.Collections;

public class EnemyAIScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Find and move towards player
        var target = FindObjectOfType<PlayerMovementScript>().GetPosition();

        var diff = target - transform.position;

        transform.position += diff.normalized*Mathf.Min(diff.magnitude, Time.deltaTime*GetMovementSpeed());



    }

    private float GetMovementSpeed()
    {
        return GetComponent<HeroScript>().ActiveMoveSpeed;
    }
}
