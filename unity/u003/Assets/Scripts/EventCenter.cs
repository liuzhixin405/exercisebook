using System;
using System.Collections;
using UnityEditorInternal;
using UnityEngine;


public class EventCenter
{
	public static EventCenter Instance
	{
		get
		{
			if (_instance == null)
				_instance = new EventCenter();
			return _instance;

        }
	}
	private static EventCenter _instance;
	public GameEvent Test = new GameEvent();

    public GameEvent<GameObject, Collision> OnCollisionEnter = new GameEvent<GameObject, Collision>();
    public GameEvent GameReStart = new GameEvent();

   
}

public class GameEvent
{
    private Action _action;
	public GameEvent()
	{

	}

	public void AddListener(Action action)
	{
		_action += action;
	}

	public void RemoveListener(Action action)
	{
		if (_action != null)
			_action -= action;
	}

	public void Broadcast()
	{
		_action?.Invoke();
	}

	public void Clear()
	{
		_action = null;
	}
}

public class GameEvent<T>
{
    private Action<T> _action;
    public GameEvent(T t1)
    {

    }
    public GameEvent()
    {

    }

    public void AddListener(Action<T> action)
    {
        _action += action;
    }

    public void RemoveListener(Action<T> action)
    {
        if (_action != null)
            _action -= action;
    }

    public void Broadcast(T t)
    {
        _action?.Invoke(t);
    }

    public void Clear()
    {
        _action = null;
    }
}

public class GameEvent<T1,T2>
{
    private Action<T1, T2> _action;
    public GameEvent(T1 t1,T2 t2)
    {

    }
    public GameEvent()
    {

    }
    public void AddListener(Action<T1, T2> action)
    {
        _action += action;
    }

    public void RemoveListener(Action<T1, T2> action)
    {
        if (_action != null)
            _action -= action;
    }

    public void Broadcast(T1 t1,T2 t2)
    {
        _action?.Invoke(t1,t2);
    }

    public void Clear()
    {
        _action = null;
    }
}

public class GameEvent<T1, T2,T3>
{
    private Action<T1, T2, T3> _action;
    public GameEvent(T1 t1, T2 t2,T3 t3)
    {

    }
    public GameEvent()
    {

    }
    public void AddListener(Action<T1, T2,T3> action)
    {
        _action += action;
    }

    public void RemoveListener(Action<T1, T2, T3> action)
    {
        if (_action != null)
            _action -= action;
    }

    public void Broadcast(T1 t1, T2 t2,T3 t3)
    {
        _action?.Invoke(t1, t2,t3);
    }

    public void Clear()
    {
        _action = null;
    }
}