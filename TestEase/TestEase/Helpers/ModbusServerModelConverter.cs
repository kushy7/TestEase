using Microsoft.UI.Xaml.Markup;
using System.Text.Json;
using System.Text.Json.Serialization;
using TestEase.Models;

public class ModbusServerModelConverter : JsonConverter<ModbusServerModel>
{
    public override ModbusServerModel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Example of starting to read an object
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected StartObject token but got a different token.");
        }

        var model = new ModbusServerModel(502);  // Default constructor logic
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return model;
            }

            // Check property names and deserialize accordingly
            if (reader.TokenType == JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();  // Move to the value token
                switch (propertyName)
                {
                    case "Port":
                        model.Port = reader.GetInt32();
                        break;
                    case "IsRunning":
                        var r = reader.GetBoolean();
                        model.IsRunning = r;
                        if (r) model.StartServer(); // Make sure the server is actually running
                        break;
                    case "IsNotSaved":
                        model.IsNotSaved = reader.GetBoolean();
                        break;
                    case "WorkingConfiguration":
                        if (reader.TokenType != JsonTokenType.Null)  // Check for null token
                        {
                            model.WorkingConfiguration = JsonSerializer.Deserialize<ConfigurationModel>(ref reader, options);
                        }
                        else
                        {
                            model.WorkingConfiguration = new ConfigurationModel(); // Or set to null, based on your handling preference
                        }
                        break;
                    // Handle other properties
                    default:
                        throw new JsonException($"Property '{propertyName}' is not expected.");
                }
            }
        }

        throw new JsonException("JSON data is incomplete and does not contain an object end token.");
    }


    public override void Write(Utf8JsonWriter writer, ModbusServerModel value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber("Port", value.Port);
        writer.WriteBoolean("IsRunning", value.IsRunning);
        writer.WriteBoolean("IsNotSaved", value.IsNotSaved);
        if (value.WorkingConfiguration != null)
        {
            writer.WritePropertyName("WorkingConfiguration");
            JsonSerializer.Serialize(writer, value.WorkingConfiguration, options);
        }

        writer.WriteEndObject();
    }
}

