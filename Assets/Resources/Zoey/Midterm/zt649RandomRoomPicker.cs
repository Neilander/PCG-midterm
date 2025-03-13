using System;
using System.Collections.Generic;
using UnityEngine;

public class zt649RandomRoomPicker : Room
{
    public GameObject[] roomPrefabs;
    public int[] roomNumbers;
    public GameObject[] validatedRoomPrefabs;
    [HideInInspector] public int m_maxRoomCount = 0;

    private List<int> m_distributedRoomArr = new List<int>();
    private bool m_roomDistributed = false;
    private int m_createdRoomCount = 0;
        
    public override Room createRoom(ExitConstraint requiredExits)
    {
        // Unity has a bug that creates cache for the private variables like m_createdRoomCount
        // We need to manually reset these values
        // However createRoom is called in Awake() of Game Manager, so I found a walkaround to solve this.
        if (m_createdRoomCount >= m_maxRoomCount)
            ResetValue();
        
        if (!m_roomDistributed) 
            DistributeRooms();
        
        // Choose which room to generate
        if (m_distributedRoomArr[m_createdRoomCount] == -1)
        {
            GameObject _validatedRoom = ValidatedRoomPicker(requiredExits);
            ++m_createdRoomCount;
            return _validatedRoom.GetComponent<Room>().createRoom(requiredExits);
        }
        else
        {
            int _roomIndex = m_distributedRoomArr[m_createdRoomCount];
            ++m_createdRoomCount;
            return roomPrefabs[_roomIndex].GetComponent<Room>().createRoom(requiredExits);
        }

    }

    private void DistributeRooms()
    {
        // Create room bag
        LevelGenerator _lvGen = GameObject.Find("_GameManager").GetComponent<GameManager>().levelGenerator;
        m_maxRoomCount = _lvGen.numXRooms * _lvGen.numYRooms;
        int _roomCount = 0;
        
        for (int i = 0; i < roomPrefabs.Length; i++)
        {
            for (int j = 0; j < roomNumbers[i]; j++)
            {
                m_distributedRoomArr.Add(i);
                // Make sure that the total room number does not exceed max room count
                if (++_roomCount == m_maxRoomCount)
                    break;
            }
        }

        // Fill the empty spots on room list with blank rooms
        for (int i = 0; i < (m_maxRoomCount - _roomCount); i++)
        {
            m_distributedRoomArr.Add(-1);
        }
        
        GlobalFuncs.shuffle(m_distributedRoomArr);

        m_roomDistributed = true;
    }

    private GameObject ValidatedRoomPicker(ExitConstraint requiredExits)
    {
        List<GameObject> roomTHatMeetConstraints = new List<GameObject>();
        foreach (GameObject room in validatedRoomPrefabs) {
            ValidatedRoomNeil validatedRoom = room.GetComponent<ValidatedRoomNeil>();
            if (validatedRoom.meetsConstraint(requiredExits))
                roomTHatMeetConstraints.Add(validatedRoom.gameObject);
        }

        return GlobalFuncs.randElem(roomTHatMeetConstraints);
    }

    private void ResetValue()
    {
        m_createdRoomCount = 0;
        m_roomDistributed = false;
    }
    
}
