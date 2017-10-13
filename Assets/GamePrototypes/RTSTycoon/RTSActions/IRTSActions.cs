
using System.Collections.Generic;
using UnityEngine;

public interface IRTSActionProvider
{
    IEnumerable<IRTSAction> GetActions();
}

public interface IRTSAction
{
    string Name { get; }
    void Execute(Component target);
}