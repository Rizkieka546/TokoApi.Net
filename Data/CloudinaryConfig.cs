using CloudinaryDotNet;

namespace TokoApi.Data;

public class CloudinaryConfig
{
    public Cloudinary Cloudinary { get; }

    public CloudinaryConfig(IConfiguration config)
    {
        var account = new Account(
            config["Cloudinary:CloudName"],
            config["Cloudinary:ApiKey"],
            config["Cloudinary:ApiSecret"]
        );

        Cloudinary = new Cloudinary(account);
    }
}
