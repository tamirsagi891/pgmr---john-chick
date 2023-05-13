using System.Collections;
using System.Collections.Generic;
using Avrahamy.EditorGadgets;
using UnityEngine;
using Logger = Nemesh.Logger;

public class SetBoolBehavior : StateMachineBehaviour
{
    public string boolName;

    public bool updateOnStateMachine;
    public bool updateOnState;
    public bool valueOnEnter;
    public bool valueOnExit;

    [Header("Nemesh additions")]
    public bool resetValueOnExit;
    private bool _valueWhenEntered;

    [Space]
    public bool callFunction;
    [ConditionalHide("callFunction")]
    public string functionToCallWhenChanging;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (callFunction && functionToCallWhenChanging == "")
        {
            functionToCallWhenChanging = boolName;
        }
        
        if (updateOnState)
        {
            _valueWhenEntered = animator.GetBool(boolName); 
        
            animator.SetBool(boolName, valueOnEnter);
            if (callFunction)
            {
                animator.SendMessage(functionToCallWhenChanging, valueOnEnter);
            }
        }
    }

    // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (updateOnState)
        {
            var val = resetValueOnExit ? _valueWhenEntered : valueOnExit;
            animator.SetBool(boolName, val);
            if (callFunction)
            {
                animator.SendMessage(functionToCallWhenChanging, val);
            }
        }
    
    }

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {
        if (callFunction && functionToCallWhenChanging == "")
        {
            functionToCallWhenChanging = boolName;
        }

        if (updateOnStateMachine)
        {
            _valueWhenEntered = animator.GetBool(boolName);
            animator.SetBool(boolName, valueOnEnter);
            if (callFunction)
            {
                animator.SendMessage(functionToCallWhenChanging, valueOnEnter);
            }
        }
    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        if (updateOnStateMachine)
        {
            var val = resetValueOnExit ? _valueWhenEntered : valueOnExit;
            animator.SetBool(boolName, val);
            if (callFunction)
            {
                animator.SendMessage(functionToCallWhenChanging, val);
            }
        }
    }
}