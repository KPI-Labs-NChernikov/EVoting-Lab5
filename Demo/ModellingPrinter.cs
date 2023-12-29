using Modelling.Extensions;
using Modelling.Models;

namespace Demo;
public sealed class ModellingPrinter
{
    private readonly DemoDataFactory _dataFactory;

    public ModellingPrinter(DemoDataFactory dataFactory)
    {
        _dataFactory = dataFactory;
    }

    public void PrintUsualVoting(CentralElectionCommission commission, IReadOnlyList<Voter> voters, IReadOnlyList<Candidate> candidates)
    {
        Console.WriteLine("Usual voting:");

        commission.SetupVoters(voters);
        commission.SetupCandidates(candidates);

        Console.WriteLine("Voters:");
        foreach (var voter in voters)
        {
            Console.WriteLine($"{voter.FullName} ({voter.Id})");
        }
        Console.WriteLine();
        Console.WriteLine("Candidates:");
        foreach (var candidate in candidates)
        {
            Console.WriteLine($"{candidate.FullName} ({candidate.Id})");
        }
        Console.WriteLine();

        var subCommission1 = _dataFactory.CreateElectionCommission(commission.VotersSignaturePublicKeys);
        var subCommission2 = _dataFactory.CreateElectionCommission(commission.VotersSignaturePublicKeys);

        var votersWithCandidateIds = _dataFactory.CreateVotersWithCandidatesIds(voters, candidates);
        foreach (var (voter, candidateId) in votersWithCandidateIds)
        {
            var ballots = voter.PrepareBallots(candidateId, commission.EncryptionPublicKey);
            var result1 = subCommission1.AcceptBallot(ballots.Item1);
            if (result1.IsFailed)
            {
                result1.PrintErrorIfFailed();
            }
            else
            {
                Console.WriteLine($"Commission 1 accepted {voter.Id} vote.");
            }
            var result2 = subCommission2.AcceptBallot(ballots.Item2);
            if (result2.IsFailed)
            {
                result2.PrintErrorIfFailed();
            }
            else
            {
                Console.WriteLine($"Commission 2 accepted {voter.Id} vote.");
            }
        }
        Console.WriteLine();

        var commission1Results = subCommission1.PublishBallots();
        var commission2Results = subCommission2.PublishBallots();

        var results = commission.AcceptBallots(commission1Results, commission2Results);
        PrintVotingResults(results);

        Console.WriteLine();
    }

    public void PrintVotingResults(VotingResults results)
    {
        Console.WriteLine("Results:");

        Console.WriteLine("Ballots:");
        foreach (var ballotResult in results.Ballots)
        {
            Console.WriteLine($"Voter ID: {ballotResult.VoterId} Candidate: {ballotResult.CandidateId}");
        }
        Console.WriteLine($"Spoiled ballots: {results.SpoiledPlainBallots.Count}");
        Console.WriteLine($"Spoiled ballots parts: {results.SpoiledBallots.Count}");
        Console.WriteLine("Candidates:");
        foreach (var candidate in results.CandidatesResults.Values.OrderByVotes())
        {
            Console.WriteLine($"{candidate.Candidate.FullName} (id: {candidate.Candidate.Id}): {candidate.Votes} votes");
        }
    }
}
