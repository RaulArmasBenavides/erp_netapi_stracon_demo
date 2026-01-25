using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentServiceNet.CrossCutting.Options
{
    public sealed class CloudinaryOptions
    {
        public string CloudName { get; init; } = default!;
        public string ApiKey { get; init; } = default!;
        public string ApiSecret { get; init; } = default!;

        // Opcionales
        public string DefaultFolder { get; init; } = "stracon";
        public string? LogoPublicId { get; init; }

        public string? ProtectedRentingPublicId { get; init; }
        public string? ProtectedEmploymentPublicId { get; init; }
        public string? ProtectedSocialHelpPublicId { get; init; }
    }
}
