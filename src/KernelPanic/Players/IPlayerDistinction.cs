namespace KernelPanic.Players
{
    internal interface IPlayerDistinction
    {
        T Select<T>(T ifActive, T ifPassive);
    }
}
