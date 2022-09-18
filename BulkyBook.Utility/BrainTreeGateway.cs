using Braintree;
using Microsoft.Extensions.Options;

namespace BulkyBook.Utility;

public class BrainTreeGateway : IBrainTreeGateway
{
    private readonly BrainTreeOptions brainTreeOptions;
    private IBraintreeGateway BraintreeGateway { get; set; }

    public BrainTreeGateway(IOptions<BrainTreeOptions> brainTreeOptions)
    {
        this.brainTreeOptions = brainTreeOptions.Value;
    }

    public IBraintreeGateway CreateGateway()
    {
        return new BraintreeGateway(brainTreeOptions.Environment, brainTreeOptions.MerchantID, brainTreeOptions.PublicKey, brainTreeOptions.PrivateKey);
    }

    public IBraintreeGateway GetGateway()
    {
        if (BraintreeGateway is null)
        {
            BraintreeGateway = CreateGateway();
        }

        return BraintreeGateway;
    }
}