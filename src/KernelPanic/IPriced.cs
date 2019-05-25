
namespace KernelPanic
{
    interface IPriced
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

    enum Currency { Bitcoin, Experience };
}
