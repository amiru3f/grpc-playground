using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ProductContracts;

namespace Services;

public class ProductAppService : ProductContracts.ProductService.ProductServiceBase
{
    private static readonly ProductContracts.Products productStorage = new();

    static ProductAppService()
    {
        productStorage.Items.Add(new Product() { Id = 1, Name = "Manju", Description = "Lukas" });
        productStorage.Items.Add(new Product() { Id = 2, Name = "Murugan", Description = "Yeho'ash" });
        productStorage.Items.Add(new Product() { Id = 3, Name = "Antinanco", Description = "Efthimia" });
        productStorage.Items.Add(new Product() { Id = 4, Name = "Yelysaveta", Description = "Fingal" });
        productStorage.Items.Add(new Product() { Id = 5, Name = "Tyra", Description = "Tenzin" });
    }

    public override Task<HelloResponse> Hello(HelloRequest request, ServerCallContext context)
    {
        return Task.FromResult(new HelloResponse()
        {
            Body = "Hello " + request.Name
        });
    }

    public override async Task StreamReply(Empty request, IServerStreamWriter<Message> responseStream, ServerCallContext context)
    {
        int i = 0;
        while (!context.CancellationToken.IsCancellationRequested)
        {
            await responseStream.WriteAsync(new Message() { Body = "Hello" + i++ });
            await Task.Delay(1000);
        }
    }

    public override Task<Product> GetById(ProductIdFilter request, ServerCallContext context)
    {
        var product = productStorage.Items.Where(x => x.Id == request.Id).FirstOrDefault();

        if (null == product)
        {
            context.Status = new Status(StatusCode.NotFound, "object not found");
            return null;
        }

        return Task.FromResult(product);
    }

    public override Task<Products> GetAll(Empty request, ServerCallContext context)
    {
        return Task.FromResult(productStorage);
    }

    public override Task<Product> InsertProduct(Product request, ServerCallContext context)
    {
        request.Id = productStorage.Items.Select(x => x.Id).Max() + 1;
        productStorage.Items.Add(request);

        return Task.FromResult(request);
    }

    public override async Task<Product> UpdateProduct(Product request, ServerCallContext context)
    {
        var product = productStorage.Items.FirstOrDefault(x => x.Id == request.Id);

        if (null == product)
        {
            context.Status = new Status(StatusCode.NotFound, "object not found");
            return await Task.FromResult<Product>(default);
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Description = request.Description;

        return product;
    }

}
