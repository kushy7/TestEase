using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using TestEase.Models;

public class RegisterModelConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(RegisterModel).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        throw new NotImplementedException("Deserialization is not required for this example.");
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        JObject jo = new JObject();
        Type type = value.GetType();

        // Serialize common properties
        jo.Add("Type", JToken.FromObject(((RegisterModel)value).Type, serializer));
        jo.Add("Address", JToken.FromObject(((RegisterModel)value).Address, serializer)); // Ensure Address is converted to JToken
        jo.Add("Name", JToken.FromObject(((RegisterModel)value).Name, serializer)); // Ensure Name is converted to JToken

        // Serialize properties based on specific derived type
        if (type == typeof(CoilOrDiscrete))
        {
            jo.Add("Value", JToken.FromObject(((CoilOrDiscrete)value).value, serializer)); // Ensure Value is converted to JToken
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Fixed<>))
        {
            jo.Add("Value", JToken.FromObject(((dynamic)value).Value, serializer)); // Correctly converting to JToken
        }
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Range<>) || type.GetGenericTypeDefinition() == typeof(Random<>) || type.GetGenericTypeDefinition() == typeof(Curve<>))
        {
            jo.Add("StartValue", JToken.FromObject(((dynamic)value).StartValue, serializer)); // Correctly converting to JToken
            jo.Add("EndValue", JToken.FromObject(((dynamic)value).EndValue, serializer)); // Correctly converting to JToken
            if (type.GetGenericTypeDefinition() == typeof(Curve<>))
            {
                jo.Add("Period", JToken.FromObject(((Curve<dynamic>)value).Period, serializer)); // Ensure Period is converted to JToken
            }
        }

        // Write type name to handle polymorphism if required
        jo.Add("RegisterType", type.Name);

        jo.WriteTo(writer);
    }

}
