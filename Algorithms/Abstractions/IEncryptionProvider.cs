namespace Algorithms.Abstractions;
public interface IEncryptionProvider<TKey>
{
    byte[] Encrypt(byte[] data, TKey publicKey);
    byte[] Decrypt(byte[] data, TKey privateKey);
    byte[] MultiplyResults(IEnumerable<byte[]> data, TKey key);
}
