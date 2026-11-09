using XFrame.Aggregates.ExecutionResults;
using XFrame.Notifications;

namespace Capi.Assessment.Invoice.Api.Extensions
{
    public static class IExecutionResultsExtensions
    {
        public static Notification GetErrors(this IExecutionResult result)
        {
            if (result is FailedExecutionResult failedResult)
            {
                return failedResult.Errors;
            }

            return Notification.CreateEmpty();
        }

        public static T GetResult<T>(this IExecutionResult result)
        {
            if (result is SuccessExecutionResult<T> successResult)
            {
                return successResult.Result;
            }

            return default;
        }
    }
}
