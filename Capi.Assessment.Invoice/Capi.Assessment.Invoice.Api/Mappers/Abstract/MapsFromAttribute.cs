namespace Capi.Assessment.Invoice.Api.Mappers.Abstract
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MapsFromAttribute(Type sourceType) : Attribute
    {
        public Type SourceType { get; } = sourceType;
    }
}
