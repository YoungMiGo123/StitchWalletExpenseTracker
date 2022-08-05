using Core.ExpenseWallet.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Net;

namespace Core.ExpenseWallet.Models
{
    public class TokenBuilder : ITokenBuilder
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IStitchSettings _stitchSettings;
        private readonly IHttpService _httpService;
        private readonly IInputOutputHelper _inputOutputHelper;
        public TokenBuilder(IHostingEnvironment  hostingEnvironment, IStitchSettings stitchSettings, IHttpService httpService, IInputOutputHelper inputOutputHelper)
        {
            _hostingEnvironment = hostingEnvironment;
            _stitchSettings = stitchSettings;
            _httpService = httpService;
            _inputOutputHelper = inputOutputHelper; 
        }
        private string GetToken()
        {
            var path = @$"{_hostingEnvironment.WebRootPath}\Keys\certificate.pem";
            var cert = X509Certificate2.CreateFromPemFile(path);
            var now = DateTime.UtcNow;
            var audience = _stitchSettings.AudienceUrl;
            var issuer = _stitchSettings.ClientId;
            var subject = _stitchSettings.ClientId;
            var jti = $"{Guid.NewGuid()}"; 
            var issuedAt = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            var notBefore = now;
            var expiresAt = now + TimeSpan.FromSeconds(3000); 

            var token = new JwtSecurityToken(
                issuer,
                audience,
                new[]
                {
                    new Claim(JwtClaimTypes.JwtId, jti),
                    new Claim(JwtClaimTypes.Subject, subject),
                    new Claim(JwtClaimTypes.IssuedAt, issuedAt, ClaimValueTypes.Integer64),
                },
                notBefore,
                expiresAt,
                new SigningCredentials(
                    new X509SecurityKey(cert),
                    SecurityAlgorithms.RsaSha256
                )
            );
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenResponse = tokenHandler.WriteToken(token);

            return tokenResponse;
        }

        public async Task<AuthenticationToken> GetTokenWithCode(string code)
        {
            var authModel = JsonConvert.DeserializeObject<AuthModel>(_inputOutputHelper.Read(SecurityUtilities.JsonFilePath));
            var assertion = GetToken();
            Dictionary<string, string> request = new Dictionary<string, string>()
            {
                { "grant_type", _stitchSettings.GrantType },
                {"client_id", _stitchSettings.ClientId},
                {"code", code },
                {"redirect_uri", _stitchSettings.RedirectUrl },
                {"code_verifier", authModel.Verifier },
                {"client_assertion", assertion },
                {"client_assertion_type",_stitchSettings.AssertionType }
            };
            string jsonBody = String.Join("&", request.Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));
            var token = await _httpService.PostWithBody<AuthenticationToken>(_stitchSettings.AudienceUrl, jsonBody);
            return token;
       
        }
    }
}
