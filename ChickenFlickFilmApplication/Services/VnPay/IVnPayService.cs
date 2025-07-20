using ChickenFlickFilmApplication.PaymentGatewayIntegration.VnPay;

namespace ChickenFlickFilmApplication.Services.VnPay
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }
}
