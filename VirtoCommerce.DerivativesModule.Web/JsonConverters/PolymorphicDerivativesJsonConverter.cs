using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VirtoCommerce.DerivativesModule.Core.Model;
using VirtoCommerce.Platform.Core.Common;

namespace VirtoCommerce.DerivativesModule.Web.JsonConverters
{
    public class PolymorphicDerivativesJsonConverter : JsonConverter
    {
        private readonly Type[] _knowTypes = new[] { typeof(Derivative), typeof(DerivativeItem), typeof(DerivativeSearchCriteria) };

        public override bool CanWrite => false;
        public override bool CanRead => true;

        public override bool CanConvert(Type objectType)
        {
            return _knowTypes.Any(x => x.IsAssignableFrom(objectType));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var tryCreateInstance = typeof(AbstractTypeFactory<>).MakeGenericType(objectType).GetMethods().FirstOrDefault(x => x.Name.EqualsInvariant("TryCreateInstance") && x.GetParameters().Length == 0);
            var result = tryCreateInstance?.Invoke(null, null);

            serializer.Populate(JObject.Load(reader).CreateReader(), result);
            return result;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}