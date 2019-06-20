using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using Soapstone.Domain;
using Soapstone.Domain.Interfaces;
using Soapstone.WebApi.ViewModels;

namespace Soapstone.WebApi.Security
{
    public class TokenService
    {
        TokenSettings _settings;
        private IRepository<User> _usersRepository;

        public TokenService(
            TokenSettings settings,
            IRepository<User> usersRepository)
        {
            _settings = settings;
            _usersRepository = usersRepository;
        }

        public async Task<TokenViewModel> GenerateTokenAsync(string username, string password)
        {
            var user = await _usersRepository.SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
                throw new ArgumentException(nameof(username));

            if (!user.ValidatePassword(password))
                throw new ArgumentException(nameof(password));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Convert.FromBase64String(_settings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(DefaultClaims.Username, user.Username),
                    new Claim(DefaultClaims.UserId, user.Id.ToString())
                }),
                IssuedAt = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddYears(7),
                Audience = _settings.Audience,
                Issuer = _settings.Issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            var viewModel = new TokenViewModel
            {
                Token = tokenHandler.WriteToken(token)
            };

            return viewModel;
}
    }
}