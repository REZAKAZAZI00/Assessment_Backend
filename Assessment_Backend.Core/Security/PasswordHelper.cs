namespace Assessment_Backend.Core.Security
{
    public class PasswordHelper
    {
        public static string EncodePasswordSHA1(string password)
        {
            Byte[] originalBytes;
            Byte[] encodeedBytes;

            SHA1 sHA1;

#pragma warning disable SYSLIB0021 // Type or member is obsolete
            sHA1 = new SHA1CryptoServiceProvider();
#pragma warning restore SYSLIB0021 // Type or member is obsolete

            originalBytes = ASCIIEncoding.Default.GetBytes(password);
            encodeedBytes = sHA1.ComputeHash(originalBytes);

            return BitConverter.ToString(encodeedBytes);

        }
    }
}
