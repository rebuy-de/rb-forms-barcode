using System;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;

namespace Sample.Pcl.Helper
{
    public class CameraPermission
    {
        private readonly IPermissions permissions;

        public CameraPermission(IPermissions permissions)
        {
            this.permissions = permissions;
        }

        public async Task<bool> RequestCameraPermissionIfNeeded()
        {
            var status = await permissions.CheckPermissionStatusAsync(Permission.Camera);
            if (status != PermissionStatus.Granted) {
                var results = await permissions.RequestPermissionsAsync(new[] {Permission.Camera});

                status = results[Permission.Camera];
            }

            return status == PermissionStatus.Granted;
        }
    }
}
