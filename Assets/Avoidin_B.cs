using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGAE
{
    public class Avoidin_B : FGAE_CharacterStateBase
    {

        public float time_recalc_escapePoint_tmp;

        //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            GetCharacterControl(animator);

            New_Point_Calc(animator);


        }

        //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (time_recalc_escapePoint_tmp < characterControl.time_recalc_escapePoint)
            {
                time_recalc_escapePoint_tmp += Time.deltaTime;
            }

            if (Vector3.Distance(characterControl.avoinding_pos, GetSpaceShip().transform.position) < 1f)
            {
                if (GetSpaceShip().Velocity.magnitude < 0.2f)
                {
                    var angle = Mathf.Atan2(characterControl.avoinding_pos.y - GetSpaceShip().transform.position.y, characterControl.avoinding_pos.x - GetSpaceShip().transform.position.x);
                    SetOrient(angle * Mathf.Rad2Deg);
                }
                else
                {
                    animator.SetBool("avoiding", false);
                }
            }
            else
            {
                var angle = Mathf.Atan2(characterControl.avoinding_pos.y - GetSpaceShip().transform.position.y, characterControl.avoinding_pos.x - GetSpaceShip().transform.position.x);
                SetOrient(angle * Mathf.Rad2Deg);
            }

            if (GetSpaceShip().Velocity.magnitude < 0.2f && time_recalc_escapePoint_tmp >= characterControl.time_recalc_escapePoint)
            {
                New_Point_Calc(animator);
            }

        }

        //OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        public void New_Point_Calc(Animator animator)
        {
            RaycastHit2D hit = Physics2D.Raycast(GetSpaceShip().transform.position, GetSpaceShip().transform.TransformDirection(Vector3.right), 3.5f, LayerMask.GetMask("Asteroid"));

            time_recalc_escapePoint_tmp = 0;

            float radius = 0;

            if (hit.collider != null)
                radius = hit.collider.GetComponent<CircleCollider2D>().radius * 1.3f;

            Vector3 pos1 = Vector3.zero;
            Vector3 pos2 = Vector3.zero;
            float degre_b;

            if (animator.GetBool("berserk_mode"))
            {
                degre_b = Mathf.Atan2((characterControl.ss_target - GetSpaceShip().transform.position).y, (characterControl.ss_target - GetSpaceShip().transform.position).x) * Mathf.Rad2Deg;
            }
            else
            {
                degre_b = Mathf.Atan2((characterControl.ss_target - GetSpaceShip().transform.position).y, (characterControl.ss_target - GetSpaceShip().transform.position).x) * Mathf.Rad2Deg;
            }
            

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
            pos1 = (Vector3)DegreeToVector2(degre_b + degre_r + 5).normalized * (hit.distance + (radius / 2)) + GetSpaceShip().transform.position;
            pos2 = (Vector3)DegreeToVector2(degre_b - degre_l - 5).normalized * (hit.distance + (radius / 2)) + GetSpaceShip().transform.position;

            if (Vector3.Distance(GetSpaceShip().transform.position, pos1) + Vector3.Distance(pos1, (Vector3)GetCharacterControl(animator).spaceShip_FGAE.GetComponent<Rigidbody2D>().velocity + GetSpaceShip().transform.position) * 30
                > Vector3.Distance(GetSpaceShip().transform.position, pos2) + Vector3.Distance(pos2, (Vector3)GetCharacterControl(animator).spaceShip_FGAE.GetComponent<Rigidbody2D>().velocity + GetSpaceShip().transform.position) * 30)
            {
                characterControl.avoinding_pos = pos2;
            }
            else
            {
                characterControl.avoinding_pos = pos1;
            }
            Debug.DrawRay(GetSpaceShip().transform.position, (characterControl.avoinding_pos - GetSpaceShip().transform.position), Color.red, 2);
        }
    }
}
