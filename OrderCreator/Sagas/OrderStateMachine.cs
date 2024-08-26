using MassTransit;
using OrderCreator.Models;

namespace OrderCreator.Sagas
{
    public class OrderStateMachine : MassTransitStateMachine<OrderState>
    {
        public State Submitted { get; private set; }
        public State Shipped { get; private set; }
        public State Completed { get; private set; }

        public Event<OrderSubmitted> OrderSubmitted { get; private set; }
        public Event<OrderShipped> OrderShipped { get; private set; }
        public Event<OrderCompleted> OrderCompleted { get; private set; }

        public OrderStateMachine(ILogger<OrderStateMachine> logger)
        {
            InstanceState(x => x.CurrentState);

            Event(() => OrderSubmitted, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderShipped, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderCompleted, x => x.CorrelateById(context => context.Message.OrderId));

            Initially(
                When(OrderSubmitted)
                .Then(context =>
                {
                    logger.LogInformation($"\nOrder has been submitted! | Details ->" +
                        $"\nOrder ID: {context.Data.OrderId}" +
                        $"\nOrder Name: {context.Data.OrderName}" +
                        $"\nOrder Description: {context.Data.OrderDescription}" +
                        $"\nOrder Customer: {context.Data.Customer}" +
                        $"\nOrder Sender: {context.Data.Sender}");
                    context.Instance.OrderName = context.Data.OrderName;
                    context.Instance.OrderDescription = context.Data.OrderDescription;
                    context.Instance.Customer = context.Data.Customer;
                    context.Instance.Sender = context.Data.Sender;
                })
                .TransitionTo(Submitted)
                .Publish(context => new OrderShipped
                {
                    OrderId = context.Data.OrderId,
                    Customer = context.Data.Customer,
                    Sender = context.Data.Sender,
                    OrderName = context.Data.OrderName,
                    OrderDescription = context.Data.OrderDescription
                })
                );

            During(Submitted,
                When(OrderShipped)
                .Then(context =>
                {
                    logger.LogInformation($"\nOrder has been shipped! | Details ->" +
                        $"\nOrder ID: {context.Data.OrderId}" +
                        $"\nOrder Name: {context.Data.OrderName}" +
                        $"\nOrder Description: {context.Data.OrderDescription}" +
                        $"\nOrder Customer: {context.Data.Customer}" +
                        $"\nOrder Sender: {context.Data.Sender}");
                    context.Instance.OrderName = context.Data.OrderName;
                    context.Instance.OrderDescription = context.Data.OrderDescription;
                    context.Instance.Customer = context.Data.Customer;
                    context.Instance.Sender = context.Data.Sender;
                })
                .TransitionTo(Shipped)
                .Publish(context => new OrderCompleted
                {
                    OrderId = context.Data.OrderId,
                    Customer = context.Data.Customer,
                    Sender = context.Data.Sender,
                    OrderName = context.Data.OrderName,
                    OrderDescription = context.Data.OrderDescription
                })
                );

            During(Shipped,
                When(OrderCompleted)
                .Then(context =>
                {
                    logger.LogInformation($"\nOrder has been delivered! | Details ->" +
                        $"\nOrder ID: {context.Data.OrderId}" +
                        $"\nOrder Name: {context.Data.OrderName}" +
                        $"\nOrder Description: {context.Data.OrderDescription}" +
                        $"\nOrder Customer: {context.Data.Customer}" +
                        $"\nOrder Sender: {context.Data.Sender}");
                    context.Instance.OrderName = context.Data.OrderName;
                    context.Instance.OrderDescription = context.Data.OrderDescription;
                    context.Instance.Customer = context.Data.Customer;
                    context.Instance.Sender = context.Data.Sender;
                })
                .Finalize());
        }
    }
}
