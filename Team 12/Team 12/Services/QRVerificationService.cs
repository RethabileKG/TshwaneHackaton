using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Team_12.Models;
using Team_12.Repositories;

namespace Team_12.Services
{
    public interface IQRVerificationService
    {
        string GenerateQRContent(Booking booking);
        Task<bool> VerifyAndMarkQRCode(string qrContent);
    }

    public class QRVerificationService : IQRVerificationService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly string _encryptionKey;

        public QRVerificationService(IBookingRepository bookingRepository, IConfiguration configuration)
        {
            _bookingRepository = bookingRepository;
            _encryptionKey = configuration["QRSettings:EncryptionKey"];
        }

        public string GenerateQRContent(Booking booking)
        {
            var verificationData = new QRVerificationModel
            {
                BookingId = booking.BookingId,
                FacilityId = booking.FacilityId,
                BookingDate = booking.BookingDate,
                IsUsed = false,
                Booking = booking,
                Facility = booking.Facility
            };

            // Convert the object to JSON and encrypt it
            string jsonData = JsonSerializer.Serialize(verificationData, new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
            });
            return EncryptData(jsonData);
        }

        public async Task<bool> VerifyAndMarkQRCode(string qrContent)
        {
            try
            {
                // Decrypt the QR content
                string decryptedData = DecryptData(qrContent);
                var verificationData = JsonSerializer.Deserialize<QRVerificationModel>(decryptedData, new JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve
                });

                // Get the booking from database with navigation properties
                var booking = await _bookingRepository.GetBookingById(verificationData.BookingId);

                if (booking == null)
                    return false;

                // Verify booking hasn't been used and is for today
                if (booking.IsUsed || booking.BookingDate.Date != DateTime.Now.Date)
                    return false;

                // Verify facility matches
                if (booking.FacilityId != verificationData.FacilityId)
                    return false;

                // Mark booking as used
                booking.IsUsed = true;
                booking.UsedDateTime = DateTime.Now;

                await _bookingRepository.UpdateBooking(booking);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private string EncryptData(string data)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                aes.IV = new byte[16];  // Use a secure IV in production
                using (var encryptor = aes.CreateEncryptor())
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(data);
                    }
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        private string DecryptData(string encryptedData)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_encryptionKey);
                aes.IV = new byte[16];  // Use the same IV as encryption
                using (var decryptor = aes.CreateDecryptor())
                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedData)))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}