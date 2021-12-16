namespace DependencyMaven;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Serialization;

public class DependencyInjector
{
    private const string pomFile = "pom.xml";
    public static Dependency DependencyFromString(string input)
    {
        // Expected string format: '<groupId>:<artifactId>:<version>'
        var splitDependencyParts = input
            .Replace("'", "")   // Replacing possible ' characters, although shell probably does that already
            .Split(':');        // Splitting on :

        return new Dependency
        {
            groupId = splitDependencyParts[0],
            artifactId = splitDependencyParts[1],
            version = splitDependencyParts[2]
        };
    }
    public static void Inject(string dependencyString)
    {
        var newDependency = DependencyFromString(dependencyString);

        Model? pomObject = ObtainPomFromFile();
        if (pomObject is null)
            return;

        // Checking if there is any duplicate
        if (!pomObject.dependencies.Any(dep => dep.artifactId == newDependency.artifactId && dep.version == newDependency.version))
            pomObject.dependencies = pomObject.dependencies.Append(newDependency).ToArray();
        else
        {
            System.Console.WriteLine("Dependency already satisfied.");
            return;
        }

        RewritePom(pomObject);
    }

    private static Model? ObtainPomFromFile()
    {
        try
        {
            var serializer = new XmlSerializer(typeof(Model));
            using (var readStream = File.OpenRead(pomFile))
            {
                Model? possiblePomObject = (Model?)serializer.Deserialize(readStream);
                if (possiblePomObject is null)
                {
                    System.Console.WriteLine("Pom is null.");
                    return null;
                }
                return possiblePomObject;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to read {pomFile} file.");
            System.Console.WriteLine(e);
            return null;
        }
    }

    private static void RewritePom(Model pomObject)
    {
        var serializer = new XmlSerializer(typeof(Model));
        File.Delete(pomFile);

        using (var writer = new StreamWriter(pomFile))
        using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
        {
            serializer.Serialize(xmlWriter, pomObject);
        }
    }
}