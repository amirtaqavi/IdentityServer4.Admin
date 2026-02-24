using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aerospike.Client;
using Microsoft.Extensions.Options;
using Xanis.DependancyInjection.Extenstions.Aerospike.Model;
using Xanis.Idp.Models.AspNetIdentity.IdentityUser;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.IdentityUser
{
    public class ApplicationUserClaimRepository : IApplicationUserClaimRepository,
        IAerospikeSchemaProvider
    {
        private readonly string Namespace = "";
        private IAsyncClient _client;
        public ApplicationUserClaimRepository(IAsyncClient client, IOptions<AerospikeSettings> settings)
        {
            _client = client;
            Namespace = settings.Value.Namespace;
        }
        public async Task AddClaimsAsync(IEnumerable<ApplicationUserClaim> claims, CancellationToken cancellationToken = default)
        {
            foreach (var claim in claims)
            {

                var bins = FillBins(claim);
                await _client.Add(new WritePolicy(), cancellationToken, GetKey(claim.Id), bins.ToArray());
            }
        }
        public async Task UpdateClaimsAsync(IEnumerable<ApplicationUserClaim> claims, CancellationToken cancellationToken = default)
        {
            foreach (var claim in claims)
            {

                var bins = FillBins(claim);
                await _client.Put(new WritePolicy(), cancellationToken, GetKey(claim.Id), bins.ToArray());
            }
        }
        public async Task UpdateClaimAsync(ApplicationUserClaim claim, CancellationToken cancellationToken = default)
        {
            var bins = FillBins(claim);
            await _client.Put(new WritePolicy(), cancellationToken, GetKey(claim.Id), bins.ToArray());
        }

        public async Task RemoveClaimsAsync(Guid userId, IEnumerable<ApplicationUserClaim> claims, CancellationToken cancellationToken = default)
        {
            var appClaims = await GetClaimsAsync(userId);
            foreach (var claim in claims)
            {
                var itemsForRemove = appClaims.Where(x => x.ClaimType == claim.ClaimType && x.ClaimValue == claim.ClaimValue);
                foreach (var item in itemsForRemove)
                {
                    item.IsDeleted = true;
                    await UpdateClaimAsync(claim);
                    await _client.Delete(new WritePolicy(), cancellationToken, GetKey(item.Id));
                }

            }
        }
        public void RemoveClaimByIdAsync(ApplicationUserClaim claim, CancellationToken cancellationToken = default)
        {
            claim.IsDeleted = true;
            var bins = FillBins(claim);
            _client.Put(new WritePolicy(), cancellationToken, GetKey(claim.Id), bins.ToArray()).GetAwaiter().GetResult();
            _client.Delete(new WritePolicy(), cancellationToken, GetKey(claim.Id)).GetAwaiter().GetResult();
        }
        public Task<List<ApplicationUserClaim>> GetClaimsAsync(Guid userId, CancellationToken cancellationToken = default)
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

            var result = new List<ApplicationUserClaim>();

            _client.Query(queryPolicy, stmt, (key, record) =>
            {
                var claim = ToUserClaim(record);
                if (CheckValidity(claim) != null)
                {
                    result.Add(claim);
                }
            });

            return Task.FromResult(result);
        }
        public Task<List<ApplicationUserClaim>> GetClaimsForRemovingAsync(Guid userId, CancellationToken cancellationToken = default)
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

            var result = new List<ApplicationUserClaim>();

            _client.Query(queryPolicy, stmt, (key, record) =>
            {
                var claim = ToUserClaim(record);
                result.Add(claim);
            });

            return Task.FromResult(result);
        }

        public Task<List<ApplicationUserClaim>> GetClaimsAsync(string type, string value, CancellationToken cancellationToken = default)
        {
            var bins = GetBinsName();

            var stmt = new Statement
            {
                Namespace = Namespace,
                SetName = Set_Name,
                BinNames = bins
            };

            var queryPolicy = new QueryPolicy();
            var exps = new List<Exp>();
            exps.Add(Exp.EQ(Exp.StringBin(Type_Bin), Exp.Val(type)));
            exps.Add(Exp.EQ(Exp.StringBin(Value_Bin), Exp.Val(value)));
            queryPolicy.filterExp = Exp.Build(Exp.And(exps.ToArray()));

            var result = new List<ApplicationUserClaim>();

            _client.Query(queryPolicy, stmt, (key, record) =>
            {
                var claim = ToUserClaim(record);
                if (CheckValidity(claim) != null)
                {
                    result.Add(claim);
                }
            });

            return Task.FromResult(result);
        }

        public async Task ReplaceClaimAsync(ApplicationUserClaim claim, ApplicationUserClaim newClaim, CancellationToken cancellationToken = default)
        {
            await RemoveClaimsAsync(claim.UserId, new List<ApplicationUserClaim> { claim });
            await AddClaimsAsync(new List<ApplicationUserClaim> { newClaim }, cancellationToken);
        }


        public List<string> GetBinNames() => GetBinsName().ToList();

        public string GetSetName() => Set_Name;

        public string GetNamespace() => Namespace;

        public List<(string IndexName, string BinName, IndexType Type)> GetIndexes()
        {
            return new List<(string IndexName, string BinName, IndexType Type)>();
        }

        private string[] GetBinsName()
        {
            return new[]
            {
                Id_Bin, UserId_Bin,
                Type_Bin, Value_Bin,
                IsDeleted_Bin,IsNew_Bin
            };
        }

        private ApplicationUserClaim ToUserClaim(Record record)
        {
            return record == null ? default : FillObject(record);
        }

        private BinList FillBins(ApplicationUserClaim claim)
        {
            var bins = new BinList();
            bins.Add(Id_Bin, claim.Id.ToString());
            bins.Add(UserId_Bin, claim.UserId.ToString());
            bins.Add(Type_Bin, claim.ClaimType);
            bins.Add(Value_Bin, claim.ClaimValue);
            bins.Add(IsDeleted_Bin, claim.IsDeleted);
            return bins;
        }

        private Key GetKey(Guid id)
        {
            return new Key(Namespace, Set_Name, id.ToString());
        }

        private ApplicationUserClaim FillObject(Record record)
        {

            var model = new ApplicationUserClaim
            {
                Id = Guid.Parse(record.GetString(Id_Bin)),
                ClaimType = record.GetString(Type_Bin),
                ClaimValue = record.GetString(Value_Bin),
                UserId = Guid.Parse(record.GetString(UserId_Bin))
            };
            if (record.bins.ContainsKey(IsDeleted_Bin))
            {
                model.IsDeleted = record.GetBool(IsDeleted_Bin);
            }
            else
            {
                model.IsDeleted = true;
            }
            return model;
        }
        private ApplicationUserClaim CheckValidity(ApplicationUserClaim claim)
        {
            if (claim.IsDeleted)
            {
                RemoveClaimByIdAsync(claim);
                return null;
            }
            return claim;
        }

        private const string Set_Name = "user_claim";

        #region Bins
        private const string Id_Bin = "id";
        private const string UserId_Bin = "userid";
        private const string Value_Bin = "clmval";
        private const string Type_Bin = "clmtyp";
        private const string IsDeleted_Bin = "isdel";
        private const string IsNew_Bin = "isnew";
        #endregion
    }
}
