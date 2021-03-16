namespace Behaviours
{
	public interface IAiBehaviour
	{
		void Execute();
		void Cancel();
		bool IsRunning { get; }
	}
}

