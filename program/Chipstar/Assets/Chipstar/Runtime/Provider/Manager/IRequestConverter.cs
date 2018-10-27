namespace Chipstar.Downloads
{
    internal interface IRequestConverter
    {
        ILoadJob Create( IRuntimeBundleData data );
    }
}