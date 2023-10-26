# Hooks

Ce dossier contient les differents Hooks

## Exemple

```c#
[Binding]
public sealed class SpecflowHooks
{
    private readonly ISpecFlowOutputHelper _specFlowOutputHelper;
    private readonly ScenarioContext _scenarioContext;
    private readonly FeatureContext _featureContext;

    public SpecflowHooks(ISpecFlowOutputHelper specFlowOutputHelper, FeatureContext featureContext, ScenarioContext scenarioContext)
    {
        _specFlowOutputHelper = specFlowOutputHelper;
        _scenarioContext = scenarioContext;
        _featureContext = featureContext;
    }

    private static string GetHookName(string name)
    {
        return string.Join(' ', Regex.Split(name, @"(?=[A-Z])"));
    }

    [BeforeTestRun(Order = 2)]
    public static void BeforeTestRun(TestThreadContext testContext)
    {
        Debug.WriteLine(GetHookName(nameof(BeforeTestRun)));
    }

    [BeforeTestRun(Order = 1)]
    public static void BeforeTestRunManager(ITestRunnerManager testRunnerManager, ITestRunner testRunner)
    {
        var location = testRunnerManager.TestAssembly.Location;
        var threadid = testRunner.ThreadId;
        Debug.WriteLine(GetHookName(nameof(BeforeTestRun)));

        Service.Instance.ValueRetrievers.Unregister<BoolValueRetriever>();
        Service.Instance.ValueRetrievers.Register<CustomBooleanRetriever>();

        Service.Instance.ValueRetrievers.Register<ClothesSizeRetriever>();
        Service.Instance.ValueComparers.Register<ClothesSizeComparer>();
        Service.Instance.ValueRetrievers.Register(new NullValueRetriever("NULL")); // Add custom null retriever with custom null value
    }

    [AfterTestRun]
    public static void AfterTestRun()
    {
        Debug.WriteLine(GetHookName(nameof(AfterTestRun)));
    }

    [AfterTestRun]
    public static void AfterTestRun1()
    {
        Debug.WriteLine(GetHookName(nameof(AfterTestRun)));
    }

    [BeforeFeature]
    public static void BeforeFeature(FeatureContext featureContext)
    {

        var msg = $@"
            Hook : {GetHookName(nameof(BeforeTestRun))}
                Feature Title : {featureContext.FeatureInfo.Title}
                Feature description: {featureContext.FeatureInfo.Description}
        ";
        Debug.WriteLine(msg);
    }

    [AfterFeature]
    public static void AfterFeature()
    {
        Debug.WriteLine(GetHookName(nameof(AfterFeature)));
    }

    [BeforeScenario]
    public void BeforeScenario(ScenarioContext scenaioContext)
    {
        var msg = $@"
            Hook : {GetHookName(nameof(BeforeScenario))}
                Feature Title : {scenaioContext.ScenarioInfo.Title}
                Feature description: {scenaioContext.ScenarioInfo.Description}
        ";
        Debug.WriteLine(msg);
        _specFlowOutputHelper.WriteLine(msg);
    }

    [AfterScenario]
    public void AfterScenario()
    {
        Debug.WriteLine(GetHookName(nameof(AfterScenario)));

        if (_scenarioContext.TestError == null) return;
        // Make something when an error is found
    }

    [BeforeScenarioBlock]
    public static void BeforeScenarioBlock()
    {
        Debug.WriteLine(GetHookName(nameof(BeforeScenarioBlock)));
    }

    [AfterScenarioBlock]
    public static void AfterScenarioBlock()
    {
        Debug.WriteLine(GetHookName(nameof(AfterScenarioBlock)));
    }

    [BeforeStep]
    public void BeforeStep(ScenarioContext scenaioContext)
    {
        var text = scenaioContext.StepContext.StepInfo.Text;
        Debug.WriteLine(GetHookName(nameof(BeforeStep)));
        _specFlowOutputHelper.WriteLine(GetHookName(nameof(BeforeStep)));
    }

    [AfterStep]
    public static void AfterStep()
    {
        Debug.WriteLine(GetHookName(nameof(AfterStep)));
    }
}
```
