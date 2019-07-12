
namespace KernelPanic.Purchasing
{
    internal interface IPriced
    {
        Currency Currency
        {
            get;
        }

        int Price
        {
            get;
        }
    }

    internal enum Currency { Bitcoin, Experience };
}
