using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FGAE
{
    public class Search_Points : FGAE_CharacterStateBase
    {

        public int count;
        public bool targetConfirmed;
        public WayPoint target;
        public float targetOrient;
        public bool avoiding;
        public Vector3 avoinding_pos;
        public float dist_avoiding_max;
        public float dist_avoiding_tmp;

        public float dist_detection = float.MaxValue;
        public WayPoint targetCheck;

        public float angleTest;
        public float test;


        //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetCharacterControl(animator);
        }

        //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetThrust(1);            
            var velocityDirection = new Vector3(GetSpaceShip().transform.right.x - GetSpaceShip().Velocity.x, GetSpaceShip().transform.right.y - GetSpaceShip().Velocity.y  , 0);            
            Debug.DrawRay(GetSpaceShip().transform.position, new Vector3(GetSpaceShip().Velocity.x - GetSpaceShip().transform.right.x, GetSpaceShip().Velocity.y - GetSpaceShip().transform.right.y, 0), Color.yellow);

            if (!targetConfirmed)
            {
                dist_detection = float.MaxValue;
                //target = GameManager.Instance.GetGameData().WayPoints[count];
                foreach (WayPoint point in GetData().WayPoints)
                {
                    float distance = Vector3.Distance(GetSpaceShip().transform.position, point.transform.position);
                    angleTest = Vector3.Angle(point.transform.position - GetSpaceShip().transform.position, GetSpaceShip().transform.right);
                    if (distance < dist_detection && (Vector3.Dot(GetSpaceShip().transform.right, point.transform.position - GetSpaceShip().transform.position) > 0) &&  point.Owner != GetSpaceShip().Owner && angleTest < 60 || angleTest <-60)
                    {
                        targetCheck = point;
                        dist_detection = distance;
                    }
                }
                target = targetCheck;
                targetConfirmed = true;
            }
            angleTest = Vector3.Angle(target.transform.position - GetSpaceShip().transform.position, GetSpaceShip().transform.right);

            if ((Vector3.Dot(GetSpaceShip().transform.right, target.transform.position - GetSpaceShip().transform.position) > 0))
            {
                targetConfirmed = false;
            }

            RaycastHit2D hit = Physics2D.Raycast(GetSpaceShip().transform.position, GetSpaceShip().transform.TransformDirection(Vector3.right), 3.5f, LayerMask.GetMask("Asteroid"));

            RaycastHit2D hit_Avoid = Physics2D.Raycast(GetSpaceShip().transform.position, (target.transform.position - GetSpaceShip().transform.position), Vector3.Distance(target.transform.position, GetSpaceShip().transform.position), LayerMask.GetMask("Asteroid"));

            float radius = 0;

            if (hit_Avoid.collider != null)
                radius = hit_Avoid.collider.GetComponent<CircleCollider2D>().radius *1.3f;
            if (hit.collider != null)
                radius = hit.collider.GetComponent<CircleCollider2D>().radius * 1.3f;

            Vector3 ss_position = GetSpaceShip().transform.position;

            if (hit.collider != null)
            {
                if (!avoiding)
                {
                    avoiding = true;
                    //Debug.DrawRay(ss_position, GetSpaceShip().transform.TransformDirection(Vector3.right) * 3.5f, Color.yellow);

                    Vector3 pos1 = Vector3.zero;
                    Vector3 pos2 = Vector3.zero;

                    float degre_b = Mathf.Atan2((target.transform.position - GetSpaceShip().transform.position).y, (target.transform.position - GetSpaceShip().transform.position).x) * Mathf.Rad2Deg;
                    if (degre_b < 0)
                    {
                        degre_b = 360 + degre_b;
                    }
                    RaycastHit2D avoid_r;
                    RaycastHit2D avoid_l;
                    float degre_r = 5;
                    float degre_l = 5;
                    bool out_b = false;
                    while (!out_b)
                    {
                        avoid_r = Physics2D.Raycast(GetSpaceShip().transform.position, DegreeToVector2(degre_b + degre_r), hit.distance + radius, LayerMask.GetMask("Asteroid"));

                        if (avoid_r.collider == null)
                        {
                            out_b = true;
                        }
                        else
                        {
                            pos1 = avoid_r.point;
                            degre_r += 5;
                            Debug.DrawRay(GetSpaceShip().transform.position, (pos1 - GetSpaceShip().transform.position), Color.blue, 2);
                        }

                    }
                    out_b = false;
                    while (!out_b)
                    {
                        avoid_l = Physics2D.Raycast(GetSpaceShip().transform.position, DegreeToVector2(degre_b - degre_l), hit.distance + radius, LayerMask.GetMask("Asteroid"));

                        if (avoid_l.collider == null)
                        {
                            out_b = true;
                        }
                        else
                        {
                            pos2 = avoid_l.point;
                            degre_l += 5;
                            Debug.DrawRay(GetSpaceShip().transform.position, (pos2 - GetSpaceShip().transform.position), Color.blue, 2);
                        }
                    }
                    pos1 = (Vector3) DegreeToVector2(degre_b + degre_r + 5).normalized * (hit.distance + (radius / 2)) + ss_position;
                    pos2 = (Vector3) DegreeToVector2(degre_b - degre_l - 5).normalized * (hit.distance + (radius / 2)) + ss_position;

                    if (Vector3.Distance(ss_position, pos1) + Vector3.Distance(pos1, (Vector3)GetCharacterControl(animator).spaceShip_FGAE.GetComponent<Rigidbody2D>().velocity + ss_position) * 30 
                        > Vector3.Distance(ss_position, pos2) + Vector3.Distance(pos2, (Vector3)GetCharacterControl(animator).spaceShip_FGAE.GetComponent<Rigidbody2D>().velocity + ss_position) * 30)
                    {
                        avoinding_pos = pos2;
                    }
                    else
                    {
                        avoinding_pos = pos1;
                    }
                    Debug.DrawRay(GetSpaceShip().transform.position, (avoinding_pos - GetSpaceShip().transform.position), Color.red, 2);
                }

                var angle = Mathf.Atan2(avoinding_pos.y - ss_position.y, avoinding_pos.x - ss_position.x);
                SetOrient(angle * Mathf.Rad2Deg);

                //Debug.DrawRay(GetSpaceShip().transform.position, (avoinding_pos - GetSpaceShip().transform.position), Color.red);

            }
            else
            {
                if (avoiding)
                {
                    if (Vector3.Distance(avoinding_pos, ss_position) < 1f)
                    {
                        avoiding = false;
                    }
                    else
                    {
                        var angle = Mathf.Atan2(avoinding_pos.y - ss_position.y, avoinding_pos.x - ss_position.x);
                        SetOrient(angle * Mathf.Rad2Deg);
                        //Debug.DrawRay(GetSpaceShip().transform.position, (avoinding_pos - GetSpaceShip().transform.position), Color.red);
                    }
                }
                else
                {
                    if (Vector3.Distance(target.transform.position, GetSpaceShip().transform.position) < 3f)
                         test = Mathf.Atan2(target.transform.position.y - ss_position.y, target.transform.position.x - ss_position.x);
                    else
                         test = Mathf.Atan2(target.transform.position.y - ss_position.y + velocityDirection.y, target.transform.position.x - ss_position.x + velocityDirection.x);
                    SetOrient(test * Mathf.Rad2Deg);
                }
            }

            velocityDirection = Vector3.Reflect(velocityDirection, GetSpaceShip().transform.right);
            if(Vector3.Distance(target.transform.position,GetSpaceShip().transform.position) < 3f)
                Debug.DrawRay(ss_position, (target.transform.position - GetSpaceShip().transform.position), Color.green);
            else
                Debug.DrawRay(ss_position, (target.transform.position - GetSpaceShip().transform.position) + velocityDirection , Color.cyan);

            Debug.DrawRay(ss_position, GetCharacterControl(animator).spaceShip_FGAE.GetComponent<Rigidbody2D>().velocity * 1, Color.black);


            if (target.GetComponent<WayPoint>().Owner == GetSpaceShip().Owner)
            {
                targetConfirmed = false;
            }

            if (count == GetData().WayPoints.Count)
                count = 0;
        }

        //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }
    }
}
