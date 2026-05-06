using FluentAssertions;

namespace Models.Tests;

public class DesignacionTests
{
    private static Dictionary<double, List<Nombre>> MapearNombres(string texto)
    {
        var nombres = new Dictionary<double, List<Nombre>>();
        var predicados = texto
            .Split('.')
            .Select(p => p.Trim())
            .Where(p => !string.IsNullOrEmpty(p))
            .ToList();
        var diccionarioVerbos = predicados
            .Select(p => p.Split(' ').First())
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => g.Count());
        var palabras = predicados.SelectMany(p => p.Split(' ')).ToList();
        var diccionarioComplementos = palabras
            .Where(p => !diccionarioVerbos.ContainsKey(p))
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => Math.Max(1, g.Count()));

        var deltaFasePredicados = 2 * Math.PI / predicados.Count;
        for (var i = 0; i < predicados.Count; i++)
        {
            var palabrasPredicado = predicados[i].Split(' ');
            var verboNucleo = palabrasPredicado.First();
            var frecuencia = diccionarioVerbos[verboNucleo];

            if (!nombres.ContainsKey(frecuencia))
            {
                nombres[frecuencia] = new List<Nombre>();
            }

            var amplitud = palabrasPredicado
                .Where(p => p != verboNucleo)
                .Sum(p => diccionarioComplementos[p]);
            var fase = i * deltaFasePredicados;

            nombres[frecuencia].Add(Nombre.Imaginar(predicados[i], fase, frecuencia, amplitud));
        }

        return nombres;
    }

    [Fact]
    public void Designar_SobreApariencia_AgregaElNombreALaDesignacionBase()
    {
        var frecuencia = 5.0;
        var fase = Math.PI / 3;
        var amplitud = 2.0;
        var nombre = Nombre.Imaginar("Vacuidad", fase, frecuencia, amplitud);
        var texto = "gozo. mente";
        var designacionApariencia = nombre.Mostrarse(texto, MapearNombres);

        var designacion = Designacion.Designar(nombre, Apariencia.Aparecer(designacionApariencia.Nombres.ToList()));

        designacion.Should().NotBeNull();
        designacion.Nombres.Should().HaveCount(designacionApariencia.Nombres.Count() + 1);
        designacion.Nombres.Last().Should().BeEquivalentTo(nombre);
    }

    [Fact]
    public void Equals_ConMismaReferencia_DevuelveTrueYConOtraDesignacion_DevuelveFalse()
    {
        var nombre = Nombre.Imaginar("Vacuidad", Math.PI / 3, 5.0, 1.0);
        var designacion = nombre.Mostrarse("ser humano. pensar lenguaje", MapearNombres);
        var mismaReferencia = designacion;
        var otra = nombre.Mostrarse("ser mente. pensar vacuidad", MapearNombres);

        designacion.Equals(mismaReferencia).Should().BeTrue();
        designacion.Equals(otra).Should().BeFalse();
        designacion.Equals("no-designacion").Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SinParametros_GeneraPorId()
    {
        var nombre = Nombre.Imaginar("Vacuidad", Math.PI / 3, 5.0, 1.0);
        var designacion = nombre.Mostrarse("ser humano. pensar lenguaje", MapearNombres);

        designacion.GetHashCode().Should().Be(designacion.Id.GetHashCode());
    }

    [Fact]
    public void Efecto_ExponeNombreAgregadoYLaAparienciaDeLaDesignacion()
    {
        var nombre = Nombre.Imaginar("Vacuidad", Math.PI / 3, 5.0, 2.0);
        var designacion = nombre.Mostrarse("ser humano. pensar lenguaje", MapearNombres);

        var efecto = designacion.Esencia;

        efecto.Nombre.Texto.Should().Be("Vacuidad");
        efecto.Nombre.Frecuencia.Should().Be(0d);
        efecto.Nombre.Causa.Should().Be(designacion);
        efecto.Nombre.Amplitud.Should().BeApproximately(designacion.Nombres.Sum(n => n.Amplitud), 1e-10);
        efecto.Apariencia.Nombres.Should().BeEquivalentTo(designacion.Nombres);
    }

    [Fact]
    public void VelocidadGrupo_UsaLaCantidadDeNombresConLaMismaFrecuencia()
    {
        var nombre = Nombre.Imaginar("Vacuidad", Math.PI / 3, 5.0, 1.0);
        var designacion = nombre.Mostrarse("ser humano. ser lenguaje. pensar mente", MapearNombres);

        var frecuencias = designacion.Nombres.Select(n => designacion.VelocidadGrupo(n)).ToList();

        frecuencias.Should().Equal(2d, 2d, 1d, 1d);
        designacion.VelocidadGrupo(Nombre.Imaginar("Vacuidad", 0d, 99d, 1d)).Should().Be(0d);
    }
}
