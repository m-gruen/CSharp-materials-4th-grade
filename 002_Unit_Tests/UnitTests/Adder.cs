using System.Numerics;

namespace UnitTests;

public class Adder
{
    public T Add<T>(T a, T b) where T : INumber<T>, IAdditiveIdentity<T, T>
    {
        return a + b + T.AdditiveIdentity;
    }
}
