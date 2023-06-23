using System;
using System.Collections;
using System.Collections.Generic;
using BitStrap;
using Elad.Events;
using Elad.Scripts;
using Unity.Mathematics;
using UnityEngine;

[DefaultExecutionOrder(200)]
public class HealthBar : MonoBehaviour
{
    [SerializeField] private GameObject heartPrefab;
    
    private List<Heart> hearts = new List<Heart>();
    private int MaxHealth => PlayerStatus.maxHealth;
    private float CurrentHealth => PlayerStatus.curHealth;
    
    private void InstantiateHearts()
    {
        for (var i = 0; i < math.ceil(MaxHealth/2f); i++)
        {
            GameObject newHeart = Instantiate(heartPrefab, transform);
            Heart heartComponent = newHeart.GetComponent<Heart>();
            heartComponent.SetHeartImage(HeartStatus.Empty);
            hearts.Add(heartComponent);
        }
    }
    
    private void DrawHearts()
    {
        if (CurrentHealth > MaxHealth)
        {
            Debug.Log("[HealthBar] Cant draw more hearts than maxHealth");
            return;
        }
        
        if (CurrentHealth < 0)
        {
            Debug.Log("[HealthBar] Cant you DrawHearts with negative values");
            return;
        }
        
        for (int i = 0; i < hearts.Count; i++)
        {
            if (i < math.floor(CurrentHealth / 2f))
            {
                hearts[i].SetHeartImage(HeartStatus.Full);
            }
            else
            {
                hearts[i].SetHeartImage(HeartStatus.Empty);
            }
        }
        if ((CurrentHealth % 2) != 0)
        {
            // print("Index: " + (int)Math.Ceiling(CurrentHealth/2f) + "  current health: " + CurrentHealth);
            hearts[(int)math.ceil(CurrentHealth/2f) - 1].SetHeartImage(HeartStatus.Half);
        }
    }

    private void DestroyHearts()
    {
        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
        hearts = new List<Heart>();
    }
    
    private void OnEnable()
    {
        InstantiateHearts();
        DrawHearts();
        characterEvents.CharacterHealed.AddListener(DrawHeartsFitted);
        characterEvents.CharacterDamaged.AddListener(DrawHeartsFitted);
        characterEvents.FunctionsLoad.AddListener(DrawHearts);
    }

    private void OnDisable()
    {
        characterEvents.CharacterHealed.RemoveListener(DrawHeartsFitted);
        characterEvents.CharacterDamaged.RemoveListener(DrawHeartsFitted);
        characterEvents.FunctionsLoad.RemoveListener(DrawHearts);
    }


    private void DrawHeartsFitted(GameObject arg0 = null, int arg1 = 0)
    {
        DrawHearts();
    }
    
}
