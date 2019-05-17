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

        private float delay_shot_tmp;
        public float threeshold_distancePoint;


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
            delay_shot_tmp = characterControl.delay_shot;
        }

        //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetThrust(1);
            SetMine(false);
            SetShot(false);

            if (delay_shot_tmp > 0)
            {
                delay_shot_tmp -= Time.deltaTime;
            }

            //Select a point to target, if the point is in a field of view, front of him, and he's not the owner of it
            Debug.DrawRay(GetSpaceShip().transform.position, new Vector3(GetSpaceShip().Velocity.x - GetSpaceShip().transform.right.x, GetSpaceShip().Velocity.y - GetSpaceShip().transform.right.y, 0), Color.yellow);
            if (!targetConfirmed)
            {
                dist_detection = float.MaxValue;
                foreach (WayPoint point in GetData().WayPoints)
                {
                    float distance = Vector3.Distance(GetSpaceShip().transform.position, point.transform.position);
                    angleTest = Vector3.Angle(point.transform.position - GetSpaceShip().transform.position, GetSpaceShip().transform.right);
                    if (distance < dist_detection && distance < threeshold_distancePoint && (Vector3.Dot(GetSpaceShip().transform.right, point.transform.position - GetSpaceShip().transform.position) > 0) &&  point.Owner != GetSpaceShip().Owner && angleTest < 65 || angleTest <-65)
                    {
                        targetCheck = point;
                        dist_detection = distance;
                    }
                }
                characterControl.target = targetCheck;
                targetConfirmed = true;
            }

            //If his target is behind him, he choose an other point but he's not in the state of avoiding cause in this state he can turn around an asteroid before come back to the target
            if ((Vector3.Dot(GetSpaceShip().transform.right, characterControl.target.transform.position - GetSpaceShip().transform.position) < 0) && !animator.GetBool("avoiding"))
                targetConfirmed = false;

            //The spaceship has a raycastHit just for mines, if he detect one then he shoot on it 
            RaycastHit2D hit_mine = Physics2D.Raycast(GetSpaceShip().transform.position, GetSpaceShip().transform.TransformDirection(Vector3.right), 2, LayerMask.GetMask("Mine"));
            if (hit_mine.collider != null)
            {
                if (delay_shot_tmp <= 0)
                {
                    SetShot(true);
                    delay_shot_tmp = characterControl.delay_shot;
                }
            }

            //He use a raycastHit for asteroid, he detect asteroid on his way, then he try to avoid them
            RaycastHit2D hit = Physics2D.Raycast(GetSpaceShip().transform.position, GetSpaceShip().transform.TransformDirection(Vector3.right), 3.5f, LayerMask.GetMask("Asteroid"));
            if (hit.collider != null)
            {
                Vector3 forward = GetSpaceShip().transform.right;
                Vector3 toOther = hit.collider.transform.position - GetSpaceShip().transform.position;
                Vector3 toOther2 = characterControl.target.transform.position - GetSpaceShip().transform.position;
                //If the target and the obstacle are front of him then he avoid the obstacle else he ignore the obstacle
                if (Vector3.Dot(forward, toOther) > 0 && Vector3.Dot(forward, toOther2) > 0)
                {
                    animator.SetBool("avoiding", true);
                }
                else//Inertia Calcul allow the spaceShip to targeta point against the inertia
                    InertiaCalcul();
            }
            else
                InertiaCalcul();

            //If he's a owner of his target, then he choose an other point
            if (characterControl.target.Owner == GetSpaceShip().Owner)
                targetConfirmed = false;

            //If he enter in a point an he's a owner of him then he drop a mine on it
            if (Physics2D.OverlapCircle(characterControl.target.Position, characterControl.target.Radius, LayerMask.GetMask("Player")) && characterControl.target.Owner == GetSpaceShip().Owner)
                SetMine(true);

            //If the AI score is too different then he enter in berserk mode, he track the player whenever the enemy is
            float angle_ss2 = Vector3.Angle(ss2.transform.position - GetSpaceShip().transform.position, GetSpaceShip().transform.right);
            if (GameManager.Instance.GetScoreForPlayer(get_index_spaceship()) - GameManager.Instance.GetScoreForPlayer(get_enemy_index_spaceship()) > characterControl.dif_score)
                animator.SetBool("berserk_mode", true);
            else
                if (angle_ss2 < characterControl.view_field && Vector2.Distance(GetSpaceShip().Position, ss2.transform.position) < characterControl.distance_view_field)
                    animator.SetBool("attack_mode", true);

            //If the enemy within a range and behind, he drop the bomb
            float dist_ = Vector2.Distance(ss2.transform.position, GetSpaceShip().Position);
            if (angle_ss2 > characterControl.view_field_mine &&  dist_ >  characterControl.dist_min_mine && dist_ < characterControl.dist_max_mine && GetSpaceShip().Energy > characterControl.energy_min_mine)
                SetMine(true);
        }

        void InertiaCalcul()
        {
            var velocityDirection = new Vector3(GetSpaceShip().transform.right.x - GetSpaceShip().Velocity.x, GetSpaceShip().transform.right.y - GetSpaceShip().Velocity.y, 0);
            velocityDirection = Vector3.Reflect(velocityDirection, GetSpaceShip().transform.right);

            float angle;
            if (Vector3.Distance(characterControl.target.transform.position, GetSpaceShip().transform.position) < 3f)
                angle = Mathf.Atan2(characterControl.target.transform.position.y - GetSpaceShip().transform.position.y, characterControl.target.transform.position.x - GetSpaceShip().transform.position.x);
            else
                angle = Mathf.Atan2(characterControl.target.transform.position.y - GetSpaceShip().transform.position.y + velocityDirection.y, characterControl.target.transform.position.x - GetSpaceShip().transform.position.x + velocityDirection.x);
            SetOrient(angle * Mathf.Rad2Deg);
        }
    }

    
}
