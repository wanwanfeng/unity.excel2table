
using System;
using System.IO;
using System.IO.Compression;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using LitJson;

public class FileHelper
{
	static System.Security.Cryptography.MD5 MD5 = System.Security.Cryptography.MD5.Create();
	static System.Security.Cryptography.SHA1 SHA1 = System.Security.Cryptography.SHA1.Create();

	public static string GetMD5(string str)
	{
		return GetMD5(Encoding.UTF8.GetBytes(str));
	}

	public static string GetMD5(byte[] bytes)
	{
		var strRes = MD5.ComputeHash(bytes);
		var enText = new StringBuilder();
		foreach (byte b in strRes)
			enText.AppendFormat("{0:x2}", b ^ 0xaf);
		return enText.ToString();
	}

	public static string GetSha1(string str)
	{
		return GetSha1(Encoding.UTF8.GetBytes(str));
	}

	public static string GetSha1(byte[] bytes)
	{
		var strRes = SHA1.ComputeHash(bytes);
		var enText = new StringBuilder();
		foreach (byte b in strRes)
			enText.AppendFormat("{0:x2}", b ^ 0xaf);
		return enText.ToString();
	}

	static FileHelper()
	{
		JsonMapper.RegisterImporter<double, float>(input => (float)input);
		JsonMapper.RegisterExporter<float>((v, w) => { w.Write(v); });
	}

	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	public static void WriteAllLines(string path, string[] contents)
	{
		File.WriteAllLines(path, contents);
	}

	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	static public void WriteAllText(string path, string contents)
	{
		Debug.Log(contents);
		File.WriteAllText(Path.Combine("Library", path), contents);
	}

	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	static public void WriteAllText(string path, object obj)
	{
		var content = ToJson(obj, indentLevel: 2);
		Debug.Log(content);
		File.WriteAllText(Path.Combine("Library", path), content);
	}

	static public string ToJson<T>(T t, int indentLevel = 0, bool validate = true)
	{
		if (indentLevel == 0 && validate)
			return JsonMapper.ToJson(t);

		StringWriter sw = new StringWriter();
		JsonWriter jsonWriter = new JsonWriter(sw)
		{
			PrettyPrint = true,
			IndentValue = indentLevel,
			Validate = validate,
		};
		JsonMapper.ToJson(t, jsonWriter);
		return sw.ToString().Trim('\r', '\n');
	}

	public static void Copy(string sourceFileName, string destFileName, bool overwrite = true)
	{
		Debug.Log(sourceFileName + "  ==>  " + destFileName);
		File.Copy(sourceFileName, destFileName, true);
	}

	public class GZIP
	{
		public static byte[] Encrypt(byte[] array, string key)
		{
			byte[] keyArray = Encoding.UTF8.GetBytes(GetMD5(key));

			RijndaelManaged rDel = new RijndaelManaged();
			rDel.Key = keyArray;
			rDel.Mode = CipherMode.ECB;
			rDel.Padding = PaddingMode.PKCS7;

			ICryptoTransform cTransform = rDel.CreateEncryptor();
			byte[] resultArray = cTransform.TransformFinalBlock(array, 0, array.Length);

			return resultArray;
		}

		public static byte[] Decrypt(byte[] array, string key)
		{
			byte[] keyArray = Encoding.UTF8.GetBytes(GetMD5(key));

			RijndaelManaged rDel = new RijndaelManaged();
			rDel.Key = keyArray;
			rDel.Mode = CipherMode.ECB;
			rDel.Padding = PaddingMode.PKCS7;

			ICryptoTransform cTransform = rDel.CreateDecryptor();
			byte[] resultArray = cTransform.TransformFinalBlock(array, 0, array.Length);

			return resultArray;
		}


		public static void Serialize(string[] sourceFileName, string destFileName)
		{
			using (FileStream saveFile = new FileStream(destFileName, FileMode.Create, FileAccess.Write))
			using (var compressedzipStream = new GZipStream(saveFile, CompressionMode.Compress, true))
			using (BinaryWriter bw = new BinaryWriter(compressedzipStream))
			{
				bw.Write(sourceFileName.Length);
				foreach (var item in sourceFileName.Select(p => p.Replace("\\", "/")))
				{
					bw.Write(item);
					var bytes = File.ReadAllBytes(item);
					bw.Write(bytes.Length);
					bw.Write(bytes);
				}
			}

			File.WriteAllBytes(destFileName, Encrypt(File.ReadAllBytes(destFileName), "sfsgddythfn"));

			TestDeserialize(destFileName);
		}

		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		static void TestDeserialize(string destFileName)
		{
			if (Deserialize(destFileName, out Dictionary<string, byte[]> result))
			{
				foreach (var item in result)
				{
					Debug.Log(item.Key);
					Debug.Log(item.Value.Length);
				}
			}
		}

		public static bool Deserialize(string sourceFileName, out Dictionary<string, byte[]> result)
		{
			var temp = sourceFileName + ".temp";
			try
			{
				File.WriteAllBytes(temp, Decrypt(File.ReadAllBytes(sourceFileName), "sfsgddythfn"));

				result = new Dictionary<string, byte[]>();
				using (FileStream fileStream = new FileStream(temp, FileMode.Open, FileAccess.Read))
				using (GZipStream decompressedStream = new GZipStream(fileStream, CompressionMode.Decompress))
				using (BinaryReader br = new BinaryReader(decompressedStream))
				{
					var count = br.ReadInt32();
					for (int i = 0; i < count; i++)
					{
						var key = br.ReadString();
						var length = br.ReadInt32();
						var value = br.ReadBytes(length);
						result[key] = value;
					}
				}
				return true;
			}
			catch (Exception)
			{
				result = null;
				return false;
            }
            finally
            {
				File.Delete(temp);
			}
		}

		public static void WriteAllText(string destFileName, string content)
		{
			WriteAllBytes(destFileName, System.Text.Encoding.UTF8.GetBytes(content));
		}

		public static void WriteAllBytes(string destFileName, byte[] bytes)
		{
			using (var fw = File.Create(Path.ChangeExtension(destFileName, ".gz")))
			using (var compressedzipStream = new GZipStream(fw, CompressionMode.Compress, true))
				compressedzipStream.Write(bytes, 0, bytes.Length);
		}

		public static string ReadAllText(string fileName)
		{
			string result = string.Empty;
			using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (GZipStream decompressedStream = new GZipStream(fileStream, CompressionMode.Decompress))
			using (StreamReader reader = new StreamReader(decompressedStream))
				result = reader.ReadToEnd();
			return result;
		}

		public static IEnumerator<string> ReadLine(string fileName)
		{
			string result = string.Empty;
			using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
			using (GZipStream decompressedStream = new GZipStream(fileStream, CompressionMode.Decompress))
			using (StreamReader reader = new StreamReader(decompressedStream))
				yield return reader.ReadLine();
		}
	}
}