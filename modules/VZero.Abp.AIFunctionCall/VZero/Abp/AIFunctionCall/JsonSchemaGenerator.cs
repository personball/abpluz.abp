using System.Reflection;
using System.Text.Json;
using Json.Schema;
using Json.Schema.Generation;

namespace VZero.Abp.AIFunctionCall;

public static class JsonSchemaGenerator
{
    public static JsonSchema GenerateSchema(this MethodInfo method)
    {
        var parameters = method.GetParameters();

        var schemaBuilder = new JsonSchemaBuilder()
            .Type(SchemaValueType.Object);

        var properties = new Dictionary<string, JsonSchema>();
        var required = new List<string>();

        foreach (var param in parameters)
        {
            var paramSchema = GenerateParameterSchema(param);
            properties[param.Name!] = paramSchema;

            // 如果没有默认值，则是必需参数
            if (!param.HasDefaultValue)
            {
                required.Add(param.Name!);
            }
        }

        schemaBuilder = schemaBuilder.Properties(properties);

        if (required.Count > 0)
        {
            schemaBuilder = schemaBuilder.Required(required);
        }

        return schemaBuilder.Build();
    }

    private static JsonSchema GenerateParameterSchema(ParameterInfo parameter)
    {
        var schemaBuilder = new JsonSchemaBuilder();
        var sgc = new SchemaGeneratorConfiguration
        {
            PropertyNameResolver = PropertyNameResolvers.CamelCase
        };

        // 设置类型
        var paramType = parameter.ParameterType;
        schemaBuilder = schemaBuilder.FromType(paramType, sgc);

        // 设置描述
        // 不支持嵌套子类型的 Description，需要等 issue 948，(自定义类型中的属性可以用 Json.Schema.Generation.DescriptionAttribute)
        // TODO: https://github.com/json-everything/json-everything/issues/948
        var descriptionAttr = parameter.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
        if (descriptionAttr != null)
        {
            schemaBuilder = schemaBuilder.Description(descriptionAttr.Description);
        }

        // 设置默认值
        if (parameter.HasDefaultValue && parameter.DefaultValue != null)
        {
            schemaBuilder = schemaBuilder.Default(JsonSerializer.SerializeToNode(parameter.DefaultValue));
        }

        return schemaBuilder.Build();
    }
}