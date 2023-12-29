using Algorithms.Abstractions;
using Modelling.Models;
using Org.BouncyCastle.Crypto;
using System.Security.Cryptography;

namespace Demo;
public sealed class DemoDataFactory
{
    private readonly IEncryptionProvider<AsymmetricKeyParameter> _encryptionProvider;
    private readonly IKeyGenerator<AsymmetricKeyParameter> _encryptionKeyGenerator;

    private readonly ISignatureProvider<DSAParameters> _signatureProvider;
    private readonly IKeyGenerator<DSAParameters> _signatureKeyGenerator;

    private readonly IObjectToByteArrayTransformer _transformer;
    private readonly IRandomProvider _randomProvider;

    public DemoDataFactory(IEncryptionProvider<AsymmetricKeyParameter> encryptionProvider, IKeyGenerator<AsymmetricKeyParameter> encryptionKeyGenerator, ISignatureProvider<DSAParameters> signatureProvider, IKeyGenerator<DSAParameters> keysignatureGenerator, IObjectToByteArrayTransformer objectToByteArrayTransformer, IRandomProvider randomProvider)
    {
        _encryptionProvider = encryptionProvider;
        _encryptionKeyGenerator = encryptionKeyGenerator;
        _signatureProvider = signatureProvider;
        _signatureKeyGenerator = keysignatureGenerator;
        _transformer = objectToByteArrayTransformer;
        _randomProvider = randomProvider;
    }

    public IReadOnlyList<Candidate> CreateCandidates()
    {
        return new List<Candidate>
        {
            new ("Ishaan Allison"),
            new ("Oliver Mendez"),
            new ("Naomi Winter"),
        };
    }

    public IReadOnlyList<Voter> CreateVoters()
    {
        return new List<Voter>
        {
            new ("Jasper Lambert", _signatureKeyGenerator.Generate(), _signatureProvider, _encryptionProvider, _transformer, _randomProvider),
            new ("Jonty Levine", _signatureKeyGenerator.Generate(), _signatureProvider, _encryptionProvider, _transformer, _randomProvider),      // Not capable.
            new ("Nathaniel Middleton", _signatureKeyGenerator.Generate(), _signatureProvider, _encryptionProvider, _transformer, _randomProvider),
            new ("Nathan Bass", _signatureKeyGenerator.Generate(), _signatureProvider, _encryptionProvider, _transformer, _randomProvider),
            new ("Aran Doyle", _signatureKeyGenerator.Generate(), _signatureProvider, _encryptionProvider, _transformer, _randomProvider),
            new ("Julian Harper", _signatureKeyGenerator.Generate(), _signatureProvider, _encryptionProvider, _transformer, _randomProvider),
            new ("Lucian Gross", _signatureKeyGenerator.Generate(), _signatureProvider, _encryptionProvider, _transformer, _randomProvider),

            new ("Alicia Sierra", _signatureKeyGenerator.Generate(), _signatureProvider, _encryptionProvider, _transformer, _randomProvider)
        };
    }

    public CentralElectionCommission CreateCentralElectionCommission()
    {
        return new CentralElectionCommission(_encryptionKeyGenerator.Generate(), _encryptionProvider, _randomProvider, _transformer);
    }

    public ElectionCommission CreateElectionCommission(IReadOnlyDictionary<int, DSAParameters> votersSignaturePublicKeys)
    {
        return new ElectionCommission(votersSignaturePublicKeys, _signatureProvider, _transformer);
    }

    public Dictionary<Voter, int> CreateVotersWithCandidatesIds(IReadOnlyList<Voter> voters, IReadOnlyList<Candidate> candidates)
    {
        var dictionary = new Dictionary<Voter, int>();
        for (var i = 0; i < voters.Count; i++)
        {
            var candidateIndex = (i % 8 + 1) switch
            {
                1 => 0,
                2 => 0,

                3 => 1,
                4 => 0,
                5 => 2,
                6 => 2,
                7 => 2,
                8 => 0,

                _ => throw new InvalidOperationException("Negative and zero voters' ids are not supported in this method.")
            };
            dictionary.Add(voters[i], candidates[candidateIndex].Id);
        }
        return dictionary;
    }
}
