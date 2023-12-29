using System.Security.Cryptography;
using Algorithms.Abstractions;
using FluentResults;

namespace Modelling.Models;
public sealed class ElectionCommission
{
    private readonly HashSet<int> _acceptedVoterIds = [];
    private readonly IReadOnlyDictionary<int, DSAParameters> _votersSignaturePublicKeys;
    private readonly List<Ballot> _savedBallots = [];
    private readonly ISignatureProvider<DSAParameters> _signatureProvider;
    private readonly IObjectToByteArrayTransformer _transformer;

    public ElectionCommission(IReadOnlyDictionary<int, DSAParameters> votersSignaturePublicKeys, ISignatureProvider<DSAParameters> signatureProvider, IObjectToByteArrayTransformer transformer)
    {
        _votersSignaturePublicKeys = votersSignaturePublicKeys;
        _signatureProvider = signatureProvider;
        _transformer = transformer;
    }

    public Result<Ballot> AcceptBallot(SignedData<Ballot> signedBallot)
    {
        return VerifyVoter(signedBallot)
            .Bind(SaveBallot);
    }

    private Result<Ballot> VerifyVoter(SignedData<Ballot> signedBallot)
    {
        var voterWasFound = _votersSignaturePublicKeys.TryGetValue(signedBallot.Data.VoterId, out var voterSignaturePublicKey);

        if (!voterWasFound)
        {
            return Result.Fail("Voter has not registered.");
        }

        var signatureIsAuthentic = _signatureProvider.Verify(_transformer.Transform(signedBallot.Data), signedBallot.Signature, voterSignaturePublicKey);
        if (!signatureIsAuthentic)
        {
            return Result.Fail(new Error("The signature is not authentic."));
        }

        var voterHasAlreadyCastedAVote = _acceptedVoterIds.Contains(signedBallot.Data.VoterId);
        if (voterHasAlreadyCastedAVote)
        {
            return Result.Fail("Voter has already casted a vote.");
        }

        return Result.Ok(signedBallot.Data);
    }

    private Result<Ballot> SaveBallot(Ballot ballot)
    {
        _acceptedVoterIds.Add(ballot.VoterId);
        _savedBallots.Add(ballot);
        return Result.Ok(ballot);
    }

    public IReadOnlyList<Ballot> PublishBallots()
    {
        return _savedBallots;
    }
}
