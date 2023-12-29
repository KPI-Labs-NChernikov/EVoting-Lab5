namespace Modelling.Models;
public sealed class Ballot
{
    public byte[] EncryptedCandidateIdPart { get; }
    public int VoterId { get; }

    public Ballot(byte[] encryptedCandidateIdPart, int voterId)
    {
        EncryptedCandidateIdPart = encryptedCandidateIdPart;
        VoterId = voterId;
    }
}
