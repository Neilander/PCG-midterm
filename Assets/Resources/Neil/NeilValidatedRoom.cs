using System;
using UnityEngine;

public class NeilValidatedRoom : Room
{
    
    public RoomExitMatrix matrix;


    public override Room createRoom(ExitConstraint requiredExits)
    {
        return base.createRoom(requiredExits);
    }

    public bool meetConstraint(ExitConstraint requiredExits)
    {
        return true;
    }
}


