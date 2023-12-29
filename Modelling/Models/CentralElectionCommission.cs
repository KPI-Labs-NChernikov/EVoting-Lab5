using System.Security.Cryptography;
using Algorithms.Abstractions;
using Algorithms.Common;
using Org.BouncyCastle.Crypto;

namespace Modelling.Models;
public sealed class CentralElectionCommission
{
    public IReadOnlyList<Candidate> Candidates => _candidates;
    private List<Candidate> _candidates = [];
    private List<Voter> _voters = [];
    public IReadOnlyDictionary<int, DSAParameters> VotersSignaturePublicKeys => _voters.ToDictionary(v => v.Id, v => v.PublicKey);

    public AsymmetricKeyParameter EncryptionPublicKey { get; }
    private readonly AsymmetricKeyParameter _encryptionPrivateKey;
    private readonly IEncryptionProvider<AsymmetricKeyParameter> _encryptionProvider;
    private readonly IRandomProvider _randomProvider;
    private readonly IObjectToByteArrayTransformer _objectToByteArrayTransformer;

    public CentralElectionCommission(Keys<AsymmetricKeyParameter> encryptionKeys,
        IEncryptionProvider<AsymmetricKeyParameter> encryptionProvider, IRandomProvider randomProvider, IObjectToByteArrayTransformer objectToByteArrayTransformer)
    {
        EncryptionPublicKey = encryptionKeys.PublicKey;
        _encryptionPrivateKey = encryptionKeys.PrivateKey;
        _encryptionProvider = encryptionProvider;
        _randomProvider = randomProvider;
        _objectToByteArrayTransformer = objectToByteArrayTransformer;
    }

    public void SetupCandidates(IReadOnlyList<Candidate> candidates)
    {
        _candidates = candidates.ToList();
        var ids = new HashSet<int>();
        foreach (var candidate in _candidates)
        {
            int id;
            do
            {
                id = _randomProvider.NextInt(10, 100);
            } while (ids.Contains(id));
            ids.Add(id);
            candidate.Id = id;
        }
    }

    public void SetupVoters(IReadOnlyList<Voter> voters)
    {
        _voters = voters.ToList();
        var ids = new HashSet<int>();
        foreach (var voter in _voters)
        {
            int id;
            do
            {
                id = _randomProvider.NextInt(10, 100);
            } while (ids.Contains(id));

            ids.Add(id);
            voter.Id = id;
        }
    }

    public VotingResults AcceptBallots(IEnumerable<Ballot> ballots1, IEnumerable<Ballot> ballots2)
    {
        var results = new VotingResults();
        foreach (var candidate in _candidates)
        {
            results.CandidatesResults.Add(candidate.Id, new CandidateResult(candidate));
        }

        var ballotsFromOnlyFirstCommission = ballots1.ExceptBy(ballots2.Select(b => b.VoterId), b => b.VoterId).ToList();
        var ballotsFromOnlySecondCommission = ballots2.ExceptBy(ballots1.Select(b => b.VoterId), b => b.VoterId).ToList();
        foreach (var ballot in ballotsFromOnlyFirstCommission.Concat(ballotsFromOnlySecondCommission))
        {
            results.SpoiledBallots.Add(ballot);
        }

        var ballotsFromBothCommissions = ballots1.IntersectBy(ballots2.Select(b => b.VoterId), b => b.VoterId);
        foreach (var ballot1 in ballotsFromBothCommissions)
        {
            var ballot2 = ballots2.First(b => b.VoterId == ballot1.VoterId);
            try
            {
                var encryptedCandidateId = _encryptionProvider.MultiplyResults(
                    new List<byte[]>() { ballot1.EncryptedCandidateIdPart, ballot2.EncryptedCandidateIdPart },
                    _encryptionPrivateKey);
                var decryptedCandidateIdAsArray =
                    _encryptionProvider.Decrypt(encryptedCandidateId, _encryptionPrivateKey);
                var candidateId = _objectToByteArrayTransformer.ReverseTransform<int>(decryptedCandidateIdAsArray);

                if (!results.CandidatesResults.ContainsKey(candidateId))
                {
                    results.SpoiledPlainBallots.Add(new(candidateId, ballot1.VoterId));
                    continue;
                }

                results.CandidatesResults[candidateId].Votes++;
                results.Ballots.Add(new (candidateId, ballot1.VoterId));
            }
            catch (Exception)
            {
                results.SpoiledBallots.Add(ballot1);
                results.SpoiledBallots.Add(ballot2);
            }
        }

        return results;
    }
}
