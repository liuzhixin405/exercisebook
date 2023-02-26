using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Updater
{
    private Action _action;

    public void Add(Action action)
    {
        _action += action;
    }

    public void Remove(Action action)
    {
        if(_action!= null)
        {
            _action -= action;
        }
    }
    public void Update()
    {
        _action?.Invoke();

    }
    public void Clear()
    {
        _action = null;
    }
}

