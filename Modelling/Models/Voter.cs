using System.Security.Cryptography;
using Algorithms.Abstractions;
using Algorithms.Common;
using Org.BouncyCastle.Crypto;

namespace Modelling.Models;
public sealed class Voter
{
    public int Id { get; set; }
    public string FullName { get; }
    public DSAParameters PublicKey { get; }
    private readonly DSAParameters _privateKey;
    private readonly ISignatureProvider<DSAParameters> _signatureProvider;
    private readonly IEncryptionProvider<AsymmetricKeyParameter> _encryptionProvider;
    private readonly IObjectToByteArrayTransformer _transformer;

    public Voter(string fullName, Keys<DSAParameters> signatureKeys, ISignatureProvider<DSAParameters> signatureProvider, IEncryptionProvider<AsymmetricKeyParameter> encryptionProvider, IObjectToByteArrayTransformer transformer)
    {
        FullName = fullName;
        PublicKey = signatureKeys.PublicKey;
        _privateKey = signatureKeys.PrivateKey;
        _signatureProvider = signatureProvider;
        _encryptionProvider = encryptionProvider;
        _transformer = transformer;
    }

    public byte[] PrepareBallot(int candidateId, AsymmetricKeyParameter centralElectionCommissionEncryptionPublicKey)
    {
        throw new NotImplementedException();
    }
}
