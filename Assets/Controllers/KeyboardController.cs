using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardController : BaseSpaceShipController
{
    public override InputData UpdateInput(SpaceShip spaceship, GameData data)
    {
        float thrust = (Input.GetAxis("Vertical") > 0.0f) ? 1.0f : 0.0f;
        float targetOrient = spaceship.Orientation;
        float direction = Input.GetAxis("Horizontal");
        if(direction != 0.0f)
        {
            targetOrient -= Mathf.Sign(direction) * 90;
        }
        bool shoot = Input.GetButtonDown("Fire1");
        bool dropMine = Input.GetButtonDown("Fire2");

        return new InputData(thrust, targetOrient, shoot, dropMine);
    }

}
