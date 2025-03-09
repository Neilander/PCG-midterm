using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The Random Choice room is a simple room that just 
// Chooses from a list of other rooms when it's created. 
// Think of it as the "pick a card from this hand of rooms" option
public class zt649ValidatedRoomPicker : Room {

	public GameObject[] roomChoices;

	public override Room createRoom(ExitConstraint requiredExits)
	{
		List<GameObject> roomsThatMeetConstraints = new List<GameObject>();

		foreach (GameObject room in roomChoices)
		{
			ValidatedRoom validatedRoom = room.GetComponent<ValidatedRoom>();
			if (validatedRoom.meetsConstraint(requiredExits))
				roomsThatMeetConstraints.Add(validatedRoom.gameObject);
		}
		
		GameObject roomPrefab = GlobalFuncs.randElem(roomsThatMeetConstraints);
		return roomPrefab.GetComponent<Room>().createRoom(requiredExits);
	}
}
