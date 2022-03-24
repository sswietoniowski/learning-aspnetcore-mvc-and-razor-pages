using Braintree;

namespace BulkyBook.Utility
{
    public interface IBrainTreeGateway
    {
        IBraintreeGateway CreateGateway();
        IBraintreeGateway GetGateway();
    }
}
