using XFrame.Notifications;

namespace Capi.Assessment.Invoice.Api.Models.ResponseModels.Abstract
{
    public abstract class ResponseModel
    {
        protected ResponseModel()
        {
            Notification = Notification.CreateEmpty();
        }

        public Notification Notification { get; set; }
    }

    public abstract class ResponseModel<TResult> : ResponseModel
    {
        public TResult Result { get; set; }
    }
}
