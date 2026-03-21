using System;

namespace DomainLogic.Services.Particulas;

public class Vector2D
{
    public double X { get; set; }
    public double Y { get; set; }

    public Vector2D(double x, double y)
    {
        X = x;
        Y = y;
    }

    public Vector2D Suma(Vector2D otro) => new(X + otro.X, Y + otro.Y);
    
    public Vector2D Escala(double factor) => new(X * factor, Y * factor);

    public double Magnitud => Math.Sqrt(X * X + Y * Y);

    public Vector2D Normalizar()
    {
        var mag = Magnitud;
        if (mag == 0) return new(0, 0);
        return new(X / mag, Y / mag);
    }

    public double ProductoPunto(Vector2D otro) => X * otro.X + Y * otro.Y;

    public override bool Equals(object? obj)
    {
        if (obj is not Vector2D other) return false;
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public override string ToString() => $"({X}, {Y})";
}
