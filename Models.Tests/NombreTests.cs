using FluentAssertions;

public class NombreTests
{
    private static Dictionary<double, List<Nombre>> MapearNombres(string texto, Func<string, string>? obtenerVerboNucleo = null)
    {
        obtenerVerboNucleo ??= predicado => predicado.Split(' ').First();
        var nombres = new Dictionary<double, List<Nombre>>();
        var predicados = texto
            .Split('.')
            .Select(p => p.Trim())
            .Where(p => !string.IsNullOrEmpty(p))
            .ToList();
        var diccionarioVerbos = predicados
            .Select(obtenerVerboNucleo)
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
            var verboNucleo = obtenerVerboNucleo(predicados[i]);
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
    public void Constructor_ConDatos_Crea()
    {
        var fase = Math.PI / 3;
        var frecuencia = 5.0;
        var amplitud = 2.5;
        var nombre = Nombre.Imaginar("Vacuidad", fase, frecuencia, amplitud);

        nombre.Should().NotBeNull();
        nombre.Fase.Should().Be(fase);
        nombre.Frecuencia.Should().Be(frecuencia);
        nombre.Amplitud.Should().Be(amplitud);
        nombre.Causa.Should().Be(Designacion.Vacuidad);
    }

    [Fact]
    public void Constructor_Copia_ReplicaLasPropiedadesDelNombreOriginal()
    {
        var original = Nombre.Imaginar("Vacuidad", Math.PI / 3, 5.0, 2.5);

        var copia = new Nombre(original);

        copia.Should().NotBeSameAs(original);
        copia.Texto.Should().Be(original.Texto);
        copia.Fase.Should().Be(original.Fase);
        copia.Frecuencia.Should().Be(original.Frecuencia);
        copia.Amplitud.Should().Be(original.Amplitud);
        copia.Causa.Should().BeSameAs(original.Causa);
    }

    [Fact]
    public void Mostrarse_CreaDesignacionAgregandoElNombreOriginalAlFinal()
    {
        var frecuencia = 5.0;
        var fase = Math.PI / 3;
        var amplitud = 2.0;
        var nombre = Nombre.Imaginar("Vacuidad", fase, frecuencia, amplitud);
        var predicados = new List<string> { "ser humano", "ser lenguaje", "pensar humano" };
        var texto = string.Join(". ", predicados);

        var designacion = nombre.Mostrarse(texto, t => MapearNombres(t));

        designacion.Should().NotBeNull();
        designacion.Nombres.Should().HaveCount(predicados.Count + 1);
        designacion.Nombres.Select(n => n.Texto).Should().Equal(predicados.Append(nameof(Designacion.Vacuidad)));
        var ultimoNombre = designacion.Nombres.Last();
        ultimoNombre.Texto.Should().Be(nombre.Texto);
        ultimoNombre.Fase.Should().BeApproximately(nombre.Fase, 1e-10);
        ultimoNombre.Frecuencia.Should().Be(frecuencia);
        ultimoNombre.Amplitud.Should().Be(amplitud);
    }

    [Fact]
    public void Mostrarse_ConFuncionObtenerVerboNucleoNull_UsaLaPrimerPalabraDelPredicado()
    {
        var nombre = Nombre.Imaginar("Vacuidad", Math.PI / 3, 5.0, 2.0);
        var predicados = new List<string> { "ser humano", "ser lenguaje", "pensar humano" };
        var texto = string.Join(". ", predicados);

        var designacion = nombre.Mostrarse(texto, t => MapearNombres(t));

        var nombresPredicados = designacion.Nombres.Take(predicados.Count).ToList();
        nombresPredicados.Select(n => n.Frecuencia).Should().Equal(2d, 2d, 1d);
        nombresPredicados.Select(n => n.Fase).Should().Equal(0d, 2d * Math.PI / 3d, 4d * Math.PI / 3d);
        nombresPredicados.Select(n => n.Amplitud).Should().Equal(2d, 1d, 2d);
    }

    [Fact]
    public void Mostrarse_ConFuncionObtenerVerboNucleo_CreaDesignacionAgrupandoPorElVerboElegido()
    {
        var frecuencia = 5.0;
        var fase = Math.PI / 3;
        var amplitud = 2.0;
        var nombre = Nombre.Imaginar("Vacuidad", fase, frecuencia, amplitud);
        var verbosNucleo = new HashSet<string> { "ser" };
        var obtenerVerboNucleo = new Func<string, string>(predicado =>
            predicado
                .Split(' ')
                .FirstOrDefault(p => verbosNucleo.Contains(p))
            ?? predicado.Split(' ').First());
        var predicados = new List<string>
        {
            "quiero ser humano",
            "puedo ser lenguaje",
            "ser pensamiento"
        };
        var texto = string.Join(". ", predicados);

        var designacion = nombre.Mostrarse(texto, t => MapearNombres(t, obtenerVerboNucleo));

        designacion.Should().NotBeNull();
        designacion.Nombres.Should().HaveCount(predicados.Count + 1);
        designacion.Nombres.Select(n => n.Texto).Take(predicados.Count).Should().Equal(predicados);

        var nombresPredicados = designacion.Nombres.Take(predicados.Count).ToList();
        nombresPredicados.Select(n => n.Frecuencia).Should().Equal(3d, 3d, 3d);
        nombresPredicados.Select(n => n.Fase).Should().Equal(0d, 2d * Math.PI / 3d, 4d * Math.PI / 3d);
        nombresPredicados.Select(n => n.Amplitud).Should().Equal(2d, 2d, 1d);

        var ultimoNombre = designacion.Nombres.Last();
        ultimoNombre.Texto.Should().Be(nombre.Texto);
        ultimoNombre.Fase.Should().BeApproximately(fase, 1e-10);
        ultimoNombre.Frecuencia.Should().Be(frecuencia);
        ultimoNombre.Amplitud.Should().Be(amplitud);
    }

    [Fact]
    public void Mostrarse_ConVerbosNucleoMixtos_AgrupaFrecuenciasPorCadaVerbo()
    {
        var nombre = Nombre.Imaginar("Vacuidad", Math.PI / 3, 5.0, 2.0);
        var verbosNucleo = new HashSet<string> { "ser", "pensar" };
        var obtenerVerboNucleo = new Func<string, string>(predicado =>
            predicado
                .Split(' ')
                .FirstOrDefault(p => verbosNucleo.Contains(p))
            ?? predicado.Split(' ').First());
        var predicados = new List<string>
        {
            "quiero ser humano",
            "decido pensar lenguaje",
            "puedo ser mente",
            "pensar vacuidad"
        };
        var texto = string.Join(". ", predicados);

        var designacion = nombre.Mostrarse(texto, t => MapearNombres(t, obtenerVerboNucleo));

        var nombresPredicados = designacion.Nombres.Take(predicados.Count).ToList();
        nombresPredicados.Select(n => n.Frecuencia).Should().Equal(2d, 2d, 2d, 2d);
        nombresPredicados.Select(n => n.Fase).Should().Equal(0d, Math.PI / 2d, Math.PI, 3d * Math.PI / 2d);
        nombresPredicados.Select(n => n.Amplitud).Should().Equal(2d, 2d, 2d, 1d);
    }

    [Fact]
    public void Mostrarse_ConComplementosParcialmenteCompartidos_CalculaAmplitudesDistintas()
    {
        var nombre = Nombre.Imaginar("Vacuidad", Math.PI / 3, 5.0, 2.0);
        var predicados = new List<string>
        {
            "ser humano lenguaje",
            "ser humano mente",
            "ser mente"
        };
        var texto = string.Join(". ", predicados);

        var designacion = nombre.Mostrarse(texto, t => MapearNombres(t));

        var nombresPredicados = designacion.Nombres.Take(predicados.Count).ToList();
        nombresPredicados.Select(n => n.Frecuencia).Should().Equal(3d, 3d, 3d);
        nombresPredicados.Select(n => n.Amplitud).Should().Equal(3d, 4d, 2d);
    }

    [Fact]
    public void Mostrarse_ConCuatroPredicados_DistribuyeFasesUniformemente()
    {
        var nombre = Nombre.Imaginar("Vacuidad", Math.PI / 3, 5.0, 2.0);
        var predicados = new List<string>
        {
            "ser humano",
            "ser lenguaje",
            "ser mente",
            "ser vacuidad"
        };
        var texto = string.Join(". ", predicados);

        var designacion = nombre.Mostrarse(texto, t => MapearNombres(t));

        var nombresPredicados = designacion.Nombres.Take(predicados.Count).ToList();
        nombresPredicados.Select(n => n.Fase).Should().Equal(0d, Math.PI / 2d, Math.PI, 3d * Math.PI / 2d);
        nombresPredicados.Select(n => n.Amplitud).Should().Equal(1d, 1d, 1d, 1d);
    }

    [Fact]
    public void ToString_ConDatos_DevuelveRepresentacionEsperada()
    {
        var nombre = Nombre.Imaginar("Vacuidad", Math.PI / 3, 5.0, 2.0);

        nombre.ToString().Should().Be($"{nameof(Designacion.Vacuidad)} (60.00º, 5.00 Hz)");
    }

    [Fact]
    public void Equals_ConMismoTextoYFrecuencia_DevuelveTrue()
    {
        var primero = Nombre.Imaginar("Vacuidad", 0d, 5.0, 1.0);
        var segundo = Nombre.Imaginar("Vacuidad", Math.PI / 3, 5.0, 4.0);

        primero.Equals(segundo).Should().BeTrue();
    }

    [Fact]
    public void Equals_ConInstanciasDistintasPeroMismoTextoYFrecuencia_UsaIgualdadSemantica()
    {
        var primero = Nombre.Imaginar("Vacuidad", 0d, 5.0, 1.0);
        var segundo = Nombre.Imaginar("Vacuidad", 0d, 5.0, 1.0);

        ReferenceEquals(primero, segundo).Should().BeFalse();
        primero.Id.Should().NotBe(segundo.Id);
        primero.Equals(segundo).Should().BeTrue();
    }

    [Fact]
    public void Equals_ConDistintaFrecuencia_DevuelveFalse()
    {
        var nombre = Nombre.Imaginar("Vacuidad", 0d, 5.0, 1.0);
        var distintaFrecuencia = Nombre.Imaginar("Vacuidad", 0d, 3.0, 1.0);

        nombre.Equals(distintaFrecuencia).Should().BeFalse();
        nombre.Equals("no-nombre").Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SinParametros_GeneraPorId()
    {
        var nombre = Nombre.Imaginar("Vacuidad", Math.PI / 3, 5.0, 2.0);

        nombre.GetHashCode().Should().Be(nombre.Id.GetHashCode());
    }
}
