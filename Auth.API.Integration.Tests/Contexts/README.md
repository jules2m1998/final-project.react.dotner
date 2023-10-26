# Contexts

Les fichiers de context permettent le partege de donnees entre plusieurs steps par exemple d'un given a un when.

Pour ce faire ils utilisent un systeme d'injection de dependance mise a disposition par specflow.

[Documentation](https://docs.specflow.org/projects/specflow/en/latest/Bindings/Context-Injection.html)

## Exemple

### Le context

```c#
public class ProductTestDataContext
{
    public IEnumerable<ProductModel> ProductQuantities = new List<ProductModel>();
    public string Id = null!;

    public ProductModel TestingProduct => ProductQuantities.First(x => x.Product == Id);
}
```

### L'utilisation

```c#
[Binding]
public class Step_Given
{
    private readonly ProductTestDataContext _productTestDataContext;

    public Step_Given(ProductTestDataContext productTestDataContext)
    {
        _productTestDataContext = productTestDataContext;
    }

    [Given(@"I have the following data")]
    public void GivenIHaveTheFollowingData(IEnumerable<ProductModel> table)
    {
        _productTestDataContext.ProductQuantities = table;
    }

    [Given(@"I am on the product detail page of product (.*)")]
    public void GivenIAmOnTheProductDetailPageOfProduct(string productId)
    {
        _productTestDataContext.Id = productId;
    }
}
```

Ici l'instance de ProductTestDataContext est partage entre les differents steps et ceux meme s'il ne sont pas definit dans la meme classe

```c#
[Binding]
public class Step_Then
{
    private readonly ProductTestDataContext _productTestDataContext;

    public Step_Then(ProductTestDataContext productTestDataContext)
    {
        _productTestDataContext = productTestDataContext;
    }

    [Then(@"the quantities are")]
    public void ThenTheQuantitiesAre(Table table)
    {
        table.CompareToInstance(_productTestDataContext.TestingProduct);
    }
}
```
