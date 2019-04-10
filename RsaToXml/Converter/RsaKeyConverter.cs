using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace RsaToXml.Converter
{
	public static class RsaKeyConverter
	{
		public static string XmlStringToPem(this string xml)
		{
			try
			{
				using (RSA rsa = RSA.Create())
				{
					rsa.FromXmlString(xml);

					var keyPair = DotNetUtilities.GetRsaKeyPair(rsa);
					if (keyPair != null)
					{
						PrivateKeyInfo privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keyPair.Private);
						return FormatPem(Convert.ToBase64String(privateKeyInfo.GetEncoded()), "RSA PRIVATE KEY");
					}

					var publicKey = DotNetUtilities.GetRsaPublicKey(rsa);
					if (publicKey != null)
					{
						SubjectPublicKeyInfo publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
						return FormatPem(Convert.ToBase64String(publicKeyInfo.GetEncoded()), "PUBLIC KEY");
					}
				}
			}
			catch (Exception e)
			{
				throw new InvalidKeyException("Invalid RSA Xml Key", e);
			}

			throw new InvalidKeyException("Keys were not found");
		}

		private static string FormatPem(string pem, string keyType)
		{
			var sb = new StringBuilder();

			sb.Append("-----BEGIN ")
				.Append(keyType)
				.Append("-----\n");

			int line = 1, width = 64;

			while ((line - 1) * width < pem.Length)
			{
				int startIndex = (line - 1) * width;
				int len = line * width > pem.Length
					? pem.Length - startIndex
					: width;

				sb.Append(pem.Substring(startIndex, len))
					.Append("\n");

				line++;
			}

			sb.Append("-----END ")
				.Append(keyType)
				.Append("-----\n");

			return sb.ToString();
		}

		public static string PemStringToXml(this string pem)
		{
			if (pem.StartsWith("-----BEGIN RSA PRIVATE KEY-----")
				|| pem.StartsWith("-----BEGIN PRIVATE KEY-----"))
			{
				return GetXmlRsaKey(pem, obj =>
				{
					if ((obj as RsaPrivateCrtKeyParameters) != null)
						return DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters)obj);
					var keyPair = (AsymmetricCipherKeyPair)obj;
					return DotNetUtilities.ToRSA((RsaPrivateCrtKeyParameters)keyPair.Private);
				}, rsa => ToXmlString(rsa, true));
			}

			if (pem.StartsWith("-----BEGIN PUBLIC KEY-----"))
			{
				return GetXmlRsaKey(pem, obj => {
					var publicKey = (RsaKeyParameters)obj;
					return DotNetUtilities.ToRSA(publicKey);
				}, rsa => ToXmlString(rsa, false));
			}

			throw new InvalidKeyException("Unsupported PEM format...");
		}

		private static string GetXmlRsaKey(string pem, Func<object, RSA> getRsa, Func<RSA, string> getKey)
		{
			using (var ms = new MemoryStream())
			using (var sw = new StreamWriter(ms))
			using (var sr = new StreamReader(ms))
			{
				sw.Write(pem);
				sw.Flush();
				ms.Position = 0;
				var pr = new PemReader(sr);
				object keyPair = pr.ReadObject();
				using (RSA rsa = getRsa(keyPair))
				{
					var xml = getKey(rsa);
					return xml;
				}
			}
		}

		private static void TryAppend(StringBuilder sb, string element, byte[] value)
		{
			sb.Append("<").Append(element).Append(">");

			if (value != null)
				sb.Append(Convert.ToBase64String(value));

			sb.Append("</").Append(element).Append(">");
		}

		private static string ToXmlString(RSA rsa, bool includePrivateParameters)
		{
			var p = rsa.ExportParameters(includePrivateParameters);

			var sb = new StringBuilder();

			sb.Append("<RSAKeyValue>");
			TryAppend(sb, "Modulus", p.Modulus);
			TryAppend(sb, "Exponent", p.Exponent);
			TryAppend(sb, "P", p.P);
			TryAppend(sb, "Q", p.Q);
			TryAppend(sb, "DP", p.DP);
			TryAppend(sb, "DQ", p.DQ);
			TryAppend(sb, "InverseQ", p.InverseQ);
			TryAppend(sb, "D", p.D);
			sb.Append("</RSAKeyValue>");

			return sb.ToString();
		}
	}
}