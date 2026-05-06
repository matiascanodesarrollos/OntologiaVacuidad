using System;
using System.Collections.Generic;
using System.Linq;

namespace DomainLogic.Services;

public static class ProcesadorTexto
{
    public static Designacion ProcesarTexto(this Nombre nombre, string texto)
    {
        Func<string, string> obtenerVerboNucleo = predicado => predicado.Split(' ').First();
        var nombres = new Dictionary<double, List<Nombre>>
        {
            { nombre.Frecuencia, new List<Nombre> { nombre } }
        };
        var predicados = texto
            .Split('.')
            .Select(p => p.Trim())
            .Where(p => !string.IsNullOrEmpty(p))
            .ToList();        
        var diccionarioVerbos = predicados
            .Select(p => obtenerVerboNucleo(p))
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => g.Count());        
        var palabras = predicados.SelectMany(p => p.Split(' ')).ToList();
        var diccionarioComplementos = palabras
            .Where(p => !diccionarioVerbos.ContainsKey(p)) //Solo los complementos
            .GroupBy(p => p)
            .ToDictionary(g => g.Key, g => Math.Max(1, g.Count()));

        var deltaFasePredicados = 2 * Math.PI / predicados.Count;
        for(var i = 0; i < predicados.Count; i++)
        {
            var palabrasPredicado = predicados[i].Split(' ');
            var verboNucleo = obtenerVerboNucleo(predicados[i]);

            var frecuencia = diccionarioVerbos[verboNucleo];
            if(!nombres.ContainsKey(frecuencia))
            {
                nombres.Add(frecuencia, new List<Nombre>());
            }
            var amplitud = palabrasPredicado
                .Where(p => p != verboNucleo)
                .Sum(p => diccionarioComplementos[p]);
            var fase = i * deltaFasePredicados;

            nombres[frecuencia].Add(Nombre.Imaginar(
                predicados[i], 
                fase, 
                frecuencia,
                amplitud));
        }
        return Apariencia.Aparecer(nombres.SelectMany(kv => kv.Value).ToList());
    }
}
