namespace Assets.RTSActions
{
    public class CreateWorkerAction :BaseRTSAction<TownCenter>
    {
        public override void Execute(TownCenter target)
        {
            target.QueueWorker();
        }

        public override string Name
        {
            get { return "Create Worker"; }
        }
    }
}