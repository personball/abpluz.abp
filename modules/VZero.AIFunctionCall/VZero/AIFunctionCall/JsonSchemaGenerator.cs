using System.Reflection;
using System.Text.Json;
using Json.Schema;
using Json.Schema.Generation;

namespace VZero.AIFunctionCall;

internal static class JsonSchemaGenerator
{
    internal static JsonSchema GenerateSchema(this MethodInfo method)
    {
        var parameters = method.GetParameters();

        var schemaBuilder = new JsonSchemaBuilder()
            .Type(SchemaValueType.Object);

        var properties = new Dictionary<string, JsonSchema>();
        var required = new List<string>();
        var allDefs = new Dictionary<string, JsonSchema>();

        foreach (var param in parameters)
        {
            var paramSchema = GenerateParameterSchema(param);
            properties[param.Name!] = paramSchema;

            // 如果没有默认值，则是必需参数
            if (!param.HasDefaultValue)
            {
                required.Add(param.Name!);
            }

            // inner defs to root
            // 手动收集每个参数schema中的definitions
            var defs = paramSchema.GetDefs();
            if (defs != null)
            {
                foreach (var def in defs)
                {
                    if (!allDefs.ContainsKey(def.Key))
                    {
                        allDefs.Add(def.Key, def.Value);
                    }
                }
            }
        }

        schemaBuilder = schemaBuilder.Properties(properties);

        if (required.Count > 0)
        {
            schemaBuilder = schemaBuilder.Required(required);
        }

        if (allDefs.Count > 0)
        {
            schemaBuilder = schemaBuilder.Defs(allDefs);
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