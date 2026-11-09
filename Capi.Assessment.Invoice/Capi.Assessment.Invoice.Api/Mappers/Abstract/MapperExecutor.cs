using Azure;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Capi.Assessment.Invoice.Api.Mappers.Abstract
{
    public class MapperExecutor
    {
        private static readonly ConcurrentDictionary<string, MapperDelegatesCache> _cachedMappers = new();

        public static object MapList(object source, Type sourceType, Type destination)
        {
            var cacheKey = $"{sourceType.FullName}->{destination.FullName}";
            var cache = _cachedMappers.GetOrAdd(cacheKey, _ => BuildMapperDelegates(sourceType, destination));
            return cache.Execute(source);
        }

        private sealed class MapperDelegatesCache
        {
            public Func<object, object> Execute { get; }

            public MapperDelegatesCache(Func<object, object> execute)
            {
                Execute = execute;
            }
        }

        private static MapperDelegatesCache BuildMapperDelegates(
            Type sourceType,
            Type destination)
        {
            var mapperType = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.IsClass
                    && t.BaseType != null
                    && t.BaseType.IsGenericType
                    && t.BaseType.GetGenericTypeDefinition() == typeof(AbstractMap<>)
                    && t.BaseType.GetGenericArguments()[0] == destination);

            if (mapperType == null)
                throw new InvalidOperationException("No mapper found");

            var listSourceType = typeof(List<>).MakeGenericType(sourceType);

            var constructor =
                mapperType.GetConstructor([listSourceType])
                ?? mapperType.GetConstructor([sourceType]);

            if (constructor == null)
                throw new InvalidOperationException(
                    "No suitable constructor found in mapper");

            var mapMethod = mapperType.GetMethod("Map");
            if (mapMethod == null)
                throw new InvalidOperationException("No Map method found in mapper");

            var sourceParameter = Expression.Parameter(typeof(object), "source");
            var constructorParameterType = constructor.GetParameters()[0].ParameterType;
            var convertedSource = Expression.Convert(sourceParameter, constructorParameterType);
            var newMapper = Expression.New(constructor, convertedSource);
            var mapCall = Expression.Call(newMapper, mapMethod);
            var castResult = Expression.Convert(mapCall, typeof(object));

            var lambda = Expression.Lambda<Func<object, object>>(
                castResult,
                sourceParameter);

            return new MapperDelegatesCache(lambda.Compile());
        }
    }
}
