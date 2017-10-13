using System.Collections.Generic;

namespace Assets.Gandalf.Domain
{
    public interface IUISelectable
    {
        ItemCollection Items { get; }
        IDictionary<string,string> Values { get; }
        string Name { get; }
        IEnumerable<string> Actions { get; }

        void DoAction(string action);
    }
}