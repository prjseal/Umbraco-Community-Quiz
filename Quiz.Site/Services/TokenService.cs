using Quiz.Site.Models;
using Quiz.Site.Models.Tokens;
using System.Text;

namespace Quiz.Site.Services
{
    public static class TokenService
    {
        public static string GenerateToken(string reason, SimpleUserModel user)
        {
            return GenerateToken(DateTime.UtcNow, reason, user);
        }

        public static string GenerateToken(DateTime startingTime, string reason, SimpleUserModel user)
        {
            byte[] _time = BitConverter.GetBytes(startingTime.ToUniversalTime().ToBinary());
            byte[] _key = Guid.Parse(user.SecurityStamp).ToByteArray();
            byte[] _Id = GetBytes(user.Id.ToString());
            byte[] _reason = GetBytes(reason);
            byte[] data = new byte[_time.Length + _key.Length + _reason.Length + _Id.Length];

            Buffer.BlockCopy(_time, 0, data, 0, _time.Length);
            Buffer.BlockCopy(_key, 0, data, _time.Length, _key.Length);
            Buffer.BlockCopy(_reason, 0, data, _time.Length + _key.Length, _reason.Length);
            Buffer.BlockCopy(_Id, 0, data, _time.Length + _key.Length + _reason.Length, _Id.Length);
            return Convert.ToBase64String(data.ToArray());
        }

        public static TokenValidationModel ValidateToken(string reason, SimpleUserModel user, string token)
        {
            return ValidateToken(reason, user, token, 24);
        }

        public static TokenValidationModel ValidateToken(string reason, SimpleUserModel user, string token, int expiryTimeHours)
        {
            var result = new TokenValidationModel();
            try
            {
                ExtractTokenData(reason, token, out var _time, out var _key, out var _reason, out var _Id);

                DateTime when = DateTime.FromBinary(BitConverter.ToInt64(_time, 0));
                if (when < DateTime.UtcNow.AddHours(-1 * expiryTimeHours))
                    result.Errors.Add(TokenValidationStatus.Expired);
                AdditionalValidation(reason, user, result, _key, _reason, _Id);
            }
            catch (Exception)
            {
                result.Errors.Add(TokenValidationStatus.UnableToParse);
            }

            return result;
        }

        public static TokenValidationModel ValidateToken(string reason, SimpleUserModel user, string token, DateTime expiryDateTime)
        {
            var result = new TokenValidationModel();
            try
            {
                ExtractTokenData(reason, token, out _, out var key, out var reasonBytes, out var id);

                if (expiryDateTime < DateTime.UtcNow)
                    result.Errors.Add(TokenValidationStatus.Expired);
                AdditionalValidation(reason, user, result, key, reasonBytes, id);
            }
            catch (Exception)
            {
                result.Errors.Add(TokenValidationStatus.UnableToParse);
            }

            return result;
        }

        private static void ExtractTokenData(string reason, string token, out byte[] _time, out byte[] _key, out byte[] _reason, out byte[] _Id)
        {
            byte[] data = Convert.FromBase64String(token);
            _time = data.Take(8).ToArray();
            _key = data.Skip(8).Take(16).ToArray();
            _reason = data.Skip(24).Take(reason.Length).ToArray();
            _Id = data.Skip(24 + reason.Length).ToArray();
        }

        private static void AdditionalValidation(string reason, SimpleUserModel user, TokenValidationModel result, byte[] _key, byte[] _reason, byte[] _Id)
        {
            Guid gKey = new(_key);
            if (gKey.ToString("N") != user.SecurityStamp)
                result.Errors.Add(TokenValidationStatus.WrongGuid);

            if (reason != GetString(_reason))
                result.Errors.Add(TokenValidationStatus.WrongPurpose);

            if (user.Id.ToString() != GetString(_Id))
                result.Errors.Add(TokenValidationStatus.WrongUser);
        }

        public static Guid ExtractKey(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return Guid.Empty;
            try
            {
                byte[] data = Convert.FromBase64String(token);
                byte[] _key = data.Skip(8).Take(16).ToArray();
                Guid gKey = new(_key);

                return gKey;
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

        private static string GetString(byte[] reason) => Encoding.ASCII.GetString(reason);

        private static byte[] GetBytes(string reason) => Encoding.ASCII.GetBytes(reason);
    }
}
