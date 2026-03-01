using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace dtc.Domain.Entities.Permissions
{
    public class Role
    {
        public int Id { get; private set; }
        public UserRole RoleName { get; private set; }

        protected Role() { }

        public Role(UserRole roleName)
        {
            if (!Enum.IsDefined(typeof(UserRole), roleName))
                throw new ArgumentException("Invalid role", nameof(roleName));

            Id = (int)roleName;
            RoleName = roleName;
        }
    }
}
