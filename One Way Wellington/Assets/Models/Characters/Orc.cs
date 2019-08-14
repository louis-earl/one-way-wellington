using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Orc : Enemy
{
    
    protected override void Init()
    {
        // Call from superclass
        base.Init();

        // Setup from here onwards
        jobQueue = JobQueueController.OrcsJobQueue;
    }

    protected override void Refresh()
    {
        // Call from superclass
        base.Refresh();

        // Program from here onwards


        // Find and set a target/current jobs
        if (targetJob == null)
        {
            // Not required to use global job queue yet 
            // targetJob = jobQueue.GetNextJob(new Vector2(currentX, currentY), failedJobs);

            FindCharacter(typeof(Passenger));

            if (targetJob == null)
            {
                // We are idle
                
            }
        }
        
    }

    // Use 2D raycast to find character in view 
    // TODO: Return a character?
    private void FindCharacter(Type characterType)
    {
        // Get a list of potential targets 
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, 10);
        
        
        foreach (Collider2D target in potentialTargets)
        {
            // Type check
            if (target.transform.parent != null)
            {
                if (target.transform.parent.TryGetComponent(characterType, out Component componentInRadius))
                {

                    // Visibility check 
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, (target.transform.position - transform.position));
                    Debug.DrawRay(transform.position, (target.transform.position - transform.position), Color.red);
                    if (hit.transform.parent.TryGetComponent(characterType, out Component componentInSight))
                    {
                        Action attackAction = delegate () { Debug.Log("Attack!!"); target.GetComponentInParent<Character>().TakeDamage(25); };
                        targetJob = new Job(attackAction, target.GetComponentInParent<Character>(), 1f, "attack");
                    }
                }
            }
        }

    }
}
