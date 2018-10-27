namespace Chipstar.Downloads
{
    public interface IRequestConverter
    {
        ILoadJob Create( IRuntimeBundleData data );
    }
}