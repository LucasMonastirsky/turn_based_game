using Combat;
using Development;
using System.Threading.Tasks;

public class PlayerController : Controller {
	private TaskCompletionSource<CombatAction> RequestActionCompletionSource;
	private int a = 0;

	public override async Task<CombatAction> RequestAction () {
		Dev.Log($"Requested action {a} {this.GetHashCode()}");
		RequestActionCompletionSource = new ();
		
		ActionDisplay.RequestAction(this.Combatant);

		return await RequestActionCompletionSource.Task;
	}

	public override void DeliverAction (CombatAction action) {
		Dev.Log($"Delivered action {a} {this.GetHashCode()}");
		RequestActionCompletionSource.SetResult(action);
	}

    public override void CancelSelection () {
        ActionDisplay.ShowActionList();
    }
}
