using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Parameters;

namespace FinTrack.API.Services.Interfaces
{
    public class BouncyCastleService : IBouncyCastleService
    {
        private readonly string pemDirectory;

        public BouncyCastleService(
            IConfiguration configuration
            )
        {
            this.pemDirectory = configuration["BouncyCastle:PemDirectory"]!;
        }

        public void GenerateRsaKeys()
        {
            var keyPair = GenerateKeyPair();
            AsymmetricKeyParameter privateKey = keyPair.Private;
            AsymmetricKeyParameter publicKey = keyPair.Public;

            SaveKeyToFile($"{pemDirectory}\\private.pem", privateKey);
            SaveKeyToFile($"{pemDirectory}\\public.pem", publicKey);
        }

        public AsymmetricCipherKeyPair GenerateKeyPair()
        {
            var rsaKeyPairGenerator = new RsaKeyPairGenerator();
            rsaKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
            return rsaKeyPairGenerator.GenerateKeyPair();
        }

        public void SaveKeyToFile(string filePath, AsymmetricKeyParameter key)
        {
            using (TextWriter writer = new StreamWriter(filePath))
            {
                PemWriter pemWriter = new PemWriter(writer);
                pemWriter.WriteObject(key);
            }
        }

        public string SignDocument(string filePath)
        {
            AsymmetricKeyParameter privateKey = LoadPrivateKey($"{pemDirectory}\\private.pem");

            byte[] data = File.ReadAllBytes(filePath);

            byte[] signature = SignData(data, privateKey);
            var signaturePath = $"{pemDirectory}\\signature-{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}.sig";
            File.WriteAllBytes(signaturePath, signature);
            return signaturePath;
        }

        public byte[] SignData(byte[] data, AsymmetricKeyParameter privateKey)
        {
            var digest = new Sha256Digest();
            digest.BlockUpdate(data, 0, data.Length);
            byte[] hashedData = new byte[digest.GetDigestSize()];
            digest.DoFinal(hashedData, 0);

            var signer = new RsaDigestSigner(new Sha256Digest());
            signer.Init(true, privateKey);
            signer.BlockUpdate(hashedData, 0, hashedData.Length);
            return signer.GenerateSignature();
        }

        public AsymmetricKeyParameter LoadPrivateKey(string path)
        {
            using (TextReader reader = new StreamReader(path))
            {
                var pemReader = new PemReader(reader);
                return ((AsymmetricCipherKeyPair)pemReader.ReadObject()).Private;
            }
        }

        public bool VerifySignature(string filePath, string signaturePath)
        {
            AsymmetricKeyParameter publicKey = LoadPublicKey($"{pemDirectory}\\public.pem");

            byte[] data = File.ReadAllBytes(filePath);

            byte[] signature = File.ReadAllBytes(signaturePath);

            return VerifySignature(data, signature, publicKey);
        }

        public bool VerifySignature(byte[] data, byte[] signature, AsymmetricKeyParameter publicKey)
        {
            var digest = new Sha256Digest();
            digest.BlockUpdate(data, 0, data.Length);
            byte[] hashedData = new byte[digest.GetDigestSize()];
            digest.DoFinal(hashedData, 0);

            var verifier = new RsaDigestSigner(new Sha256Digest());
            verifier.Init(false, publicKey);
            verifier.BlockUpdate(hashedData, 0, hashedData.Length);
            return verifier.VerifySignature(signature);
        }

        public AsymmetricKeyParameter LoadPublicKey(string path)
        {
            using (TextReader reader = new StreamReader(path))
            {
                var pemReader = new PemReader(reader);
                return (RsaKeyParameters)pemReader.ReadObject();
            }
        }
    }
}
