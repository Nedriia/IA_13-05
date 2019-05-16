using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FGAE
{
    public class Search_Points : FGAE_CharacterStateBase
    {
        public bool targetConfirmed;
        public float targetOrient;
        public SpaceShip ss2;

        public float dist_detection = float.MaxValue;
        public WayPoint targetCheck;

        public float angleTest;
        public float test;


        //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetCharacterControl(animator);
            /*if (GetSpaceShip().name == "SpaceShip1")
            {
                animator.SetBool("berserk_mode", true);
            }*/
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
        }

        //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetThrust(1);                     
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
                characterControl.target = targetCheck;

                targetConfirmed = true;
            }
            angleTest = Vector3.Angle(characterControl.target.transform.position - GetSpaceShip().transform.position, GetSpaceShip().transform.right);

            if ((Vector3.Dot(GetSpaceShip().transform.right, characterControl.target.transform.position - GetSpaceShip().transform.position) > 0))
            {
                targetConfirmed = false;
            }

            RaycastHit2D hit = Physics2D.Raycast(GetSpaceShip().transform.position, GetSpaceShip().transform.TransformDirection(Vector3.right), 3.5f, LayerMask.GetMask("Asteroid"));

            Vector3 ss_position = GetSpaceShip().transform.position;

            if (hit.collider != null)
            {
                //avoiding = true;
                Vector3 forward = GetSpaceShip().transform.right;
                Vector3 toOther = hit.collider.transform.position - GetSpaceShip().transform.position;
                Vector3 toOther2 = characterControl.target.transform.position - GetSpaceShip().transform.position;
                if (Vector3.Dot(forward, toOther) > 0 && Vector3.Dot(forward, toOther2) > 0)
                {
                    animator.SetBool("avoiding", true);
                }
                else
                {
                    var velocityDirection = new Vector3(GetSpaceShip().transform.right.x - GetSpaceShip().Velocity.x, GetSpaceShip().transform.right.y - GetSpaceShip().Velocity.y, 0);
                    velocityDirection = Vector3.Reflect(velocityDirection, GetSpaceShip().transform.right);

                    if (Vector3.Distance(characterControl.target.transform.position, GetSpaceShip().transform.position) < 3f)
                        test = Mathf.Atan2(characterControl.target.transform.position.y - ss_position.y, characterControl.target.transform.position.x - ss_position.x);
                    else
                        test = Mathf.Atan2(characterControl.target.transform.position.y - ss_position.y + velocityDirection.y, characterControl.target.transform.position.x - ss_position.x + velocityDirection.x);
                    SetOrient(test * Mathf.Rad2Deg);
                }

            }
            else
            {
                var velocityDirection = new Vector3(GetSpaceShip().transform.right.x - GetSpaceShip().Velocity.x, GetSpaceShip().transform.right.y - GetSpaceShip().Velocity.y, 0);
                velocityDirection = Vector3.Reflect(velocityDirection, GetSpaceShip().transform.right);

                if (Vector3.Distance(characterControl.target.transform.position, GetSpaceShip().transform.position) < 3f)
                        test = Mathf.Atan2(characterControl.target.transform.position.y - ss_position.y, characterControl.target.transform.position.x - ss_position.x);
                else
                        test = Mathf.Atan2(characterControl.target.transform.position.y - ss_position.y + velocityDirection.y, characterControl.target.transform.position.x - ss_position.x + velocityDirection.x);
                SetOrient(test * Mathf.Rad2Deg);
            }

            if (characterControl.target.GetComponent<WayPoint>().Owner == GetSpaceShip().Owner)
            {
                targetConfirmed = false;
            }

            if (GameManager.Instance.GetScoreForPlayer(get_index_spaceship()) - GameManager.Instance.GetScoreForPlayer(get_enemy_index_spaceship()) > characterControl.dif_score)
            {
                animator.SetBool("berserk_mode", true);
            }

            float angle_ss2 = Vector3.Angle(ss2.transform.position - GetSpaceShip().transform.position, GetSpaceShip().transform.right);

            if (angle_ss2 < characterControl.view_field && Vector2.Distance(GetSpaceShip().transform.position, ss2.transform.position) < characterControl.distance_view_field)
            {
                animator.SetBool("attack_mode" , true);
            }

        }

    }
}
