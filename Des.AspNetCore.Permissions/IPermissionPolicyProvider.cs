using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Des.AspNetCore.Permissions
{
    public interface IPermissionPolicyProvider
    {
        Task<IEnumerable<string>> GetPermissionsAsync(ClaimsPrincipal principal);
    }
}