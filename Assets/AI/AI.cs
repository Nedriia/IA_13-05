using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : BaseSpaceShipController
{
    public float distance= float.MaxValue;
    public bool targetConfirmed;
    public WayPoint target;
    public bool alreadyDone;
    public int count;
    public bool avoidance;
    public float targetOrient;

    public override InputData UpdateInput(SpaceShip spaceship, GameData data)
    {
        RaycastHit2D hit = Physics2D.Raycast(spaceship.transform.position, spaceship.transform.TransformDirection(transform.right),3.5f, LayerMask.GetMask("Asteroid"));
        float thrust = 1f;

        if (hit.collider != null)
        {
            Debug.DrawRay(spaceship.transform.position, spaceship.transform.TransformDirection(transform.right) * 3.5f, Color.red);
            avoidance = true;
        }
        else
        {
            Debug.DrawRay(spaceship.transform.position, spaceship.transform.TransformDirection(transform.right) * 3.5f, Color.yellow);
            avoidance = false;
        }

        if (count == GameManager.Instance.GetGameData().WayPoints.Count)
            count = 0;

        if (!targetConfirmed)
        {
            target = GameManager.Instance.GetGameData().WayPoints[count];
            Debug.Log(target);
            Debug.Log("...");
            targetConfirmed = true;
            alreadyDone = false;
        }

        if (target.GetComponentInChildren<SpriteRenderer>().color.r == spaceship.color.r && target.GetComponentInChildren<SpriteRenderer>().color.g == spaceship.color.g && target.GetComponentInChildren<SpriteRenderer>().color.b == spaceship.color.b && !alreadyDone)
        {
            /*Debug.Log(target.GetComponentInChildren<SpriteRenderer>().color);
            Debug.Log(spaceship.color);*/
            targetConfirmed = false;
            alreadyDone = true;
            count++;
        }

        //float targetOrient = spaceship.Orientation;
        if (!avoidance)
        {
            var test = Quaternion.LookRotation(target.transform.position - spaceship.transform.position);
            var angle = Mathf.Atan2(target.transform.position.y - spaceship.transform.position.y, target.transform.position.x - spaceship.transform.position.x);
            //Debug.Log(angle);
            //Debug.Log(angle * Mathf.Rad2Deg);
            targetOrient = angle * Mathf.Rad2Deg;
        }
        else if(avoidance)
        {
            //Test which point is the closest to avoid the asteroid
            var test = Quaternion.LookRotation(target.transform.position - spaceship.transform.position);

            //test where the spaceship is coming from the point of view of the the asteroid
            if(spaceship.transform.position.x > target.transform.position.x)
            {
                //Debug.Log("1");
                if(hit.collider != null)
                {
                    var angle = Mathf.Atan2(hit.collider.bounds.max.y + spaceship.transform.position.y, hit.collider.bounds.min.x - spaceship.transform.position.x);
                    targetOrient = angle * Mathf.Rad2Deg;
                }
                
            }
            else if(spaceship.transform.position.x < target.transform.position.x)
            {
                //Debug.Log("2");
                if (hit.collider != null)
                {
                    var angle = Mathf.Atan2(hit.collider.bounds.max.y + spaceship.transform.position.y, hit.collider.bounds.max.x + spaceship.transform.position.x);
                    targetOrient = angle * Mathf.Rad2Deg;
                }
            }         
        }

        /*bool shoot = Input.GetButtonDown("Fire1");
        bool dropMine = Input.GetButtonDown("Fire2");*/
        return new InputData(thrust, targetOrient, false, false);
    }
}
