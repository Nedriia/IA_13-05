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
            GetCharacterControl(animator);

            timer_attack_tmp = 0;

            ss2 = get_EnemySpaceShip();
        }

        //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Reset some var
            SetThrust(1);
            SetShot(false);

            // Timers / Delays
            if (animator.GetBool("attack_mode"))
            {
                timer_attack_tmp += Time.deltaTime;
            }

            if (delay_shot_tmp > 0)
            {
                delay_shot_tmp -= Time.deltaTime;
            }

            //The tarjet will be the enemy Space Ship
            characterControl.ss_target = ss2.transform.position;

            // If the SpaceShip is in front and very close the IA shoots
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
                //If the Space Enemy SpaceShip is in the field view
                if (Vector3.Angle(characterControl.ss_target - GetSpaceShip().transform.position, GetSpaceShip().transform.right) < 30 && Vector2.Distance(GetSpaceShip().transform.position, characterControl.ss_target) < 10)
                {
                    // Prediction of the shoot, if the prediction fins a good shoot he will fire 
                    float time_tmp = 0.2f;
                    float time_max = 2;
                    while (time_tmp < time_max)
                    {
                        if (Vector2.Distance(((GetSpaceShip().transform.right * 5 * time_tmp) + GetSpaceShip().transform.position), ((Vector2)characterControl.ss_target + (ss2.GetComponent<Rigidbody2D>().velocity * time_tmp))) < 0.3f)
                        {
                            if (GetSpaceShip().Energy >= 0.5f && !ss2.IsHit() && delay_shot_tmp <= 0)
                            {
                                SetShot(true);
                                delay_shot_tmp = characterControl.delay_shot;
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

            // Raycast to detect if a mine is in front, if is tha IA shoots
            RaycastHit2D hit_mine = Physics2D.Raycast(GetSpaceShip().transform.position, GetSpaceShip().transform.TransformDirection(Vector3.right), 2, LayerMask.GetMask("Mine"));

            if (hit_mine.collider != null)
            {
                if (delay_shot_tmp <= 0)
                {
                    SetShot(true);
                    delay_shot_tmp = characterControl.delay_shot;
                }
            }

            // Raycast to detect if a rock is in front, if is the IA will pas in mode Avoiding
            RaycastHit2D hit = Physics2D.Raycast(GetSpaceShip().transform.position, GetSpaceShip().transform.right, 3.5f, LayerMask.GetMask("Asteroid"));

            // Variables ini
            float radius = 0;
            if (hit.collider != null)
                radius = hit.collider.GetComponent<CircleCollider2D>().radius;

            Debug.DrawRay(GetSpaceShip().transform.position, GetCharacterControl(animator).spaceShip_FGAE.GetComponent<Rigidbody2D>().velocity * 1, Color.black);

            //if a rock is in front, if is the IA will pas in mode Avoiding
            if (hit.collider != null)
            {
                Vector3 forward = GetSpaceShip().transform.right;
                Vector3 toOther = hit.collider.transform.position - GetSpaceShip().transform.position;
                Vector3 toOther2 = characterControl.ss_target - GetSpaceShip().transform.position;

                // If the rock is not is the way to go the next point we ignore the rock if not we pas in mode Avoiding
                if (Vector3.Dot(forward, toOther) > 0 && Vector3.Dot(forward, toOther2) > 0)
                {
                    animator.SetBool("avoiding", true);
                }
                else
                {
                    var angle = Mathf.Atan2(characterControl.ss_target.y - GetSpaceShip().transform.position.y, characterControl.ss_target.x - GetSpaceShip().transform.position.x);
                    SetOrient(angle * Mathf.Rad2Deg);
                    Debug.DrawRay(GetSpaceShip().transform.position, (characterControl.ss_target - GetSpaceShip().transform.position), Color.magenta);
                }

            }
            else
            {
                // If there is nothing in front we move to the next point
                var angle = Mathf.Atan2(characterControl.ss_target.y - GetSpaceShip().transform.position.y, characterControl.ss_target.x - GetSpaceShip().transform.position.x);
                SetOrient(angle * Mathf.Rad2Deg);
                Debug.DrawRay(GetSpaceShip().transform.position, (characterControl.ss_target - GetSpaceShip().transform.position), Color.magenta);
            }

            pass_to_BerserkMode(animator);

            pass_to_atackMode(animator);

        }

        // if the timer for the attack mode ends we pass in Searching points mode
        public void pass_to_atackMode(Animator animator)
        {
            float angle_ss2 = Vector3.Angle(ss2.transform.position - GetSpaceShip().transform.position, GetSpaceShip().transform.right);

            if (timer_attack_tmp >= characterControl.attack_mode_time)
            {
                animator.SetBool("attack_mode", false);
                timer_attack_tmp = 0;
            }
        }

        // If we are in +5 in score tha IA pass in berserk mode
        public void pass_to_BerserkMode(Animator animator)
        {
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
        }

    }


}
