using System.Reflection;
using XFrame.Common.Extensions;

namespace Capi.Assessment.Invoice.Api.Mappers.Abstract
{
    public class MappingRegistry : IMappingRegistry
    {
        private readonly Dictionary<Type, MappingDefinition> _map = [];

        public MappingRegistry(Type typeToScan, params Assembly[] assemblies)
        {
            Scan(assemblies, typeToScan);
        }


        public MappingDefinition Get(Type responseType)
        {
            if (!_map.TryGetValue(responseType, out var def))
                throw new Exception($"No mapping found for {responseType.Name}");

            return def;
        }

        #region Private Methods

        private void Scan(Assembly[] assemblies, Type typeToScan)
        {
            var responseTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.BaseType.IsNotNull() 
                    && t.BaseType.IsGenericType 
                    && t.BaseType.GetGenericTypeDefinition() == typeToScan);

            foreach (var responseType in responseTypes)
            {
                var baseType = responseType.BaseType;

                var listType = baseType.GetGenericArguments().FirstOrDefault();
                var itemType = listType.GetGenericArguments().FirstOrDefault();

                MapsFromAttribute attr = null;

                if (itemType.IsNull())
                {
                    attr = listType
                    .GetCustomAttributes(typeof(MapsFromAttribute), false)
                    .FirstOrDefault() as MapsFromAttribute;
                }
                else
                {
                    attr = itemType
                    .GetCustomAttributes(typeof(MapsFromAttribute), false)
                    .FirstOrDefault() as MapsFromAttribute;
                }

                if (attr == null)
                    continue;

                _map[responseType] = new MappingDefinition
                {
                    ResponseType = responseType,
                    DestinationType = listType,
                    DbType = attr.SourceType
                };
            }
        }

        #endregion
    }
}
