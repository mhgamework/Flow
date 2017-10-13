using System.Collections.Generic;

namespace Assets.Gandalf.Domain
{
    public interface ICellEntity
    {
        string GetCellInfo();
         IDictionary<string, string> Values { get; }
    }
}