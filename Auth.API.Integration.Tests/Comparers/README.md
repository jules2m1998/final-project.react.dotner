# Comparers

Ce dossier stocke les classes qui permettront la configuration des retrievers des tests afin d'assigner des valeurs a un type personnalise dans les differents steps;

## **Exemple :**

### Creation du comparer

```c#
public class ClothesSizeComparer : IValueComparer
{
    public bool CanCompare(object actualValue)
    {
        return actualValue is InternalSize;
    }

    public bool Compare(string expectedValue, object actualValue)
    {
        if (actualValue is not InternalSize data) return false;
        return data.InternalName == expectedValue;
    }
}
```

### Configuration du comparer

```c#
[BeforeTestRun(Order = 1)]
public static void BeforeTestRunManager(ITestRunnerManager testRunnerManager, ITestRunner testRunner)
{
    // .... code before
    Service.Instance.ValueRetrievers.Register<ClothesSizeRetriever>();
    Service.Instance.ValueRetrievers.Register(new ClothesSizeRetriever());
}
```
