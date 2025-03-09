using UnityEngine;
using System.Collections.Generic;

public class ValidatedRoomPicker:Room
{
    public GameObject[] roomChoices;

    public override Room createRoom(ExitConstraint requiredExits)
    {
        List<GameObject> roomTHatMeetConstraints = new List<GameObject>();
        foreach (GameObject room in roomChoices) {
            ValidatedRoom validatedRoom = room.GetComponent<ValidatedRoom>();
            if (validatedRoom.meetsConstraint(requiredExits))
                roomTHatMeetConstraints.Add(validatedRoom.gameObject);
        }

        GameObject roomPrefab = GlobalFuncs.randElem(roomTHatMeetConstraints);
        return roomPrefab.GetComponent<Room>().createRoom(requiredExits);

    }
}
