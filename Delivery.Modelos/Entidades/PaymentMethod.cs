namespace Delivery.Modelos.Entidades
{
    public class PaymentMethod
    {
        public string TypePayment { get; set; } = string.Empty;

        public PaymentMethod() { }

        public PaymentMethod(string typePayment)
        {
            TypePayment = typePayment;
        }

        public string getTypePayment() { return TypePayment; }
        public void setTypePayment(string typePayment) { TypePayment = typePayment; }

        public bool ProcessPayment(double amount)
        {
            // UML logic: process payment
            // In a real app this would connect to Stripe/PayPal
            // Here we just return true to simulate PaymentSuccessful()
            return true;
        }
    }
}
