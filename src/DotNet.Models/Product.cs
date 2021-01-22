namespace DotNet.Models
{
    public enum Product
    {
        DotNet,
        DotNetCore,
        DotNetFramework
    }

    public static class ProductExtensions
    {
        public static Product ToProduct(string product) => product switch
        {
            ".NET" => Product.DotNet,
            ".NET Core" => Product.DotNetCore,

            _ => Product.DotNetFramework
        };
    }
}
