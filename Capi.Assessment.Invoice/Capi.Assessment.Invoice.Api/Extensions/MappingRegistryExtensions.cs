using Capi.Assessment.Invoice.Api.Mappers.Abstract;

namespace Capi.Assessment.Invoice.Api.Extensions
{
    public static class MappingRegistryExtensions
    {
        public static T Map<T>(
            this IMappingRegistry mappingRegistry,
            object source)
        {
            ArgumentNullException.ThrowIfNull(mappingRegistry);

            if (source is null)
            {
                return default;
            }

            var definition = mappingRegistry.Get(typeof(T));
            var mapped = MapperExecutor.MapList(
                source,
                definition.DbType,
                definition.DestinationType);

            return SetResult<T>(mapped);
        }

        private static TResponse SetResult<TResponse>(object mappedResult)
        {
            var response = Activator.CreateInstance<TResponse>();
            typeof(TResponse).GetProperty("Result")!.SetValue(response, mappedResult);
            return response;
        }

    }
}
