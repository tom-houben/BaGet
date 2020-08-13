using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BaGet.Scaleway
{
    public class ScalewayObjectStorageOptions: IValidatableObject
    {
        public string BucketEndpoint { get; set; }

        public string AccessKey { get; set; }

        public string SecretKey { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(BucketEndpoint))
            {
                yield return new ValidationResult($"The '{nameof(BucketEndpoint)}' is required!");
            }

            if (string.IsNullOrEmpty(AccessKey))
            {
                yield return new ValidationResult($"The '{nameof(AccessKey)}' is required!");
            }

            if (string.IsNullOrEmpty(SecretKey))
            {
                yield return new ValidationResult($"The '{nameof(SecretKey)}' is required!");
            }
        }
    }
}
