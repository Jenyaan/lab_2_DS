using System.Security.Cryptography;
using System.Text;

public static class Hash
{
    public static string ToSHA1(string data)
    {
        using (SHA1 sha1 = SHA1.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hashBytes = sha1.ComputeHash(bytes);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}