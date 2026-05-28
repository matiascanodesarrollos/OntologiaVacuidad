using FluentAssertions;
using HallucinationLab.Guard;

namespace Models.Tests;

public class OntologiaOutputGuardTests
{
    [Theory]
    [InlineData(
        "Explicar la capital de Francia en una oración.", 
        "La capital de Francia es París.",
        new[] { "París", "capital" },
        new[] { "Lyon es la capital", "Marsella es la capital" })]
    public void Apply_CuandoNoAlucina_ConservaLaSalida(
        string prompt, 
        string expected,
        string[] expectedFacts,
        string[] forbiddenClaims)
    {
        var guard = new OntologiaOutputGuard();
        var result = guard.Apply(
            prompt,
            expected,
            expectedFacts,
            forbiddenClaims);

        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(
        "", 
        new[] { "París" },
        new[] { "Lyon es la capital" })]
    public void Apply_CuandoAlucina_Abstiene(
        string prompt, 
        string[] expectedFacts,
        string[] forbiddenClaims)
    {
        var guard = new OntologiaOutputGuard();
        var expected = "Me abstengo: no hay acoplamiento suficiente.";
        var result = guard.Apply(
            prompt,
            expected,
            expectedFacts,
            forbiddenClaims);

        result.Should().Be(expected);
    }
}