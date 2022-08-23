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
        private readonly IEncryptionHelper _encryptionHelper;
        private readonly IStitchSettings _stitchSettings;
        private readonly IHttpService _httpService;

        public TokenBuilder(IHostingEnvironment  hostingEnvironment,IEncryptionHelper encryptionHelper, IStitchSettings stitchSettings, IHttpService httpService)
        {
            _hostingEnvironment = hostingEnvironment;
            _encryptionHelper = encryptionHelper;
            _stitchSettings = stitchSettings;
            _httpService = httpService;
        
        }
        private string GetToken()
        {
            var slashType = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() ? "/" : @"\";

            var path = @$"{_hostingEnvironment.WebRootPath}{slashType}Keys{slashType}certificate.pem";
            var cert = X509Certificate2.CreateFromPemFile(path);
            var now = DateTime.UtcNow;
            var audience = _stitchSettings.AudienceUrl;
            var issuer = _stitchSettings.ClientId;
            var subject = _stitchSettings.ClientId;
            var jti = $"{Guid.NewGuid()}"; 
            var issuedAt = $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            var notBefore = now;
            var expiresAt = now + TimeSpan.FromSeconds(8000); 

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
            var authModel = await _encryptionHelper.GetAuthModel(useExistingValues: true);
            var assertion = GetToken();
            var request = new Dictionary<string, string>()
            {
                {"grant_type", _stitchSettings.GrantType },
                {"client_id", _stitchSettings.ClientId},
                {"code", code },
                {"redirect_uri", _stitchSettings.RedirectUrls.First() },
                {"code_verifier", authModel.Verifier },
                {"client_secret", _stitchSettings.ClientSecret },
                {"client_assertion_type",_stitchSettings.AssertionType }
            };
            string jsonBody = string.Join("&", request.Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));
            var token = await _httpService.PostWithBody<AuthenticationToken>(_stitchSettings.AudienceUrl, jsonBody);
            return token;
       
        }

        public async Task<AuthenticationToken> GetClientToken()
        {
            var authModel = await _encryptionHelper.GetAuthModel(useExistingValues: true);
            var assertion = GetToken();
            var request = new Dictionary<string, string>()
            {
                {"grant_type", "client_credentials"},
                {"client_id", _stitchSettings.ClientId},
                {"audience", _stitchSettings.AudienceUrl},
                {"scope", "client_paymentauthorizationrequest" },
                {"code_verifier", authModel.Verifier },
                {"client_secret", _stitchSettings.ClientSecret },
                {"client_assertion_type",_stitchSettings.AssertionType }
            };
            string jsonBody = String.Join("&", request.Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));
            var token = await _httpService.PostWithBody<AuthenticationToken>(_stitchSettings.AudienceUrl, jsonBody);
            return token;

        }

        public async Task<AuthenticationToken> GetTokenWithCode(string code, bool useExistingAuth)
        {
            var authModel = await _encryptionHelper.GetAuthModel(useExistingValues: useExistingAuth);
            var assertion = GetToken();
            var request = new Dictionary<string, string>()
            {
                {"grant_type", _stitchSettings.GrantType },
                {"client_id", _stitchSettings.ClientId},
                {"code", code },
                {"redirect_uri", _stitchSettings.RedirectUrls.Last() },
                {"code_verifier", authModel.Verifier },
                {"client_secret", _stitchSettings.ClientSecret },
                {"client_assertion_type",_stitchSettings.AssertionType }
            };
            string jsonBody = string.Join("&", request.Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));
            var token = await _httpService.PostWithBody<AuthenticationToken>(_stitchSettings.AudienceUrl, jsonBody);
            return token;

        }
    }
}
