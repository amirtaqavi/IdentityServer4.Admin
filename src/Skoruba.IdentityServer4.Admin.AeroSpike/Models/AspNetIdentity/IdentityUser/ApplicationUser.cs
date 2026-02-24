
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Identity;

namespace Skoruba.IdentityServer4.Admin.AeroSpike.Models.AspNetIdentity.IdentityUser
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        private Guid _id;

        public override Guid Id
        {
            get
            {
                if (_id == Guid.Empty)
                {
                    _id = Guid.NewGuid();
                }

                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public bool ForceChangePassword { get; set; } = false;


        public Guid? PersonageId { get; set; }

        public string Title { get; set; }

        public GenderType Gender { get; set; }

        public string NationalCode { get; set; }

        public string CellPhoneNumber { get; set; }

        public DateTimeOffset? BirthDate { get; set; }

        public UserTypes UserType { get; set; }

        public RegisterStatus RegisterStatus { get; set; }

        public SejamStatus SejamStatus { get; set; }

        public bool IsSuspended { get; set; } = false;


        public string SuspensionDesc { get; set; }

        public List<string> AllowedIPs { get; set; } = new List<string>();


        public DateTime LastPasswordChange { get; set; }

        public string Pid { get; set; }

        public string AdditionalClaim { get; set; }

        public bool IsDeleted { get; set; }

        public bool SetAllowedIPs(IEnumerable<string> ips)
        {
            foreach (string ip in ips)
            {
                if (!IPAddress.TryParse(ip, out IPAddress _))
                {
                    return false;
                }
            }

            AllowedIPs = ips.ToList();
            return true;
        }

        public void ResetPassword(string passwordHash, bool forceChangePassword = false, DateTime? lastPasswordChange = null)
        {
            ForceChangePassword = forceChangePassword;
            PasswordHash = passwordHash;
            LastPasswordChange = lastPasswordChange ?? DateTime.Now;
        }

        public void ChangePassword(string passwordHash, bool forceChangePassword = false, DateTime? lastPasswordChange = null)
        {
            ForceChangePassword = forceChangePassword;
            PasswordHash = passwordHash;
            LastPasswordChange = lastPasswordChange ?? DateTime.Now;
        }
    }
}

