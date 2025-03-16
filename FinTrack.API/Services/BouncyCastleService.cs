using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.OpenSsl;

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
    }
}
