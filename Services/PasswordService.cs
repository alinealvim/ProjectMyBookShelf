using System.Security.Cryptography;
using System.Text;

namespace MyBookShelf.Services
{
    public class PasswordService : IPasswordService
    {
        private const string SERVICE_KEY = "senha_secreta_1234567890"; // 32 bytes para AES-256        

        // Método para criptografar a senha antes de armazenar na base de dados
        public string EncryptPassword(string password)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Gerar IV aleatório
                aesAlg.GenerateIV();

                // A chave precisa ter 32 bytes para AES-256
                aesAlg.Key = Encoding.UTF8.GetBytes(SERVICE_KEY.PadRight(32, ' '));

                // Criptografar a senha
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                using MemoryStream msEncrypt = new();
                // Salvar IV no início do fluxo de criptografia
                msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                using (CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (StreamWriter swEncrypt = new(csEncrypt))
                {
                    swEncrypt.Write(password);
                }

                // Retorna a senha criptografada em Base64
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }

        // Método para verificar se a senha fornecida corresponde à senha armazenada (criptografada)
        public bool CheckPassword(string password, string encryptedPassword)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedPassword);

            using Aes aesAlg = Aes.Create();
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            // A chave precisa ter 32 bytes para AES-256
            aesAlg.Key = Encoding.UTF8.GetBytes(SERVICE_KEY.PadRight(32, ' '));

            // Extrair o IV do início da senha criptografada
            byte[] iv = new byte[aesAlg.BlockSize / 8]; // Tamanho do IV (geralmente 16 bytes para AES)
            Array.Copy(cipherTextBytes, iv, iv.Length);

            // Definir IV para a descriptografia
            aesAlg.IV = iv;

            // Descriptografar
            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            using MemoryStream msDecrypt = new(cipherTextBytes, iv.Length, cipherTextBytes.Length - iv.Length);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);
            string senhaDescriptografada = srDecrypt.ReadToEnd();

            // Comparar a senha fornecida com a senha descriptografada
            return password == senhaDescriptografada;
        }
    }
    public interface IPasswordService
    {
        string EncryptPassword(string password);
        bool CheckPassword(string password, string encryptedPassword);
    }
}