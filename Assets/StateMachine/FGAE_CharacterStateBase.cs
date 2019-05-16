using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGAE
{
    public class FGAE_CharacterStateBase : StateMachineBehaviour
    {
        public AI_Controller characterControl;

        public AI_Controller GetCharacterControl(Animator animator)
        {
            if(characterControl == null)
            {
                characterControl = animator.GetComponentInParent<AI_Controller>();
            }

            return characterControl;
        }

        public SpaceShip GetSpaceShip()
        {
            return characterControl.spaceShip_FGAE;
        }

        public GameData GetData()
        {
            return characterControl.data_FGAE;
        }

        public void SetThrust(float thrust)
        {
            characterControl.thrust = thrust;
        }

        public void SetOrient(float orient)
        {
            characterControl.targetOrient = orient;
        }

        public void SetShot(bool shoot_)
        {
            characterControl.shoot = shoot_;
        }

        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }

        public int get_index_spaceship()
        {
            if (GetSpaceShip().name == "SpaceShip1" )
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }


        public int get_enemy_index_spaceship()
        {
            if (GetSpaceShip().name == "SpaceShip1")
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }


    }
}
