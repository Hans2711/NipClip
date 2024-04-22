﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Xml.Serialization;

namespace NipClip.Classes.Clipboard
{
    [Serializable]
    [XmlInclude(typeof(StringClipboardEntry))]
    public abstract class ClipboardEntry : INotifyPropertyChanged
    {
        public virtual string type { get ; set; }

        public virtual DateTime crDate { get; set; }

        [XmlIgnore]
        public virtual object content { get; set; }

        public virtual string? ToString()
        {
            return (string)Content;
        }

        public ClipboardEntry()
        {
            crDate = DateTime.Now;
        }

        [XmlIgnore]
        public virtual object Content { get { return (object)content; }
            set
            {
                if (value != this.content)
                {
                    this.content = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool isEqual(ClipboardEntry other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (this.Content == null && other.Content == null)
            {
                return true;
            }

            if (this.Content == null || other.Content == null)
            {
                return false;
            }

            return this.Content.Equals(other.Content);
        }

        public virtual byte[] EncryptedContent { get; set; }

        public virtual void Encrypt(string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256; // Set the key size to 256 bits
                aesAlg.BlockSize = 128; // Set the block size to 128 bits

                // Ensure the key is of correct length
                byte[] keyBytes = new byte[32]; // 256 bits = 32 bytes
                byte[] keyHash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));
                Array.Copy(keyHash, keyBytes, Math.Min(keyHash.Length, keyBytes.Length));

                aesAlg.Key = keyBytes;
                aesAlg.IV = new byte[16]; // Initialization vector for AES

                // Create an encryptor to perform the stream transform
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        // Convert the content to bytes and encrypt it
                        byte[] bytesContent = Encoding.UTF8.GetBytes(this.Content.ToString());
                        csEncrypt.Write(bytesContent, 0, bytesContent.Length);
                    }
                    EncryptedContent = msEncrypt.ToArray();
                }
            }
        }

        public virtual void Decrypt(string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256; // Set the key size to 256 bits
                aesAlg.BlockSize = 128; // Set the block size to 128 bits

                // Ensure the key is of correct length
                byte[] keyBytes = new byte[32]; // 256 bits = 32 bytes
                byte[] keyHash = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));
                Array.Copy(keyHash, keyBytes, Math.Min(keyHash.Length, keyBytes.Length));

                aesAlg.Key = keyBytes;
                aesAlg.IV = new byte[16]; // Initialization vector for AES

                // Create a decryptor to perform the stream transform
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption
                using (MemoryStream msDecrypt = new MemoryStream(EncryptedContent))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream and return as string
                            string decryptedContent = srDecrypt.ReadToEnd();
                            Content = decryptedContent;
                        }
                    }
                }
            }
        }
    }
}
