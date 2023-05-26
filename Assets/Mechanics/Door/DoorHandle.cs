using UnityEngine;

public class DoorHandle : MonoBehaviour
{
    public Door door;

    public void OpenDoor()
    {
        if(door != null)
        {
            door.OpenDoor();
        }
        else
        {
            Debug.LogWarning("No door assigned to this handle.");
        }
    }
}