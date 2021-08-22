using System;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.DataProtection;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace CLARiNET
{
    class Crypto
    {
        public static string Protect(string stringToEncrypt)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Convert.ToBase64String(
                    ProtectedData.Protect(
                        Encoding.UTF8.GetBytes(stringToEncrypt)
                        , null
                        , DataProtectionScope.CurrentUser));
            }
            else
            {
                return Convert.ToBase64String(
                    ProtectMac(stringToEncrypt)
                );
            }
        }

        public static string Unprotect(string encryptedString)
        {
            if (String.IsNullOrEmpty(encryptedString))
            {
                return "";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Encoding.UTF8.GetString(
                ProtectedData.Unprotect(
                    Convert.FromBase64String(encryptedString)
                    , null
                    , DataProtectionScope.CurrentUser));
            }
            else
            {
                return Encoding.UTF8.GetString(
                    UnProtectMac(Convert.FromBase64String(encryptedString)));
            }
        }

        // Credit: Oleg Batashov
        // https://simplecodesoftware.com/articles/how-to-encrypt-data-on-macos-without-dpapi
        private static IDataProtector MacEncryption()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            IDataProtector dataProtector = serviceCollection
                .BuildServiceProvider()
                .GetDataProtector(purpose: "MacOsEncryption");

            return dataProtector;
        }

        private static byte[] ProtectMac(string stringToEncrypt)
        {
            return MacEncryption().Protect(Encoding.UTF8.GetBytes(stringToEncrypt));
        }

        private static byte[] UnProtectMac(byte[] encryptedBytes)
        {
            return MacEncryption().Unprotect(encryptedBytes);
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            X509Certificate2 cert = SetupDataProtectionCertificate();

            serviceCollection.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory))
                .SetApplicationName("CLARiNET")
                .ProtectKeysWithCertificate(cert);
        }

        static X509Certificate2 SetupDataProtectionCertificate()
        {
            string subjectName = "CN=CLARiNET Data Protection Certificate";
            string subjectNameFind = subjectName.Substring(3);

            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadOnly))
            {
                X509Certificate2Collection certificateCollection = store.Certificates.Find(X509FindType.FindBySubjectName,
                    subjectNameFind,
                    // self-signed certificate won't pass X509 chain validation
                    validOnly: false);
                if (certificateCollection.Count > 0)
                {
                    return certificateCollection[0];
                }

                X509Certificate2 certificate = CreateSelfSignedDataProtectionCertificate(subjectName);
                InstallCertificateAsNonExportable(certificate);
                return certificate;
            }
        }

        static X509Certificate2 CreateSelfSignedDataProtectionCertificate(string subjectName)
        {            
            using (RSA rsa = RSA.Create(2048))
            {
                CertificateRequest request = new CertificateRequest(subjectName, rsa, HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1);
                X509Certificate2 cert = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-1), DateTimeOffset.UtcNow.AddYears(50));
                return cert;
            }
        }

        static void InstallCertificateAsNonExportable(X509Certificate2 cert)
        {
            byte[] rawData = cert.Export(X509ContentType.Pkcs12, password: "CLARiNET" );
            X509Certificate2 certPersistKey = new X509Certificate2(rawData, "CLARiNET", X509KeyStorageFlags.PersistKeySet);

            using (X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.MaxAllowed | OpenFlags.ReadWrite);
                store.Add(certPersistKey);                
                store.Close();
            }
        }
    }
}
