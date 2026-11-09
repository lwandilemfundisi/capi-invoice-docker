namespace Capi.Assessment.Invoice.Api.Mappers.Abstract
{
    public interface IMappingRegistry
    {
        MappingDefinition Get(Type responseType);
    }
}
