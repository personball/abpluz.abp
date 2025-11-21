using System.Text.Json.Serialization;

namespace Pluz.Sample.Demo.Dto;


public class ComplexInputDto
{
    [Json.Schema.Generation.Required]
    [Json.Schema.Generation.Description("It's a nullable required property")]
    public string? ANullableButRquiredProperty { get; set; }

    [Json.Schema.Generation.MaxLength(10)]
    [Json.Schema.Generation.Description("Its max length is 10.")]
    public string MaxLength10 { get; set; } = string.Empty;

    [Json.Schema.Generation.MinLength(5)]
    [Json.Schema.Generation.Description("Its min length is 5.")]
    public string MinLength5 { get; set; } = string.Empty;


    [Json.Schema.Generation.Description("Something enum")]
    public CategoryEnum Type { get; set; }

    [Json.Schema.Generation.MinItems(2)]
    [Json.Schema.Generation.MaxItems(5)]
    [Json.Schema.Generation.Description("bla bla bla")]
    public CategoryEnum[] CategoryEnums { get; set; } = [];

}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CategoryEnum
{
    Room,
    Bike,
    Plane
}