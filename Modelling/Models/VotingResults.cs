namespace Modelling.Models;
public sealed class VotingResults
{
    public SortedDictionary<int, CandidateResult> CandidatesResults { get; } = [];

    public IList<PlainBallot> Ballots { get; } = new List<PlainBallot>();

    public IList<Ballot> SpoiledBallots { get; } = new List<Ballot>();
    public IList<PlainBallot> SpoiledPlainBallots { get; } = new List<PlainBallot>();
}
