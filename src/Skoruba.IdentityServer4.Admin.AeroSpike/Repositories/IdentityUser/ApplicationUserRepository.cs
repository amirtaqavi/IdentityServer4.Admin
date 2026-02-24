using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aerospike.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Xanis.DependancyInjection.Extenstions.Aerospike.Model;
using Xanis.Idp.Models.AspNetIdentity.IdentityUser;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Repositories.IdentityUser
{
    public class ApplicationUserRepository : IApplicationUserRepository, IAerospikeSchemaProvider
    {


        private readonly string Namespace = "";
        private IAsyncClient _client;

        public ApplicationUserRepository(IAsyncClient client, IOptions<AerospikeSettings> settings)
        {
            _client = client;
            Namespace = settings.Value.Namespace;
        }

        public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            var bins = FillBins(user);
            await _client.Add(new WritePolicy(), cancellationToken, GetKey(user.Id.ToString()), bins.ToArray());
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            var bins = FillBins(user);
            await _client.Put(new WritePolicy(), CancellationToken.None, GetKey(user.Id.ToString()), bins.ToArray());
            return IdentityResult.Success;
        }
        public async Task<IdentityResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var userForDelete = await GetUserForDeleteAsync(id.ToString(), cancellationToken);
            if (userForDelete != null)
            {
                userForDelete.IsDeleted = true;
                await UpdateAsync(userForDelete);
                await _client.Delete(new WritePolicy(), cancellationToken, GetKey(id.ToString()));
            }


            return IdentityResult.Success;
        }
        public async Task<ApplicationUser> GetUserForDeleteAsync(string userId, CancellationToken cancellationToken = default)
        {
            var record = await _client.Get(new QueryPolicy(), cancellationToken, GetKey(userId));
            var result = ToApplicationUser(record);
            return result;

        }
        public Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, List<int> userType = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(normalizedEmail))
                return Task.FromResult<ApplicationUser>(default);

            var predicate = Exp.EQ(Exp.StringBin(NormalizedEmail_Bin), Exp.Val(normalizedEmail));

            var queryPolicy = new QueryPolicy(_client.QueryPolicyDefault)
            {
                filterExp = Exp.Build(predicate)
            };

            var stmt = new Statement
            {
                Namespace = Namespace,
                SetName = Set_Name,
                BinNames = GetBinsName()
            };

            var users = _client.Query(queryPolicy, stmt);

            var selectedUsers = new List<ApplicationUser>();

            try
            {
                while (users.Next())
                {
                    var record = users.Record;
                    var user = ToApplicationUser(record);
                    if (user != null && !user.IsDeleted)
                    {
                        selectedUsers.Add(user);
                    }
                }
            }
            finally
            {
                users.Close();
            }
            ApplicationUser result = null;
            if (userType != null && userType.Count > 0)
            {
                result = selectedUsers.Where(q => userType.Contains((int)q.UserType)).FirstOrDefault();
            }
            else
            {
                result = selectedUsers.FirstOrDefault();
            }
            return Task.FromResult(result);
        }

        public async Task<ApplicationUser> FindByIdAsync(string userId, List<int> userType = null, CancellationToken cancellationToken = default)
        {
            var record = await _client.Get(new QueryPolicy(), cancellationToken, GetKey(userId));
            var user = ToApplicationUser(record);
            if (user != null && !user.IsDeleted)
            {
                if (userType != null && userType.Count > 0)
                {
                    return userType.Contains((int)user.UserType) ? user : null;
                }
                else
                {
                    return user;
                }

            }
            return null;

        }


        public Task<ApplicationUser> FindByNameAsync(string normalizedUserName, List<int> userType = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(normalizedUserName))
                return Task.FromResult<ApplicationUser>(default);

            var predicate = Exp.EQ(Exp.StringBin(NormalizedUserName_Bin), Exp.Val(normalizedUserName));

            var queryPolicy = new QueryPolicy(_client.QueryPolicyDefault)
            {
                filterExp = Exp.Build(predicate)
            };

            var stmt = new Statement
            {
                Namespace = Namespace,
                SetName = Set_Name,
                BinNames = GetBinsName()
            };

            var users = _client.Query(queryPolicy, stmt);

            var selectedUsers = new List<ApplicationUser>();

            try
            {
                while (users.Next())
                {
                    var record = users.Record;
                    var user = ToApplicationUser(record);
                    if (user != null && !user.IsDeleted)
                    {
                        selectedUsers.Add(user);
                    }

                }
            }
            finally
            {
                users.Close();
            }

            ApplicationUser result = null;
            if (userType != null && userType.Count > 0)
            {
                result = selectedUsers.Where(q => userType.Contains((int)q.UserType)).FirstOrDefault();
            }
            else
            {
                result = selectedUsers.FirstOrDefault();
            }
            return Task.FromResult(result);
        }
        public Task<ApplicationUser> FindByNationalCodeAsync(string nationalCode, List<int> userType = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(nationalCode))
                return Task.FromResult<ApplicationUser>(default);

            var predicate = Exp.EQ(Exp.StringBin(NationalCode_Bin), Exp.Val(nationalCode));

            var queryPolicy = new QueryPolicy(_client.QueryPolicyDefault)
            {
                filterExp = Exp.Build(predicate)
            };

            var stmt = new Statement
            {
                Namespace = Namespace,
                SetName = Set_Name,
                BinNames = GetBinsName()
            };

            var users = _client.Query(queryPolicy, stmt);

            var selectedUsers = new List<ApplicationUser>();

            try
            {
                while (users.Next())
                {
                    var record = users.Record;
                    var user = ToApplicationUser(record);
                    if (user != null && !user.IsDeleted)
                    {
                        selectedUsers.Add(user);
                    }
                }
            }
            finally
            {
                users.Close();
            }

            ApplicationUser result = null;
            if (userType != null && userType.Count > 0)
            {
                result = selectedUsers.Where(q => userType.Contains((int)q.UserType)).FirstOrDefault();
            }
            else
            {
                result = selectedUsers.FirstOrDefault();
            }
            return Task.FromResult(result);
        }
        public Task<ApplicationUser> FindByCellPhoneAsync(string cellPhone, List<int> userType = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(cellPhone))
                return Task.FromResult<ApplicationUser>(default);

            var predicate = Exp.EQ(Exp.StringBin(CellPhoneNumber_Bin), Exp.Val(cellPhone));

            var queryPolicy = new QueryPolicy(_client.QueryPolicyDefault)
            {
                filterExp = Exp.Build(predicate)
            };

            var stmt = new Statement
            {
                Namespace = Namespace,
                SetName = Set_Name,
                BinNames = GetBinsName()
            };

            var users = _client.Query(queryPolicy, stmt);

            var selectedUsers = new List<ApplicationUser>();

            try
            {
                while (users.Next())
                {
                    var record = users.Record;
                    var user = ToApplicationUser(record);
                    if (user != null && !user.IsDeleted)
                    {
                        selectedUsers.Add(user);
                    }
                }
            }
            finally
            {
                users.Close();
            }

            ApplicationUser result = null;
            if (userType != null && userType.Count > 0)
            {
                result = selectedUsers.Where(q => userType.Contains((int)q.UserType)).FirstOrDefault();
            }
            else
            {
                result = selectedUsers.FirstOrDefault();
            }
            return Task.FromResult(result);
        }



        public async Task<ApplicationUser> GetByUsernameOrEmailOrNationalCodeAsync(ApplicationUser user, List<int> userType = null)
        {
            var existingUserByUsername = await FindByNameAsync(user.NormalizedUserName, userType);
            if (existingUserByUsername != null) return existingUserByUsername;

            var existingUserByNationalCode = await FindByNationalCodeAsync(user.NationalCode, userType);
            if (existingUserByNationalCode != null) return existingUserByNationalCode;

            var existingUserByEmail = await FindByEmailAsync(user.Email, userType);
            if (existingUserByEmail != null) return existingUserByEmail;

            var existingUserByCellPhone = await FindByCellPhoneAsync(user.CellPhoneNumber, userType);
            if (existingUserByCellPhone != null && existingUserByCellPhone != user) return existingUserByCellPhone;

            return null;
        }
        public Task<ApplicationUser> FindByPersonageIdAsync(string personageId, List<int> userType = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(personageId))
                return default;

            var predicate = Exp.EQ(Exp.StringBin(PersonageId_Bin), Exp.Val(personageId));

            var queryPolicy = new QueryPolicy(_client.QueryPolicyDefault)
            {
                filterExp = Exp.Build(predicate)
            };

            var stmt = new Statement
            {
                Namespace = Namespace,
                SetName = Set_Name,
                BinNames = GetBinsName()
            };

            var users = _client.Query(queryPolicy, stmt);

            var selectedUsers = new List<ApplicationUser>();

            try
            {
                while (users.Next())
                {
                    var record = users.Record;
                    var user = ToApplicationUser(record);
                    if (user != null && !user.IsDeleted)
                    {
                        selectedUsers.Add(user);
                    }
                }
            }
            finally
            {
                users.Close();
            }

            ApplicationUser result = null;
            if (userType != null && userType.Count > 0)
            {
                result = selectedUsers.Where(q => userType.Contains((int)q.UserType)).FirstOrDefault();
            }
            else
            {
                result = selectedUsers.FirstOrDefault();
            }
            return Task.FromResult(result);
        }
        public Task<List<ApplicationUser>> FindAllByPersonageIdAsync(string personageId, List<int> userType = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(personageId))
                return default;

            var predicate = Exp.EQ(Exp.StringBin(PersonageId_Bin), Exp.Val(personageId));

            var queryPolicy = new QueryPolicy(_client.QueryPolicyDefault)
            {
                filterExp = Exp.Build(predicate)
            };

            var stmt = new Statement
            {
                Namespace = Namespace,
                SetName = Set_Name,
                BinNames = GetBinsName()
            };

            var users = _client.Query(queryPolicy, stmt);

            var selectedUsers = new List<ApplicationUser>();

            try
            {
                while (users.Next())
                {
                    var record = users.Record;
                    var user = ToApplicationUser(record);
                    if (user != null && !user.IsDeleted)
                    {

                        if (userType != null && userType.Contains((int)user.UserType))
                        {
                            selectedUsers.Add(user);
                        }
                        else
                        {
                            selectedUsers.Add(user);
                        }

                    }
                }
            }
            finally
            {
                users.Close();
            }

            return Task.FromResult(selectedUsers);
        }
        private ApplicationUser ToApplicationUser(Record record)
        {
            return record != null ? FillObject(record) : default;
        }
        private Key GetKey(string id)
        {
            return new Key(Namespace, Set_Name, id);
        }

        private async Task<ApplicationUser> GetByIdAsync(Guid id)
        {
            var record = await _client.Get(new QueryPolicy(), CancellationToken.None, GetKey(id.ToString()));
            return ToApplicationUser(record);
        }

        public async Task<List<ApplicationUser>> GetAllAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default)
        {
            var users = new List<ApplicationUser>();
            var records = await _client.Get(new BatchPolicy(), cancellationToken, userIds.Select(x => GetKey(x)).ToArray());
            foreach (var record in records)
            {
                var item = ToApplicationUser(record);
                if (item != null)
                    users.Add(item);
            }
            return users;
        }
        public IEnumerable<ApplicationUser> GetAll()
        {
            var stmt = new Statement();
            stmt.SetNamespace(Namespace);
            stmt.SetSetName(GetSetName());
            stmt.SetBinNames(GetBinNames().ToArray());

            var userRecord = _client.Query(new QueryPolicy(), stmt);

            try
            {
                var users = new List<ApplicationUser>();

                while (userRecord.Next())
                {
                    var record = userRecord.Record;
                    var user = ToApplicationUser(record);
                    users.Add(user);

                }

                return users;
            }
            finally
            {
                userRecord.Close();
            }
        }
        private string[] GetBinsName()
        {
            return new[] {
                Id_Bin,
                Username_Bin,
                NormalizedUserName_Bin,
                EmailConfirmed_Bin,
                Email_Bin,
                NormalizedEmail_Bin,
                PhoneNumber_Bin,
                PhoneNumberConfirm_Bin,
                CellPhoneNumber_Bin,
                NationalCode_Bin,
                ConcurrencyStamp_Bin,
                PasswordHash_Bin,
                SecurityStamp_Bin,
                BirthDate_Bin,
                TwoFactorEnabeled_Bin,
                PersonageId_Bin,
                LockoutEnabled_Bin,
                LockoutEnd_Bin,
                AccessFailedCount_Bin,
                UserType_Bin,
                RegisterStatus_Bin,
                SejamStatus_Bin,
                IsSuspended_Bin,
                IsSuspensionDesk_Bin,
                AllowedIPs_Bin,
                Title_Bin,
                Gender_Bin,
                Pid_Bin,
                AdditionalClaim_Bin,
                ForceChangePassword_Bin,
                LastPasswordChange_Bin

            };
        }

        private BinList FillBins(ApplicationUser user)
        {
            var binList = new BinList();
            binList.Add(Id_Bin, user.Id.ToString());
            binList.Add(Username_Bin, user.UserName);
            binList.Add(NormalizedUserName_Bin, user.NormalizedUserName);
            binList.Add(EmailConfirmed_Bin, user.EmailConfirmed);
            binList.Add(Email_Bin, user.Email);
            binList.Add(NormalizedEmail_Bin, user.NormalizedEmail);
            binList.Add(PhoneNumber_Bin, user.PhoneNumber);
            binList.Add(PhoneNumberConfirm_Bin, user.PhoneNumberConfirmed);
            binList.Add(CellPhoneNumber_Bin, user.CellPhoneNumber);
            binList.Add(NationalCode_Bin, user.NationalCode);
            binList.Add(ConcurrencyStamp_Bin, user.ConcurrencyStamp);
            binList.Add(PasswordHash_Bin, user.PasswordHash);
            binList.Add(SecurityStamp_Bin, user.SecurityStamp);
            if (user.BirthDate != null) binList.Add(BirthDate_Bin, user.BirthDate.Value.Ticks);
            binList.Add(TwoFactorEnabeled_Bin, user.TwoFactorEnabled);
            binList.Add(PersonageId_Bin, user.PersonageId.ToString());
            binList.Add(LockoutEnabled_Bin, user.LockoutEnabled);
            binList.Add(LockoutEnd_Bin, user.LockoutEnd.HasValue ? user.LockoutEnd.Value.Ticks : 0);
            binList.Add(AccessFailedCount_Bin, user.AccessFailedCount);
            binList.Add(UserType_Bin, (short)user.UserType);
            binList.Add(RegisterStatus_Bin, (short)user.RegisterStatus);
            binList.Add(SejamStatus_Bin, (short)user.SejamStatus);
            binList.Add(Title_Bin, user.Title);
            binList.Add(Gender_Bin, (short)user.Gender);
            binList.Add(IsSuspended_Bin, (bool)user.IsSuspended);
            binList.Add(IsSuspensionDesk_Bin, user.SuspensionDesc);
            binList.Add(IsDeleted_Bin, (bool)user.IsDeleted);
            binList.Add(AllowedIPs_Bin, user.AllowedIPs.ToArray());
            binList.Add(Pid_Bin, user.Pid);
            binList.Add(AdditionalClaim_Bin, user.AdditionalClaim);
            binList.Add(ForceChangePassword_Bin, user.ForceChangePassword);
            binList.Add(LastPasswordChange_Bin, user.LastPasswordChange.Ticks);
            return binList;
        }
        private ApplicationUser FillObject(Record record)
        {
            var isDeleted = false;
            if (record.bins.ContainsKey(IsDeleted_Bin))
            {
                isDeleted = record.GetBool(IsDeleted_Bin);
            }
            var forceChangePassword = true;
            if (record.bins.ContainsKey(ForceChangePassword_Bin))
            {
                forceChangePassword = record.GetBool(ForceChangePassword_Bin);
            }
            DateTime lastPasswordChange = DateTime.Now;
            if (record.bins.ContainsKey(LastPasswordChange_Bin))
            {
                lastPasswordChange = record.GetLong(LastPasswordChange_Bin) > 0 ? new DateTime(record.GetLong(LastPasswordChange_Bin)) : DateTime.Now;
            }
            var sejamStatus = SejamStatus.None;
            if (record.bins.ContainsKey(SejamStatus_Bin))
            {
                try
                {
                    sejamStatus = (SejamStatus)record.GetShort(SejamStatus_Bin);
                }
                catch (Exception)
                {
                    sejamStatus = SejamStatus.None;
                }

            }
            var user = new ApplicationUser
            {
                Id = Guid.Parse(record.GetString(Id_Bin)),
                AccessFailedCount = record.GetInt(AccessFailedCount_Bin),
                UserName = record.GetString(Username_Bin),
                NormalizedUserName = record.GetString(NormalizedUserName_Bin),
                PhoneNumber = record.GetString(PhoneNumber_Bin),
                PhoneNumberConfirmed = record.GetBool(PhoneNumberConfirm_Bin),
                Email = record.GetString(Email_Bin),
                NormalizedEmail = record.GetString(NormalizedEmail_Bin),
                EmailConfirmed = record.GetBool(EmailConfirmed_Bin),
                BirthDate = record.GetLong(BirthDate_Bin) > 0 ? new DateTime(record.GetLong(BirthDate_Bin)) : null,
                CellPhoneNumber = record.GetString(CellPhoneNumber_Bin),
                ConcurrencyStamp = record.GetString(ConcurrencyStamp_Bin),
                LockoutEnabled = record.GetBool(LockoutEnabled_Bin),
                LockoutEnd = record.GetLong(LockoutEnd_Bin) > 0 ? new DateTime(record.GetLong(LockoutEnd_Bin)) : null,
                NationalCode = record.GetString(NationalCode_Bin),
                PasswordHash = record.GetString(PasswordHash_Bin),
                PersonageId = record.GetString(PersonageId_Bin) == "NULL" ? null : Guid.Parse(record.GetString(PersonageId_Bin)),
                SecurityStamp = record.GetString(SecurityStamp_Bin),
                TwoFactorEnabled = record.GetBool(TwoFactorEnabeled_Bin),
                UserType = (UserTypes)record.GetShort(UserType_Bin),
                RegisterStatus = (RegisterStatus)record.GetShort(RegisterStatus_Bin),
                SejamStatus = sejamStatus,
                IsSuspended = record.GetBool(IsSuspended_Bin),
                SuspensionDesc = record.GetString(IsSuspensionDesk_Bin),
                Title = record.GetString(Title_Bin),
                Gender = (GenderType)record.GetInt(Gender_Bin),
                Pid = record.GetString(Pid_Bin),
                AdditionalClaim = record.GetString(AdditionalClaim_Bin),
                ForceChangePassword = forceChangePassword,
                LastPasswordChange = lastPasswordChange,
                IsDeleted = isDeleted
            };
            user.SetAllowedIPs(record.GetList(AllowedIPs_Bin)?.OfType<string>() ?? new List<string>());
            return user;
        }
        public List<string> GetBinNames() => GetBinsName().ToList();

        public string GetSetName() => Set_Name;

        public string GetNamespace() => Namespace;

        public List<(string IndexName, string BinName, IndexType Type)> GetIndexes()
        {
            return new List<(string IndexName, string BinName, IndexType Type)>();
        }

        private const string Set_Name = "users";

        #region Bins
        private const string Id_Bin = "id";
        private const string NormalizedUserName_Bin = "usr_nrmusrnm";
        private const string Username_Bin = "usr_usrnm";
        private const string PhoneNumber_Bin = "usr_phn";
        private const string PhoneNumberConfirm_Bin = "usr_phnconf";
        private const string PasswordHash_Bin = "usr_psshsh";
        private const string EmailConfirmed_Bin = "usr_mailconf";
        private const string NormalizedEmail_Bin = "usr_nrmmail";
        private const string Email_Bin = "usr_mail";
        private const string LockoutEnabled_Bin = "usr_lckenb";
        private const string AccessFailedCount_Bin = "usr_acsfldcnt";
        private const string SecurityStamp_Bin = "usr_scrtstmp";
        private const string ConcurrencyStamp_Bin = "usr_cncrstmp";
        private const string TwoFactorEnabeled_Bin = "usr_twofctenb";
        private const string LockoutEnd_Bin = "usr_lckend";
        private const string PersonageId_Bin = "usr_prsnid";
        private const string NationalCode_Bin = "usr_nc";
        private const string CellPhoneNumber_Bin = "usr_cllphn";
        private const string BirthDate_Bin = "usr_brtdt";
        private const string Title_Bin = "usr_title";
        private const string Gender_Bin = "usr_gender";
        private const string UserType_Bin = "usr_typ";
        private const string RegisterStatus_Bin = "reg_stat";
        private const string SejamStatus_Bin = "sejam_stat";
        private const string IsSuspended_Bin = "usr_spnd";
        private const string IsDeleted_Bin = "usr_isdel";
        private const string IsSuspensionDesk_Bin = "usr_spnddsc";
        private const string AllowedIPs_Bin = "usr_alwdips";
        private const string Pid_Bin = "usr_pid";
        private const string AdditionalClaim_Bin = "usr_addclaim";
        private const string ForceChangePassword_Bin = "usr_passforcch";
        private const string LastPasswordChange_Bin = "usr_lastpassch";


        #endregion
    }
}
