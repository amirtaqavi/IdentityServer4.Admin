using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aerospike.Client;
using Microsoft.Extensions.Options;
using Skoruba.IdentityServer4.Admin.AeroSpike.Models.AspNetIdentity.IdentityUser;
using Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.IdentityUser.Interfaces;
using Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.Interfaces;
using Xanis.DependancyInjection.Extenstions.Aerospike.Model;
using Xanis.Idp.Models.AspNetIdentity.IdentityUser;
using Xanis.Idp.Repository;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.IdentityUser
{
    public class ApplicationUserTokenRepository : IApplicationUserTokenRepository, IAerospikeSchemaProvider
    {
        private readonly string Namespace = "";
        private readonly IAsyncClient _client;

        public ApplicationUserTokenRepository(IAsyncClient client, IOptions<AerospikeSettings> settings)
        {
            _client = client;
            Namespace = settings.Value.Namespace;
        }

        public async Task AddUserTokenAsync(ApplicationUserToken token)
        {
            var bins = FillBins(token);
            await _client.Add(new WritePolicy(), CancellationToken.None,
                GetKey(token.UserId.ToString(), token.LoginProvider, token.Name),
                bins.ToArray());
        }

        public async Task<ApplicationUserToken> FindTokenAsync(Guid userId, string loginProvider, string name, CancellationToken cancellationToken = default)
        {
            var record = await _client.Get(new QueryPolicy(), cancellationToken, GetKey(userId.ToString(), loginProvider, name));
            return ToUserToken(record);
        }

        public async Task RemoveUserTokenAsync(ApplicationUserToken token)
        {
            await _client.Delete(new WritePolicy(), CancellationToken.None, GetKey(token.UserId.ToString(), token.LoginProvider, token.Name));
        }



        private ApplicationUserToken ToUserToken(Record record)
        {
            return record == null ? default : FillObject(record);
        }

        private Key GetKey(string userId, string loginProvider, string name)
        {
            return new Key(Namespace, Set_Name, GetKeyValue(userId, loginProvider, name));
        }

        private string GetKeyValue(string userId, string loginProvider, string name) => $"{userId}:{loginProvider}:{name}";
        private ApplicationUserToken FillObject(Record record)
        {
            return new ApplicationUserToken
            {
                UserId = Guid.Parse(record.GetString(UserId_Bin)),
                LoginProvider = record.GetString(LoginProvider_Bin),
                Name = record.GetString(Name_Bin),
                Value = record.GetString(Value_Bin)
            };
        }

        private BinList FillBins(ApplicationUserToken token)
        {
            var bins = new BinList();

            bins.Add(Id_Bin, GetKeyValue(token.UserId.ToString(), token.LoginProvider, token.Name));
            bins.Add(UserId_Bin, token.UserId.ToString());
            bins.Add(Name_Bin, token.Name);
            bins.Add(LoginProvider_Bin, token.LoginProvider);
            bins.Add(Value_Bin, token.Value);
            return bins;

        }

        private string[] GetBinsName()
        {
            return new[] {
                Id_Bin,
                Name_Bin,
                UserId_Bin,
                LoginProvider_Bin,
                Value_Bin,
            };

        }
        public List<string> GetBinNames() => GetBinsName().ToList();

        public string GetSetName() => Set_Name;

        public string GetNamespace() => Namespace;

        public List<(string IndexName, string BinName, IndexType Type)> GetIndexes()
        {
            return new List<(string IndexName, string BinName, IndexType Type)>();
        }

        private readonly string Set_Name = "user_token";

        #region Bins
        private const string Id_Bin = "id";
        private const string Name_Bin = "name";
        private const string LoginProvider_Bin = "lgnprv";
        private const string Value_Bin = "value";
        private const string UserId_Bin = "userid";


        #endregion
    }
}
