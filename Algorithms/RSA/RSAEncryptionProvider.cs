using Algorithms.Abstractions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;

namespace Algorithms.RSA;
public sealed class RSAEncryptionProvider : IEncryptionProvider<AsymmetricKeyParameter>
{
    public byte[] Encrypt(byte[] data, AsymmetricKeyParameter publicKey)
    {
        var engine = new RsaEngine();
        engine.Init(true, publicKey);

        return engine.ProcessBlock(data, 0, data.Length);
    }

    public byte[] Decrypt(byte[] data, AsymmetricKeyParameter privateKey)
    {
        var engine = new RsaEngine();
        engine.Init(false, privateKey);

        return engine.ProcessBlock(data, 0, data.Length);
    }

    public byte[] MultiplyResults(IEnumerable<byte[]> data, AsymmetricKeyParameter key)
    {
        var m = ((RsaKeyParameters)key).Modulus;
        //var result = BigInteger.One;
        //foreach (var dataPart in data)
        //{
        //    result = result.Multiply(new BigInteger(1, dataPart, 0, dataPart.Length));
        //}
        //result = result.Mod(m);
        BigInteger? result = null;
        foreach (var dataPart in data)
        {
            if (result is null)
            {
                result = new BigInteger(1, dataPart, 0, dataPart.Length);
            }
            else
            {
                result = result.Multiply(new BigInteger(1, dataPart, 0, dataPart.Length));
            }
        }
        result = result.Mod(m);
        return result.ToByteArrayUnsigned();
    }
}
