using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Revamp_Ank_App.DomainEntites.Repositores.Entites
{
    public class IntToLongConverter : JsonConverter

    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(long) || objectType == typeof(long?));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Integer)
            {
                return token.ToObject<long>();
            }
            else if (token.Type == JTokenType.String)
            {
                if (long.TryParse(token.ToString(), out long result))
                {
                    return result;
                }
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
