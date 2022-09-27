using System;
using System.Collections.Generic;

namespace PrimerParcial.Gameplay.Entities.Agent
{
    [Serializable]
    public class AgentFSM
    {
        private bool isEnable = false;
        private int currentState;
        private int[,] relations;
        private Dictionary<int, List<Action>> behaviours;

        public AgentFSM(int states, int flags)
        {
            currentState = -1;

            relations = new int[states, flags];
            for (int i = 0; i < states; i++)
                for (int j = 0; j < flags; j++)
                    relations[i, j] = -1;

            behaviours = new Dictionary<int, List<Action>>();
        }

        public void ToggleBehaviour(bool state)
        {
            isEnable = state;
        }

        public void ForceCurretState(int state)
        {
            currentState = state;
        }

        public void SetRelation(int sourceState, int flag, int destinationState)
        {
            relations[sourceState, flag] = destinationState;
        }

        public void SetFlag(int flag)
        {
            if (relations[currentState, flag] != -1)
                currentState = relations[currentState, flag];
        }

        public int GetCurrentState()
        {
            return currentState;
        }

        public void SetBehaviour(int state, Action behaviour)
        {
            List<Action> newBehaviours = new List<Action>();
            newBehaviours.Add(behaviour);

            if (behaviours.ContainsKey(state))
                behaviours[state] = newBehaviours;
            else
                behaviours.Add(state, newBehaviours);
        }

        public void AddBehaviour(int state, Action behaviour)
        {

            if (behaviours.ContainsKey(state))
                behaviours[state].Add(behaviour);
            else
            {
                List<Action> newBehaviours = new List<Action>();
                newBehaviours.Add(behaviour);
                behaviours.Add(state, newBehaviours);
            }
        }

        public void Update()
        {
            if(!isEnable)
            {
                return;
            }

            if (behaviours.ContainsKey(currentState))
            {
                List<Action> actions = behaviours[currentState];
                if (actions != null)
                {
                    for (int i = 0; i < actions.Count; i++)
                    {
                        if (actions[i] != null)
                        {
                            actions[i].Invoke();
                        }
                    }
                }
            }
        }
    }
}