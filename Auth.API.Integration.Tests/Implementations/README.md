# Implementation

Ce dossier contient les differents implementations de nos testes separes dans les fichiers ```Step_Given.cs``` pour les given, ```Step_Then.cs``` pour les differents them et ```Step_When.cs```

```c#
[Binding]
public class Step_Given
{
    private readonly ProductTestDataContext _productTestDataContext;
    private readonly OfferCodesContext _offerCodesContext;
    private readonly ClosesSizeContext _closesSizeContext;

    public Step_Given(ProductTestDataContext productTestDataContext, OfferCodesContext offerCodesContext, ClosesSizeContext closesSizeContext)
    {
        _productTestDataContext = productTestDataContext;
        _offerCodesContext = offerCodesContext;
        _closesSizeContext = closesSizeContext;
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

    [Given(@"I have the following codes")]
    public void GivenIHaveTheFollowingCodes(IEnumerable<OfferCodes> table)
    {
        _offerCodesContext.OfferCodes = table;
    }

    [Given(@"today is '([^']*)'")]
    public void GivenTodayIs(DateTime today)
    {
        _offerCodesContext.Today = today;
    }

    [Given(@"I habe the following clothes size data")]
    public void GivenIHabeTheFollowingClothesSizeData(Table table)
    {
        _closesSizeContext.Closthes = table.CreateSet<Clothes_Size>();
    }

    [Given(@"I have offer code '([^']*)' witch expires in '([^']*)'")]
    public void GivenIHaveOfferCodeWitchExpiresIn(string code, DateTime date)
    {
    }



}

```
