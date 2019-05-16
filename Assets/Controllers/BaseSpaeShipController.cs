using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InputData {
    public float thrust;
    public float targetOrientation;
    public bool shoot;
    public bool dropMine;
    public InputData(float thrust, float targetOrient, bool shoot, bool dropMine) {
        this.thrust = thrust; this.targetOrientation = targetOrient; this.shoot = shoot; this.dropMine = dropMine;
    }
}


public abstract class BaseSpaceShipController : MonoBehaviour
{
    public virtual void Initialize(SpaceShip spaceship, GameData data)
    {

    }

    public virtual InputData UpdateInput(SpaceShip spaceship, GameData data)
    {
        return new InputData(0.0f, 0.0f, false, false);
    }
}
