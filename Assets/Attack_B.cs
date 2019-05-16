using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGAE
{
    public class Attack_B : FGAE_CharacterStateBase
    {

        public SpaceShip ss2;
        public bool avoiding;
        public Vector3 avoinding_pos;
        private float delay_shot_tmp;

        public float timer_attack_tmp;

        //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            timer_attack_tmp = 0;
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
            if (animator.GetBool("attack_mode"))
            {
                timer_attack_tmp += Time.deltaTime;
            }

            if (delay_shot_tmp > 0)
            {
                delay_shot_tmp -= Time.deltaTime;
            }

            characterControl.ss_target = ss2.transform.position;
            SetShot(false);

            if (Vector3.Angle(characterControl.ss_target - GetSpaceShip().transform.position, GetSpaceShip().transform.right) < 10 && Vector2.Distance(GetSpaceShip().transform.position, characterControl.ss_target) < 1)
            {
                if (GetSpaceShip().Energy >= 1 && !ss2.IsHit() && delay_shot_tmp <= 0)
                {
                    SetShot(true);
                    delay_shot_tmp = characterControl.delay_shot;
                }
            }
            else
            {
                if (Vector3.Angle(characterControl.ss_target - GetSpaceShip().transform.position, GetSpaceShip().transform.right) < 30 && Vector2.Distance(GetSpaceShip().transform.position, characterControl.ss_target) < 10)
                {
                    float time_tmp = 0.2f;
                    float time_max = 2;
                    while (time_tmp < time_max)
                    {
                        // Debug.Log(((GetSpaceShip().transform.right * 5 * time_tmp) + GetSpaceShip().transform.position)  + " // " + (target + (Vector3)ss2.GetComponent<Rigidbody2D>().velocity) * time_tmp);
                        if (Vector2.Distance(((GetSpaceShip().transform.right * 5 * time_tmp) + GetSpaceShip().transform.position), ((Vector2)characterControl.ss_target + (ss2.GetComponent<Rigidbody2D>().velocity * time_tmp))) < 0.3f)
                        {
                            if (GetSpaceShip().Energy >= 0.5f && !ss2.IsHit() && delay_shot_tmp <= 0)
                            {
                                SetShot(true);
                                delay_shot_tmp = characterControl.delay_shot;
                                Debug.Log(((GetSpaceShip().transform.right * 5 * time_tmp) + GetSpaceShip().transform.position) + "////" + ((Vector2)characterControl.ss_target + (ss2.GetComponent<Rigidbody2D>().velocity * time_tmp)));
                                Debug.Log(time_tmp);
                                time_tmp = time_max;
                            }
                            else if (!ss2.IsHit() && delay_shot_tmp <= 0 && Vector2.Distance(((GetSpaceShip().transform.right * 5 * time_tmp) + GetSpaceShip().transform.position), ((Vector2)characterControl.ss_target + (ss2.GetComponent<Rigidbody2D>().velocity * time_tmp))) < 0.1f)
                            {
                                SetShot(true);
                                delay_shot_tmp = characterControl.delay_shot;
                            }
                        }
                        time_tmp += 0.2f;
                    }
                }
            }

            RaycastHit2D hit_mine = Physics2D.Raycast(GetSpaceShip().transform.position, GetSpaceShip().transform.TransformDirection(Vector3.right), 2, LayerMask.GetMask("Mine"));

            if (hit_mine.collider != null)
            {
                if (delay_shot_tmp <= 0)
                {
                    SetShot(true);
                    delay_shot_tmp = characterControl.delay_shot;
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
                Vector3 forward = GetSpaceShip().transform.right;
                Vector3 toOther = hit.collider.transform.position - GetSpaceShip().transform.position;
                Vector3 toOther2 = characterControl.ss_target - GetSpaceShip().transform.position;
                if (Vector3.Dot(forward, toOther) > 0 && Vector3.Dot(forward, toOther2) > 0)
                {
                    animator.SetBool("avoiding", true);
                }
                else
                {
                    var angle = Mathf.Atan2(characterControl.ss_target.y - ss_position.y, characterControl.ss_target.x - ss_position.x);
                    SetOrient(angle * Mathf.Rad2Deg);
                    Debug.DrawRay(GetSpaceShip().transform.position, (characterControl.ss_target - GetSpaceShip().transform.position), Color.magenta);
                }

            }
            else
            {
                var angle = Mathf.Atan2(characterControl.ss_target.y - ss_position.y, characterControl.ss_target.x - ss_position.x);
                SetOrient(angle * Mathf.Rad2Deg);
                Debug.DrawRay(GetSpaceShip().transform.position, (characterControl.ss_target - GetSpaceShip().transform.position), Color.magenta);
            }

            int index_player;
            int index_player2;
            if (GetSpaceShip().name == "SpaceShip1")
            {
                index_player = 0;
                index_player2 = 1;
            }
            else
            {
                index_player = 1;
                index_player2 = 0;
            }
            if (GameManager.Instance.GetScoreForPlayer(index_player) - GameManager.Instance.GetScoreForPlayer(index_player2) > characterControl.dif_score)
            {
                animator.SetBool("berserk_mode", true);
                animator.SetBool("attack_mode", false);
            }

            float angle_ss2 = Vector3.Angle(ss2.transform.position - GetSpaceShip().transform.position, GetSpaceShip().transform.right);

            if (timer_attack_tmp >= characterControl.attack_mode_time)
            {
                animator.SetBool("attack_mode", false);
                timer_attack_tmp = 0;
            }
        }

        //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

    }
}
