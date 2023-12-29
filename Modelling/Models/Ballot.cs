namespace Modelling.Models;
public sealed class Ballot
{
    public byte[] EncryptedCandidateIdPart { get; set; }
    public int VoterId { get; set; }

    public Ballot(byte[] encryptedCandidateIdPart, int voterId)
    {
        EncryptedCandidateIdPart = encryptedCandidateIdPart;
        VoterId = voterId;
    }
}
