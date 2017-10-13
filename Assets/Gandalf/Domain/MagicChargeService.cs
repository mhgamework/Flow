using System.Collections.Generic;
using System.Linq;

namespace Assets.Gandalf.Domain
{
    public class MagicChargeService
    {
        public List<IMagicChargeDistributor> MagicChargeDistributors { get; private set; }

        public MagicChargeService()
        {
            MagicChargeDistributors = new List<IMagicChargeDistributor>();
        }

        public bool HasMagic(Cell c)
        {
            return getDistributors(c).Any(k => k.HasMagic);
        }

        private IEnumerable<IMagicChargeDistributor> getDistributors(Cell cell)
        {
            return MagicChargeDistributors.Where(k => k.InRange(cell));
        }
    }
}