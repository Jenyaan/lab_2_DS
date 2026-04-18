using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Lab_RSA_SHA1;

class Program
{
    static void Main()
    {
        Console.WriteLine("=== Lab 1-2: RSA Algorithm & SHA-1 ===\n");

        Console.WriteLine("[*] Generating 512-bit RSA keys (please wait a moment)...");
        CustomRSA rsa = new CustomRSA(512);

        Console.WriteLine("\n--- Generated Keys ---");
        Console.WriteLine($"Public Key (e):  {rsa.E}");
        Console.WriteLine($"Private Key (d): {rsa.D.ToString().Substring(0, 20)}... (hidden for security)");
        Console.WriteLine($"Modulus (n):     {rsa.N.ToString().Substring(0, 20)}...\n");

        while (true)
        {
            Console.WriteLine("======================================");
            Console.WriteLine("Please choose an action:");
            Console.WriteLine("1. Encrypt a message (RSA)");
            Console.WriteLine("2. Decrypt a HEX message (RSA)");
            Console.WriteLine("3. Hash a message (SHA-1)");
            Console.WriteLine("4. Exit");
            Console.Write("> ");
            
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.WriteLine("\n--- ENCRYPTION ---");
                Console.WriteLine("Enter the text you want to encrypt:");
                Console.Write("> ");
                string textToEncrypt = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(textToEncrypt))
                {
                    Console.WriteLine("[!] Input cannot be empty.\n");
                    continue;
                }

                byte[] encryptedBytes = rsa.Encrypt(textToEncrypt);
                string encryptedHex = BitConverter.ToString(encryptedBytes).Replace("-", "");

                Console.WriteLine("\n[+] Encrypted Text (HEX):");
                Console.WriteLine(encryptedHex + "\n");
            }
            else if (choice == "2")
            {
                Console.WriteLine("\n--- DECRYPTION ---");
                Console.WriteLine("Paste the encrypted HEX string here:");
                Console.Write("> ");
                string hexInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(hexInput))
                {
                    Console.WriteLine("[!] Input cannot be empty.\n");
                    continue;
                }

                try
                {
                    byte[] cipherBytes = HexStringToByteArray(hexInput);
                    string decryptedText = rsa.Decrypt(cipherBytes);
                    
                    Console.WriteLine("\n[+] Decrypted Text:");
                    Console.WriteLine(decryptedText + "\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n[-] Error during decryption: Make sure the HEX string is correct. ({ex.Message})\n");
                }
            }
            else if (choice == "3")
            {
                // Виклик SHA-1 з вашого окремого файлу Hash.cs
                Console.WriteLine("\n--- SHA-1 HASHING ---");
                Console.WriteLine("Enter the text you want to hash:");
                Console.Write("> ");
                string textToHash = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(textToHash))
                {
                    Console.WriteLine("[!] Input cannot be empty.\n");
                    continue;
                }

                // Використовуємо клас Hash
                string hashResult = Hash.ToSHA1(textToHash);
                
                Console.WriteLine("\n[+] SHA-1 Hash Result:");
                Console.WriteLine(hashResult + "\n");
            }
            else if (choice == "4")
            {
                Console.WriteLine("\nExiting program... Goodbye!");
                break;
            }
            else
            {
                Console.WriteLine("\n[-] Invalid choice. Please enter 1, 2, 3, or 4.\n");
            }
        }
    }

    public static byte[] HexStringToByteArray(string hex)
    {
        hex = hex.Replace("-", "").Replace(" ", "");
        
        if (hex.Length % 2 != 0)
            throw new ArgumentException("Invalid HEX string length.");

        byte[] bytes = new byte[hex.Length / 2];
        for (int i = 0; i < hex.Length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
        return bytes;
    }
}

public class CustomRSA
{
    public BigInteger N { get; private set; }
    public BigInteger E { get; private set; }
    public BigInteger D { get; private set; }

    public CustomRSA(int keySizeBits)
    {
        GenerateKeys(keySizeBits);
    }

