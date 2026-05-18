using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VulpesX.Shared.Utilities
{
    public class SignHelper
    {
        public static Tuple<bool, string?> SignXMLCAdESBES(string SourceXML, string SignedXML)
        {
            // trial 30 giorni dal 8/2/2023
            new Chilkat.Global().UnlockBundle("Start-Trial");
            Chilkat.Crypt2 crypt = new Chilkat.Crypt2();
            crypt.VerboseLogging = true;

            Chilkat.Cert cert = new Chilkat.Cert();
            // Use your smart card user PIN for signing.
            cert.SmartCardPin = "0000";

            bool success = cert.LoadFromSmartcard("serial=1fe6bb");
            if (success == false)
            {
                return new Tuple<bool, string?>(false, $"Errore durante l'accesso alla smartcard. Verificare il lettore \n\n{cert.LastErrorText}");
            }

            success = crypt.SetSigningCert(cert);
            if (success == false)
            {
                return new Tuple<bool, string?>(false, $"Errore durante il recupero del certificato per la firma. Verificare la smartcard \n\n {crypt.LastErrorText}");
            }

            // The CadesEnabled property applies to all methods that create PKCS7 signatures. 
            // To create a CAdES-BES signature, set this property equal to true.
            crypt.CadesEnabled = true;

            crypt.HashAlgorithm = "sha256";

            Chilkat.JsonObject signedAttrs = new Chilkat.JsonObject();
            signedAttrs.UpdateInt("contentType", 1);
            signedAttrs.UpdateInt("signingTime", 1);
            signedAttrs.UpdateInt("messageDigest", 1);
            signedAttrs.UpdateInt("signingCertificateV2", 1);
            crypt.SigningAttributes = signedAttrs.Emit();

            // Create the CAdES-BES attached signature, which contains the original data.
            success = crypt.CreateP7M(SourceXML, SignedXML);
            if (success == false)
            {
                return new Tuple<bool, string?>(false, $"Errore durante l'apposizione della firma \n\n{crypt.LastErrorText}");
            }
            return new Tuple<bool, string?>(true, null);
        }
    }
}
