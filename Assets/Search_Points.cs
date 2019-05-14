using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace FGAE
{
    public class Search_Points : FGAE_CharacterStateBase
    {
        public bool avoidance;

        public int count;
        public bool targetConfirmed;
        public WayPoint target;
        public float targetOrient;
        //public Collider2D avoiding_rock;
        public bool avoiding;
        public Vector3 avoinding_pos;


        //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetCharacterControl(animator);
        }

        //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            SetThrust(1);
            if (!targetConfirmed)
            {
                target = GameManager.Instance.GetGameData().WayPoints[count];
                targetConfirmed = true;
            }

            RaycastHit2D hit = Physics2D.Raycast(GetSpaceShip().transform.position, GetSpaceShip().transform.TransformDirection(Vector3.right), 3.5f, LayerMask.GetMask("Asteroid"));

            RaycastHit2D hit_Avoid = Physics2D.Raycast(GetSpaceShip().transform.position, (target.transform.position - GetSpaceShip().transform.position), Vector3.Distance(target.transform.position, GetSpaceShip().transform.position), LayerMask.GetMask("Asteroid"));

            float radius = 0;

            if (hit_Avoid.collider != null)
                radius = hit_Avoid.collider.GetComponent<CircleCollider2D>().radius *1.3f;
            Vector3 ss_position = GetSpaceShip().transform.position;

            if (hit.collider == null && hit_Avoid.collider != null)
            {
                if (!avoiding)
                {
                    avoiding = true;
                    Debug.DrawRay(ss_position, GetSpaceShip().transform.TransformDirection(Vector3.right) * 3.5f, Color.yellow);
                    Vector3 pos1 = Vector3.zero;
                    Vector3 pos2 = Vector3.zero;

                    if ((ss_position.x > hit_Avoid.collider.transform.position.x && ss_position.y > hit_Avoid.collider.transform.position.y) || (ss_position.x < hit_Avoid.collider.transform.position.x && ss_position.y < hit_Avoid.collider.transform.position.y))
                    {
                        pos1 = new Vector3(-radius, radius, 0);
                        pos2 = new Vector3(radius, -radius, 0);
                    }
                    else
                    {
                        pos1 = new Vector3(radius, radius, 0);
                        pos2 = new Vector3(-radius, -radius, 0);
                    }
                    if (Vector3.Distance(ss_position, pos1) > Vector3.Distance(ss_position, pos2))
                    {
                        avoinding_pos = pos2;
                    }
                    else
                    {
                        avoinding_pos = pos1;
                    }
                    
                }
                if (Vector3.Distance(ss_position, new Vector3(hit_Avoid.collider.transform.position.x + avoinding_pos.x, (hit_Avoid.collider.transform.position.y + avoinding_pos.y), 0)) > 0.5f)
                {
                    var angle_ = Mathf.Atan2((hit_Avoid.collider.transform.position.y + avoinding_pos.y) - ss_position.y, (hit_Avoid.collider.transform.position.x + avoinding_pos.x) - ss_position.x);
                    SetOrient(angle_ * Mathf.Rad2Deg);
                }
                else
                {
                    var angle = Mathf.Atan2(target.transform.position.y - ss_position.y, target.transform.position.x - ss_position.x);
                    SetOrient(angle * Mathf.Rad2Deg);
                }

                /*var angle = Mathf.Atan2((hit_Avoid.collider.transform.position.y + radius) - ss_position.y, (hit_Avoid.collider.transform.position.x + radius) - ss_position.x);
                SetOrient(angle * Mathf.Rad2Deg);*/
            }
            else if (hit.collider != null && hit_Avoid.collider != null)
            {
                if (!avoiding)
                {
                    avoiding = true;
                    Debug.DrawRay(ss_position, GetSpaceShip().transform.TransformDirection(Vector3.right) * 3.5f, Color.yellow);

                    Vector3 pos1 = Vector3.zero;
                    Vector3 pos2 = Vector3.zero;

                    if ((ss_position.x > hit.collider.transform.position.x && ss_position.y > hit.collider.transform.position.y) || (ss_position.x < hit.collider.transform.position.x && ss_position.y < hit.collider.transform.position.y))
                    {
                        pos1 = new Vector3(-radius, radius, 0);
                        pos2 = new Vector3(radius, -radius, 0);
                        Debug.Log("DROITE-HAUT + GAUCHE + HAUT");
                    }
                    else
                    {
                        pos1 = new Vector3(radius, radius, 0);
                        pos2 = new Vector3(-radius, -radius, 0);
                    }
                    if (Vector3.Distance(ss_position, pos1) > Vector3.Distance(ss_position, pos2))
                    {
                        avoinding_pos = pos2;
                    }
                    else
                    {
                        avoinding_pos = pos1;
                    }
                }

                if (Vector3.Distance(ss_position, new Vector3(hit_Avoid.collider.transform.position.x + avoinding_pos.x, (hit_Avoid.collider.transform.position.y + avoinding_pos.y), 0)) > 0.5f)
                {
                    var angle = Mathf.Atan2((hit.collider.transform.position.y + avoinding_pos.y) - ss_position.y, (hit.collider.transform.position.x + avoinding_pos.x) - ss_position.x);
                    SetOrient(angle * Mathf.Rad2Deg);
                }
                else
                {
                    var angle = Mathf.Atan2(target.transform.position.y - ss_position.y, target.transform.position.x - ss_position.x);
                    SetOrient(angle * Mathf.Rad2Deg);
                }

                //
                /*Debug.DrawRay(ss_position, GetSpaceShip().transform.TransformDirection(Vector3.right) * 3.5f, Color.red);
                var angle = Mathf.Atan2((hit.collider.transform.position.y + radius) - ss_position.y, (hit.collider.transform.position.x + radius) - ss_position.x);
                SetOrient(angle * Mathf.Rad2Deg);*/
            }
            else if (hit_Avoid.collider == null)
            {
                var angle = Mathf.Atan2(target.transform.position.y - ss_position.y, target.transform.position.x - ss_position.x);
                SetOrient(angle * Mathf.Rad2Deg);
                avoiding = false;
            }

            Debug.DrawRay(ss_position, (target.transform.position - GetSpaceShip().transform.position), Color.green);


            if (GetData().WayPoints[count].GetComponent<WayPoint>().Owner == GetSpaceShip().Owner)
            {
                targetConfirmed = false;
                //count++;
                count = Random.Range(0, GetData().WayPoints.Count);
            }

            if (count == GetData().WayPoints.Count)
                count = 0;
        }

        //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }
    }
}
