using XFrame.Aggregates;
using XFrame.Notifications;
using XFrame.Specifications;

namespace Capi.Assessment.Invoice.Domain.InvoiceDomainModel.Specifications
{
    public static class AggregateSpecifications
    {
        public static ISpecification<IAggregateRoot> AggregateIsNew { get; } = new AggregateIsNewSpecification();
        public static ISpecification<IAggregateRoot> AggregateIsCreated { get; } = new AggregateIsCreatedSpecification();

        public class AggregateIsNewSpecification : Specification<IAggregateRoot>
        {
            protected override Notification IsNotSatisfiedBecause(IAggregateRoot obj)
            {
                if (!obj.IsNew)
                {
                    return Notification.Create(new Message($"'{obj.Name}' with ID '{obj.GetIdentity()}' is not new", SeverityType.Critical));
                }

                return Notification.CreateEmpty();
            }
        }

        private class AggregateIsCreatedSpecification : Specification<IAggregateRoot>
        {
            protected override Notification IsNotSatisfiedBecause(IAggregateRoot obj)
            {
                if (obj.IsNew)
                {
                    return Notification.Create(new Message
                    {
                        Text = $"Aggregate '{obj.Name}' with ID '{obj.GetIdentity()}' is new",
                        Severity = SeverityType.Critical
                    });
                }

                return Notification.CreateEmpty();
            }
        }
    }
}
