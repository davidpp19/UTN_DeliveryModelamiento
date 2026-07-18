namespace Delivery.Modelos.Interfaces
{
    public interface ICalculate
    {
        double ICalculateIVA(double valuePay);
        double ICalculateTotal();
        double ICalculateSubtotal();
        double ICalculateCurrentTotal();
    }
}
