using MassTransit;

namespace OrderCreator.Models
{
    public class OrderState : SagaStateMachineInstance
    {
        //Saga State
        public int Id { get; set; }
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }

        //Order Details
        public string OrderName { get; set; }
        public string OrderDescription { get; set; }

        //Order People
        public string Customer { get; set; }
        public string Sender { get; set; }
    }
}
