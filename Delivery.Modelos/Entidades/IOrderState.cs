namespace Delivery.Modelos.Entidades
{
    // UML: ComponentOrder -> IOrderState
    public interface IOrderState
    {
        void UpdateStatus(string newStatus);
    }
}
