using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum ReactionTiming
{
    PRE,
    POST
}

public class ActionSystem : Singleton<ActionSystem>
{
    private List<GameAction> _reactions = null;
    public bool IsPerforming { get; private set; } = false;
    private static Dictionary<Type, List<Action<GameAction>>> _preSubs = new();
    private static Dictionary<Type, List<Action<GameAction>>> _postSubs = new();
    private static Dictionary<Type, Func<GameAction, IEnumerator>> _performers = new();

    public void PerformQueue(List<GameAction> queue)
    {
        foreach (GameAction action in queue)
        {
            Perform(action);
        }

        queue.Clear();
    }

    public void Perform(GameAction action, System.Action OnPerformFinished = null)
    {
        //Debug.LogError("Performing a game action: " + action.ToString());
        //if (IsPerforming)
        //{
        //    Debug.LogError("Action System is already performing a game action: " + action.ToString());
        //    return;
        //}
        IsPerforming = true;
        StartCoroutine(Flow(action, () =>
        {
            IsPerforming = false;
            OnPerformFinished?.Invoke();
        }));
    }

    

    public void AddReaction(GameAction gameAction)
    {
        _reactions?.Add(gameAction);
    }

    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    {
        //Debug.LogError("Pre Performer: " + action.ToString());
        _reactions = action.PreReactions;
        PerformSubscribers(action, _preSubs);
        yield return PerformReactions();


        //Debug.LogError("Perform Performer: " + action.ToString());
        _reactions = action.PerformReactions;
        yield return PerformPerformer(action);
        yield return PerformReactions();

        //Debug.LogError("Post Performer: " + action.ToString());
        _reactions = action.PostReactions;
        PerformSubscribers(action, _postSubs);
        yield return PerformReactions();

        OnFlowFinished?.Invoke();
    }

    private IEnumerator PerformPerformer(GameAction action)
    {
        Type type = action.GetType();
        if (_performers.ContainsKey(type))
        {
            yield return _performers[type](action);
        }
    }

    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
    {
        Type type = action.GetType();
        if (subs.ContainsKey(type))
        {
            foreach (var sub in subs[type])
            {
                sub(action);
            }
        }
    }

    private IEnumerator PerformReactions()
    {
        foreach (var reaction in _reactions)
        {
            yield return Flow(reaction);
        }
    }

    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {
        Type type = typeof(T);
        IEnumerator wrapperPerformer(GameAction action) => performer((T)action);
        if (_performers.ContainsKey(type)) _performers[type] = wrapperPerformer;
        else _performers.Add(type, wrapperPerformer);
    }

    public static void DetachPerformer<T>() where T : GameAction
    {
        Type type = typeof(T);
        if (_performers.ContainsKey((Type)type)) _performers.Remove(type);
    }

    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? _preSubs : _postSubs;
        void wrappedReaction(GameAction action) => reaction((T)action);
        if (subs.ContainsKey(typeof(T)))
        {
            subs[typeof(T)].Add(wrappedReaction);
        }
        else
        {
            subs.Add(typeof(T), new());
            subs[typeof(T)].Add(wrappedReaction);
        }
    }

    public static void UnsubscribeReaction<T>(Action <T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? _preSubs : _postSubs;
        if (subs.ContainsKey(typeof(T)))
        {
            void wrappedReaction(GameAction action) => reaction((T)action);
            subs[typeof(T)].Remove(wrappedReaction);
        }
    }

    public void Reset()
    {
        _preSubs.Clear();
        _postSubs.Clear();
        _performers.Clear();

        Instance = null;
    }
}
