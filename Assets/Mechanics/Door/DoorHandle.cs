using System;
using UnityEngine;

public class DoorHandle : MonoBehaviour
{
    [SerializeField] private Door door;
    private Animator _animator;
    private bool isOpen;
    private static readonly int TriggerLever = Animator.StringToHash("TriggerLever");

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        if (door != null && door.OpenDoor())
        {
            // play lever sound
            _animator.SetTrigger(TriggerLever);
        }
        else
        {
            Debug.LogWarning("No door assigned to this handle.");
        }
    }
}