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
    private readonly IRandomProvider _randomProvider;

    public Voter(string fullName, Keys<DSAParameters> signatureKeys, ISignatureProvider<DSAParameters> signatureProvider, IEncryptionProvider<AsymmetricKeyParameter> encryptionProvider, IObjectToByteArrayTransformer transformer, IRandomProvider randomProvider)
    {
        FullName = fullName;
        PublicKey = signatureKeys.PublicKey;
        _privateKey = signatureKeys.PrivateKey;
        _signatureProvider = signatureProvider;
        _encryptionProvider = encryptionProvider;
        _transformer = transformer;
        _randomProvider = randomProvider;
    }

    public (SignedData<Ballot>, SignedData<Ballot>) PrepareBallots(int candidateId, AsymmetricKeyParameter centralElectionCommissionEncryptionPublicKey)
    {
        var candidateIdParts = IntHelpers.PickRandomDivisors(candidateId, _randomProvider);
        var candidateIdPartsAsArray = new [] { candidateIdParts.Item1, candidateIdParts.Item2 };

        var ballots = new List<Ballot>();
        foreach (var part in candidateIdPartsAsArray)
        {
            var partAsByteArray = _transformer.Transform(part);
            var encryptedPart = _encryptionProvider.Encrypt(partAsByteArray, centralElectionCommissionEncryptionPublicKey);
            ballots.Add(new Ballot(encryptedPart, Id));
        }

        var signedBallots = new List<SignedData<Ballot>>();
        foreach (var ballot in ballots)
        {
            var ballotAsByteArray = _transformer.Transform(ballot);
            var signature = _signatureProvider.Sign(ballotAsByteArray, _privateKey);
            signedBallots.Add(new SignedData<Ballot>(ballot, signature));
        }

        return (signedBallots[0], signedBallots[1]);
    }
}
