namespace Nanoray.Shrike
{
#if NET7_0_OR_GREATER
    public interface IGenerable<T> where T : IGenerable<T>
    {
        static abstract T Generate();
    }
#endif
}
