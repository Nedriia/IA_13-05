using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FGAE
{
    public class AI_Controller : BaseSpaceShipController
    {
        // Data vars
        public SpaceShip spaceShip_FGAE;
        public GameData data_FGAE;
        public float thrust;
        public float targetOrient;
        public bool shoot;
        public bool mine;

        // Points to go
        public WayPoint target;
        public Vector3 ss_target;
        public Vector3 avoinding_pos;

        //GD values
        public int dif_score;
        // The field of view of the SpaceShip
        public float view_field;
        // How much time stays in Attack Mode
        public float attack_mode_time;
        // Distance detection for attack mode
        public float distance_view_field;
        // a Delay to recalcul a point when the SpaceShip is blocked
        public float time_recalc_escapePoint;
        // // The field of view of the SpaceShip (behind)
        public float view_field_mine;
        // Distance to put Mines
        public float dist_min_mine;
        public float dist_max_mine;
        // Min enegy to put a mine
        public float energy_min_mine;
        // Delay Shoot
        public float delay_shot = 0.5f;


        private void Start()
        {
            if(GetComponent<Animator>() == null)
            {
                Animator animator = gameObject.AddComponent<Animator>();
                animator.runtimeAnimatorController = Resources.Load("FGAE_StateMachineController_Easy") as RuntimeAnimatorController;
            }
        }

        public override InputData UpdateInput(SpaceShip spaceship, GameData data)
        {
            if(spaceShip_FGAE == null)
                spaceShip_FGAE = spaceship;
            data_FGAE = data;

            return new InputData(thrust, targetOrient, shoot, mine);
        }
    }
}