using FluentAssertions;

public class NombreTests
{
    [Fact]
    public void Constructor_ConFrecuencia_CreaFaseInstanea()
    {
        var frecuencia = 5.0;
        var nombre = new Nombre("logos", Math.PI / 3, frecuencia);
        var palabra = nombre as Palabra;

        palabra.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_ConDatos_Crea()
    {
        var texto = "logos";
        var fase = Math.PI / 3;
        var frecuencia = 5.0;
        var nombre = new Nombre(texto, fase, frecuencia);

        nombre.Should().NotBeNull();
        nombre.Texto.Should().Be(texto);
        nombre.Fase.Should().Be(fase);
        nombre.Frecuencia.Should().Be(frecuencia);
        nombre.FaseInstanea.Should().NotBeNull();
        for(var t = 0d; t <= 4d; t += 0.25d)
        {
            nombre.FaseInstanea!(t).Should().BeApproximately(frecuencia * t, 1e-10);
        }
        nombre.Esencia.Should().NotBeNull();
        nombre.Esencia.Nombres.Should().ContainSingle().Which.Should().BeSameAs(nombre);
        for(var t = 0d; t <= 4d; t += 0.25d)
        {
            var valor = nombre.Esencia.Funcion(t);
            valor.EjeReal.Should().BeApproximately(Math.Cos(frecuencia * t), 1e-10);
            valor.EjeImaginario.Should().BeApproximately(Math.Sin(frecuencia * t), 1e-10);
        }
    }

    [Fact]
    public void Mostrarse_ConAparienciaSinNombres_CreaDesignacion()
    {
        var frecuencia = 5.0;
        var fase = Math.PI / 3;
        var nombre = new Nombre("logos", fase, frecuencia);
        var predicados = new List<string> { "ser humano", "ser lenguaje", "pensar humano" };
        var texto = string.Join(". ", predicados);
        var apariencia = Apariencia.Mente;

        var designacion = nombre.Mostrarse(apariencia, texto);

        designacion.Should().NotBeNull();
        designacion.Nombres.Should().HaveCount(predicados.Count + 1);
        designacion.Nombres.Select(n => n.Texto).SkipLast(1).Should()
            .BeEquivalentTo(predicados);
        var ultimoNombre = designacion.Nombres.Last();
        ultimoNombre.Texto.Should().Be(texto);
        ultimoNombre.Fase.Should().BeApproximately(Math.PI / 4d, 1e-10);
        ultimoNombre.Frecuencia.Should().Be(frecuencia);
    }

    [Fact]
    public void Mostrarse_ConFuncionObtenerVerboNucleoNull_UsaLaPrimerPalabraDelPredicado()
    {
        var nombre = new Nombre("logos", Math.PI / 3, 5.0);
        var predicados = new List<string> { "ser humano", "ser lenguaje", "pensar humano" };
        var texto = string.Join(". ", predicados);

        var designacion = nombre.Mostrarse(Apariencia.Mente, texto, null);

        var nombresPredicados = designacion.Nombres.SkipLast(1).ToList();
        nombresPredicados.Select(n => n.Frecuencia).Should().Equal(2d, 2d, 1d);
        nombresPredicados.Select(n => n.Fase).Should().Equal(0d, 2d * Math.PI / 3d, 4d * Math.PI / 3d);
        nombresPredicados[0].Esencia.Funcion(0d).EjeReal.Should().BeApproximately(2d, 1e-10);
        nombresPredicados[1].Esencia.Funcion(2d * Math.PI / 3d).EjeReal.Should().BeApproximately(1d, 1e-10);
        nombresPredicados[2].Esencia.Funcion(2d * Math.PI / 3d).EjeReal.Should().BeApproximately(2d, 1e-10);
    }

    [Fact]
    public void Mostrarse_ConAparienciaNormal_CreaDesignacion()
    {
        var frecuencia = 5.0;
        var fase = Math.PI / 3;
        var nombre = new Nombre("logos", fase, frecuencia);
        var predicados = new List<string> { "ser humano", "ser lenguaje", "pensar humano" };
        var texto = string.Join(". ", predicados);
        var predicadosApariencia = new List<string> { "vacuidad", "mente" };
        var textoApariencia = string.Join(". ", predicadosApariencia);
        var designacionApariencia = nombre.Mostrarse(Apariencia.Mente, textoApariencia);
        var apariencia = Apariencia.Aparecer(designacionApariencia);

        var designacion = nombre.Mostrarse(apariencia, texto);

        designacion.Should().NotBeNull();
        designacion.Nombres.Should().HaveCount(predicados.Count + 1);
        designacion.Nombres.Select(n => n.Texto).SkipLast(1).Should()
            .BeEquivalentTo(predicados);
        var ultimoNombre = designacion.Nombres.Last();
        ultimoNombre.Frecuencia.Should().Be(frecuencia);
        ultimoNombre.Fase.Should().BeApproximately(Math.Abs(Math.Atan2(
            apariencia.Funcion(0d).EjeImaginario - apariencia.Funcion(-0.001d).EjeImaginario,
            apariencia.Funcion(0d).EjeReal - apariencia.Funcion(-0.001d).EjeReal)) % (2 * Math.PI), 1e-10);
        ultimoNombre.Texto.Should().Be(texto);
    }

    [Fact]
    public void Mostrarse_ConFuncionObtenerVerboNucleo_CreaDesignacion()
    {
        var frecuencia = 5.0;
        var fase = Math.PI / 3;
        var nombre = new Nombre("logos", fase, frecuencia);
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

        var designacion = nombre.Mostrarse(Apariencia.Mente, texto, obtenerVerboNucleo);

        designacion.Should().NotBeNull();
        designacion.Nombres.Should().HaveCount(predicados.Count + 1);
        designacion.Nombres.Select(n => n.Texto).SkipLast(1).Should().Equal(predicados);

        var nombresPredicados = designacion.Nombres.SkipLast(1).ToList();
        nombresPredicados.Select(n => n.Frecuencia).Should().Equal(3d, 3d, 3d);
        nombresPredicados.Select(n => n.Fase).Should().Equal(0d, 2d * Math.PI / 3d, 4d * Math.PI / 3d);
        nombresPredicados[0].Esencia.Funcion(0d).EjeReal.Should().BeApproximately(2d, 1e-10);
        nombresPredicados[1].Esencia.Funcion(4d * Math.PI / 9d).EjeReal.Should().BeApproximately(2d, 1e-10);
        nombresPredicados[2].Esencia.Funcion(2d * Math.PI / 9d).EjeReal.Should().BeApproximately(1d, 1e-10);

        var ultimoNombre = designacion.Nombres.Last();
        ultimoNombre.Texto.Should().Be(texto);
        ultimoNombre.Fase.Should().BeApproximately(Math.PI / 4d, 1e-10);
        ultimoNombre.Frecuencia.Should().Be(frecuencia);
    }

    [Fact]
    public void Mostrarse_ConVerbosNucleoMixtos_AgrupaFrecuenciasPorCadaVerbo()
    {
        var nombre = new Nombre("logos", Math.PI / 3, 5.0);
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

        var designacion = nombre.Mostrarse(Apariencia.Mente, texto, obtenerVerboNucleo);

        var nombresPredicados = designacion.Nombres.SkipLast(1).ToList();
        nombresPredicados.Select(n => n.Frecuencia).Should().Equal(2d, 2d, 2d, 2d);
        nombresPredicados.Select(n => n.Fase).Should().Equal(0d, Math.PI / 2d, Math.PI, 3d * Math.PI / 2d);
        nombresPredicados[0].Esencia.Funcion(0d).EjeReal.Should().BeApproximately(2d, 1e-10);
        nombresPredicados[1].Esencia.Funcion(3d * Math.PI / 4d).EjeReal.Should().BeApproximately(2d, 1e-10);
        nombresPredicados[2].Esencia.Funcion(Math.PI / 2d).EjeReal.Should().BeApproximately(2d, 1e-10);
        nombresPredicados[3].Esencia.Funcion(Math.PI / 4d).EjeReal.Should().BeApproximately(1d, 1e-10);
    }

    [Fact]
    public void Mostrarse_ConComplementosParcialmenteCompartidos_CalculaAmplitudesDistintas()
    {
        var nombre = new Nombre("logos", Math.PI / 3, 5.0);
        var predicados = new List<string>
        {
            "ser humano lenguaje",
            "ser humano mente",
            "ser mente"
        };
        var texto = string.Join(". ", predicados);

        var designacion = nombre.Mostrarse(Apariencia.Mente, texto);

        var nombresPredicados = designacion.Nombres.SkipLast(1).ToList();
        nombresPredicados.Select(n => n.Frecuencia).Should().Equal(3d, 3d, 3d);
        nombresPredicados[0].Esencia.Funcion(0d).EjeReal.Should().BeApproximately(3d, 1e-10);
        nombresPredicados[1].Esencia.Funcion(4d * Math.PI / 9d).EjeReal.Should().BeApproximately(4d, 1e-10);
        nombresPredicados[2].Esencia.Funcion(2d * Math.PI / 9d).EjeReal.Should().BeApproximately(2d, 1e-10);
    }

    [Fact]
    public void Mostrarse_ConCuatroPredicados_DistribuyeFasesUniformemente()
    {
        var nombre = new Nombre("logos", Math.PI / 3, 5.0);
        var predicados = new List<string>
        {
            "ser humano",
            "ser lenguaje",
            "ser mente",
            "ser vacuidad"
        };
        var texto = string.Join(". ", predicados);

        var designacion = nombre.Mostrarse(Apariencia.Mente, texto);

        var nombresPredicados = designacion.Nombres.SkipLast(1).ToList();
        nombresPredicados.Select(n => n.Fase).Should().Equal(0d, Math.PI / 2d, Math.PI, 3d * Math.PI / 2d);
        nombresPredicados[0].Esencia.Funcion(0d).EjeReal.Should().BeApproximately(1d, 1e-10);
        nombresPredicados[1].Esencia.Funcion(3d * Math.PI / 8d).EjeReal.Should().BeApproximately(1d, 1e-10);
        nombresPredicados[2].Esencia.Funcion(Math.PI / 4d).EjeReal.Should().BeApproximately(1d, 1e-10);
        nombresPredicados[3].Esencia.Funcion(5d * Math.PI / 8d).EjeReal.Should().BeApproximately(1d, 1e-10);
    }

    [Fact]
    public void ToString_ConDatos_DevuelveRepresentacionEsperada()
    {
        var nombre = new Nombre("logos", Math.PI / 3, 5.0);

        nombre.ToString().Should().Be("logos (60.00º, 5.00 Hz)");
    }

    [Fact]
    public void Equals_ConMismoTextoYFrecuencia_DevuelveTrue()
    {
        var primero = new Nombre("logos", 0d, 5.0);
        var segundo = new Nombre("logos", Math.PI / 3, 5.0);

        primero.Equals(segundo).Should().BeTrue();
    }

    [Fact]
    public void Equals_ConInstanciasDistintasPeroMismoTextoYFrecuencia_UsaIgualdadSemantica()
    {
        var primero = new Nombre("logos", 0d, 5.0);
        var segundo = new Nombre("logos", 0d, 5.0);

        ReferenceEquals(primero, segundo).Should().BeFalse();
        primero.Id.Should().NotBe(segundo.Id);
        primero.Equals(segundo).Should().BeTrue();
    }

    [Fact]
    public void Equals_ConDistintoTextoOFrecuencia_DevuelveFalse()
    {
        var nombre = new Nombre("logos", 0d, 5.0);
        var distintoTexto = new Nombre("ethos", 0d, 5.0);
        var distintaFrecuencia = new Nombre("logos", 0d, 3.0);

        nombre.Equals(distintoTexto).Should().BeFalse();
        nombre.Equals(distintaFrecuencia).Should().BeFalse();
        nombre.Equals("no-nombre").Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_SinParametros_GeneraPorId()
    {
        var nombre = new Nombre("logos", Math.PI / 3, 5.0);

        nombre.GetHashCode().Should().Be(nombre.Id.GetHashCode());
    }
}