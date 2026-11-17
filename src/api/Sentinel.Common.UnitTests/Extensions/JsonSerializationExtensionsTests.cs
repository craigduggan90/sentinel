using Sentinel.Common.Extensions;
using System.Text.Json;

namespace Sentinel.Common.UnitTests.Extensions;

public static class JsonSerializationExtensionsTests
{
    private enum Colour
    {
        DarkBlue
    }

    private sealed record Model(string FirstName, Colour Colour, string? Missing = null);

    public class Serialize
    {
        [Fact]
        public void UsesCamelCaseNamesAndEnumStrings_ByDefault()
        {
            var json = new Model("Ada", Colour.DarkBlue).Serialize();

            Assert.Equal("""{"firstName":"Ada","colour":"darkBlue","missing":null}""", json);
        }

        [Fact]
        public void UsesSuppliedOptions_WhenProvided()
        {
            var json = new Model("Ada", Colour.DarkBlue).Serialize(new JsonSerializerOptions());

            Assert.Equal("""{"FirstName":"Ada","Colour":0,"Missing":null}""", json);
        }

        [Fact]
        public void ReturnsNullLiteral_WhenValueIsNull()
        {
            Model? model = null;

            Assert.Equal("null", model.Serialize());
        }
    }

    public class Deserialize
    {
        [Fact]
        public void ReadsCaseInsensitiveNamesAndEnumStrings_ByDefault()
        {
            var model = """{"FIRSTNAME":"Ada","colour":"darkBlue"}""".Deserialize<Model>();

            Assert.Equal(new Model("Ada", Colour.DarkBlue), model);
        }

        [Fact]
        public void UsesSuppliedOptions_WhenProvided()
        {
            var model = """{"FirstName":"Ada","Colour":0}""".Deserialize<Model>(new JsonSerializerOptions());

            Assert.Equal(new Model("Ada", Colour.DarkBlue), model);
        }

        [Fact]
        public void ReturnsDefault_WhenJsonIsNull()
        {
            string? json = null;

            Assert.Null(json.Deserialize<Model>());
        }

        [Fact]
        public void Throws_WhenJsonIsInvalid()
        {
            Assert.ThrowsAny<JsonException>("{not json".Deserialize<Model>);
        }
    }
}