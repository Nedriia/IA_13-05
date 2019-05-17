using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGAE
{
    public class Berserk_B : FGAE_CharacterStateBase
    {
        public SpaceShip ss2;
        public bool avoiding;
        public Vector3 avoinding_pos;
        public float delay_shot = 0.5f;
        public float delay_shot_tmp;

        //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetCharacterControl(animator);
            ss2 = get_EnemySpaceShip();
        }

        //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // Reset some var
            SetThrust(1);
            SetShot(false);

            // Timers
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
                    delay_shot_tmp = delay_shot;
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
                                delay_shot_tmp = delay_shot;
                                Debug.Log(((GetSpaceShip().transform.right * 5 * time_tmp) + GetSpaceShip().transform.position) + "////" + ((Vector2)characterControl.ss_target + (ss2.GetComponent<Rigidbody2D>().velocity * time_tmp)));
                                Debug.Log(time_tmp);
                                time_tmp = time_max;
                            }else if(!ss2.IsHit() && delay_shot_tmp <= 0 && Vector2.Distance(((GetSpaceShip().transform.right * 5 * time_tmp) + GetSpaceShip().transform.position), ((Vector2)characterControl.ss_target + (ss2.GetComponent<Rigidbody2D>().velocity * time_tmp))) < 0.1f)
                            {
                                SetShot(true);
                                delay_shot_tmp = delay_shot;
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

            float radius = 0;

            if (hit.collider != null)
                radius = hit.collider.GetComponent<CircleCollider2D>().radius;  

            Vector3 ss_position = GetSpaceShip().transform.position;

            Debug.DrawRay(ss_position, GetCharacterControl(animator).spaceShip_FGAE.GetComponent<Rigidbody2D>().velocity * 1, Color.black);
            
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
                    var angle = Mathf.Atan2(characterControl.ss_target.y - ss_position.y, characterControl.ss_target.x - ss_position.x);
                    SetOrient(angle * Mathf.Rad2Deg);
                    Debug.DrawRay(GetSpaceShip().transform.position, (characterControl.ss_target - GetSpaceShip().transform.position), Color.magenta);
                }

            }
            else
            {
                // If there is nothing in front we move to the next point
                var angle = Mathf.Atan2(characterControl.ss_target.y - ss_position.y, characterControl.ss_target.x - ss_position.x);
                SetOrient(angle * Mathf.Rad2Deg);
                Debug.DrawRay(GetSpaceShip().transform.position, (characterControl.ss_target - GetSpaceShip().transform.position), Color.magenta);
            }

            exit_BerserkMode(animator);

        }

        // If we are not more in +5 in the score, we exit the berserk mode
        public void exit_BerserkMode(Animator animator)
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
            if (GameManager.Instance.GetScoreForPlayer(index_player) - GameManager.Instance.GetScoreForPlayer(index_player2) < characterControl.dif_score)
            {
                animator.SetBool("berserk_mode", false);
            }
        }



        }
}
