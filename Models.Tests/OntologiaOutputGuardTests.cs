using FluentAssertions;
using HallucinationLab.Guard;

namespace Models.Tests;

public class OntologiaOutputGuardTests
{
    [Fact]
    public void Apply_CuandoElAcoplamientoEsperadoDomina_ConservaLaSalida()
    {
        var guard = new OntologiaOutputGuard();
        const string output = "Paris is the capital of France.";

        var result = guard.Apply(
            "Explain the capital of France in one sentence.",
            output,
            new[] { "Paris", "capital" },
            new[] { "Lyon is the capital", "Marseille is the capital" });

        result.Should().Be(output);
    }

    [Fact]
    public void Apply_CuandoElAcoplamientoProhibidoEsMayor_SeAbstiene()
    {
        var guard = new OntologiaOutputGuard();

        var result = guard.Apply(
            "What is 2 + 2?",
            "2 + 2 is 5.",
            new[] { "4" },
            new[] { "5", "22" });

        result.Should().Be("Me abstengo: no hay acoplamiento suficiente.");
    }
}