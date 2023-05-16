


using System;
using UnityEngine;
using UnityEngine.Events;

public static class characterEvents
{
    public static UnityAction<GameObject, int> CharacterDamaged;
    public static UnityAction<GameObject, int> CharacterHealed;
}