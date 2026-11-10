using XFrame.Common;
using XFrame.Notifications;
using XFrame.Rules;

namespace Capi.Assessment.Invoice.Domain.Extensions
{
    public static class IValidatorExtensions
    {
        public static async Task<Notification> ValidateAsync<TCommand, TValidationContext>(
            this IValidator validator, 
            TCommand command, 
            CancellationToken cancellationToken = default)
            where TValidationContext : new()
        {
            return await validator.Validate(
                command, 
                new TValidationContext(), 
                SystemCulture.Default(), 
                typeof(IValidatorExtensions).Assembly, cancellationToken);
        }
    }
}
