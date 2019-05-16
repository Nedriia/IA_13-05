using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGAE
{
    public class Attack_B : FGAE_CharacterStateBase
    {

        public Vector3 target;
        public SpaceShip ss2;
        public bool avoiding;
        public Vector3 avoinding_pos;
        public float delay_shot = 0.5f;
        public float delay_shot_tmp;

        //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetCharacterControl(animator);
            string ss_name = "";
            if (GetSpaceShip().name == "SpaceShip2")
            {
                ss_name = "SpaceShip1";
            }
            else
            {
                ss_name = "SpaceShip2";
            }
            ss2 = GameObject.Find(ss_name).GetComponent<SpaceShip>();

            if (ss2.name == "SpaceShip2")
            {
                animator.SetBool("go_to_point", true);
            }
        }

        //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetThrust(1);

            if (delay_shot_tmp > 0)
            {
                delay_shot_tmp -= Time.deltaTime;
            }

            target = ss2.transform.position;
            SetShot(false);

            if (Vector3.Angle(target - GetSpaceShip().transform.position, GetSpaceShip().transform.right) < 10 && Vector2.Distance(GetSpaceShip().transform.position, target) < 1)
            {
                if (GetSpaceShip().Energy >= 1 && !ss2.IsHit() && delay_shot_tmp <= 0)
                {
                    SetShot(true);
                    delay_shot_tmp = delay_shot;
                }
            }
            else
            {
                if (Vector3.Angle(target - GetSpaceShip().transform.position, GetSpaceShip().transform.right) < 30 && Vector2.Distance(GetSpaceShip().transform.position, target) < 10)
                {
                    float time_tmp = 0.2f;
                    float time_max = 2;
                    while (time_tmp < time_max)
                    {
                       // Debug.Log(((GetSpaceShip().transform.right * 5 * time_tmp) + GetSpaceShip().transform.position)  + " // " + (target + (Vector3)ss2.GetComponent<Rigidbody2D>().velocity) * time_tmp);
                        if (Vector2.Distance(((GetSpaceShip().transform.right * 5 * time_tmp) + GetSpaceShip().transform.position), ((Vector2)target + (ss2.GetComponent<Rigidbody2D>().velocity * time_tmp))) < 0.3f)
                        {
                            if (GetSpaceShip().Energy >= 0.5f && !ss2.IsHit() && delay_shot_tmp <= 0)
                            {
                                SetShot(true);
                                delay_shot_tmp = delay_shot;
                                Debug.Log(((GetSpaceShip().transform.right * 5 * time_tmp) + GetSpaceShip().transform.position) + "////" + ((Vector2)target + (ss2.GetComponent<Rigidbody2D>().velocity * time_tmp)));
                                Debug.Log(time_tmp);
                                time_tmp = time_max;
                            }else if(!ss2.IsHit() && delay_shot_tmp <= 0 && Vector2.Distance(((GetSpaceShip().transform.right * 5 * time_tmp) + GetSpaceShip().transform.position), ((Vector2)target + (ss2.GetComponent<Rigidbody2D>().velocity * time_tmp))) < 0.1f)
                            {
                                SetShot(true);
                                delay_shot_tmp = delay_shot;
                            }
                        }
                        time_tmp += 0.2f;
                    }
                }
            }


            RaycastHit2D hit = Physics2D.Raycast(GetSpaceShip().transform.position, GetSpaceShip().transform.right, 3.5f, LayerMask.GetMask("Asteroid"));

            float radius = 0;

            if (hit.collider != null)
                radius = hit.collider.GetComponent<CircleCollider2D>().radius;  

            Vector3 ss_position = GetSpaceShip().transform.position;

            Debug.DrawRay(ss_position, GetCharacterControl(animator).spaceShip_FGAE.GetComponent<Rigidbody2D>().velocity * 1, Color.black);

            if (hit.collider != null)
            {
                if (!avoiding)
                {
                    Vector3 forward = GetSpaceShip().transform.right;
                    Vector3 toOther = hit.collider.transform.position - GetSpaceShip().transform.position;
                    Vector3 toOther2 = target - GetSpaceShip().transform.position;
                    if (Vector3.Dot(forward, toOther) > 0 && Vector3.Dot(forward, toOther2) > 0)
                    {
                        Debug.Log("EVITER");
                        avoiding = true;
                        //Debug.DrawRay(ss_position, GetSpaceShip().transform.TransformDirection(Vector3.right) * 3.5f, Color.yellow);

                        Vector3 pos1 = Vector3.zero;
                        Vector3 pos2 = Vector3.zero;

                        float degre_b = Mathf.Atan2((hit.transform.position - GetSpaceShip().transform.position).y, (hit.transform.position - GetSpaceShip().transform.position).x) * Mathf.Rad2Deg;
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
                            avoid_r = Physics2D.Raycast(GetSpaceShip().transform.position, DegreeToVector2(degre_b + degre_r), hit.distance + (radius / 3), LayerMask.GetMask("Asteroid"));

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
                            avoid_l = Physics2D.Raycast(GetSpaceShip().transform.position, DegreeToVector2(degre_b - degre_l), hit.distance + (radius / 3), LayerMask.GetMask("Asteroid"));

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
                        pos1 = (Vector3)DegreeToVector2(degre_b + degre_r + 5).normalized * (hit.distance) + ss_position;
                        pos2 = (Vector3)DegreeToVector2(degre_b - degre_l - 5).normalized * (hit.distance) + ss_position;

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
                    else
                    {
                        Debug.Log("IGNORE");
                        var angle_ = Mathf.Atan2(target.y - ss_position.y, target.x - ss_position.x);
                        SetOrient(angle_ * Mathf.Rad2Deg);
                        Debug.DrawRay(GetSpaceShip().transform.position, (target - GetSpaceShip().transform.position), Color.magenta);
                    }

                }

                var angle = Mathf.Atan2(avoinding_pos.y - ss_position.y, avoinding_pos.x - ss_position.x);
                SetOrient(angle * Mathf.Rad2Deg);

                Debug.DrawRay(GetSpaceShip().transform.position, (avoinding_pos - GetSpaceShip().transform.position), Color.magenta);

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
                        Debug.DrawRay(GetSpaceShip().transform.position, (avoinding_pos - GetSpaceShip().transform.position), Color.magenta);
                    }


                }
                else
                {
                    var angle = Mathf.Atan2(target.y - ss_position.y, target.x - ss_position.x);
                    SetOrient(angle * Mathf.Rad2Deg);
                    Debug.DrawRay(GetSpaceShip().transform.position, (target - GetSpaceShip().transform.position), Color.magenta);
                }
            }
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
