using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Signers;

namespace FinTrack.API.Services.Interfaces
{
    public class BouncyCastleService : IBouncyCastleService
    {
        private readonly ApplicationDbContext dbContext;

        private readonly string pemDirectory;

        public BouncyCastleService(
            IConfiguration configuration,
            ApplicationDbContext dbContext
            )
        {
            this.dbContext = dbContext;
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

        public static AsymmetricCipherKeyPair GenerateKeyPair()
        {
            var rsaKeyPairGenerator = new RsaKeyPairGenerator();
            rsaKeyPairGenerator.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
            return rsaKeyPairGenerator.GenerateKeyPair();
        }

        public static void SaveKeyToFile(string filePath, AsymmetricKeyParameter key)
        {
            using (TextWriter writer = new StreamWriter(filePath))
            {
                PemWriter pemWriter = new PemWriter(writer);
                pemWriter.WriteObject(key);
            }
        }

        public string SignDocument(string filePath)
        {
            AsymmetricKeyParameter privateKey = LoadPrivateKey("private.pem");

            byte[] data = File.ReadAllBytes(filePath);

            byte[] signature = SignData(data, privateKey);

            File.WriteAllBytes("signature.sig", signature);
            var fileName = Path.GetFileName(filePath);
            return $"{pemDirectory}\\{fileName}-signature-{new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds()}.sig";
        }

        public static byte[] SignData(byte[] data, AsymmetricKeyParameter privateKey)
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

        public static AsymmetricKeyParameter LoadPrivateKey(string path)
        {
            using (TextReader reader = new StreamReader(path))
            {
                var pemReader = new PemReader(reader);
                return ((AsymmetricCipherKeyPair)pemReader.ReadObject()).Private;
            }
        }
    }
}
