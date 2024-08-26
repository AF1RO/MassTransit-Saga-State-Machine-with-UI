namespace OrderCreator.Sagas
{
    public class OrderShipped
    {
        public Guid OrderId { get; set; }
        public string Customer { get; set; }
        public string Sender { get; set; }
        public string OrderName { get; set; }
        public string OrderDescription { get; set; }
    }
}