    private void GenerateKeys(int keySizeBits)
    {
        int primeSize = keySizeBits / 2;

        BigInteger p = GenerateLargePrime(primeSize);
        BigInteger q = GenerateLargePrime(primeSize);

        N = p * q;
        BigInteger m = (p - 1) * (q - 1);

        E = 65537;
        D = ModInverse(E, m);
    }

    public byte[] Encrypt(string plaintext)
    {
        byte[] dataBytes = Encoding.UTF8.GetBytes(plaintext);
        int maxBlockSize = N.GetByteCount(isUnsigned: true) - 1;
        
        using (MemoryStream ms = new MemoryStream())
        {
            for (int i = 0; i < dataBytes.Length; i += maxBlockSize)
            {
                int currentBlockSize = Math.Min(maxBlockSize, dataBytes.Length - i);
                byte[] block = new byte[currentBlockSize];
                Array.Copy(dataBytes, i, block, 0, currentBlockSize);

                BigInteger m = new BigInteger(block, isUnsigned: true, isBigEndian: true);
                BigInteger c = BigInteger.ModPow(m, E, N);

                byte[] encryptedBlock = c.ToByteArray(isUnsigned: true, isBigEndian: true);
                byte[] paddedBlock = new byte[N.GetByteCount(isUnsigned: true)];
                Array.Copy(encryptedBlock, 0, paddedBlock, paddedBlock.Length - encryptedBlock.Length, encryptedBlock.Length);
                
                ms.Write(paddedBlock, 0, paddedBlock.Length);
            }
            return ms.ToArray();
        }
    }

    public string Decrypt(byte[] ciphertext)
    {
        int blockSize = N.GetByteCount(isUnsigned: true);
        
        using (MemoryStream ms = new MemoryStream())
        {
            for (int i = 0; i < ciphertext.Length; i += blockSize)
            {
                byte[] block = new byte[blockSize];
                Array.Copy(ciphertext, i, block, 0, blockSize);

                BigInteger c = new BigInteger(block, isUnsigned: true, isBigEndian: true);
                BigInteger m = BigInteger.ModPow(c, D, N);

                byte[] decryptedBlock = m.ToByteArray(isUnsigned: true, isBigEndian: true);
                ms.Write(decryptedBlock, 0, decryptedBlock.Length);
            }
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }

    private BigInteger ModInverse(BigInteger a, BigInteger n)
    {
        BigInteger t = 0, newt = 1;
        BigInteger r = n, newr = a;

        while (newr != 0)
        {
            BigInteger quotient = r / newr;

            BigInteger tempT = t;
            t = newt;
            newt = tempT - quotient * newt;

            BigInteger tempR = r;
            r = newr;
            newr = tempR - quotient * newr;
        }

        if (r > 1) throw new Exception("a and n are not coprime");
        if (t < 0) t += n;

        return t;
    }

    private BigInteger GenerateLargePrime(int bits)
    {
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            byte[] bytes = new byte[bits / 8];
            BigInteger p;

            do
            {
                rng.GetBytes(bytes);
                bytes[bytes.Length - 1] |= 1;      
                bytes[0] &= 0x7F;                  
                bytes[0] |= 0x40;                  

                p = new BigInteger(bytes, isUnsigned: true, isBigEndian: true);

            } while (!IsProbablePrime(p, 10)); 

            return p;
        }
    }

    private bool IsProbablePrime(BigInteger source, int certainty)
    {
        if (source == 2 || source == 3) return true;
        if (source < 2 || source % 2 == 0) return false;

        BigInteger d = source - 1;
        int s = 0;

        while (d % 2 == 0)
        {
            d /= 2;
            s += 1;
        }

        byte[] bytes = new byte[source.ToByteArray().Length];
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            for (int i = 0; i < certainty; i++)
            {
                BigInteger a;
                do
                {
                    rng.GetBytes(bytes);
                    a = new BigInteger(bytes);
                } while (a < 2 || a >= source - 2);

                BigInteger x = BigInteger.ModPow(a, d, source);
                if (x == 1 || x == source - 1) continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, source);
                    if (x == 1) return false;
                    if (x == source - 1) break;
                }

                if (x != source - 1) return false;
            }
        }
        return true;
    }
}