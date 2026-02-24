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

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.IdentityUser
{
    public class ApplicationUserLoginRepository : IApplicationUserLoginRepository, IAerospikeSchemaProvider
    {

        private readonly IAsyncClient _client;
        private readonly string Namespace = "";

        public ApplicationUserLoginRepository(IAsyncClient client, IOptions<AerospikeSettings> settings)
        {
            _client = client;
            Namespace = settings.Value.Namespace;
        }

        public async Task AddLoginAsync(Guid userId, ApplicationUserLogin login, CancellationToken cancellationToken = default)
        {
            login.UserId = userId;

            var bins = FillBins(login);
            await _client.Add(new WritePolicy(), cancellationToken, GetKey(login.LoginProvider, login.LoginKey), bins.ToArray());
        }

        public async Task<ApplicationUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken = default)
        {
            var record = await _client.Get(new QueryPolicy(), cancellationToken, GetKey(loginProvider, providerKey));
            return ToUserLogin(record);
        }

        public Task<List<ApplicationUserLogin>> GetLoginsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var bins = GetBinsName();

            var stmt = new Statement
            {
                Namespace = Namespace,
                SetName = Set_Name,
                BinNames = bins
            };

            var queryPolicy = new QueryPolicy();
            queryPolicy.filterExp = Exp.Build(Exp.EQ(Exp.StringBin(UserId_Bin), Exp.Val(userId.ToString())));

            var result = new List<ApplicationUserLogin>();

            _client.Query(queryPolicy, stmt, (key, record) =>
            {
                var login = ToUserLogin(record);

                result.Add(login);
            });

            return Task.FromResult(result);
        }

        public async Task RemoveLoginAsync(Guid userId, string loginProvider, string providerKey, CancellationToken cancellationToken = default)
        {
            var key = GetKey(loginProvider, providerKey);
            var record = await _client.Get(new QueryPolicy(), cancellationToken, key);
            var item = ToUserLogin(record);
            if (item.UserId == userId)
                await _client.Delete(new WritePolicy(), cancellationToken, key);
        }
        private Key GetKey(string loginProvider, string loginkey)
        {
            return new Key(Namespace, Set_Name, GetKeyValue(loginProvider, loginkey));
        }

        private string GetKeyValue(string loginProvider, string loginkey) => $"{loginProvider}:{loginkey}";

        private string[] GetBinsName()
        {
            return new[] {
                Id_Bin,
                LoginKey_Bin,
                LoginProvider_Bin,
                ProviderDisplayName_Bin,
                UserId_Bin,
            };
        }

        private BinList FillBins(ApplicationUserLogin login)
        {
            var bins = new BinList();
            bins.Add(Id_Bin, GetKeyValue(login.LoginProvider, login.LoginKey));
            bins.Add(LoginKey_Bin, login.LoginKey);
            bins.Add(LoginProvider_Bin, login.LoginProvider);
            bins.Add(ProviderDisplayName_Bin, login.ProviderDisplayName);
            bins.Add(UserId_Bin, login.UserId.ToString());
            return bins;
        }

        private ApplicationUserLogin FillObject(Record record)
        {
            return new ApplicationUserLogin
            {
                UserId = Guid.Parse(record.GetString(UserId_Bin)),
                LoginKey = record.GetString(LoginKey_Bin),
                LoginProvider = record.GetString(LoginProvider_Bin),
                ProviderDisplayName = record.GetString(ProviderDisplayName_Bin)
            };
        }

        private ApplicationUserLogin ToUserLogin(Record record)
        {
            return record == null ? null : FillObject(record);
        }

        public List<string> GetBinNames() => GetBinsName().ToList();

        public string GetSetName() => Set_Name;

        public string GetNamespace() => Namespace;

        public List<(string IndexName, string BinName, IndexType Type)> GetIndexes()
        {
            return new List<(string IndexName, string BinName, IndexType Type)>();
        }

        private const string Set_Name = "user_login";

        #region Bins
        private const string Id_Bin = "id";
        private const string LoginProvider_Bin = "lgnprv";
        private const string LoginKey_Bin = "lgnky";
        private const string ProviderDisplayName_Bin = "prvdspnm";
        private const string UserId_Bin = "userid";
        #endregion
    }
}
