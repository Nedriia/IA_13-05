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


    }
}
