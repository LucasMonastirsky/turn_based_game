using Combat;
using System.Threading.Tasks;

public class PlayerController : Controller {
	private TaskCompletionSource<CombatAction> RequestActionCompletionSource;

	public override async Task<CombatAction> RequestAction () {
		RequestActionCompletionSource = new ();
		
		ActionDisplay.RequestAction(this.Combatant);

		return await RequestActionCompletionSource.Task;
	}

	public override void DeliverAction (CombatAction action) {
		RequestActionCompletionSource.SetResult(action);
	}

    public override void CancelSelection () {
        ActionDisplay.ShowActionList();
    }
}
