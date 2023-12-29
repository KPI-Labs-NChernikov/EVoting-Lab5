namespace Modelling.Models;
public sealed class PlainBallot
{
    public int CandidateId { get; }
    public int VoterId { get; }

    public PlainBallot(int candidateId, int voterId)
    {
        CandidateId = candidateId;
        VoterId = voterId;
    }
}
