namespace Assets.SimpleGame.PowerBeams
{
    public interface IBeamPowerReceiver
    {
        void OnBeamRemoved(BeamScript beamScript);
        void OnBeamHit(BeamScript beamScript);
        void ReceivePacket(BeamScript beamScript);
    }
}