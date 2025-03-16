using Org.BouncyCastle.Crypto;

namespace FinTrack.API.Services.Interfaces
{
    public interface IBouncyCastleService
    {
        AsymmetricCipherKeyPair GenerateKeyPair();
        AsymmetricKeyParameter LoadPrivateKey(string path);
        AsymmetricKeyParameter LoadPublicKey(string path);
        void SaveKeyToFile(string filePath, AsymmetricKeyParameter key);
        byte[] SignData(byte[] data, AsymmetricKeyParameter privateKey);
        void GenerateRsaKeys();
        string SignDocument(string filePath);
        bool VerifySignature(string filePath, string signaturePath);
        bool VerifySignature(byte[] data, byte[] signature, AsymmetricKeyParameter publicKey);
    }
}
