namespace Chipstar.Downloads
{
    internal interface IRequestConverter
    {
        ILoadRequest Create( IRuntimeBundleData data );
    }
}