using System.Threading.Tasks;

namespace Combat {
    public interface IController {
        public Task<ICombatant> RequestSingleTarget (TargetSelector selector);
    }
}