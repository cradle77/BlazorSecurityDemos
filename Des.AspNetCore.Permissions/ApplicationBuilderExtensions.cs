using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Des.AspNetCore.Permissions
{
    public static class ApplicationBuilderExtensions
    {
        public static void UseMyPermissions(this IApplicationBuilder app)
        {
            app.UseMyPermissions("/api/MyPermissions");
        }

        public static void UseMyPermissions(this IApplicationBuilder app, PathString path)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(app));

            if (app.ApplicationServices.GetService(typeof(IPermissionPolicyProvider)) == null)
            {
                throw new InvalidOperationException("You must register the services by calling services.AddPermissions()");
            }

            Func<HttpContext, bool> predicate = c =>
            {
                return
                    (!path.HasValue ||

                        // If you do provide a PathString, want to handle all of the special cases that 
                        // StartsWithSegments handles, but we also want it to have exact match semantics.
                        //
                        // Ex: /Foo/ == /Foo (true)
                        // Ex: /Foo/Bar == /Foo (false)
                        (c.Request.Path.StartsWithSegments(path, out var remaining) &&
                        string.IsNullOrEmpty(remaining)));
            };

            app.MapWhen(predicate, b => b.UseMiddleware<MyPermissionsMiddleware>());
        }
    }
}
